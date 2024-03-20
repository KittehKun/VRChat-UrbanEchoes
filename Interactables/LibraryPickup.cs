
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class LibraryPickup : UdonSharpBehaviour
{
    [Header("Library Minigame")]
    [SerializeField] private LibraryMinigame libraryMinigame;

    private float INITIAL_Y_POSITION;
    private readonly float FLOAT_SPEED = 0.5f;
    private readonly float FLOAT_HEIGHT = 0.1f;
    private readonly float ROTATION_SPEED = 2.0f;

	private void Start()
	{
        transform.gameObject.SetActive(false);
        INITIAL_Y_POSITION = transform.position.y;
	}

    private void Update()
    {
		SpinFloatObject();
	}

	public override void Interact()
    {
        libraryMinigame.BookCollected();
        DisablePickup();
    }

    private void DisablePickup()
    {
        transform.gameObject.SetActive(false);
    }

    private void SpinFloatObject()
    {
        //Spin the object on the local Y axis
        transform.Rotate(0, ROTATION_SPEED, 0);

        //Float the object on the local Y axis based on the initial position
        float newY = INITIAL_Y_POSITION + Mathf.Sin(Time.time * FLOAT_SPEED) * FLOAT_HEIGHT;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
