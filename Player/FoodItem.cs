//This script will be responsible for cloning objects that the player can consume as food items to restore hunger. Food items will be outside of the world in a hidden area and will be cloned into the place where the player can pick them up. For example, a shopkeeper's counter.

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FoodItem : UdonSharpBehaviour
{
    //FoodSystem Reference
    [Header("FoodSystem Script")]
    [SerializeField] private FoodSystem foodSystem; //Reference to the FoodSystem script.
    
    //Food Item Properties
    [field: SerializeField]
    public int FoodTier { get; private set; } = 1; //The tier of the food item. Ranges from 1 to 3. The higher the tier, the more hunger it restores. Set in the inspector based on the FoodTier item.

    void Start()
    {
        
    }

    /// <summary>
    /// Method called when the player interacts with the food item with it being held and interacted with.
    /// </summary>
    public override void Interact()
    {
        //Restore hunger based off of the food tier
        foodSystem.RestoreHunger(FoodTier);
    }
}

public enum FoodTier
{
    Snack = 1,
    LightMeal = 2,
    NormalMeal = 3,
    Feast = 4,
    THEBIGLOAD = 5
}