
using System.Threading.Tasks.Sources;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class GenericTaskItem : UdonSharpBehaviour
{
    [Header("Target Job Script")]
    [SerializeField] private UdonBehaviour targetJob; //Assigned in Unity | The UdonBehaviour of the job that this task item is associated with.
    
    [Header("Audio SFX")]
    [SerializeField] private AudioSource itemSFX; //Set in Unity Inspector | The audio source that will play all item audio clips.
    [SerializeField] private AudioClip itemPickupSFX; //Set in Unity Inspector | The sound effect that will play when the player picks up an item.

    [Header("Player Stats")]
    [SerializeField] private PlayerStats playerStats; //Assigned in Unity | Important Player Stats are all accessible as public properties in the PlayerStats class
    
    [Header("Player HUD")]
    [SerializeField] private PlayerHUD playerHUD; //Assigned in Unity | PlayerHUD is separate from this script and is used to update the player's HUD.

	private void Start()
	{
        transform.gameObject.SetActive(false);
	}

	/// <summary>
	/// The entry method for the task item. This method is called when the player interacts with the task item.
	/// </summary>
	public override void Interact()
    {
        PickupItem();
    }

    /// <summary>
    /// A method that is called when the player interacts with the task item. This method is responsible for handling the task item pickup.
    /// </summary>
    private void PickupItem()
    {
        PlayItemSFX();
        AddBonusToPlayer();
        DisablePickup();
        TaskComplete();
    }

    /// <summary>
    /// Plays the item SFX when the player picks up the item.
    /// </summary>
    private void PlayItemSFX()
    {
        itemSFX.clip = itemPickupSFX;
        itemSFX.Play();
    }
    
    /// <summary>
    /// Calculates the bonus pay and adds it to the player's money.
    /// </summary>
    private void AddBonusToPlayer()
    {
        double payBonus = Random.Range(0.01f, 0.5f);
        playerStats.AddMoney(payBonus);
        playerHUD.UpdateMoneyToAdd(payBonus);
        playerHUD.UpdateMoney();
    }

    /// <summary>
    /// Disables the pickup item so that it cannot be picked up again.
    /// </summary>
    private void DisablePickup()
    {
        transform.GetComponent<MeshRenderer>().enabled = false;
        transform.GetComponent<Collider>().enabled = false;
    }

    /// <summary>
    /// Reenables the pickup item so that it can be picked up again.
    /// </summary>
    private void EnablePickup()
    {
        transform.GetComponent<MeshRenderer>().enabled = true;
        transform.GetComponent<Collider>().enabled = true;
    }

    /// <summary>
    /// Sends a custom event to the target job script to notify it that the task has been completed. Target job must have a public method called "CompleteTask".
    /// </summary>
    private void TaskComplete()
    {
        targetJob.SendCustomEvent("CompleteTask");
        EnablePickup();
    }
}
