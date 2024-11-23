
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// This utility class inherits the TeleportPlayer class and is used to teleport the player to a target location upon entering an interior marker's collider.
/// </summary>
public class InteriorMarkerTeleport : TeleportPlayer
{
    private const float TELEPORT_TIMER = 1f; // The amount of time it takes before a teleport is initiated.
    private float _teleportTimer = 0.0f;
    private bool _isTeleporting = false;
    
    void Start()
    {
        
    }

    private void Update()
    {
        if (_isTeleporting)
        {
            _teleportTimer += Time.deltaTime;
            if (_teleportTimer >= TELEPORT_TIMER)
            {
                Teleport();
            }
        }
    }

    public override void OnPlayerCollisionEnter(VRCPlayerApi player)
    {
        Debug.Log($"{player.displayName} has entered an interior marker collider. Teleporting user to {_targetLocation}");

        _isTeleporting = true;
        _teleportTimer = 0.0f;
    }

    public override void OnPlayerCollisionExit(VRCPlayerApi player)
    {
        Debug.Log($"{player.displayName} has exited an interior marker collider. Cancelling teleport.");

        _isTeleporting = false;
        _teleportTimer = 0.0f;
    }
}
