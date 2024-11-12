
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerAudio : UdonSharpBehaviour
{
    [Header("Player Audio Source")]
    [SerializeField] private AudioSource _playerAudioSource;
    
    [Header("Footstep SFX")]
    [SerializeField] private AudioClip[] _footstepSFX;
    private float _lastFootstepTime = 0; //Measured in seconds. Used to determine when to play footstep sound.
    
    [Header("Jump SFX")]
    [SerializeField] private AudioClip _jumpSFX;
    private float _lastJumpTime = 0; //Measured in seconds. Used to determine when to play jump sound.
    
    void Start()
    {
        BeginPrecheck();
    }

    private void BeginPrecheck()
    {
        //Check all variables above to make sure they're not null. If they are, output an error/warning.
        if(!_playerAudioSource) Debug.LogError("[PlayerAudio.cs] PlayerAudioSource is null. Please assign a source to it.");
        if(_footstepSFX.Length == 0) Debug.LogWarning("[PlayerAudio.cs] FootstepSFX is null. Please assign footstep sounds to it.");
        if(!_jumpSFX) Debug.LogWarning("[PlayerAudio.cs] JumpSFX is null. Please assign a jump sound to it.");
    }
}
