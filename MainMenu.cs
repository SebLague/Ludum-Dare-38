using UnityEngine;
using System.Collections;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public Slider masterSlider;
	public Slider sfxSlider;
	public Slider musicSlider;

	public InputField nameInput;

	void Start() {
		if (masterSlider != null && sfxSlider != null && musicSlider != null && nameInput != null) {
			masterSlider.value = AudioManager.instance.masterVolumePercent;
			sfxSlider.value = AudioManager.instance.sfxVolumePercent;
			musicSlider.value = AudioManager.instance.musicVolumePercent;

			if (UserInfo.Username != "Anonymous") {
				nameInput.placeholder.GetComponent<Text> ().text = UserInfo.Username;
			}
		}
	}

	void Update() {
		if (nameInput != null) {
			if (!nameInput.isFocused) {
				nameInput.ActivateInputField ();
			
			}
		}
	}

	public void Play() {
		SceneManager.LoadScene (1);
	}

	public void BackToMainMenu() {
		SceneManager.LoadScene (0);
	}


	public void Quit() {
		Application.Quit ();
	}

	public void UpdateName(string name) {
		UserInfo.Username = name;
	}
	
	public void SetMasterVolume(float volumePercent) {
		AudioManager.instance.SetDefaultVolume (AudioManager.Channel.Master, volumePercent);
	}

	public void SetMusicVolume(float volumePercent) {
		AudioManager.instance.SetDefaultVolume (AudioManager.Channel.Music, volumePercent);
	}

	public void SetSfxVolume(float volumePercent) {
		AudioManager.instance.SetDefaultVolume (AudioManager.Channel.Sfx, volumePercent);
	}

	public void SetFullscreen(bool isFullscreen) {
		print ("Set f");
		Screen.fullScreen = isFullscreen;
	}

	public void SetResolution(int x) {
		print ("Set s");
		int y = (int)(x * 9f / 16f);
		Screen.SetResolution (x, y, Screen.fullScreen);
	}
}
