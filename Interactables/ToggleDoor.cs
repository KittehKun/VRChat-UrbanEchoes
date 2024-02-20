
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleDoor : UdonSharpBehaviour
{
    [SerializeField] private Animator doorAnimation;
    
    [UdonSynced] private bool isOpen = false;
	[SerializeField] private AudioSource doorSound;

    public override void Interact()
    {
		Networking.SetOwner(Networking.LocalPlayer, gameObject); //Set the owner of the object to the local player
		
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
