
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FitnessEquipmentCheckpoint : UdonSharpBehaviour
{
    [Header("Fitness Equipment Controller")]
    [SerializeField] private FitnessEquipmentController fitnessEquipmentController; //Used for altering the player stats depending on the gym equipment | Set in Inspector

    [Header("Audio SFX")]
    [SerializeField] private AudioSource audioSource; //Used for playing the sound of the checkpoint's completion noise
    [SerializeField] private AudioClip checkpointSFX; //The sound effect that plays when the player completes a checkpoint

    //Minigame timers
    private float deactivateTimer = 0; //Used to track the internal time between switching the checkpoint off and on, players hitting an off checkpoint lose the minigame
    private float deactivateTime; //The time limit for the minigame represented in seconds
    private bool isEnabled = false; //Used to check if the checkpoint is currently enabled
    private MeshRenderer meshRender; //The mesh renderer of the checkpoint
    [Header("Enabled/Disabled Materials")]
    [SerializeField] private Material enabledCheckpoint;
    [SerializeField] private Material disabledCheckpoint;

	//Switchs the checkpoint to the disabled material and sets the deactivate timer
    private void Update()
	{
		deactivateTimer += Time.deltaTime; //Incrememnt the deactivation timer

		//Disable the checkpoint if it's been enabled for too long
		if (isEnabled)
        {
			if (deactivateTimer >= deactivateTime)
            {
                meshRender.material = disabledCheckpoint;
                isEnabled = false;
                deactivateTimer = 0;
			}
		}
        else //Enable the checkpoint after a short delay
        {
            if(deactivateTimer >= deactivateTime)
            {
				meshRender.material = enabledCheckpoint;
				isEnabled = true;
				deactivateTimer = 0;
			}
        }
	}

	private void Start()
	{
        transform.gameObject.SetActive(false);
        meshRender = transform.GetComponent<MeshRenderer>();
        deactivateTime = Random.Range(0.5f, 1.25f); //Randomize the deactivate time
	}

	public override void Interact()
	{
        if (!isEnabled) fitnessEquipmentController.EndTraining(false);
        
        fitnessEquipmentController.CompleteCheckpoint();
        audioSource.clip = checkpointSFX; //Plays the checkpoint SFX
        audioSource.Play();
        DisableCheckpoint();
	}

    private void DisableCheckpoint()
    {
        transform.gameObject.SetActive(false);
    }

}
