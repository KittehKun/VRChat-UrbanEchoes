
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class TaskItem : UdonSharpBehaviour
{
	[Header("Goal Points")]
	[SerializeField] private Transform goalPoints; //The points the player must deliver the job task items to. Set in Unity Inspector.

	[Header("Job Script")]
	[SerializeField] private JobScript jobScript; //The JobScript that the task item is associated with. Set in Unity Inspector.

	[Header("Audio SFX")]
	[SerializeField] private AudioSource audioSource; //The sound effect that plays when the player picks up the task item. Set in Unity Inspector.
	[SerializeField] private AudioClip pickupSFX; //The sound effect that plays when the player picks up the task item. Set in Unity Inspector.

	[Header("Particle FX")]
	[SerializeField] private ParticleSystem taskCompletedFX; //The particle effect that plays when the task item is delivered to a goal point. Set in Unity Inspector.

	//Method to check if the task item has been delivered to a goal point
	public void OnTriggerEnter(Collider other)
	{
		Collider[] colliders = goalPoints.GetComponentsInChildren<Collider>(); //Returns only active objects

		//Check if the task item has been delivered to a goal point
		foreach (Collider collider in colliders)
		{
			if (other == collider)
			{
				//If the task item has been delivered to a goal point, play the particle effect and notify the JobScript
				ResetTaskItem();
				return;
			}
		}
	}

	/// <summary>
	/// Respawns the task item at the original spawn point.
	/// </summary>
	private void ResetTaskItem()
	{
		//Play the particle effect at the task item's current position
		taskCompletedFX.transform.position = transform.position;
		taskCompletedFX.Emit(10);

		transform.GetComponent<VRCPickup>().Drop(); //Force drop the task item if the player is holding it
		Transform taskSpawner = this.transform.parent; //Get the task spawner that the task item was spawned from
		transform.SetPositionAndRotation(taskSpawner.position, taskSpawner.rotation); //Respawn the task item at the original spawn point

		jobScript.TaskCompleted(); //Notify the JobScript that the task item has been delivered to a goal point
	}

	public override void OnPickup()
	{
		audioSource.clip = pickupSFX;
		audioSource.Play();
	}
}
