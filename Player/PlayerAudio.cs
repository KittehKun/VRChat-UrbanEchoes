
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// This unique class handles all the player's audio. Only one instance of this class should exist in the scene.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PlayerAudio : UdonSharpBehaviour
{
    [Header("Player Audio Source")]
    [SerializeField] private AudioSource _playerAudioSource;
    
    [Header("Footstep SFX")]
    [SerializeField] private AudioClip[] _footstepSFX;
    private float _lastFootstepTime = 0; //Measured in seconds. Used to determine when to play footstep sound.
    private const float FOOTSTEP_INTERVAL = 0.5f;
    
    [Header("Jump SFX")]
    [SerializeField] private AudioClip _jumpSFX;
    private bool inAir = false;
    
    void Start()
    {
        BeginPrecheck();
    }

    private void Update()
    {
        PlayFootstepSound();
        PlayJumpSound();
    }

    private void BeginPrecheck()
    {
        //Check all variables above to make sure they're not null. If they are, output an error/warning.
        if(!_playerAudioSource) Debug.LogError("[PlayerAudio.cs] PlayerAudioSource is null. Please assign a source to it.");
        if(_footstepSFX.Length == 0) Debug.LogWarning("[PlayerAudio.cs] FootstepSFX is null. Please assign footstep sounds to it.");
        if(!_jumpSFX) Debug.LogWarning("[PlayerAudio.cs] JumpSFX is null. Please assign a jump sound to it.");
    }

    private void PlayFootstepSound()
    {
        //Check if the player is on the ground, has a velocity, and has moved since the last footstep sound.
        float playerVelocity = Networking.LocalPlayer.GetVelocity().magnitude;
        bool isGrounded = Networking.LocalPlayer.IsPlayerGrounded();

        if(isGrounded && playerVelocity > 0.1f && Time.time - _lastFootstepTime > FOOTSTEP_INTERVAL)
        {
            //Play the footstep sound and update the last footstep time.
            _playerAudioSource.PlayOneShot(_footstepSFX[Random.Range(0, _footstepSFX.Length)]);
            _lastFootstepTime = Time.time;
        }
    }

    private void PlayJumpSound()
    {
        if(!inAir)
        {
            _playerAudioSource.PlayOneShot(_jumpSFX);
            inAir = true;
        }
    }

    public override void InputJump(bool jumpPressed, VRC.Udon.Common.UdonInputEventArgs args)
    {
        Debug.Log("Jump Pressed: " + jumpPressed);
        
        if(jumpPressed && !inAir)
        {
            PlayJumpSound();
        }
        else if(!jumpPressed && Networking.LocalPlayer.IsPlayerGrounded() && inAir)
        {
            inAir = false;
        }
    }
}
