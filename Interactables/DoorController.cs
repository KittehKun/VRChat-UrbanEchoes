
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class DoorController : UdonSharpBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private bool isOpen = false; //Serialized field for showing the value in inspector | Should not be changed
    [SerializeField] private bool isLocked = false; //Used for checking if the door should be locked | Assigned in Inspector
    [SerializeField] private bool opensLeft = false; //Used for checking if the door opens to the left - Determines which way the door will pivot rotate | Assigned in Inspector
    [SerializeField] private float doorOpenAngle = 90.0f; //Used for setting the door open angle | Assigned in Inspector
    [SerializeField] private float doorOpenSpeed = 2.0f; //Used for setting the door open speed | Assigned in Inspector

    [Header("Door SFX")]
    [SerializeField] private AudioSource doorAudioSource; //Used for playing the door audio | Assigned in Inspector
    [SerializeField] private AudioClip doorSFX; //Used for playing the door open audio | Assigned in Inspector

    private Quaternion initialDoorRotation; //Used for storing the initial door rotation
    private Quaternion targetDoorRotation; //Used for storing the target door rotation

	private void Start()
    {
        if(!doorAudioSource) doorAudioSource = transform.GetComponent<AudioSource>(); Debug.LogWarning("WARNING: Audio Source was not assigned in Inspector. Please be sure to assign the AudioSource before publishing."); //If the doorAudioSource is not assigned in the inspector, get the AudioSource component from the door

		initialDoorRotation = transform.localRotation; //Set the initial door rotation to the current door rotation
		targetDoorRotation = Quaternion.Euler(0, doorOpenAngle, 0) * initialDoorRotation; //Set the target door rotation to the door open angle if the door is open, otherwise set it to 0
	}

	private void Update()
	{
        if(isOpen) //If the door is open
        {
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetDoorRotation, doorOpenSpeed * Time.deltaTime); //Lerp the door rotation to the target door rotation
		}
		else //If the door is closed
        {
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, initialDoorRotation, doorOpenSpeed * Time.deltaTime); //Lerp the door rotation to the initial door rotation
		}
	}

	private void ToggleDoor()
    {
		isOpen = !isOpen; //Toggle the door open state
	}

	public override void Interact()
    {
		if (isLocked) return;
		ToggleDoor(); //Toggle the door open state
		doorAudioSource.clip = doorSFX; //Set the door audio source clip to the door SFX
		doorAudioSource.Play(); //Play the door audio source
	}

}
