
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SpinFloatGoalMarker : UdonSharpBehaviour
{
    private float xOffset;

    private void Start()
    {
		xOffset = transform.localPosition.x;
	}

    void Update()
    {
        //Rotate the object around the Z axis
        transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * 100);

        //Move the object up and down on the local X axis using a sine wave from the current position
        transform.localPosition = new Vector3(Mathf.Sin(Time.time) * 0.25f + xOffset, transform.localPosition.y, transform.localPosition.z);
    }
}
