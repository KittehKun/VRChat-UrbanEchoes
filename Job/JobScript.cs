
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class JobScript : UdonSharpBehaviour
{

    //Variables - Set in Unity Inspector depending on the job
	[SerializeField] private double _basePay; //The base amount of money the player will receive for completing a job wave.
	private double _taskPay; //The amount of money the player will receive for completing a job task. A random between 0.1 and 1.0 is added to the base pay.
	private double _bonusPay = 0; //The bonus amount the player will receive for completing a job task. This is calculated based on the wave number.
	private int _jobWave; //Maximum of 5 waves per difficulty level. Player's receive a bonus each wave they complete.
	[SerializeField] private int[] _jobRequirements = new int[5]; //Set in Unity Inspector | The array is as follows: [0] = Intelligence, [1] = Fitness, [2] = Cooking, [3] = Creativity, [4] = Charisma

	//Wave Settings
	private float _waveTimeLimit = 150; //The time limit for each the very first wave. Represented in seconds. The time limit decreases by 10% each wave.
	private int _waveTaskAmount; //The number of tasks the player must complete in each wave. Can be represented as customers served, items cooked, clothes folded, etc.
	private float _waveTimer; //The timer used to calculate the time remaining in the wave.

	[Header("Job Task Items")]
	[SerializeField] private Transform[] _jobTaskItems; //Set in Unity Inspector | The items the player must interact with to complete a job task.
	[SerializeField] private Transform _jobTaskSpawnPoint; //Set in Unity Inspector | The position where the job task items will spawn.
	[SerializeField] private Transform[] _taskGoalPoints; //Set in Unity Inspector | The points the player must deliver the job task items to. Randomized every time a task is generated.
	[SerializeField] private bool _hasCustomers = false; //Set in Unity Inspector | If the job has customer spawns, this will be true. By default, it is false.
	[SerializeField] private Transform _customerSpawnPoint; //Set in Unity Inspector | The position where the customer will spawn. Usually in front of a register or computer station.

	[Header("Player Stats")]
	[SerializeField] private PlayerStats _playerStats; //Assigned in Unity | Important Player Stats are all accessible as public properties in the PlayerStats class
	[Header("Player HUD")]
	[SerializeField] private PlayerHUD _playerHUD; //Assigned in Unity | PlayerHUD is separate from this script and is used to update the player's HUD.

	public void Awake()
	{
		if (!_playerStats) _playerStats = GameObject.Find("Player").GetComponent<PlayerStats>(); Debug.LogError("PlayerStats was not assigned in Unity Inspector. Please assign before publishing.");
		if(!_playerHUD) _playerHUD = GameObject.Find("Player").GetComponent<PlayerHUD>(); Debug.LogError("PlayerHUD was not assigned in Unity Inspector. Please assign before publishing.");
		if(_hasCustomers && _customerSpawnPoint == null) Debug.LogError("The job has customers, but the customer spawn point was not assigned in Unity Inspector. Please assign a spawn point for customers.");
	}

	//Methods
	public void AcceptJob()
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

		if (allRequirementsMet)
		{
			// Display the job accept notification
			_playerHUD.DisplayJobAcceptNotification();
			BeginJob();
		}
		else
		{
			// Display the job declined notification
			_playerHUD.DisplayJobDeclinedNotification();
		}
	}

	private void BeginJob()
	{
		ResetJob();
		GenerateTasks();
	}

	private void CalculateBonus()
	{
		//Bonus is calculated based on the wave number and increases by 5% each wave completed.
		_bonusPay = _basePay * (_jobWave * 0.05);

		//Display the bonus notification
		_playerHUD.DisplayJobBonusNotification(_bonusPay);

		//Add the bonus to the player's money
		_playerStats.AddMoney(_bonusPay);
	}

	private void GenerateTasks()
	{

	}

	private void ResetJob()
	{
		_jobWave = 1;
		_waveTimer = _waveTimeLimit;
		_bonusPay = 0;
		_waveTaskAmount = Random.Range(2, 6); //Randomly selects a number between 2 and 5 for the first wave.
	}
}
