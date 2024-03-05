using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PlayerStats : UdonSharpBehaviour
{
	//Global Variables
	private readonly float ENERGY_TICK = 0.1f; //The amount of energy to remove every tick.
	private readonly int SKILL_MAX = 10; //The maximum skill level a player can have.

	//Tick Variables
	private readonly float tickInterval = 2f; //The timer used to determine when to remove energy and sleep. Represented in seconds.
	private float timer = 0.0f; //The timer used to determine when to remove energy and sleep.

	//Player Properties
	public int PlayerHealth { get; private set; } = 100; //Ranges from 0 to 100. If it reaches 0, the player respawns at the hospital and loses some money.
	public double PlayerMoney { get; private set; } = 0;
	public float PlayerEnergy { get; private set; } = 100;
	public int[] PlayerSkills { get; private set; } //Skills start at 0 and can be increased by doing activities.
	public int PlayerReputation { get; private set; } = 0; //It will be used to determine the player's standing in the community. Ranges from -20 to 20.
	public bool OnJob { get; set; } = false; //If the player is currently on a job, this will be true. If the player is not on a job, this will be false.

	//HUD Elements
	[SerializeField] private PlayerHUD playerHUD; //Assigned in Unity | PlayerHUD is separate from this script and is used to update the player's HUD.

	//Audio Elements
	[SerializeField] private AudioSource audioSource; //Assigned in Unity | The audio source used to play sound effects.
	[SerializeField] private AudioClip addMoneySFX; //Assigned in Unity | The sound effect that plays when the player receives money.

	private void Start()
	{
		PlayerSkills = new int[5]; //The array is as follows: [0] = Intelligence, [1] = Fitness, [2] = Cooking, [3] = Creativity, [4] = Charisma
		playerHUD.UpdateHUD();
	}

	private void Update()
	{
		// Update the timer
		timer += Time.deltaTime;

		// Check if it's time for the next tick
		if (timer >= tickInterval)
		{
			// Perform energy and sleep calculations
			NeedsTick();
			//Debug.Log("Player needs have been adjusted.");

			// Reset the timer
			timer = 0f;
		}
	}

	//Methods
	/// <summary>
	/// Adds an amount of money to the player's money.
	/// </summary>
	/// <param name="amount">The amount of money to add.</param>
	public void AddMoney(double amount)
	{
		PlayerMoney += amount;
		audioSource.clip = addMoneySFX;
		audioSource.Play();
	}

	/// <summary>
	/// The amount of money to remove. Will play an error sound if the player does not have enough money. This method should only be used on purchase events.
	/// </summary>
	/// <param name="amount">The amount of money to remove.</param>
	public void RemoveMoney(double amount)
	{
		if (PlayerMoney >= amount)
		{
			PlayerMoney -= amount;
		}
	}

	/// <summary>
	/// The amount of health to add to the player's health.
	/// </summary>
	/// <param name="amount">The amount of health to add.</param>
	public void AddHealth(int amount)
	{
		PlayerHealth += amount;
	}

	/// <summary>
	/// The amount of health to remove from the player's health.
	/// </summary>
	/// <param name="health">The amount of health to remove.</param>
	public void RemoveHealth(int health)
	{
		PlayerHealth -= health;
	}

	/// <summary>
	/// Adjusts the player's needs. This method should be called every tick.
	/// </summary>
	private void NeedsTick()
	{
		if(PlayerEnergy > 0)
		PlayerEnergy -= ENERGY_TICK;
		playerHUD.UpdateTick();
	}

	/// <summary>
	/// Adjusts the player's intelligence skill by 1 point.
	/// </summary>
	public void IncreaseIntelligence()
	{
		if (PlayerSkills[0] != SKILL_MAX)
		{
			PlayerSkills[0]++;
		}

	}

	/// <summary>
	/// Adjusts the player's intelligence skill by 1 point. Should only be used during skill loss events.
	/// </summary>
	public void DecreaseIntelligence()
	{
		if (PlayerSkills[0] != 0)
		{
			PlayerSkills[0]--;
		}
	}

	/// <summary>
	/// Adjusts the player's fitness skill by 1 point.
	/// </summary>
	public void IncreaseFitness(int amount)
	{
		if (PlayerSkills[1] != SKILL_MAX)
		{
			PlayerSkills[1] += amount;
		}
	}

	/// <summary>
	/// Adjusts the player's fitness skill by 1 point. Should only be used during skill loss events.
	/// </summary>
	public void DecreaseFitness()
	{
		if (PlayerSkills[1] != 0)
		{
			PlayerSkills[1]--;
		}
	}

	/// <summary>
	/// Adjusts the player's cooking skill by 1 point.
	/// </summary>
	public void IncreaseCooking()
	{
		if (PlayerSkills[2] != SKILL_MAX)
		{
			PlayerSkills[2]++;
		}
	}

	/// <summary>
	/// Adjusts the player's cooking skill by 1 point. Should only be used during skill loss events.
	/// </summary>
	public void DecreaseCooking()
	{
		if (PlayerSkills[2] != 0)
		{
			PlayerSkills[2]--;
		}
	}

	/// <summary>
	/// Adjusts the player's creativity skill by 1 point.
	/// </summary>
	public void IncreaseCreativity()
	{
		if (PlayerSkills[3] != SKILL_MAX)
		{
			PlayerSkills[3]++;
		}
	}

	/// <summary>
	/// Adjusts the player's creativity skill by 1 point. Should only be used during skill loss events.
	/// </summary>
	public void DecreaseCreativity()
	{
		if (PlayerSkills[3] != 0)
		{
			PlayerSkills[3]--;
		}
	}

	/// <summary>
	/// Adjusts the player's charisma skill by 1 point.
	/// </summary>
	public void IncreaseCharisma()
	{
		if (PlayerSkills[4] != SKILL_MAX)
		{
			PlayerSkills[4]++;
		}
	}

	/// <summary>
	/// Adjusts the player's charisma skill by 1 point. Should only be used during skill loss events.
	/// </summary>
	public void DecreaseCharisma()
	{
		if (PlayerSkills[4] != 0)
		{
			PlayerSkills[4]--;
		}
	}

	/// <summary>
	/// Adjusts the player's reputation by 1 point.
	/// </summary>
	public void IncreaseReputation()
	{
		if (PlayerReputation != 20)
		{
			PlayerReputation++;
		}
	}

	/// <summary>
	/// Adjusts the player's reputation by 1 point. Should only be used during reputation loss events.
	/// </summary>
	public void DecreaseReputation()
	{
		if (PlayerReputation != -20)
		{
			PlayerReputation--;
		}
	}

	/// <summary>
	/// Adjusts the player's energy by a random amount between 0.5 and 3. Called when the player completes an activity.
	/// </summary>
	public void DecreaseEnergy()
	{
        if (PlayerEnergy != 0)
        {
			PlayerEnergy -= UnityEngine.Random.Range(0.1f, 1f);
			playerHUD.DecreaseEnergy();
        }
    }

	/// <summary>
	/// Adjusts the player's energy by a specific amount. Called when the player completes an activity.
	/// </summary>
	/// <param name="amount"></param>
	public void DecreaseEnergy(float amount)
	{
		if (PlayerEnergy != 0)
		{
			PlayerEnergy -= amount;
			playerHUD.DecreaseEnergy();
		}
	}

}
