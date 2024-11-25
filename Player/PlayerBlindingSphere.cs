
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// This unique class moves a blinding sphere over the player with transparent shader. On PC, this script will be enabled, on other platforms, this blinding sphere will be disabled. The sphere will constantly follow the player's head disabled and only enabled on events such as interior/exterior transitions.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PlayerBlindingSphere : UdonSharpBehaviour
{
    [SerializeField] private Animator _sphereAnimator;

    private VRCPlayerApi _localPlayer;
    [SerializeField] private MeshRenderer _meshRenderer;


    private void Start()
    {
        _localPlayer = Networking.LocalPlayer;

        if (!_sphereAnimator) _sphereAnimator = GetComponent<Animator>();
        if (!_meshRenderer) _meshRenderer = GetComponent<MeshRenderer>();

        // Disable the blinding sphere by default.
        DisableBlindingSphere();
    }

    private void Update()
    {
        // Move the blinding sphere to the player's head position and rotation.
        transform.SetPositionAndRotation(_localPlayer.GetBonePosition(HumanBodyBones.Head), _localPlayer.GetBoneRotation(HumanBodyBones.Head));
    }

    public void EnableBlindingSphere()
    {
        // Enable the blinding sphere.
        _meshRenderer.enabled = true;
        Debug.Log("Blinding sphere enabled.");
    }

    public void DisableBlindingSphere()
    {
        // Disable the blinding sphere.
        _meshRenderer.enabled = false;
        Debug.Log("Blinding sphere disabled.");
    }

    public void BeginFadeInOut()
    {
        EnableBlindingSphere();
        _sphereAnimator.Play("FadeInOut");
        Debug.Log("Fade in and out started.");
    }
}