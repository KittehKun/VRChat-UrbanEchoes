﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PurchaseProperty : UdonSharpBehaviour
{
    [Header("Player Stats Reference")]
    [SerializeField] private PlayerStats _playerStats;

    [Header("Property Audio Reference")]
    [Tooltip("The unique Property Audio script within the scene. Only one unique instance the Property Audio script should exist in the scene.")]
    [SerializeField] private PropertyAudio _propertyAudio;

    [Header("Target Property")]
    [SerializeField] private Transform _targetProperty;

    [Header("Property Info and Price")]
    [Tooltip("0 = Apartment, 1 = House | TODO: Add businesses")]
    [SerializeField] private int _propertyType; // 0 = Apartment, 1 = House | TODO: Add businesses
    [SerializeField] private double _propertyPrice;

    void Start()
    {
        if (!_playerStats)
        {
            Debug.LogError("PurchaseProperty: PlayerStats not found. Please assign the PlayerStats script to the _playerStats field.");
            _playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStats>(); // Only one unique instance of PlayerStats should exist in the scene.
        }

        if (_propertyPrice == 0) Debug.LogWarning("PurchaseProperty: No property price set. Please assign a value to the _propertyPrice field. Otherwise, this property is assumed to be free.");
    }

    /// <summary>
    /// Handles the interaction event by attempting to purchase the property. This is the entry point for the player to interact with the property.
    /// If the purchase is valid, the player's money is deducted and a success sound is played.
    /// Otherwise, an error sound is played.
    /// </summary>
    private void BuyProperty()
    {
        if (IsValidPurchase())
        {
            _playerStats.SubtractMoney(_propertyPrice); // Subtract the property price from the player's money.
            _propertyAudio.PlayPurchasePropertySFX();

            return;
        }

        _propertyAudio.PlayErrorSFX();
    }

    /// <summary>
    /// Checks if the player has enough money to purchase the property.
    /// </summary>
    /// <returns>True if the player has enough money, false otherwise.</returns>
    /// 
    private bool IsValidPurchase()
    {
        return _playerStats.PlayerMoney >= _propertyPrice;
    }

    /// <summary>
    /// Handles the interaction event by attempting to purchase the property. This is the entry point for the player to interact with the property.
    /// If the purchase is valid, the player's money is deducted and a success sound is played.
    /// Otherwise, an error sound is played.
    /// </summary>
    public override void Interact()
    {
        BuyProperty();
    }

    private void OnValidate()
    {
        if(this.gameObject.GetComponent<Collider>() == null) Debug.LogWarning("PurchaseProperty: No collider found. Please assign a collider to the GameObject otherwise this object will be unable to be interacted with.");
    }
}
