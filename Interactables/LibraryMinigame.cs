
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class LibraryMinigame : UdonSharpBehaviour
{
    //Object Float Settings
	private float INITIAL_Y_POSITION;
	private readonly float FLOAT_SPEED = 0.5f;
	private readonly float FLOAT_HEIGHT = 0.1f;
	private readonly float ROTATION_SPEED = 2.0f;

	//Timer Settings
    private bool timerStarted = false;
    private readonly float timeLimit = 180;
    private float timer;

	[Header("Book Items")]
	[SerializeField] private Transform taskItems; //Set in Unity Inspector | Contains all the books that the player must collect to complete the minigame.
	private int booksToCollect = 0; //The number of books the player has to collect to complete the minigame.

	[Header("Library SFX")]
	[SerializeField] private AudioSource librarySFX; //Set in Unity Inspector | The audio source that will play all library audio clips.
	[SerializeField] private AudioClip libraryStartSFX; //Set in Unity Inspector | The sound effect that will play when the player starts the library minigame.
	[SerializeField] private AudioClip libraryCompleteSFX; //Set in Unity Inspector | The sound effect that will play when the player completes the library minigame.
	[SerializeField] private AudioClip libraryFailedSFX; //Set in Unity Inspector | The sound effect that will play when the player fails the library minigame.
	[SerializeField] private AudioClip bookCollectedSFX; //Set in Unity Inspector | The sound effect that will play when the player collects a book.
    
    [Header("Player Stats")]
    [SerializeField] PlayerStats playerStats;

    [Header("Player HUD")]
    [SerializeField] PlayerHUD playerHUD;

	private void Start()
	{
		INITIAL_Y_POSITION = transform.position.y;
	}

	private void Update()
	{
		//Spin and float the object
		SpinFloatObject();

        if (timerStarted)
        {
			timer -= Time.deltaTime;

			playerHUD.EnableTimerText();
			playerHUD.UpdateTimer(timer);

			if (timer <= 0)
            {
				timerStarted = false;
				//Fail the minigame
				FailMinigame();
			}
		}
	}

	private void SpinFloatObject()
	{
		//Spin the object on the local Z axis
		transform.Rotate(0, 0, ROTATION_SPEED);

		//Float the object on the Y axis
		float newY = Mathf.Sin(Time.time * FLOAT_SPEED) * FLOAT_HEIGHT + INITIAL_Y_POSITION;
		transform.position = new Vector3(transform.position.x, newY, transform.position.z);
	}

	/// <summary>
	/// Starts the process of the library minigame. Disables the library mesh and collider, sets the player's on job status to true, and enables the task items.
	/// </summary>
	private void StartMinigame()
    {
		//Start the timer
		timer = timeLimit;
		timerStarted = true;

		//Disable the mesh and collider
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		gameObject.GetComponent<Collider>().enabled = false;

		//Set the player's on job status to true
		playerStats.OnJob = true;

		//Enable the task items
		EnableTaskItems();

		//Play the start SFX
		PlayStartSFX();

		//Enable the player HUD and update it
		playerHUD.EnableObjectiveGUI();
		playerHUD.UpdateJobTitle("Studying");
	}

	/// <summary>
	/// Stops the timer. This method is called when the player completes the minigame or fails the minigame.
	/// </summary>
	private void StopMinigame()
	{
		timerStarted = false;
	}

	/// <summary>
	/// Fails the minigame. Begins the process of resetting the minigame and returns the library to its default state.
	/// </summary>
	private void FailMinigame()
	{
		ResetMinigame();
		PlayFailSFX();
	}

	/// <summary>
	/// Resets the minigame and returns the library to its default state.
	/// </summary>
	private void ResetMinigame()
	{
		StopMinigame();
		timer = 0;

		//Enable the mesh and collider
		gameObject.GetComponent<MeshRenderer>().enabled = true;
		gameObject.GetComponent<Collider>().enabled = true;

		//Set the player's on job status to false
		playerStats.OnJob = false;

		//Disable the task items
		for (int i = 0; i < taskItems.childCount; i++)
		{
			taskItems.GetChild(i).gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Enables all the tasks items in random order in the Hierarchy.
	/// </summary>
	private void EnableTaskItems()
	{
		//Enable a random amount of task items based on child count
		booksToCollect = Random.Range(1, taskItems.childCount + 1);

		//Create an array to store indices of task items
		int[] indices = new int[taskItems.childCount];
		for (int i = 0; i < indices.Length; i++)
		{
			indices[i] = i;
		}

		//Shuffle the array of indices
		for (int i = 0; i < indices.Length; i++)
		{
			int randomIndex = Random.Range(i, indices.Length);
			int temp = indices[i];
			indices[i] = indices[randomIndex];
			indices[randomIndex] = temp;
		}

		//Randomize the enabled tasks using the shuffled indices
		for (int i = 0; i < taskItems.childCount; i++)
		{
			taskItems.GetChild(indices[i]).gameObject.SetActive(i < booksToCollect);
		}

		//Update the player HUD and reset the current task count
		playerHUD.UpdateJobTaskCount(booksToCollect);
		playerHUD.UpdateMinigameTaskCount(booksToCollect, "Books");
	}

	/// <summary>
	/// Plays the start sound effect.
	/// </summary>
	private void PlayStartSFX()
	{
		librarySFX.PlayOneShot(libraryStartSFX);
	}

	/// <summary>
	/// Plays the complete sound effect.
	/// </summary>
	private void PlayCompleteSFX()
	{
		librarySFX.PlayOneShot(libraryCompleteSFX);
	}

	/// <summary>
	/// Plays the failed sound effect.
	/// </summary>
	private void PlayFailSFX()
	{
		librarySFX.PlayOneShot(libraryFailedSFX);
	}

	/// <summary>
	/// This method is called by the LibraryPickup script when the player interacts with a book.
	/// </summary>
	/// <param name="clip">The clip to be played.</param>
	private void PlayBookCollectedSFX()
	{
		librarySFX.PlayOneShot(bookCollectedSFX);
	}

	/// <summary>
	/// This method is called by the LibraryPickup script when the player interacts with a book. Begins the process of collecting a book.
	/// </summary>
	public void BookCollected()
	{
		//Play the book collected SFX
		PlayBookCollectedSFX();

		//Check if the player has collected all the books
		booksToCollect--;
		playerHUD.UpdateMinigameTaskCount(booksToCollect, "Books");

		if (booksToCollect <= 0)
		{
			//Complete the minigame
			ResetMinigame();
			PlayCompleteSFX();
		}
	}

	/// <summary>
	/// The script's entry point when the player interacts with the library minigame.
	/// </summary>
	public override void Interact()
	{
		StartMinigame();
	}
}
