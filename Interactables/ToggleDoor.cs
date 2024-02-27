
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ToggleDoor : UdonSharpBehaviour
{
	[SerializeField] private bool isLocked = false; //By default, all doors are unlocked unless specified otherwise in the Unity Inspector.
    [SerializeField] private Animator doorAnimation;
    
    private bool isOpen = false;
	[SerializeField] private AudioSource doorSound;
	[SerializeField] private AudioClip unlockedSound;
	[SerializeField] private AudioClip lockedSound;

    public override void Interact()
    {		
		if(isLocked)
		{
			//Play the locked sound
			doorSound.clip = lockedSound;
			doorSound.Play();
			return;
		}

		if (!isOpen)
		{
			//Play Animation
			doorAnimation.Play("OpenDoor");

			//Set the synced variable to true
			isOpen = true;
		}
		else
		{
			//Play close animation
			doorAnimation.Play("CloseDoor");

			//Set the synced variable to false
			isOpen = false;
		}

		//Play the door sound
		doorSound.clip = unlockedSound;
		doorSound.Play();
	}

	public override void OnDeserialization()
	{
		//If the door is open, play the open animation
		if (isOpen)
		{
			doorAnimation.Play("OpenDoor");
		}
		else
		{
			doorAnimation.Play("CloseDoor");
		}

		//Play the door sound
		doorSound.Play();
	}

}
