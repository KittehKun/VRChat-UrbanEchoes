
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class LibraryMinigame : UdonSharpBehaviour
{
    //Timer Settings
    private bool timerStarted = false;
    private readonly float timeLimit = 180;
    private float timer;

	[Header("Book Items")]
	[SerializeField] private Transform taskItems; //Set in Unity Inspector | Contains all the books that the player must collect to complete the minigame.
	private int booksToCollect = 0; //The number of books the player has to collect to complete the minigame.

	[Header("Library SFX")]
	[SerializeField] private AudioSource librarySFX; //Set in Unity Inspector | The audio source that will play all library audio clips.
	[SerializeField] private AudioClip libraryCompleteSFX; //Set in Unity Inspector | The sound effect that will play when the player completes the library minigame.
	[SerializeField] private AudioClip libraryFailedSFX; //Set in Unity Inspector | The sound effect that will play when the player fails the library minigame.
    
    [Header("Player Stats")]
    [SerializeField] PlayerStats playerStats;

    [Header("Player HUD")]
    [SerializeField] PlayerHUD playerHUD;

	private void Update()
	{
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
	}

	private void StopMinigame()
	{
		timerStarted = false;
	}

	private void FailMinigame()
	{
		ResetMinigame();
		PlayFailSFX();
	}

	private void ResetMinigame()
	{
		StopMinigame();
		timer = 0;

		//Enable the mesh and collider
		gameObject.GetComponent<MeshRenderer>().enabled = true;
		gameObject.GetComponent<Collider>().enabled = true;

		//Set the player's on job status to false
		playerStats.OnJob = false;
	}

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
	}

	private void PlayCompleteSFX()
	{
		librarySFX.clip = libraryCompleteSFX;
		librarySFX.Play();
	}

	private void PlayFailSFX()
	{
		librarySFX.clip = libraryFailedSFX;
		librarySFX.Play();
	}

	public void BookCollected()
	{
		//Check if the player has collected all the books
		booksToCollect--;
		playerHUD.UpdateJobTaskCount(booksToCollect);

		if (booksToCollect <= 0)
		{
			//Complete the minigame
			ResetMinigame();
			PlayCompleteSFX();
		}
	}

	public override void Interact()
	{
		StartMinigame();
	}
}
