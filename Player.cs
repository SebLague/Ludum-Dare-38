using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Destructable {

	public event System.Action MetalEaten;
	public const float forwardSpeed = 10;

	public float maxSpeed = 10;
	public float edgeMaxSpeed = 5;
	public float moveSmoothTime = .2f;
	public float rotSmoothTime = .2f;

	public float rollMax = 35;
	public float pitchMax = 35;
	public float yawMax = 10;

	const float constraintCircleBuffer = 1;
	const float boundsRadius = GameConstants.gameRadius - Player.constraintCircleBuffer;
	const float slowDownBufferRadius = 10;
	const float slowDownRadius = boundsRadius - slowDownBufferRadius;

	public Animator animator;
	SfxLibrary lib;
	Vector3 velocity;

	float pitch;
	float yaw;
	float roll;

	Vector3 smoothMoveV;
	float smoothPitchV;
	float smoothYawV;
	float smoothRollV;

	public float bobHeight = .5f;
	public float bobSpeed = 1;
	float yBobOld;
	float yBob;
	float currentMaxSpeed;
	World worldObj;
	bool disabled;
	public GameObject graphic;

	public override void Start ()
	{
		//base.Start ();
		myRadius = 1.4f;
		worldObj = FindObjectOfType<World> ();
		GameOver.instance.OnGameOver += OnGameOver;
		lib = FindObjectOfType<SfxLibrary> ();
	}

	public override void OnHit ()
	{
		base.OnHit ();
		if (!disabled) {
			worldObj.OnHit ();
			GameOver.EndGame ();
			graphic.SetActive (false);
			GetComponent<BoxCollider> ().enabled = false;
		}
		AudioSource.PlayClipAtPoint(lib.GetClip("Explosion"),Camera.main.transform.position,AudioManager.instance.sfxVolumePercent * AudioManager.instance.masterVolumePercent * .2f);
	}

	void OnGameOver() {
		if (!disabled) {
			disabled = true;
			AudioManager.instance.PlaySfx ("Explosion",Camera.main.transform.position);
		}
	}


	void Update() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (MetalEaten != null) {
				MetalEaten ();
			}
			//AudioSource.PlayClipAtPoint(lib.GetClip("Beep"),Camera.main.transform.position,AudioManager.instance.DefaultSfxVolume * AudioManager.instance.DefaultMasterVolume);
		}
	
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		if (disabled) {
			input = Vector2.zero;
		}

		float inputMagnitude = input.magnitude;

		Vector3 targetVelocity = new Vector3 (input.x, input.y).normalized * currentMaxSpeed;

		velocity = Vector3.SmoothDamp (velocity, targetVelocity, ref smoothMoveV, moveSmoothTime);
		if (velocity.magnitude > currentMaxSpeed) {
			velocity = velocity.normalized * currentMaxSpeed;
		}

		float targetRoll = -rollMax * input.x;
		float targetYaw = yawMax * input.x;
		float targetPitch = -pitchMax * input.y;

		pitch = Mathf.SmoothDampAngle (pitch, targetPitch, ref smoothPitchV, rotSmoothTime);
		yaw = Mathf.SmoothDampAngle (yaw, targetYaw, ref smoothYawV, rotSmoothTime);
		roll = Mathf.SmoothDampAngle (roll, targetRoll, ref smoothRollV, rotSmoothTime);

		transform.eulerAngles = new Vector3 (pitch, yaw, roll);

		// bob
		yBob = Mathf.Sin (Time.time * bobSpeed) * bobHeight;
		float deltaYBob = yBob - yBobOld;
		yBobOld = yBob;

		// calc target position
		Vector3 targetPos = transform.position + Vector3.up * deltaYBob + (velocity + Vector3.forward * forwardSpeed) * Time.deltaTime;

		// constrain pos to cylinder
		Vector2 targetPos2D = new Vector2 (targetPos.x, targetPos.y);
		float sqrDstFromCircleCentre = targetPos2D.sqrMagnitude;

	
		if (sqrDstFromCircleCentre > slowDownRadius * slowDownRadius) {
			float dstFromCentre = targetPos2D.magnitude;
			float slowDownPercent = 1 - (boundsRadius - dstFromCentre) / slowDownBufferRadius;
			currentMaxSpeed = Mathf.Lerp (maxSpeed, edgeMaxSpeed, slowDownPercent);
			//print (slowDownPercent);
		} else {
			currentMaxSpeed = maxSpeed;
		}

		if (sqrDstFromCircleCentre >boundsRadius*boundsRadius) {
			Vector2 offset2D = targetPos2D.normalized * boundsRadius;
			targetPos = new Vector3 (offset2D.x, offset2D.y, targetPos.z);
		}

		transform.position = targetPos;

		if (!disabled) {
			animator.SetFloat ("Turn", Mathf.Sign (input.x) * Mathf.Abs (input.x), .2f, Time.deltaTime);
		}

	}

	void OnTriggerEnter(Collider hitC) {
		if (hitC.tag == "Metal") {
			if (!disabled) {
				Destroy (hitC.gameObject);
				AudioSource.PlayClipAtPoint (lib.GetClip ("Beep"), Camera.main.transform.position, AudioManager.instance.sfxVolumePercent * AudioManager.instance.masterVolumePercent);
				if (MetalEaten != null) {
					MetalEaten ();
				}
			}
		}
	}

	void OnDestroy() {
		if (GameOver.instance != null) {
			GameOver.instance.OnGameOver -= OnGameOver;
		}
	}

}
