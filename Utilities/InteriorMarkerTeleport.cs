
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// This utility class inherits the TeleportPlayer class and is used to teleport the player to a target location upon entering an interior marker's collider.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class InteriorMarkerTeleport : TeleportPlayer
{
    [Tooltip("This is the marker that will be disabled for a few seconds after teleporting into an interior/exterior space to avoid teleporting back into the interior/exterior space.")]
    [SerializeField] private Transform _markerToDisable;

    [SerializeField] private bool _blindPlayer = false; // By default, this value will be false unless specified in the inspector.
    [SerializeField] private PlayerBlindingSphere _playerBlindingSphere;

    [SerializeField] private PlayerAudio _playerAudio;

    private const float TELEPORT_TIMER = 1f; // The amount of time it takes before a teleport is initiated.
    private const float MARKER_COOLDOWN_TIMER = 1.5f; // The amount of time it takes before the marker can teleport again.
    private float _teleportTimer = 0.0f;
    private float _markerCooldownTimer = 0.0f;
    private bool _isTeleporting = false;
    private bool _hasTeleported = false;
    private bool _markerCooldown = false; // Used to prevent the marker from teleporting back into the interior/exterior space. Begins a countdown when this bool is activated.

    public override void Start()
    {
        base.Start(); // Assigns the local player from the base class.
        if (_markerToDisable == null) Debug.LogWarning("[InteriorMarkerTeleport.cs] No marker was set to disable. Please assign a marker to disable. If the marker was intentionally unassigned, this warning can be safely ignored.");
        if (_blindPlayer == true && !_playerBlindingSphere) Debug.LogError("[InteriorMarkerTeleport.cs] The player blinding tag is set to true, but no blinding sphere was found. Please assign a blinding sphere to the _playerBlindingSphere field.");
    }

    private void Update()
    {
        TeleportToTargetLocation();
        MarkerCooldownTimer();
    }

    private void TeleportToTargetLocation()
    {
        if (_isTeleporting)
        {
            _teleportTimer += Time.deltaTime;
            if (_teleportTimer >= TELEPORT_TIMER && !_hasTeleported)
            {
                Teleport();
                _hasTeleported = true;
            }
            if (_teleportTimer >= TELEPORT_TIMER / 2f && _teleportTimer < TELEPORT_TIMER && _blindPlayer)
            {
                Debug.Log("Begin fade in and out condition met from InteriorMarkerTeleport.cs");
                _playerBlindingSphere.BeginFadeInOut();
            }
        }
    }

    private void MarkerCooldownTimer()
    {
        if (_markerCooldown)
        {
            _markerCooldownTimer += Time.deltaTime;
            if (_markerCooldownTimer >= MARKER_COOLDOWN_TIMER)
            {
                _markerCooldown = false;
                _markerToDisable.gameObject.SetActive(true);
            }
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        Debug.Log($"{player.displayName} has entered an interior marker collider. Teleporting user to {_targetLocation}");

        _isTeleporting = true;
        _teleportTimer = 0.0f;
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        Debug.Log($"{player.displayName} has exited an interior marker collider. Cancelling teleport.");

        _isTeleporting = false;
        _hasTeleported = false;
        _teleportTimer = 0.0f;
    }

    public override void Teleport()
    {
        base.Teleport(); // Teleports the player to the target location.
        _markerToDisable.gameObject.SetActive(false); // Disables the interior/exterior marker and starts the cooldown timer.
        _markerCooldown = true;
        _markerCooldownTimer = 0.0f;

        _playerAudio.PlayBuildingTransitionSound();
    }

}
