
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

//This is a general purpose script for playing audio on the player.
public class PlayerAudio : UdonSharpBehaviour
{
	//Global Player AudioSource
    [SerializeField] private AudioSource PLAYER_AUDIO;

	//AudioSource Variables - Assigned in the Unity Editor
	[Header("Player Movement")]
    [SerializeField] private AudioClip[] footstepSounds; //Pitch and speed are dictated by player's movement

    [Header("Player Level Up")]
    [SerializeField] private AudioClip levelUpSFX;

    [Header("Player Money Sound")]
    [SerializeField] private AudioClip moneySound;

	private float _footstepCooldown;
	private float _lastFootstepTime;

	void Start()
    {
        if (!PLAYER_AUDIO) Debug.LogError("Player Audio: No AudioSource assigned to playerFootstep");
    }

	public void Update()
	{
        //Calculate the pitch and speed of the footstep sound based on the player's velocity
        Vector3 playerVelocity = Networking.LocalPlayer.GetVelocity();
        
        //Check if the player is moving at a runnning speed (6u/s)
        if (playerVelocity.magnitude >= 6)
		{
			PLAYER_AUDIO.pitch = playerVelocity.magnitude / 6;
            _footstepCooldown = 1 / playerVelocity.magnitude + 0.1f; //Offset calculation by 0.1 to prevent footstep spam
		}
		else
		{
			PLAYER_AUDIO.pitch = 1;
            _footstepCooldown = 0.5f;
		}

        //Move this object to the player's position
        this.transform.position = Networking.LocalPlayer.GetPosition();
        
        //Check if player is the moving
        if(playerVelocity != Vector3.zero && Time.time >= _lastFootstepTime + _footstepCooldown)
        {
            PlayFootstep();

			// Update the last footstep time
			_lastFootstepTime = Time.time;
		}
	}

    private void PlayFootstep()
    {
		//Play a footstep sound
		PLAYER_AUDIO.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
    }

    public void PlayMoneySound()
    {
        //Play the money sound
		PLAYER_AUDIO.PlayOneShot(moneySound);
    }
}
