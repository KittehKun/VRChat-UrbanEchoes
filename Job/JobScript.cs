
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class JobScript : UdonSharpBehaviour
{

	//Variables - Set in Unity Inspector depending on the job
	[SerializeField] private string _jobName; //The name of the job. Displayed onto the HUD.
	[SerializeField] private double _basePay; //The base amount of money the player will receive for completing a job wave.
	private double _taskPay; //The amount of money the player will receive for completing a job task. A random between 0.1 and 1.0 is added to the base pay.
	private double _bonusPay = 0; //The bonus amount the player will receive for completing a job task. This is calculated based on the wave number.
	private int _jobWave; //Maximum of 5 waves per difficulty level. Player's receive a bonus each wave they complete.
	[SerializeField] private int[] _jobRequirements = new int[5]; //Set in Unity Inspector | The array is as follows: [0] = Intelligence, [1] = Fitness, [2] = Cooking, [3] = Creativity, [4] = Charisma

	//Wave Settings
	private bool _timerStarted = false; //If the wave timer has started, this will be true. If the wave timer has not started, this will be false.
	private float _waveTimeLimit = 150; //The time limit for each the very first wave. Represented in seconds. The time limit decreases by 10% each wave.
	private int _currentWaveTaskAmount; //The number of tasks the player must complete in the current wave. Can be represented as customers served, items cooked, clothes folded, etc.
	private int _waveTaskAmount; //The number of tasks the player must complete in each wave. Can be represented as customers served, items cooked, clothes folded, etc.
	private float _waveTimer; //The timer used to calculate the time remaining in the wave.

	[Header("Job Task Items")]
	[SerializeField] private Transform _jobPickupMesh; //Set in Unity Inspector | The GameObject that will be enabled and disabled as the job task item.
	[SerializeField] private Transform _jobTaskSpawner; //Set in Unity Inspector | The GameObject that will be enabled and disabled as the job task item spawner.
	[SerializeField] private Transform _possibleTaskGoalPoints; //Set in Unity Inspector | The points the player must deliver the job task items to.
	private Transform[] _goalPoints; //The points the player must deliver the job task items to. Disabled after job reset or task completed.
	private Transform _activeGoalPoint; //The active goal point the player must deliver the job task item to. Disabled after job reset or task completed.

	[Header("Job SFX")]
	[SerializeField] private AudioSource _jobSFX; //Set in Unity Inspector | The audio source that will play all job audio clips.
	[SerializeField] private AudioClip _jobAcceptSFX; //Set in Unity Inspector | The sound effect that will play when the player accepts a job.
	[SerializeField] private AudioClip _jobCompleteSFX; //Set in Unity Inspector | The sound effect that will play when the player completes a job wave.
	[SerializeField] private AudioClip _jobFailedSFX; //Set in Unity Inspector | The sound effect that will play when the player fails a job wave and gets fired.

	[Header("Player Stats")]
	[SerializeField] private PlayerStats _playerStats; //Assigned in Unity | Important Player Stats are all accessible as public properties in the PlayerStats class
	[Header("Player HUD")]
	[SerializeField] private PlayerHUD _playerHUD; //Assigned in Unity | PlayerHUD is separate from this script and is used to update the player's HUD.

	void Start()
	{
		if (!_playerStats) _playerStats = GameObject.Find("Player").GetComponent<PlayerStats>(); Debug.LogError("PlayerStats was not assigned in Unity Inspector. Please assign before publishing.");
		if (!_playerHUD) _playerHUD = GameObject.Find("Player").GetComponent<PlayerHUD>(); Debug.LogError("PlayerHUD was not assigned in Unity Inspector. Please assign before publishing.");

		//Fill the goal points array with the possible goal points - Only includes immediate children
		int childCount = _possibleTaskGoalPoints.childCount;
		_goalPoints = new Transform[childCount];
		for (int i = 0; i < childCount; i++)
		{
			_goalPoints[i] = _possibleTaskGoalPoints.GetChild(i);
		}
	}

	void Update()
	{
		if (_timerStarted)
		{
			//Increase the wave timer
			_waveTimer -= Time.deltaTime;

			//Enable the timer text on the player's HUD
			_playerHUD.EnableTimerText();

			//Update the player's HUD with the time remaining in the wave
			_playerHUD.UpdateTimer(_waveTimer);

			//If the wave timer reaches the time limit, the job is failed
			if (_waveTimer <= 0)
			{
				FailJob();
			}
		}
	}

	//Methods
	/// <summary>
	/// This method is called when a player attempts to accept a job. It checks if the player meets the requirements and then begins the job.
	/// </summary>
	private void AcceptJob()
	{
		bool allRequirementsMet = true;

		for (int i = 0; i < _jobRequirements.Length; i++)
		{
			if (_playerStats.PlayerSkills[i] < _jobRequirements[i])
			{
				allRequirementsMet = false;
				break; // No need to continue checking
			}
		}

		if (allRequirementsMet && !_playerStats.OnJob)
		{
			// Display the job accept notification
			_playerHUD.DisplayJobAcceptNotification();
			BeginJob();
		}
		else
		{
			// Display the job declined notification
			_playerHUD.DisplayJobDeclinedNotification();

			//Reset the job
			ResetJob();

			//Disable job items
			DisableJobItemns();
		}
	}

	/// <summary>
	/// Begins the process of starting the job minigame.
	/// </summary>
	private void BeginJob()
	{
		if (_playerStats.OnJob) return; //If the player is already on a job, do not start another job.

		//Disable the job pickup mesh
		_jobPickupMesh.gameObject.SetActive(false);
		transform.GetComponent<BoxCollider>().enabled = false;

		//Play the job accept sound effect
		_jobSFX.clip = _jobAcceptSFX;
		_jobSFX.Play();

		//Update job status
		_playerHUD.UpdateJobTitle(_jobName, 1);
		_playerStats.OnJob = true;

		//Reset the job first
		ResetJob();

		//Start the timer to begin the job
		_timerStarted = true;

		//Generate a task
		GenerateTask();
	}

	/// <summary>
	/// Calculates the bonus the player will receive for completing a wave and adds the bonus to the player.
	/// </summary>
	private void CalculatePayout()
	{
		//Bonus is calculated based on the wave number and increases by 5% each wave completed.
		_bonusPay = _basePay * (_jobWave * 0.05) + Random.Range(0f, 1f);

		//Display the bonus notification
		_playerHUD.DisplayJobBonusNotification(_bonusPay);

		//Add the bonus to the player's money
		_playerStats.AddMoney(_basePay + _bonusPay);

		//Update the player's HUD
		_playerHUD.UpdateMoney();

		//Play the job complete sound effect
		_jobSFX.clip = _jobCompleteSFX;
		_jobSFX.Play();
	}

	/// <summary>
	/// Generates a task for the player to complete.
	/// </summary>
	private void GenerateTask()
	{
		//Enable the job task spawner
		_jobTaskSpawner.gameObject.SetActive(true); //Object will have a trigger collider that will collide with the goal point to complete the task.

		//Enable a random task goal point
		_activeGoalPoint = _goalPoints[Random.Range(0, _goalPoints.Length)];
		_activeGoalPoint.gameObject.SetActive(true);

		_playerHUD.UpdateJobTaskCount(_currentWaveTaskAmount, _waveTaskAmount);
	}

	/// <summary>
	/// Method called from the job task item when a player completes a task.
	/// </summary>
	public void TaskCompleted()
	{
		//Check if the player has completed the task amount for the wave
		_currentWaveTaskAmount++;

		//Add the task pay to the player's money
		_taskPay = Random.Range(0.01f, 0.26f);
		_playerStats.AddMoney(_taskPay);
		_playerHUD.UpdateMoney();

		//Disable the goal point
		_activeGoalPoint.gameObject.SetActive(false);

		if (_currentWaveTaskAmount >= _waveTaskAmount)
		{
			//Debug.Log("All tasks completed in wave. Increasing difficulty and rewarding player with a bonus.");

			//Calculate the bonus for the player
			CalculatePayout();

			//Increase the wave number
			_jobWave++;

			//Decrease the wave time limit by 10%
			_waveTimeLimit *= 0.9f;
			_waveTimer = _waveTimeLimit;

			//Reset the wave task amount
			_currentWaveTaskAmount = 0;

			//Generate a task amount for the next wave that increases depending on the wave number and a random amount.
			int taskMin = _jobWave + Random.Range(1, 4);
			int taskMax = _jobWave + Random.Range(3, 11);
			if (_jobWave <= 3) taskMax += Random.Range(1, 3);
			_waveTaskAmount = Random.Range(taskMin, taskMax);
			Debug.Log($"Task Amount is {_waveTaskAmount} this wave.");

			//Update the player's HUD
			_playerHUD.UpdateJobTitle(_jobName, _jobWave);

			//Generate a new task
			GenerateTask();
		}
		else
		{
			//Generate a new task
			GenerateTask();
		}
	}

	/// <summary>
	/// Resets the job back to its initial state.
	/// </summary>
	private void ResetJob()
	{
		_jobWave = 1;
		_waveTimeLimit = 150;
		_waveTimer = _waveTimeLimit;
		_bonusPay = 0;
		_waveTaskAmount = Random.Range(2, 5);
		_currentWaveTaskAmount = 0;
		_playerHUD.DisableTimerText();
	}

	/// <summary>
	/// Resets the job completely and removes the player from the job.
	/// </summary>
	private void FailJob()
	{
		//Display the job failed notification
		_playerHUD.DisplayJobFailedNotification();

		//Reset the job
		ResetJob();

		//Play the job failed sound effect
		_jobSFX.clip = _jobFailedSFX;
		_jobSFX.Play();

		//Disable the job timer
		_timerStarted = false;

		//Disable job items
		DisableJobItemns();

		//Reset the player's job status
		_playerStats.OnJob = false;

		//Reenable the job pickup mesh
		_jobPickupMesh.gameObject.SetActive(true);
		transform.GetComponent<BoxCollider>().enabled = true;
	}

	/// <summary>
	/// Disables all GameObjects associated with the job. This method is called when the player fails a job.
	/// </summary>
	private void DisableJobItemns()
	{
		_jobTaskSpawner.gameObject.SetActive(false);
		_activeGoalPoint.gameObject.SetActive(false);
	}

	public override void Interact()
	{
		//Change the interact text to "Accept Job"
		AcceptJob();
	}
}
