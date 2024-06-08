//This script will be used for the player's hunger system. It will contain the methods that will be used to add and remove hunger from the player. This script will contain its own seperate NeedsTick method with its own timer and tick rate. The tick will be slower than the EnergyTick from PlayerStats but not by much.

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FoodSystem : UdonSharpBehaviour
{
	//Player Stats Reference
	private PlayerStats playerStats; //Reference to the PlayerStats script.
	
	//Global Variables
	private readonly float HUNGER_TICK = 0.1f; //The amount of hunger to remove every tick.

	//Tick Variables
	private readonly float tickInterval = 3f; //The interval at which the NeedsTick method will be called. Represented in seconds.
	private float timer = 0.0f; //The timer used to determine when to remove hunger.

	//Player Hunger Properties
	public float PlayerHunger { get; private set; } = 100; //Ranges from 0 to 100. If it reaches 0, the player will lose health over time.
	private readonly int HUNGER_MAX = 100; //Denotes the maximum hunger the player can have.

	void Start()
	{
		// Get the PlayerStats script reference
		playerStats = this.gameObject.GetComponent<PlayerStats>();
		if (!playerStats) Debug.LogError("[FOOD SYSTEM]: Missing reference to PlayerStats!"); //Check if the PlayerStats reference is missing.
	}

	private void Update()
	{
		Timer();
	}

	/// <summary>
	/// This method will be called every Update tick to remove hunger from the player.
	/// </summary>
	private void Timer()
	{
		// Update the timer
		timer += Time.deltaTime;

		// Check if it's time for the next tick
		if (timer >= tickInterval)
		{
			// Perform hunger calculations
			NeedsTick();
			//Debug.Log("Player hunger has been adjusted.");

			// Reset the timer
			timer = 0f;
		}
	}

	private void NeedsTick()
	{
		// Remove hunger from the player
		PlayerHunger -= HUNGER_TICK;

		// Check if the player's hunger has reached 0. If so, damage the player.
		if (PlayerHunger <= 0) playerStats.RemoveHealth(1);
	}

	/// <summary>
	/// Resets the player's hunger. This will be used when the player respawns at the hospital after losing all their health.
	/// </summary>
	public void ResetHunger()
	{
		PlayerHunger = HUNGER_MAX;
	}
}
