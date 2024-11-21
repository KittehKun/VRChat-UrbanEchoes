
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
[RequireComponent(typeof(AudioSource))]
/// <summary>
/// This unique script plays all the audio for the local player in retaining to their owned properties.
/// </summary>
public class PropertyAudio : UdonSharpBehaviour
{
    [Header("Target Audio Source")]
    [Tooltip("The audio source that will play all of the property audio.")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Audio Clips")]
    [SerializeField]
    [Tooltip("SFX for when the player purchases a property")]
    private AudioClip _purchasePropertySFX;

    [SerializeField]
    [Tooltip("SFX for when the player sells a property")]
    private AudioClip _sellPropertySFX;

    [SerializeField]
    [Tooltip("SFX for when the player upgrades a property")]
    private AudioClip _upgradePropertySFX;

    [SerializeField]
    [Tooltip("SFX for when the player fails to purchase a property")]
    private AudioClip _errorSFX;
    
    void Start()
    {
        
    }

    public void PlayPurchasePropertySFX()
    {
        _audioSource.PlayOneShot(_purchasePropertySFX);
    }

    public void PlaySellPropertySFX()
    {
        _audioSource.PlayOneShot(_sellPropertySFX);
    }

    public void PlayUpgradePropertySFX()
    {
        _audioSource.PlayOneShot(_upgradePropertySFX);
    }

    public void PlayErrorSFX()
    {
        _audioSource.PlayOneShot(_errorSFX);
    }
}
