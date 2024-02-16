
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

//This is a general purpose script for playing audio on the player.
public class PlayerAudio : UdonSharpBehaviour
{
    //AudioSource Variables - Assigned in the Unity Editor
    [Header("Player Movement")]
    [SerializeField] private AudioSource playerFootstep;
    [SerializeField] private AudioClip[] footstepSounds;

	private readonly float _footstepCooldown = 0.5f;
	private float _lastFootstepTime;

	void Start()
    {
        if (!playerFootstep) Debug.LogError("Player Audio: No AudioSource assigned to playerFootstep");
    }

	public void Update()
	{
        //Move this object to the player's position
        this.transform.position = Networking.LocalPlayer.GetPosition();
        
        //Check if player is the moving
        if(Networking.LocalPlayer.GetVelocity() != Vector3.zero && Time.time >= _lastFootstepTime + _footstepCooldown)
        {
            PlayFootstep();

			// Update the last footstep time
			_lastFootstepTime = Time.time;
		}
	}

    private void PlayFootstep()
    {
        //Play a footstep sound
        playerFootstep.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
    }
}
