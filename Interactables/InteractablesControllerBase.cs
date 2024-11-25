
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// This script will be the base script for any interactable item that plays audio, animations, effects, etc.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class InteractablesControllerBase : UdonSharpBehaviour
{
    [Header("Flags")]
    [Tooltip("Flag to check if the interactable has an animation. If not, variables can be set to null.")]
    public bool _hasAnimation = true; // Flag to check if the interactable has an animation. If not, variables can be set to null.
    [Tooltip("Flag to check if the interactable has an audio clip. If not, variables can be set to null.")]
    public bool _hasAudio = true; // Flag to check if the interactable has an audio clip. If not, variables can be set to null.

    [Header("Animator & Audio")]
    [SerializeField]
    [Tooltip("The animator that will play the animation.")]
    private Animator _interactableAnimator; // The animator that will play the animation.

    [SerializeField]
    [Tooltip("The audio source that will play the audio clip.")]
    private AudioSource _interactableAudioSource; // The audio source that will play the audio clip.
    [SerializeField]
    [Tooltip("The audio clip that will be played.")]
    private AudioClip _interactableAudioClip; // The audio clip that will be played.

    /// <summary>
    /// Plays the interact animation on the animator. The animation must be named "Interact" otherwise the animation will not be played.
    /// </summary>
    private void PlayAnimation()
    {
        _interactableAnimator.Play("Interact");
    }

    /// <summary>
    /// Plays the audio clip assigned to the interactable.
    /// </summary>
    private void PlayInteractableAudio()
    {
        _interactableAudioSource.clip = _interactableAudioClip;
        _interactableAudioSource.Play();
    }

    /// <summary>
    /// This function is called whenever the script is validated in the Unity editor, i.e. when the user changes a value in the inspector.
    /// It is used to check if the required components are assigned in the inspector.
    /// </summary>
    private void OnValidate()
    {
        if (_hasAnimation && !_interactableAnimator) Debug.LogError("[InteractablesControllerBase.cs] InteractableAnimator is null. Please assign an animator to it.");
        if (_hasAudio && !_interactableAudioSource) Debug.LogError("[InteractablesControllerBase.cs] InteractableAudioSource is null. Please assign an audio source to it.");
        if (_hasAudio && !_interactableAudioClip) Debug.LogError("[InteractablesControllerBase.cs] InteractableAudioClip is null. Please assign an audio clip to it.");
    }

    /// <summary>
    /// This is the public method that gets called by the script which sends the interact event locally. This event is triggered for all players when a player interacts with the object.
    /// </summary>
    public void ActivateInteractable()
    {
        PlayAnimation();
        PlayInteractableAudio();
    }

    /// <summary>
    /// Overrides the UdonSharp method to handle player interaction with the object.
    /// Sends a network event to activate the interactable for all players.
    /// </summary>
    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ActivateInteractable");
    }
}
