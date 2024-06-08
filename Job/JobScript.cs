//This will act as the generic script that will be used for every single job. It will contain the basic methods that will be used for every job, such as AddMoney, RemoveMoney, Energy drain. This script will be used as a base for all other job scripts.

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class JobScript : UdonSharpBehaviour
{
    [SerializeField] private double baseReward; //Assigned in Unity based on Job | Awarded to the player for completing the job minigame.
    [SerializeField] private PlayerStats playerStats; //Assigned in Unity | PlayerStats is separate from this script and is used to add money and drain energy from the player.

    [SerializeField] private Transform minigame; //Assigned in Unity | The minigame GameObject that will be enabled when the player starts the job.
    
    void Start()
    {
        if (!playerStats) Debug.LogError("[JOB SCRIPT]: Missing reference to PlayerStats!"); //Check if the PlayerStats reference is missing.
    }

    /// <summary>
    /// Adds money to the player's money based on the reward of the job.
    /// </summary>
    private void AddMoney()
    {
        playerStats.AddMoney(CalculateReward());
    }

    /// <summary>
    /// Calculates the full reward for job completion.
    /// </summary>
    /// <returns></returns>
    private double CalculateReward()
    {
        return baseReward + (baseReward * Random.Range(0.0f, 1.0f));
    }

    /// <summary>
    /// Calculates the energy drain for the player when they complete the job.
    /// </summary>
    /// <returns></returns>
    private float CalculateEnergyDrain()
    {
        return Random.Range(0f, 1f);
    }

    /// <summary>
    /// Serves as the entry point for the script. This method will be called when the player completes the job minigame.
    /// </summary>
    public void CompleteJob()
    {
        //This method will be called when the player completes the job minigame.
		AddMoney();
		playerStats.DecreaseEnergy(CalculateEnergyDrain());
    }

    /// <summary>
    /// Stats the job by enabling the player's OnActivity status as well as enabling minigame mechanics based on the job. All minigames will be held within a Minigame GameObject which SHOULD be a child of the JobScript's GameObject.
    /// </summary>
    public void StartJob()
    {
        //Enable OnActivity status
        playerStats.OnActivity = true;

        //Enable the minigame GameObject
        minigame.gameObject.SetActive(true);
    }
        
}
