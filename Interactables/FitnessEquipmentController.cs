
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FitnessEquipmentController : UdonSharpBehaviour
{   
    [Header("Player Stats")]
    [SerializeField] private PlayerStats playerStats; //Used for altering the player stats depending on the gym equipment

    [Header("Player HUD")]
    [SerializeField] private PlayerHUD playerHUD; //Used for updating the player HUD when the player levels up their fitness skill
    
    [Header("Audio SFX")]
    [SerializeField] private AudioSource audioSource; //Used for playing the sound of the gym equipment
    [SerializeField] private AudioClip whistleSFX; //The sound effect that plays when the player uses the fitness equipment
    [SerializeField] private AudioClip levelupSFX; //The sound effect that plays when the player levels up their fitness skill

    [Header("Training Minigame")]
    [SerializeField] private Transform minigameCheckpoints; //The checkpoints for the fitness skill training minigame used for enabling a random amount | Set in Inspector
    private int checkpointCount; //The number of checkpoints in the minigame

    private bool isTraining = false; //Used to check if the player is currently training
    private float minigameTimer = 0; //Used to track the time the player has been training
    private readonly float timeLimit = 30; //The time limit for the minigame represented in seconds

	private void Update()
	{
		if (isTraining)
        {
			minigameTimer += Time.deltaTime;

            //Update the player HUD with the minigame timer
            playerHUD.UpdateTimer(minigameTimer);

            //Check if the player has reached the time limit
            if (minigameTimer >= timeLimit)
            {
                EndTraining(false);
                audioSource.clip = whistleSFX; //Plays the whistle SFX
                audioSource.Play();
            }
		}
	}

	public override void Interact()
    {
        if (isTraining) return;
        StartTraining();
    }

    private void StartTraining()
    {
        //Activate the minigame timer
        isTraining = true;

        //Disable the collider to prevent the player from interacting with the equipment during training
        transform.GetComponent<Collider>().enabled = false; //Disable the collider to prevent the player from interacting with the equipment during training

        //Play the whistle SFX
        audioSource.clip = whistleSFX; //Plays the whistle SFX
        audioSource.Play();

        EnableCheckpoints();
    }

    private void EnableCheckpoints()
    {
        //Set a random amount of checkpoints to enable based on the minigameCheckpoints
        checkpointCount = Random.Range(1, minigameCheckpoints.childCount + 1);
        
        //Randomize the enabled checkpoints in the array
        for (int i = 0; i < minigameCheckpoints.childCount; i++)
        {
			minigameCheckpoints.GetChild(i).gameObject.SetActive(i < checkpointCount);
		}

        //Enable and update the Player HUD
        playerHUD.EnableObjectiveGUI();
        playerHUD.UpdateMinigameTaskCount(checkpointCount, "Checkpoints");
    }

    private void DisableCheckpoints()
    {
        //Disable all checkpoints to reset the minigame
        for (int i = 0; i < minigameCheckpoints.childCount; i++)
        {
            minigameCheckpoints.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void EndTraining(bool finishedTraining)
    {
        if(finishedTraining)
        {
            playerStats.IncreaseFitness(Random.Range(0, 3));
            playerStats.DecreaseEnergy(Random.Range(5, 10));
        }
        else
        {
            playerStats.DecreaseEnergy(Random.Range(10, 20));
            audioSource.clip = whistleSFX; //Plays the whistle SFX
            audioSource.Play();
        }

        ResetMinigame();
    }

    private void ResetMinigame()
    {
        isTraining = false;
		minigameTimer = 0;
		transform.GetComponent<Collider>().enabled = true; //Enable the collider to allow the player to interact with the equipment

        DisableCheckpoints();

        playerHUD.DisableObjectiveGUI();
    }

    public void CompleteCheckpoint()
    {
        if(--checkpointCount == 0)
        {
			EndTraining(true);
			audioSource.clip = levelupSFX; //Plays the level up SFX
			audioSource.Play();
		}
    }
}
