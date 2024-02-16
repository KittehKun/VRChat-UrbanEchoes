
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerHUD : UdonSharpBehaviour
{
    [Header("Player Stats Script")]
    [SerializeField] private PlayerStats playerStats; //Assigned in Unity | Important Player Stats are all accessible as public properties in the PlayerStats class

    [Header("Player Stats")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI sleepText;
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("HUD Canvas")]
    [SerializeField] private Transform hudCanvas;


    void Start()
    {
        if (!playerStats) playerStats = GetComponent<PlayerStats>(); //If the playerStats field is not assigned in Unity, script should be on the same GameObject as the PlayerStats component
    }

    void Update()
    {
        MoveHUD();
	}

    /// <summary>
    /// Updates all the HUD elements with the player's current stats. This method should be called whenever the player's stats change.
    /// </summary>
    public void UpdateHUD()
    {
        healthText.text = $"H: {playerStats.PlayerHealth}";
        energyText.text = $"E: {playerStats.PlayerEnergy:F0}"; //Displays with no decimal places
        sleepText.text = $"S: {playerStats.PlayerSleep:F0}"; //Displays with no decimal places
        moneyText.text = $"${playerStats.PlayerMoney:F2}"; //Displays with 2 decimal places
    }

    /// <summary>
    /// Updates the energy and sleep HUD elements with the player's current stats. This method should be called on tick.
    /// </summary>
    public void UpdateTick()
    {
        energyText.text = $"E: {playerStats.PlayerEnergy:F0}";
        sleepText.text = $"S: {playerStats.PlayerSleep:F0}";
    }

    private void MoveHUD()
    {
		// Get the player's head tracking data
		VRCPlayerApi.TrackingData headTrackingData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

		// Calculate the desired position (e.g., 1 meter in front of the head)
		Vector3 desiredPosition = headTrackingData.position + headTrackingData.rotation * Vector3.forward * 1f;

		// Smoothly move the HUD towards the desired position
		// Match the HUD rotation to the head rotation
		hudCanvas.SetPositionAndRotation(Vector3.Lerp(hudCanvas.position, desiredPosition, Time.deltaTime * 5f), headTrackingData.rotation);
	}
    
}
