using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// This script handles the day/night cycle by rotating the directional light, rotating the skybox, and changing the color of the directional light.
[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class DayNightCycle : UdonSharpBehaviour
{
	[SerializeField] private Light sunLight; // Directional light representing the sun
	[SerializeField] private Light moonLight; // Directional light representing the moon
	[SerializeField] private Material skybox; // Skybox material
	[SerializeField] private Gradient ambientColorGradient; // Gradient defining ambient color over time
	[SerializeField] private int startingHour = 10; // Starting hour of the day (By default 10AM unless specified in the Inspector)
	[SerializeField] private AnimationCurve sunIntensityCurve; // Curve defining sun intensity over time
	[SerializeField] private float timeScale = 30f; // Time scale (how many seconds in real time is equivalent to 1 hour in game time)

	// Number of hours in a day
	private const float hoursInDay = 24f;

	// Current time of day in hours (start at 10:00 AM)
	[SerializeField] private float timeOfDay = 0;

	private void Start()
	{
		// Set the starting time of day
		timeOfDay = startingHour;

		// Set the initial brightness
		SetBrightness(sunLight, sunIntensityCurve.Evaluate(startingHour / hoursInDay));
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		// Calculate the time of day
		CalculateTime();

		// Set the brightness
		SetBrightness(sunLight, CalculateSunBrightness());
		SetBrightness(moonLight, CalculateMoonBrightness());

		// Set the ambient colors
		SetAmbientColors();

		// Set the rotation and exposure of the skybox
		SetSkyboxSettings();

		// Rotate the sunLight
		RotateSunLight(CalculateNormalizedTime());
	}

	private void CalculateTime()
	{
		// Increment timeOfDay (assuming each frame represents 1 hour)
		timeOfDay += Time.deltaTime / timeScale;

		// Ensure timeOfDay stays within 24 hours
		timeOfDay %= hoursInDay;
	}

	private float CalculateSunBrightness()
	{
		return sunIntensityCurve.Evaluate(CalculateNormalizedTime());
	}

	private float CalculateMoonBrightness()
	{
		return 1f - CalculateSunBrightness();
	}

	private float CalculateNormalizedTime()
	{
		return timeOfDay / hoursInDay;
	}

	// Method to set the brightness (placeholder)
	private void SetBrightness(Light targetLight, float brightness)
	{
		targetLight.intensity = brightness;
	}

	private void SetAmbientColors()
	{
		// Set the ambient color based on the time of day
		Color ambientSkyColor = ambientColorGradient.Evaluate(CalculateNormalizedTime());
		Color ambientEquatorColor = ambientSkyColor * 0.8f;
		Color ambientGroundColor = ambientSkyColor * 0.5f;

		RenderSettings.ambientSkyColor = ambientSkyColor;
		RenderSettings.ambientEquatorColor = ambientEquatorColor;
		RenderSettings.ambientGroundColor = ambientGroundColor;
	}

	private void SetSkyboxSettings()
	{
		// Set the rotation of the skybox based on the time of day
		RenderSettings.skybox.SetFloat("_Rotation1", CalculateNormalizedTime() * 360f);
		RenderSettings.skybox.SetFloat("_Rotation2", CalculateNormalizedTime() * 360f);

		// Set the float value "_Blend" depending on the time of day - This is a custom property added to the skybox shader
		// Calculate the blend value for smooth transition between day and night
		float blend = 1f - Mathf.PingPong(CalculateNormalizedTime() * 2f, 1f);
		RenderSettings.skybox.SetFloat("_Blend", blend);

	}

	// Method to rotate the sunLight based on the time of day - Indirectly rotates the moonLight as well as moonLight is a child of sunLight
	private void RotateSunLight(float normalizedTime)
	{
		// Calculate the angle to rotate the sunLight
		float angle;
		if (timeOfDay < 12f)
		{
			angle = Mathf.Lerp(-90f, 90f, normalizedTime / 0.5f); // Rotate from -90 degrees (12AM) to 0 degrees (12PM)
		}
		else
		{
			angle = Mathf.Lerp(90f, -90f, (normalizedTime - 0.5f) / 0.5f); // Rotate from 90 degrees (12PM) to -90 degrees (12AM)
		}

		// Set the rotation of the sunLight - The moonLight will rotate with it
		sunLight.transform.rotation = Quaternion.Euler(angle, 0f, 0f);
	}
}
