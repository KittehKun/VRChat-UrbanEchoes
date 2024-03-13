
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

	//Job & Wave Settings
	private bool timerStarted = false; //If the wave timer has started, this will be true. If the wave timer has not started, this will be false.
	private readonly float waveTimeLimit = 180; //The time limit for each the very first wave. Represented in seconds.
	private float waveTimer; //The timer used to calculate the time remaining in the wave.
	private int taskCount = 0; //The number of tasks the player has completed in the current wave/job.

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

		//Generate the task amount and enable the task items based on task amount
		GenerateTasks();
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

	/// <summary>
	/// Generates a random amount of tasks for the player to complete. The GameObjects are then enabled randomly based on the task amount.
	/// </summary>
	private void GenerateTasks()
	{
		//Generate the task amount based on the taskItems children count. Then enable a random amount of children based on the task amount.
		int totalChildren = taskItems.childCount;
		int numToActivate = Random.Range(0, totalChildren + 1); // Random number between 0 and totalChildren (inclusive)

		// Activate a random subset of children
		for (int i = 0; i < numToActivate; i++)
		{
			int randomChildIndex = Random.Range(0, totalChildren);
			taskItems.GetChild(randomChildIndex).gameObject.SetActive(true);
		}

		taskCount = numToActivate;
	}

	/// <summary>
	/// Completes the job. Adds the task pay and bonus pay to the player's money and decreases the player's energy. Called by SendCustomEvent on the job task item and thus should not have a reference.
	/// </summary>
	public void CompleteTask()
	{
		taskCount--; //Decrease the task count
		if (taskCount != 0) return;

		//Play the job complete sound effect
		PlayJobComplete();

		//Add the task pay and bonus pay to the player's money
		taskPay = basePay + Random.Range(0.1f, 1.0f);
		bonusPay = basePay * (1 * 0.1) + Random.Range(0.1f, 1f);
		double total = taskPay + bonusPay;
		playerStats.AddMoney(total);

		//Decrease the player's energy
		playerStats.DecreaseEnergy();

		//Update the player's HUD
		playerHUD.UpdateMoneyToAdd(total);

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
