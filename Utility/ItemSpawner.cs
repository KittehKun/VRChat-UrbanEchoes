
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ItemSpawner : UdonSharpBehaviour
{
    [SerializeField] private Transform spawnLocation; // Set in Unity Inspector | The location where the item will spawn.
    [SerializeField] private GameObject itemPrefab; // Set in Unity Inspector | The item that will spawn.

	private void Update()
	{
        SpinFloatItem();
	}

    private void SpinFloatItem()
    {
        // Spin the item
		itemPrefab.transform.Rotate(100 * Time.deltaTime * Vector3.up);

        // Float the item
		itemPrefab.transform.position = new Vector3(itemPrefab.transform.position.x, itemPrefab.transform.position.y + Mathf.Sin(Time.time) * 0.01f, itemPrefab.transform.position.z);
    }

	private void SpawnItem()
    {
        //Move the item to the spawn location and enable it
        itemPrefab.transform.position = spawnLocation.position;
        itemPrefab.SetActive(true);
    }
    
    public override void Interact()
	{
        SpawnItem();
	}
}
