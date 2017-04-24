using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (SfxLibrary))]
public sealed class AudioManager : MonoBehaviour {

	const int sfxPoolSize = 10;

	public enum Channel{Master,Music,Sfx};
	enum MusicChannel{A,B};
	MusicChannel activeMusicChannel;
	static AudioManager myInstance;
	SfxLibrary sfxLibrary;

	public AudioMixer masterMixer;
	public AudioSource musicSourceA, musicSourceB;
	public AudioSource sfxSource;

	AudioSource[] sfxSourcePool;
	List<int> sfxPoolIndexQueue;
	float[] sfxTimesRemaining;
	float timeSinceLastSfxCall;

	const float defaultMasterVolumePercent = .85f;
	const float defaultMusicVolumePercent = .65f;
	const float defaultSfxVolumePercent = .65f;

	public float masterVolumePercent {get;private set;}
	public float musicVolumePercent {get;private set;}
	public float sfxVolumePercent {get;private set;}

	#region Initialization
	void Awake() {
		if (myInstance == null) {
			myInstance = this;
			Init();
		}
	}

	void Init() {
		sfxLibrary = GetComponent<SfxLibrary> ();

		// Initialize sound effect pool
		sfxSourcePool = new AudioSource[sfxPoolSize];
		sfxTimesRemaining = new float[sfxPoolSize];
		sfxPoolIndexQueue = new List<int>();
		for (int i =0; i <sfxPoolSize; i ++) {
			AudioSource newSfx = Instantiate(sfxSource) as AudioSource;
			sfxSourcePool[i] = newSfx;
			sfxSourcePool[i].transform.parent = transform;
			sfxPoolIndexQueue.Add(i);
		}

		// Retrieve saved audio levels
		SetDefaultVolume(Channel.Master,PlayerPrefs.GetFloat("masterVolumePercent",defaultMasterVolumePercent));
		SetDefaultVolume(Channel.Music,PlayerPrefs.GetFloat("musicVolumePercent",defaultMusicVolumePercent));
		SetDefaultVolume(Channel.Sfx,PlayerPrefs.GetFloat("sfxVolumePercent",defaultSfxVolumePercent));

	}

	void Start() {
		SetVolume (Channel.Master, masterVolumePercent);
		SetVolume (Channel.Music, musicVolumePercent);
		SetVolume (Channel.Sfx, sfxVolumePercent);
	}

	/// <summary>
	/// Gets the active AudioManager instance
	/// </summary>
	public static AudioManager instance {
		get {
			if (myInstance == null) {
				myInstance = GameObject.FindObjectOfType<AudioManager>();
				myInstance.Init ();
			}
			return myInstance;
		}
	}
	#endregion

	#region Public Methods

	/// <summary>
	/// Stops any current music and play new clip.
	/// </summary>
	public void PlayMusic (AudioClip clip, float volumeScale = 1) {
		musicSourceA.Stop ();
		musicSourceB.Stop ();
		musicSourceA.clip = clip;
		musicSourceA.Play ();
		activeMusicChannel = MusicChannel.A;
	}

	/// <summary>
	/// Crossfades between current music and new clip
	/// </summary>
	public void CrossFadeMusic (AudioClip clip, float fadeDuration, float volumeScale = 1) {
		if (activeMusicChannel == MusicChannel.A) {
			activeMusicChannel = MusicChannel.B;
			musicSourceB.clip = clip;
			musicSourceB.Play();
		} else if (activeMusicChannel == MusicChannel.B) {
			activeMusicChannel = MusicChannel.A;
			musicSourceA.clip = clip;
			musicSourceA.Play();
		}
		StartCoroutine (AnimateCrossFadeMusic (volumeScale, fadeDuration));
	}

	/// <summary>
	/// Plays given sound effect clip.
	/// </summary>
	public void PlaySfx(AudioClip clip, Vector3 position, float volumeScale = 1) {
		if (clip != null) {
			// one source is reserved for playing all clips without a specified world position (these sounds can be played simultaneously)
			if (position == Vector3.zero) { 
				print ("play" + sfxSource.volume);
				sfxSource.PlayOneShot (clip, volumeScale);
			} else {
				float sfxDeltaTime = Time.time - timeSinceLastSfxCall;
				timeSinceLastSfxCall = Time.time;

				// update all sfx times and ensure that that sources whose clips have finished playing are pushed to front of queue
				for (int i =0; i < sfxPoolSize; i ++) {
					if (sfxTimesRemaining [i] > 0) {
						sfxTimesRemaining [i] -= sfxDeltaTime;
						if (sfxTimesRemaining [i] < 0) {
							sfxTimesRemaining [i] = 0;
							sfxPoolIndexQueue.Remove (i);
							sfxPoolIndexQueue.Insert (0, i);
						}
					}
				}

				// play clip using first source in queue. Push that source to the back of the quee
				int sourceIndex = sfxPoolIndexQueue [0];
				sfxPoolIndexQueue.RemoveAt (0);
				sfxPoolIndexQueue.Add (sourceIndex);
				sfxSourcePool [sourceIndex].transform.position = position;
				sfxSourcePool [sourceIndex].clip = clip;
				sfxSourcePool [sourceIndex].Play ();
				sfxTimesRemaining [sourceIndex] = clip.length;
			}
		}
		else if (Application.isEditor) {
			Debug.LogWarning("AudioManager was given null clip");
		}

	}

	/// <summary>
	/// Plays given sound effect clip.
	/// </summary>
	public void PlaySfx(AudioClip clip, float volumeScale = 1) {
		PlaySfx(clip, Vector3.zero, volumeScale);
	}

	/// <summary>
	/// Plays clip with specified name from SfxLibrary
	/// </summary>
	public void PlaySfx(string name, Vector3 position, float volumeScale = 1) {
		//print (sfxLibrary.GetClip (name).name);
		PlaySfx(sfxLibrary.GetClip(name), position, volumeScale);
	}
	
	/// <summary>
	/// Plays clip with specified name from SfxLibrary
	/// </summary>
	public void PlaySfx(string name, float volumeScale = 1) {
		PlaySfx(sfxLibrary.GetClip(name), Vector3.zero, volumeScale);
	
	}


	/// <summary>
	/// Changes the volume of the specified channel over specified duration
	/// </summary>
	public void SetVolume(Channel channel, float volumePercent, float duration = 0) {

		if (duration <= 0) {
			masterMixer.SetFloat (GetVolumeID (channel), PercentToDecibels (volumePercent));
		} else {
			StartCoroutine(AnimateVolumeChange(channel,PercentToDecibels(volumePercent),duration));
		}

	}

	/// <summary>
	/// Sets the channel volume and saves as the default setting
	/// This should be used when modifying audio levels from an options menu
	/// </summary>
	public void SetDefaultVolume(Channel channel, float volumePercent) {
		SetVolume (channel, volumePercent);

		// store the current volume percent so that it can be saved
		switch (channel) {
		case Channel.Master:
			masterVolumePercent = volumePercent;
			break;
		case Channel.Music:
			musicVolumePercent = volumePercent;
			break;
		case Channel.Sfx:
			sfxVolumePercent = volumePercent;
			break;
		}
	}
		

	#endregion

	#region Private methods
	/// <summary>
	/// Fades out inactive music source, while fading in active source to target volume.
	/// </summary>
	private IEnumerator AnimateCrossFadeMusic(float targetVolumeScale, float fadeDuration) {
		float percent = 0;
		float speed = 1 / fadeDuration;
		float fromA = musicSourceA.volume;
		float fromB = musicSourceB.volume;
		float targetA = (activeMusicChannel == MusicChannel.A)?targetVolumeScale:0;
		float targetB = (activeMusicChannel == MusicChannel.B)?targetVolumeScale:0;
		
		while (percent <= 1) {
			percent += Time.deltaTime * speed;
			musicSourceA.volume = Mathf.Lerp(fromA,targetA,percent);
			musicSourceB.volume = Mathf.Lerp(fromB,targetB,percent);
			yield return null;
		}
	}

	/// <summary>
	/// Smoothly adjusts the volume of the specified channel over time.
	/// </summary>
	private IEnumerator AnimateVolumeChange(Channel channel, float targetVolume, float fadeDuration) {
		float percent = 0;
		float speed = 1 / fadeDuration;
		float startVolume = 0;
		masterMixer.GetFloat(GetVolumeID(channel), out startVolume);
		
		while (percent <= 1) {
			percent += Time.deltaTime * speed;
			float currentVolume = Mathf.Lerp(startVolume,targetVolume,percent);
			masterMixer.SetFloat(GetVolumeID(channel), currentVolume);
			yield return null;
		}
	}

	/// <summary>
	/// Returns the exposed variable name for the volume of the specified channel.
	/// </summary>
	private string GetVolumeID(Channel channel) {
		switch (channel) {
		case Channel.Master:
			return "Master volume";
		case Channel.Sfx:
			return "Sfx volume";
		case Channel.Music:
			return "Music volume";
		}
		return "";
	}

	/// <summary>
	/// Given a percentage (0-1), returns the volume in dB (range -80 to 20)
	/// </summary>
	private float PercentToDecibels(float percent) {
		return Mathf.Lerp (-60, 0, Mathf.Pow(percent,.2f));
	}

	private void OnApplicationQuit() {
		// Save new default volume settings
		PlayerPrefs.SetFloat("masterVolumePercent",masterVolumePercent);
		PlayerPrefs.SetFloat("musicVolumePercent",musicVolumePercent);
		PlayerPrefs.SetFloat("sfxVolumePercent",sfxVolumePercent);
		PlayerPrefs.Save ();
	
	}

	#endregion
}
