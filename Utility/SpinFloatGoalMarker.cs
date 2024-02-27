
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SpinFloatGoalMarker : UdonSharpBehaviour
{
    private float yOffset;

    private void Start()
    {
		yOffset = transform.localPosition.y;
	}

    void Update()
    {
        //Rotate the object around the Z axis
        transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * 100);

        //Move the object up and down on the local Y axis using a sine wave from the current position
        transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Sin(Time.time) * 0.1f + yOffset, transform.localPosition.z);
    }
}
