
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class RadioController : UdonSharpBehaviour
{
    [SerializeField] private AudioSource audioSource; //Attached to the same GameObject
    [SerializeField] private AudioClip[] radioSongs; //Assigned in Inspector
    private bool isOn = true; //Default behavior is to have the radio on

    // Play the next song in the playlist
    public void NextSong()
    {
		if (isOn)
        {
			audioSource.Stop();
			audioSource.clip = radioSongs[Random.Range(0, radioSongs.Length)];
			audioSource.Play();
		}
	}

    // Turn the radio on or off
	public void ToggleRadio()
	{
		isOn = !isOn;
		if (isOn)
		{
			audioSource.Play();
		}
		else
		{
			audioSource.Stop();
		}
	}

	private void Start()
	{
		audioSource.clip = radioSongs[Random.Range(0, radioSongs.Length)];
		audioSource.Play();
	}

	private void Update()
	{
		if (!audioSource.isPlaying && isOn)
		{
			NextSong();
		}
	}

	public override void Interact()
	{
		//Flip the radio on or off
		ToggleRadio();
	}
}
