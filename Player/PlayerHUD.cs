
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PlayerHUD : UdonSharpBehaviour
{
    [Header("Player Stats Script")]
    [SerializeField] private PlayerStats playerStats; //Assigned in Unity | Important Player Stats are all accessible as public properties in the PlayerStats class

    [Header("Player Stats")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI moneyToAddText;

    [Header("Job Specific Text")]
    [SerializeField] private TextMeshProUGUI jobAcceptNotification; //Displays when a player accepts a job and meets the requirements.
    [SerializeField] private TextMeshProUGUI jobDeclinedNotification; //Displays when a player fails to meet the requirements of a job.
    [SerializeField] private TextMeshProUGUI jobFailedNotification; //Displays when a player fails to complete a job wave in time.
    [SerializeField] private TextMeshProUGUI jobBonusNotification; //This is used to display the bonus amount the player received for completing a job wave.
    [SerializeField] private TextMeshProUGUI jobTimerText; //Displays the time remaining in the current job wave.
    [SerializeField] private TextMeshProUGUI jobTaskText; //Displays the current job wave number.
    [SerializeField] private TextMeshProUGUI jobTitleText; //Displays the name of the job the player is currently on.
    [SerializeField] private Transform objectiveDisplay;

    [Header("HUD Canvas")]
    [SerializeField] private Transform hudCanvas;
    [SerializeField] private Image healthSlider; //Represents the health bar foreground image - Used for as the health bar slider
    [SerializeField] private Image energySlider; //Represents the energy bar foreground image - Used for as the energy bar slider


    private void Start()
    {
        if (!playerStats) playerStats = GetComponent<PlayerStats>(); //If the playerStats field is not assigned in Unity, script should be on the same GameObject as the PlayerStats component
        objectiveDisplay.gameObject.SetActive(false);
        moneyToAddText.gameObject.SetActive(false);
    }

    private void Update()
    {
        MoveHUD();
	}

    /// <summary>
    /// Updates all the HUD elements with the player's current stats. This method should be called whenever the player's stats change.
    /// </summary>
    public void UpdateHUD()
    {
        //healthText.text = $"H: {playerStats.PlayerHealth}";
        //energyText.text = $"E: {playerStats.PlayerEnergy:F0}"; //Displays with no decimal places
        moneyText.text = $"${playerStats.PlayerMoney:F2}"; //Displays with 2 decimal 

        //Normalizes the player's health to a value between 0 and 1
        float fillAmount = playerStats.PlayerHealth / playerStats.MaxHealth;
        healthSlider.fillAmount = fillAmount; //Updates the health bar slider fill amount

        //Normalizes the player's energy to a value between 0 and 1
        fillAmount = playerStats.PlayerEnergy / playerStats.MaxEnergy;
        energySlider.fillAmount = fillAmount; //Updates the energy bar slider fill amount
    }

    /// <summary>
    /// Updates only the money HUD element with the player's current money. This method should be called whenever the player's money changes.
    /// </summary>
    public void UpdateMoney()
    {
        moneyText.text = $"${playerStats.PlayerMoney:F2}"; //Displays with 2 decimal places
    }

    public void UpdateMoneyToAdd(double amount)
    {
        moneyToAddText.gameObject.SetActive(true);
		moneyToAddText.text = $"+${amount:F2}";
		SendCustomEventDelayedSeconds("ResetMoneyToAdd", 2);
	}

    public void ResetMoneyToAdd()
    {
        moneyToAddText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the energy HUD elements with the player's current stats. This method should be called on tick.
    /// </summary>
    public void UpdateTick()
    {
        float fillAmount = playerStats.PlayerEnergy / playerStats.MaxEnergy;
        energySlider.fillAmount = fillAmount; //Updates the energy bar slider fill amount
    }

    /// <summary>
    /// Updates the job task HUD element with the current job title.
    /// </summary>
    /// <param name="jobTitle"></param>
    public void UpdateJobTitle(string jobTitle, int jobWave)
    {
        jobTitleText.text = $"{jobTitle} - Lvl {jobWave}";
    }

    /// <summary>
    /// Updates the job title HUD element with the current job title.
    /// </summary>
    /// <param name="jobTitle"></param>
    public void UpdateJobTitle(string jobTitle)
    {
        jobTitleText.text = jobTitle;
    }
    
    /// <summary>
    /// Updates the current task count and the maximum task count. This method should be called whenever the player completes a wave or begins a job.
    /// </summary>
    /// <param name="currentTaskCount"></param>
    /// <param name="maxTaskCount"></param>
    public void UpdateJobTaskCount(int currentTaskCount, int maxTaskCount)
    {
        jobTaskText.text = $"Job Tasks: {currentTaskCount}/{maxTaskCount}";
    }

    public void UpdateJobTaskCount(int taskCount)
    {
        jobTaskText.text = $"Job Tasks: {taskCount}";
    }

    /// <summary>
    /// Updates the job timer HUD element with the time remaining in the current job wave.
    /// </summary>
    /// <param name="time"></param>
    public void UpdateTimer(float time)
    {
        jobTimerText.text = $"Time Remaining: {time:F0}";
    }

    /// <summary>
    /// Moves the HUD to a position in front of the player's head and matches the rotation to the head rotation.
    /// </summary>
    private void MoveHUD()
    {
		// Get the player's head tracking data
		VRCPlayerApi.TrackingData headTrackingData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

		// Calculate the desired position (e.g., 1 meter in front of the head)
		Vector3 desiredPosition = headTrackingData.position + headTrackingData.rotation * Vector3.forward * 1f;

		// Smoothly move the HUD towards the desired position
		// Match the HUD rotation to the head rotation
		hudCanvas.SetPositionAndRotation(Vector3.Lerp(hudCanvas.position, desiredPosition, Time.deltaTime * 5f), headTrackingData.rotation);
	}

    public void EnableTimerText()
    {
        jobTimerText.gameObject.SetActive(true);
    }

    public void DisableTimerText()
    {
        jobTimerText.gameObject.SetActive(false);
    }

	public void EnableTaskText()
	{
        jobTaskText.gameObject.SetActive(true);
	}

    public void DisableTaskText()
    {
		jobTaskText.gameObject.SetActive(false);
	}

	public void DisplayJobAcceptNotification()
    {
		//Display the job accept notification text for 5 seconds
        jobAcceptNotification.gameObject.SetActive(true);
        SendCustomEventDelayedSeconds("DisableJobAcceptNotification", 5);

        //Enable the objective display
        objectiveDisplay.gameObject.SetActive(true);
	}

    public void DisplayJobDeclinedNotification()
    {
        if (playerStats.OnJob) return; //If the player is already on a job, don't display the declined notification

        //Display the job declined notification text for 5 seconds
		jobDeclinedNotification.gameObject.SetActive(true);
        SendCustomEventDelayedSeconds("DisableJobDeclinedNotification", 5);
	}

    public void DisplayJobFailedNotification()
    {
        jobFailedNotification.gameObject.SetActive(true);
        jobTaskText.gameObject.SetActive(false);
        jobTitleText.text = "Unemployed";
        SendCustomEventDelayedSeconds("DisableJobFailedNotification", 5);

        //Disable the objective display
        objectiveDisplay.gameObject.SetActive(false);
    }

    public void DisableJobFailedNotification()
    {
		jobFailedNotification.gameObject.SetActive(false);
	}
    
    public void DisableBonusNotification()
    {
        jobBonusNotification.gameObject.SetActive(false);
    }

    public void DisableJobDeclinedNotification()
    {
        jobDeclinedNotification.gameObject.SetActive(false);
    }

    public void DisableJobAcceptNotification()
    {
        jobAcceptNotification.gameObject.SetActive(false);
    }

    public void DisableObjectiveGUI()
    {
        objectiveDisplay.gameObject.SetActive(false);
    }

    public void EnableObjectiveGUI()
    {
        objectiveDisplay.gameObject.SetActive(true);
    }

    /// <summary>
    /// Marks the energy text as red for a short duration to indicate the player has lost energy.
    /// </summary>
    public void DecreaseEnergy()
    {
        energySlider.color = Color.red;
		SendCustomEventDelayedSeconds("ResetEnergyColor", 0.5f);
    }

    /// <summary>
    /// Resets the energy text color to white. Called from Udon Behaviour event.
    /// </summary>
    public void ResetEnergyColor()
    {
        energySlider.color = Color.yellow;
    }
}
