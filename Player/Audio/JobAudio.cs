
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// This unique script handles all the job audio for the local player. Only one instance of this class should exist in the scene.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)] 
public class JobAudio : UdonSharpBehaviour
{
    [Header("Audio Source")]
    [Tooltip("The audio source for the job audio. This is used to play audio clips.")]
    [SerializeField]
    private AudioSource _audioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _jobAcceptedSFX;
    [SerializeField] private AudioClip _jobCompletedSFX;
    [SerializeField] private AudioClip _jobFailedSFX;
    [SerializeField] private AudioClip _jobRewardSFX;
    
    void Start()
    {
        if(!_audioSource) _audioSource = GetComponent<AudioSource>();
    }

    public void PlayJobAcceptedSFX()
    {
        _audioSource.PlayOneShot(_jobAcceptedSFX);
    }

    public void PlayJobCompletedSFX()
    {
        _audioSource.PlayOneShot(_jobCompletedSFX);
    }

    public void PlayJobFailedSFX()
    {
        _audioSource.PlayOneShot(_jobFailedSFX);
    }

    public void PlayJobRewardSFX()
    {
        _audioSource.PlayOneShot(_jobRewardSFX);
    }
}
