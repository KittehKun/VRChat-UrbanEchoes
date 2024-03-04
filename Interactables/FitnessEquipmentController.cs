
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FitnessEquipmentController : UdonSharpBehaviour
{   
    [Header("Player Stats")]
    [SerializeField] private PlayerStats playerStats; //Used for altering the player stats depending on the gym equipment
    
    [Header("Audio SFX")]
    [SerializeField] private AudioSource audioSource; //Used for playing the sound of the gym equipment
    [SerializeField] private AudioClip fitnessSFX; //The sound effect that plays when the player uses the fitness equipment
    [SerializeField] private AudioClip altFitnessSFX; //The sound effect that plays when the player uses the fitness equipment

    [Header("Training Minigame")]
    [SerializeField] private Transform fitnessMinigame; //The minigame for fitness skill training

    private bool isTraining = false; //Used to check if the player is currently training
    private float minigameTimer = 0; //Used to track the time the player has been training
    private readonly float timeLimit = 30; //The time limit for the minigame represented in seconds

	private void Update()
	{
		if (isTraining)
        {
			minigameTimer += Time.deltaTime;

            if (minigameTimer >= timeLimit)
            {
                EndTraining(false);
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
        isTraining = true;
        fitnessMinigame.gameObject.SetActive(true);
    }

    private void EndTraining(bool finishedTraining)
    {
        if(finishedTraining)
        {
            playerStats.IncreaseFitness(Random.Range(0, 3));
            playerStats.DecreaseEnergy(Random.Range(5, 10));
        }
        else
        {
            playerStats.DecreaseEnergy(Random.Range(10, 20));
        }

        fitnessMinigame.gameObject.SetActive(false);
    }
}
