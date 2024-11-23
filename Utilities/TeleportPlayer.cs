
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class TeleportPlayer : UdonSharpBehaviour
{
    [Tooltip("This is the target location the player will teleport to upon interacting with a teleport object such as a door or a forced teleport. Ideally, this should be an empty Transform object with the x-position the forward direction of where the player should face after teleporting.")]
    [SerializeField] protected Transform _targetLocation;

    protected VRCPlayerApi _localPlayer;
    
    public virtual void Start()
    {
        _localPlayer = Networking.LocalPlayer;
    }

    public virtual void Teleport()
    {
        _localPlayer.TeleportTo(_targetLocation.position, _targetLocation.rotation);
    }
}
