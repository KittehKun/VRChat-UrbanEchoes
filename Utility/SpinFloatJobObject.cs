
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SpinFloatJobObject : UdonSharpBehaviour
{	
	private void Update()
	{
		SpinObject();
		FloatObject();
	}

	/// <summary>
	/// Spins the object around the Y axis.
	/// </summary>
	private void SpinObject()
	{
		transform.Rotate(new Vector3(0, 1, 0), Time.deltaTime * 100);
	}

	/// <summary>
	/// Floats the object up and down.
	/// </summary>
	private void FloatObject()
	{
		float floatSpeedMultiplier = 2.0f; //The speed at which the object floats
		float floatAmplitude = 0.1f; //The amplitude of the float | How high the object floats
		
		float floatOffset = Mathf.Sin(Time.time * floatSpeedMultiplier) * floatAmplitude; //Calculates the offset based on time

		transform.localPosition = new Vector3(transform.localPosition.x, floatOffset, transform.localPosition.z);
	}
}
