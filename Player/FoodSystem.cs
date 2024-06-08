//This script will be used as a base for all of the food in the game. This script handles buffs, debuffs, and the player's hunger.

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FoodSystem : UdonSharpBehaviour
{    
    //Player Stats Reference
    private PlayerStats playerStats; //Assigned in Start method | Attached to the Player GameObject in Hierarchy. | Used for applying buffs, debuffs, draining hunger, and healing the player.
    
    void Start()
    {
        playerStats = this.transform.GetComponent<PlayerStats>();
		if (!playerStats) Debug.LogError("[FOOD SYSTEM]: Error! PlayerStats Reference is missing. Please add this script to where the PlayerStats script will be.")
;    }

	private void Update()
	{
		
	}
}

public enum StatusType
{
	Buff,
	Debuff,
	Heal
}
