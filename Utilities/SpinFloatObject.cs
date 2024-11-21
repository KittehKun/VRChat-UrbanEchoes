
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// This utility class is used for spinning and floating a GameObject based on set parameters for the local player. If the float is set to 0, a warning will be displayed in the message log and the object will default to floating at a value of 1.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SpinFloatObject : UdonSharpBehaviour
{
    [SerializeField] private float spinSpeed = 0.0f; // By default, spin at 0 degrees per second. Must be set within the inspector.
    [SerializeField] private float floatSpeed = 0.0f; // By default, float at 0 units per second. Must be set within the inspector.
    [SerializeField] private float floatHeight = 0.0f; // By default, float at 0 units. Must be set within the inspector.

    void Start()
    {
        if (spinSpeed == 0.0f)
        {
            Debug.LogWarning("SpinFloatObject: No spin speed set. Defaulting to 20 degree per second.");
            spinSpeed = 20.0f;
        }

        if (floatSpeed == 0.0f)
        {
            Debug.LogWarning("SpinFloatObject: No float speed set. Defaulting to 1 unit per second.");
            floatSpeed = 1.0f;
        }

        if (floatHeight == 0.0f)
        {
            Debug.LogWarning("SpinFloatObject: No float height set. Defaulting to 0.001 unit.");
            floatHeight = 0.001f;
        }
    }

    private void Update()
    {
        SpinObject();
        FloatObject();
    }

    private void SpinObject()
    {
        transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
    }

    private void FloatObject()
    {
        // Float the object based on the object's current Y position in a Sin wave.
        float newY = transform.position.y + floatHeight * Mathf.Sin(Time.time * floatSpeed);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
