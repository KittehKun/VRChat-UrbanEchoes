
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TaskItem : UdonSharpBehaviour
{
	[SerializeField] private Transform goalPoints; //The points the player must deliver the job task items to. Set in Unity Inspector.
	[SerializeField] private JobScript jobScript; //The JobScript that the task item is associated with. Set in Unity Inspector.

	//Method to check if the task item has been delivered to a goal point
	public void OnTriggerEnter(Collider other)
	{
		Collider[] colliders = goalPoints.GetComponentsInChildren<Collider>(); //Returns only active objects

		//Check if the task item has been delivered to a goal point
		foreach (Collider collider in colliders)
		{
			if (other == collider)
			{				
				//If the task item has been delivered to a goal point, disable the task item and update the job script
				jobScript.TaskCompleted();
			}
		}
	}
}
