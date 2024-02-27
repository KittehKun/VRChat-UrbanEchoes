
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class OfficeJobAnswerItem : UdonSharpBehaviour
{
    [Header("Goal Point")]
    [SerializeField] private Transform goalPoint; //The point the player must deliver the answer item to. Set in Unity Inspector.
    
    [Header("Office Job Script")]
    [SerializeField] private OfficeJobScript officeJobScript; //The OfficeJobScript that the answer item is associated with. Set in Unity Inspector.

    [Header("Audio SFX")]
    [SerializeField] private AudioSource audioSource; //The sound effect that plays when the player picks up the answer item. Set in Unity Inspector.
    [SerializeField] private AudioClip pickupSFX; //The sound effect that plays when the player picks up the answer item. Set in Unity Inspector.
    [SerializeField] private AudioClip popSFX; //The sound effect that plays when the answer item is delivered to the answer point. Set in Unity Inspector.

    [Header("Particle FX")]
    [SerializeField] private ParticleSystem taskCompletedFX; //The particle effect that plays when the answer item is delivered to the answer point. Set in Unity Inspector

    //Method to check if the answer item has been delivered to the answer point
    public void OnTriggerEnter(Collider other)
    {
		if (other == goalPoint.GetComponent<Collider>())
        {
			//If the answer item has been delivered to the answer point, play the particle effect and notify the OfficeJobScript
			audioSource.clip = popSFX;
            audioSource.Play();
            taskCompletedFX.transform.position = transform.position;
            taskCompletedFX.Emit(10);

            //Force drop the answer item if the player is holding it
            transform.GetComponent<VRCPickup>().Drop();

            int answer = int.Parse(transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text);

            if(officeJobScript.CheckAnswer(answer))
            {
                officeJobScript.TaskComplete();
            }
            else
            {
                officeJobScript.FailJob();
            }
            
		}
	}
}
