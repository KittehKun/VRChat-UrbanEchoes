
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// This unique class handles the base logic for any of the jobs in the game. This class and other classes that inherit this class will always be triggered by the dollar sign GameObject when a player interacts with it. This class is designed to activate a container GameObject that has all of the job objects.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class JobController : UdonSharpBehaviour
{
    [Header("Player Stats Reference")]
    [Tooltip("The PlayerStats component that this script is attached to. This is used to access the player's money and experience points.")]
    [SerializeField]
    protected PlayerStats _playerStats;

    [Header("Job Container Reference")]
    [Tooltip("The job container GameObject that this script is attached to. This is used to activate the job container GameObject when the player interacts with the job GameObject.")]
    [SerializeField]
    protected GameObject _jobContainer;

    // Local player reference
    private VRCPlayerApi _localPlayer;

    [Header("Job Details")]
    [SerializeField] protected string _jobName;
    [SerializeField] protected string _jobDescription;
    [SerializeField] protected int _jobReward;
    [SerializeField] protected float _jobXPCompletion;
    [SerializeField] protected TextMeshProUGUI _jobFloatText;
    
    [Header("Job Status")]
    protected float _jobTimer; // Represents the time remaining for the job. All jobs must be completed in a certain amount of time to prevent a player from staying in a job for too long. This job timer will be reset once a player completes the job or an important objective.
    protected float _jobTimeLimit; // The maximum time allowed for the job or the objective.
    
    void Start()
    {
        _localPlayer = Networking.LocalPlayer;
    }

    protected void Update()
    {
        RotateAndFaceText();
    }

    private void RotateAndFaceText()
    {
        // Face the property price text towards the player only rotating the x-axis. | Current implementation works BUT rotation is backwards on the x-axis. TODO: Fix
        Vector3 direction = _localPlayer.GetPosition() - _jobFloatText.transform.position;
        direction.y = 0; // ignore y-axis
        Quaternion rotation = Quaternion.LookRotation(direction);
        _jobFloatText.transform.transform.rotation = rotation; // Rotate the Canvas | Text object must be a direct child of the Canvas
    }

    protected void ActivateJob()
    {
        Debug.Log($"{_jobName} has been interacted with.");
        if(_playerStats.ActiveActivity) return;

        _playerStats.ActiveActivity = true;
        _jobContainer.SetActive(true);
    }

    /// <summary>
    /// This is the entry method for the player to interact with the job. This is a built-in UdonSharp method that allows objects to be interacted with like buttons.
    /// </summary>
    public override void Interact()
    {
        ActivateJob();
    }
}
