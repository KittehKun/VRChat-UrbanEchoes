
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

	public override void Interact()
    {
        PickupItem();
    }

    private void PickupItem()
    {
        PlayItemSFX();
        AddBonusToPlayer();
        DisablePickup();
    }

    private void PlayItemSFX()
    {
        itemSFX.clip = itemPickupSFX;
        itemSFX.Play();
    }
    
    private void AddBonusToPlayer()
    {
        double payBonus = Random.Range(0.01f, 0.5f);
        playerStats.AddMoney(payBonus);
        playerHUD.UpdateMoneyToAdd(payBonus);
    }

    private void DisablePickup()
    {
        transform.GetComponent<MeshRenderer>().enabled = false;
        transform.GetComponent<Collider>().enabled = false;
    }

    public void EnablePickup()
    {
        transform.GetComponent<MeshRenderer>().enabled = true;
        transform.GetComponent<Collider>().enabled = true;
    }
}
