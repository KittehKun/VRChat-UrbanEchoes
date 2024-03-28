
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class CookingJob : UdonSharpBehaviour
{
	// Timer settings
	private bool timerStarted = false;
	private readonly float timeLimit = 300; // 5 minutes timer
	private float timer;

	[Header("Cooking Stages")]
	[SerializeField] private Transform ingredientItems; // Set in Unity Inspector | Contains all the ingredients that the player must collect for cooking.
	[SerializeField] private Transform possibleIngredientPos; // Set in Unity Inspector | Contains all the possible positions where the ingredients can spawn.
	[SerializeField] private Transform cookingStages; // Set in Unity Inspector | Contains all the cooking stages that the player must complete to finish the minigame.

	[Header("Cooking SFX")]
	[SerializeField] private AudioSource cookingSFX; // Set in Unity Inspector | The audio source that will play all cooking-related audio clips.
	[SerializeField] private AudioClip cookingStartSFX; // Set in Unity Inspector | The sound effect that will play when the player starts the cooking minigame.
	[SerializeField] private AudioClip cookingCompleteSFX; // Set in Unity Inspector | The sound effect that will play when the player completes the cooking minigame.
	[SerializeField] private AudioClip cookingFailedSFX; // Set in Unity Inspector | The sound effect that will play when the player fails the cooking minigame.
	[SerializeField] private AudioClip ingredientCollectedSFX; // Set in Unity Inspector | The sound effect that will play when the player collects an ingredient.

	[Header("Player Stats")]
	[SerializeField] private PlayerStats playerStats;

	[Header("Player HUD")]
	[SerializeField] private PlayerHUD playerHUD;

	private void Start()
	{
		timer = timeLimit;
	}

	private void Update()
	{
		if (timerStarted)
		{
			timer -= Time.deltaTime;

			if (timer <= 0)
			{
				timerStarted = false;
				// Fail the minigame
				FailMinigame();
			}
		}
	}

	// Starts the cooking minigame
	private void StartMinigame()
	{
		// Start the timer
		timerStarted = true;

		// Disable the mesh and collider of cooking area or object
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		gameObject.GetComponent<Collider>().enabled = false;

		// Set the player's on job status to true
		playerStats.OnJob = true;

		// Play the start SFX
		PlayStartSFX();

		// Enable the player HUD and update it
		playerHUD.EnableObjectiveGUI();
		playerHUD.UpdateJobTitle("Cooking");
	}

	// Stops the cooking minigame
	private void StopMinigame()
	{
		timerStarted = false;
	}

	// Fails the cooking minigame
	private void FailMinigame()
	{
		StopMinigame();
		ResetMinigame();
		PlayFailSFX();
	}

	// Resets the cooking minigame
	private void ResetMinigame()
	{
		timer = 0;

		// Enable the mesh and collider of cooking area or object
		gameObject.GetComponent<MeshRenderer>().enabled = true;
		gameObject.GetComponent<Collider>().enabled = true;

		// Set the player's on job status to false
		playerStats.OnJob = false;

		// Disable the ingredient items
		for (int i = 0; i < ingredientItems.childCount; i++)
		{
			ingredientItems.GetChild(i).gameObject.SetActive(false);
		}

		// Disbale the HUD
		playerHUD.DisableObjectiveGUI();
	}

	// Plays the start sound effect
	private void PlayStartSFX()
	{
		cookingSFX.PlayOneShot(cookingStartSFX);
	}

	// Plays the complete sound effect
	private void PlayCompleteSFX()
	{
		cookingSFX.PlayOneShot(cookingCompleteSFX);
	}

	// Plays the failed sound effect
	private void PlayFailSFX()
	{
		cookingSFX.PlayOneShot(cookingFailedSFX);
	}


	// This method is called when the player completes the cooking minigame.
	private void CompleteMinigame()
	{
		// Stop the minigame
		StopMinigame();

		// Reset the minigame
		ResetMinigame();

		// Play the complete sound effect
		PlayCompleteSFX();
	}

	// The script's entry point when the player interacts with the cooking area or object.
	public override void Interact()
	{
		// Start the cooking minigame
		StartMinigame();
	}
}
