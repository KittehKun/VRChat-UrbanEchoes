using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// This script handles the day/night cycle by rotating the directional light, rotating the skybox, and changing the color of the directional light.
[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class DayNightCycle : UdonSharpBehaviour
{
	[SerializeField] private Light sunLight; // Directional light representing the sun
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
		SetBrightness(sunIntensityCurve.Evaluate(startingHour / hoursInDay));
	}

	// Update is called once per frame
	void Update()
	{
		// Increment timeOfDay (assuming each frame represents 1 hour)
		timeOfDay += Time.deltaTime / timeScale;

		// Ensure timeOfDay stays within 24 hours
		timeOfDay %= hoursInDay;

		// Normalize timeOfDay to a value between 0 and 1
		float normalizedTime = timeOfDay / hoursInDay;

		// Sample the AnimationCurve to get the brightness value
		float brightness = sunIntensityCurve.Evaluate(normalizedTime);

		// Set the brightness
		SetBrightness(brightness);
	}

	// Method to set the brightness (placeholder)
	private void SetBrightness(float brightness)
	{
		if (sunLight != null)
		{
			sunLight.intensity = brightness;
		}
	}
}
