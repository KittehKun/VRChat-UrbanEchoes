
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SpinFloatObject : UdonSharpBehaviour
{
    void Update()
    {
        //Rotate the object around the Z axis
        transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * 100);
    }
}
