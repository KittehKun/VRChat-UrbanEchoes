
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
	}

	private void StopMinigame()
	{
		timerStarted = false;
	}

	private void FailMinigame()
	{
		ResetMinigame();
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

	public override void Interact()
	{
		StartMinigame();
	}
}
