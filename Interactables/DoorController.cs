
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// This utility class is used for opening and closing a door. This class is intended to be fully synced with other clients.
/// /// </summary>
public class DoorController : UdonSharpBehaviour
{
    [Header("Door Animation")]
    [SerializeField] private Animator _doorAnimator; // The door animator.

    [Header("Door Audio")]
    private AudioSource _doorAudioSource; // The door audio source.
    [SerializeField] private AudioClip _doorAudioOpenSFX; // The door audio clip.
    [SerializeField] private AudioClip _doorAudioCloseSFX; // The door audio clip.

    [UdonSynced] private bool _isOpen = false; // The door open state.

    /// <summary>
    /// Called on the frame when a script is enabled just before
    /// this script's Update method is called the first time.
    /// Used to get the door audio source and animator components if they are not assigned in the inspector.
    /// </summary>
    void Start()
    {
        if (!_doorAudioSource) _doorAudioSource = GetComponent<AudioSource>();
        if (!_doorAnimator) _doorAnimator = GetComponent<Animator>();
        if (!_doorAudioOpenSFX) Debug.LogError("DoorController: No door audio SFX found. Please assign a door audio SFX to the _doorAudioSFX field.");
    }

    /// <summary>
    /// Handles player interaction with the door. Toggles the door's open state and updates the owner of the door object to the local player.
    /// </summary>
    public override void Interact()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        _isOpen = !_isOpen;
        PoopAndScoop();
    }

    /// <summary>
    /// Called after the object's state has been deserialized from another player's update.
    /// This will be called after the object has been deserialized on all players, including the owner.
    /// This function is used to animate the door when it is opened or closed by another player.
    /// </summary>
    public override void OnDeserialization()
    {
        PoopAndScoop();
    }

    /// <summary>
    /// Plays the door audio SFX and sets the door animator state to the opposite of the current state.
    /// </summary>
    private void PoopAndScoop()
    {
        if (_isOpen)
        {
            PlayOpenDoorAudioSFX();
            _doorAnimator.Play("OpenDoor");
            return;
        }

        PlayOpenDoorAudioSFX();
        _doorAnimator.Play("CloseDoor");
    }

    /// <summary>
    /// Plays the door audio SFX by setting the clip of the door audio source to the door audio SFX and calling Play() on the door audio source.
    /// </summary>
    private void PlayOpenDoorAudioSFX()
    {
        _doorAudioSource.clip = _doorAudioOpenSFX;
        _doorAudioSource.Play();
    }

    /// <summary>
    /// Plays the door audio SFX by setting the clip of the door audio source to the door audio SFX and calling Play() on the door audio source.
    /// </summary>
    private void PlayOpenDoorAudioSFXClose()
    {
        _doorAudioSource.clip = _doorAudioCloseSFX;
        _doorAudioSource.Play();
    }
}
