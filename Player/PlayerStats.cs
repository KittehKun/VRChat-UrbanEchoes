using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerStats : UdonSharpBehaviour
{
	//Global Variables
	private float HUNGER_TICK = 0.001f; //This is the amount of hunger that is removed every tick. This should be adjusted to fit the game's pace.
	private float THIRST_TICK = 0.001f; //This is the amount of thirst that is removed every tick. This should be adjusted to fit the game's pace.
	
	//Player Properties
	public int PlayerHealth { get; private set; }
	public double PlayerMoney { get; private set; }
	public float PlayerHunger { get; private set; }
	public float PlayerThirst { get; private set; }
	public string JobTitle { get; private set; } //This will be used to display the player's job.

	void Start()
	{

	}

	//Methods
	/// <summary>
	/// Adds an amount of money to the player's money.
	/// </summary>
	/// <param name="amount">The amount of money to add.</param>
	public void AddMoney(double amount)
	{
		PlayerMoney += amount;
	}

	/// <summary>
	/// The amount of money to remove. Will play an error sound if the player does not have enough money. This method should only be used on purchase events.
	/// </summary>
	/// <param name="amount">The amount of money to remove.</param>
	public void RemoveMoney(double amount)
	{
		if(PlayerMoney >= amount)
		{
			PlayerMoney -= amount;
		}
		else
		{
			//Play global money error sound once implemented
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

	public void SetJob(string job)
	{
		JobTitle = job;
	}

	/// <summary>
	/// Adjusts the player's needs. This method should be called every tick.
	/// </summary>
	public void NeedsTick()
	{
		PlayerHunger -= HUNGER_TICK;
		PlayerThirst -= THIRST_TICK;
	}
}
