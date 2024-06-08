using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PlayerStats : UdonSharpBehaviour
{
	//Global Variables
	private readonly float ENERGY_TICK = 0.1f; //The amount of energy to remove every tick. | Represented in seconds.
	private readonly float HUNGER_TICK = 0.01f; //The amount of hunger to remove every tick. | Represented in seconds.

	//Tick Variables
	private readonly float tickInterval = 2f; //The timer used to determine when to remove energy and sleep. Represented in seconds.
	private float timer = 0.0f; //The timer used to determine when to remove energy and sleep.

	//Player Properties
	public int PlayerHealth { get; private set; } = 100; //Ranges from 0 to 100. If it reaches 0, the player respawns at the hospital and loses some money.
	public int MaxHealth { get; } = 100; //The maximum health the player can have.
	public double PlayerMoney { get; private set; } = 0; //The amount of money the player has. Can be increased by doing activities.
	public float PlayerEnergy { get; private set; } = 100; //Ranges from 0 to 100. If it reaches 0, the player will lose hunger over time.
	public float PlayerHunger { get; private set; } = 100; //Ranges from 0 to 100. If it reaches 0, the player will lose hunger over time.
	private readonly int ENERGY_MAX = 100; //Denotes the maximum energy the player can have.
	private readonly int SKILL_MAX = 10; //Denotes the maximum skill level a player can have.
	public int PlayerIntelligence { get; private set; } = 0; //Ranges from 0 to 10. Represents the player's intelligence skill.
	public int PlayerAthleticism { get; private set; } = 0; //Ranges from 0 to 10. Represents the player's athleticism skill.
	public int PlayerFinesse { get; private set; } = 0; //Ranges from 0 to 10. Represents the player's finesse skill.
	public bool OnActivity { get; set; } = false; //If the player is currently on a job, this will be true. If the player is not on a job, this will be false.

	//TODO: Add HUD update functionality when ready.
	//HUD Variables
	//public PlayerHUD playerHUD; //Assigned in Unity | PlayerHUD is separate from this script and is used to update the player's HUD.
	
	//Player Audio
	[SerializeField] private PlayerAudio playerAudio; //Assigned in Unity | PlayerAudio is separate from this script and is used to play audio on the player.

	private void Start()
	{
		PlayerMoney = 20.0; //Start the player with $20.
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

		// Play the money sound
		playerAudio.PlayMoneySound();
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
		if(PlayerEnergy > 0) PlayerEnergy -= ENERGY_TICK;
		if(PlayerHunger > 0) PlayerHunger -= HUNGER_TICK;

		Debug.Log($"Player Energy at: {PlayerEnergy:0}");
		Debug.Log($"Player Hunger at: {PlayerHunger:0}");
	}

	/// <summary>
	/// Adjusts the player's energy by a random amount between 0.5 and 3. Called when the player completes an activity.
	/// </summary>
	public void DecreaseEnergy()
	{
        if (PlayerEnergy != 0)
        {
			PlayerEnergy -= Random.Range(0.1f, 1f);
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
		}
	}

	/// <summary>
	/// Resets the player's energy back to full. Ideally used when the player respawns at the hospital or uses a bed to sleep in.
	/// </summary>
	public void ResetEnergy()
	{
		PlayerEnergy = ENERGY_MAX;
	}

	/// <summary>
	/// Chance roll for the player's intelligence to increase. Called when the player completes an activity to level intelligence such as the Library minigame.
	/// </summary>
	public void RollIntelligenceIncrease()
	{
		//Chance roll is based off the player's current intelligence. The higher the intelligence, the lower the chance.
		int roll = Random.Range(1, SKILL_MAX);
		if (roll <= PlayerIntelligence)
		{
			PlayerIntelligence++;

			playerAudio.PlayLevelSFX();

			return;
		}

		Debug.Log($"The player's Intelligence skill was not upgraded. Rolled a {roll}");
	}

	/// <summary>
	/// Chance roll for the player's athleticism to increase. Called when the player completes an activity to level athleticism such as the Gym minigame.
	/// </summary>
	public void RollAthleticismIncrease()
	{
		//Chance roll is based off the player's current athleticism. The higher the athleticism, the lower the chance.
		int roll = Random.Range(1, SKILL_MAX);
		if (roll <= PlayerAthleticism)
		{
			PlayerAthleticism++;

			playerAudio.PlayLevelSFX();

			return;
		}

		Debug.Log($"The player's Athleticism skill was not upgraded. Rolled a {roll}");
	}

	/// <summary>
	/// Chance roll for the player's finesse to increase. Called when the player completes an activity to level finesse such as the Casino minigame.
	/// </summary>
	public void RollFinesseIncrease()
	{
		//Chance roll is based off the player's current finesse. The higher the finesse, the lower the chance.
		int roll = Random.Range(1, SKILL_MAX);
		if (roll <= PlayerFinesse)
		{
			PlayerFinesse++;

			playerAudio.PlayLevelSFX();

			return;
		}

		Debug.Log($"The player's Finesse skill was not upgraded. Rolled a {roll}");
	}

}
