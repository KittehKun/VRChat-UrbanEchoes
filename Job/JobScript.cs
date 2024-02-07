
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class JobScript : UdonSharpBehaviour
{

    //Variables - Set in Unity Inspector depending on the job
    [SerializeField] private string _jobTitle;
	[SerializeField] private double _salary;
	[SerializeField] private int[] _jobRequirements;
    [SerializeField] private string _jobDescription;
    [SerializeField] private PlayerStats _playerStats;
    
    void Start()
    {
        
    }

    public void ApplyForJob()
    {
		bool meetsRequirements = true;

		// Check if the player meets the requirements
		for (int i = 0; i < _playerStats.PlayerSkills.Length; i++)
		{
			if (_playerStats.PlayerSkills[i] < _jobRequirements[i])
			{
				meetsRequirements = false;
				break; // Exit loop if any requirement is not met
			}
		}

		if (meetsRequirements)
		{
			// If the player meets the requirements, set the job title and salary
			_playerStats.SetJob(_jobTitle);
		}
		else
		{
			// If the player does not meet the requirements, display a message to console - will later be displayed to the player in game
			Debug.Log("You do not meet the requirements for this job.");
		}

	}
}
