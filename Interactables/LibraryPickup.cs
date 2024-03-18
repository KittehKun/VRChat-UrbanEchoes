
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class LibraryPickup : UdonSharpBehaviour
{
    [Header("Library Minigame")]
    [SerializeField] private LibraryMinigame libraryMinigame;

	private void Start()
	{
        transform.gameObject.SetActive(false);
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
}
