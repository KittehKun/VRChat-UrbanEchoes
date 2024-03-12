
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class AprilEntertainmentJob : UdonSharpBehaviour
{
	//Variables - Set in Unity Inspector depending on the job
	private readonly string jobName = "Associate"; //The name of the job. Displayed onto the HUD.
	private readonly double basePay = 10.00; //The base amount of money the player will receive for completing a job wave.
	private double taskPay; //The amount of money the player will receive for completing a job task. A random between 0.1 and 1.0 is added to the base pay.
	private double bonusPay = 0; //The bonus amount the player will receive for completing a job task. This is calculated based on the wave number.
	[SerializeField] private int[] jobRequirements = new int[5]; //Set in Unity Inspector | The array is as follows: [0] = Intelligence, [1] = Fitness, [2] = Cooking, [3] = Creativity, [4] = Charisma

	//Wave Settings
	private bool timerStarted = false; //If the wave timer has started, this will be true. If the wave timer has not started, this will be false.
	private readonly float waveTimeLimit = 300; //The time limit for each the very first wave. Represented in seconds. The time limit decreases by 10% each wave.
	private float waveTimer; //The timer used to calculate the time remaining in the wave.

	[Header("Job Task Items")]
	[SerializeField] private Transform taskItems; //Set in Unity Inspector | Contains all the cassette tapes that the player must collect to complete the job task.

	[Header("Job SFX")]
	[SerializeField] private AudioSource jobSFX; //Set in Unity Inspector | The audio source that will play all job audio clips.
	[SerializeField] private AudioClip jobAcceptSFX; //Set in Unity Inspector | The sound effect that will play when the player accepts a job.
	[SerializeField] private AudioClip jobCompleteSFX; //Set in Unity Inspector | The sound effect that will play when the player completes a job wave.
	[SerializeField] private AudioClip jobFailedSFX; //Set in Unity Inspector | The sound effect that will play when the player fails a job wave and gets fired.

	[Header("Player Stats")]
	[SerializeField] private PlayerStats playerStats; //Assigned in Unity | Important Player Stats are all accessible as public properties in the PlayerStats class
	[Header("Player HUD")]
	[SerializeField] private PlayerHUD playerHUD; //Assigned in Unity | PlayerHUD is separate from this script and is used to update the player's HUD.

	private void Start()
    {
        
    }

	private void Update()
	{
		if (timerStarted)
		{
			waveTimer -= Time.deltaTime;

			playerHUD.EnableTimerText();
			playerHUD.UpdateTimer(waveTimer);

			if (waveTimer <= 0)
			{
				timerStarted = false;
				FailJob();
				//Fire the player
			}
		}
	}

	//Job Methods
	/// <summary>
	/// Accepts the job if the player meets the job requirements and is not already on a job.
	/// </summary>
	private void AcceptJob()
	{
		bool requirementsMet = true;

		//Check if the player meets the job requirements
		for (int i = 0; i < jobRequirements.Length; i++)
		{
			if (playerStats.PlayerSkills[i] < jobRequirements[i])
			{
				requirementsMet = false;
				break;
			}
		}

		//If the player meets the job requirements, start the job
		if (requirementsMet && !playerStats.OnJob)
		{
			//Begin the job
			BeginJob();

			//Play the job accept sound effect
			PlayJobAccept();
		}
	}

	/// <summary>
	/// Begins the job and sets the player's on job status to true.
	/// </summary>
	private void BeginJob()
	{
		if (playerStats.OnJob) return;

		//Disable the job pickup
		transform.GetComponent<MeshRenderer>().enabled = false;
		transform.GetComponent<BoxCollider>().enabled = false;

		//Set the wave timer to the time limit
		waveTimer = waveTimeLimit;

		//Set the timer started to true
		timerStarted = true;

		//Set the player's on job status to true
		playerStats.OnJob = true;

		//Update the player's HUD
		playerHUD.UpdateJobTitle(jobName, 1);
	}

	/// <summary>
	/// Fails the job. Resets the job and displays the job failed notification.
	/// </summary>
	private void FailJob()
	{
		playerHUD.DisplayJobFailedNotification();
		ResetJob();
	}

	/// <summary>
	/// Resets the job and re-enables the job pickup.
	/// </summary>
	private void ResetJob()
	{
		PlayJobFailed();
		timerStarted = false;
		waveTimer = waveTimeLimit;

		//Reenable the job pickup
		transform.GetComponent<MeshRenderer>().enabled = true;
		transform.GetComponent<BoxCollider>().enabled = true;
	}

	public void CompleteTask()
	{
		//Play the job complete sound effect
		PlayJobComplete();

		//Calculate the task pay
		taskPay = basePay + Random.Range(0.1f, 1.0f);

		//Calculate the bonus pay
		bonusPay = basePay * 0.1;

		//Calculate the total pay
		double totalPay = taskPay + bonusPay;

		//Add the task pay and bonus pay to the player's money
		playerStats.AddMoney(taskPay + bonusPay);
		playerHUD.UpdateMoney();
		playerHUD.UpdateMoneyToAdd(totalPay);

		//Decrease the player's energy
		playerStats.DecreaseEnergy();

		//Play Job Complete SFX
		PlayJobComplete();

		//Reset the job
		ResetJob();
	}

	//Audio Methods
	/// <summary>
	/// Plays the job accept sound effect.
	/// </summary>
	private void PlayJobAccept()
	{
		jobSFX.clip = jobAcceptSFX;
		jobSFX.Play();
	}

	/// <summary>
	/// Plays the job complete sound effect.
	/// </summary>
	private void PlayJobComplete()
	{
		jobSFX.clip = jobCompleteSFX;
		jobSFX.Play();
	}

	/// <summary>
	/// Plays the job failed sound effect.
	/// </summary>
	private void PlayJobFailed()
	{
		jobSFX.clip = jobFailedSFX;
		jobSFX.Play();
	}

	/// <summary>
	/// Begins the process on accepting the job. This method should be used as an entry point to this script.
	/// </summary>
	public override void Interact()
	{
		AcceptJob();
	}
}
