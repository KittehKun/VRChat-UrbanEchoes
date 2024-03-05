
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

	public override void Interact()
	{
		fitnessEquipmentController.CompleteCheckpoint();
	}
}
