
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OfficeJobScript : UdonSharpBehaviour
{
    //Variables - Set in Unity Inspector depnding on the job
    [SerializeField] private string jobName;
    [SerializeField] private double basePay;
    private double taskPay;
    private double bonusPay;
    private int jobWave;

    [SerializeField] private int[] jobRequirements = new int[5]; //Set in Unity Inspector | The array is as follows: [0] = Intelligence, [1] = Fitness, [2] = Cooking, [3] = Creativity, [4] = Charisma

    //Wave Settings
    private bool timerStarted = false;
    private float waveTimeLimit = 150;
    private float waveTimer;

    [Header("Job Task Items")]
    [SerializeField] private Transform computerScreen; //Used for disabling and enabling the computer screen
    [SerializeField] private Transform jobPickup; //Used for disabling and enabling the job pickup
    [SerializeField] private Transform jobAnswerSpawn; //Used for spawning the job answers
    private Vector3[] answerSpawnPointsPos = new Vector3[3]; //Used for storing the job answer spawn points
    private Vector3[] answerSpawnPointsRot = new Vector3[3]; //Used for storing the job answer spawn rotations
    [SerializeField] private TextMeshProUGUI[] jobAnswersText; //Used for storing the job answers
    [SerializeField] private TextMeshProUGUI firstNumber; //Used for displaying the first number in the math equation
    [SerializeField] private TextMeshProUGUI mathOperator; //Used for displaying the math operator in the math equation
    [SerializeField] private TextMeshProUGUI secondNumber; //Used for displaying the second number in the math equation
    private int answer; //Used for storing the answer to the math equation

    [Header("Job SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jobStartSFX;
    [SerializeField] private AudioClip jobCompleteSFX;
    [SerializeField] private AudioClip jobFailSFX;

    [Header("Player Stats")]
    [SerializeField] private PlayerStats playerStats; //Used for getting the player stats

    [Header("Player HUD")]
    [SerializeField] private PlayerHUD playerHUD; //Used for updating the player HUD

	void Start()
    {
        //Set the spawn points of the job answers to their current positions
        for (int i = 0; i < 3; i++)
        {
            answerSpawnPointsPos[i] = jobAnswerSpawn.GetChild(i).position;
            answerSpawnPointsRot[i] = jobAnswerSpawn.GetChild(i).eulerAngles;
		}
        Debug.Log($"Found {jobAnswerSpawn.childCount} children in JobAnswerSpawns"); //Expected: 3
    }

	private void Update()
	{
		if (timerStarted)
		{
			//Increase the wave timer
			waveTimer -= Time.deltaTime;

			//Enable the timer text on the player's HUD
			playerHUD.EnableTimerText();

			//Update the player's HUD with the time remaining in the wave
			playerHUD.UpdateTimer(waveTimer);

			//If the wave timer reaches the time limit, the job is failed
			if (waveTimer <= 0)
			{
				FailJob();
			}
		}
	}

	private void AcceptJob()
    {
        bool requirementsMet = true;

        //Check if the player meets the job requirements
        for (int i = 0; i < jobRequirements.Length; i++)
        {
			if (playerStats.PlayerSkills[i] < jobRequirements[i])
            {
				requirementsMet = false;
				break;
			}
		}

        if(requirementsMet && !playerStats.OnJob)
        {
            BeginJob();
            playerHUD.DisplayJobAcceptNotification();
        }
        else
        {
			// Display the job declined notification
			playerHUD.DisplayJobDeclinedNotification();

			//Reset the job
			ResetJob();
		}
    }

    private void BeginJob()
    {
        if (playerStats.OnJob) return;

        //Disable the job pickup
        jobPickup.GetComponent<MeshRenderer>().enabled = false;
        transform.GetComponent<BoxCollider>().enabled = false;

        //Enable the job answer spawn
        jobAnswerSpawn.gameObject.SetActive(true);

        //Enable the computer screen
        computerScreen.gameObject.SetActive(true);

        //Play the job start SFX
        audioSource.clip = jobStartSFX;
        audioSource.Play();

        //Set the wave timer to the time limit
        waveTimer = waveTimeLimit;

        //Set the timer started to true
        timerStarted = true;

        //Set the player's on job status to true
        playerStats.OnJob = true;

        //Update the player's HUD with the job name
        playerHUD.UpdateJobTitle("Office", 1);
        
        GenerateMathEquation();
    }

    private void GenerateMathEquation()
    {
        ResetAnswerPositions();
        
        int firstNum = Random.Range(-20, 20);
        int secondNum = Random.Range(-20, 20);
        int mathOperator = Random.Range(0, 4);

        switch (mathOperator)
        {
            //Addition
            case 0:
                answer = firstNum + secondNum;
                firstNumber.text = firstNum.ToString();
                this.mathOperator.text = "+";
                secondNumber.text = secondNum.ToString();
                break;
            //Subtraction
            case 1:
                answer = firstNum - secondNum;
				firstNumber.text = firstNum.ToString();
				this.mathOperator.text = "-";
				secondNumber.text = secondNum.ToString();
				break;
            //Multiplication - Round to the nearest whole number
            case 2:
                answer = firstNum * secondNum;
                answer = Mathf.RoundToInt(answer);
                firstNumber.text = firstNum.ToString();
                this.mathOperator.text = "*";
                secondNumber.text = secondNum.ToString();
                break;
            //Division - Round to the nearest whole number
            case 3:
                answer = firstNum / secondNum;
                answer = Mathf.RoundToInt(answer);
                firstNumber.text = firstNum.ToString();
                this.mathOperator.text = "/";
                secondNumber.text = secondNum.ToString();
                break;
            default:
                answer = firstNum + secondNum;
                firstNumber.text = firstNum.ToString();
                this.mathOperator.text = "+";
                secondNumber.text = secondNum.ToString();
                break;
        }

        UpdateAnswerObjects(answer);
    }

    private void ResetAnswerPositions()
    {
        //Reset the position of the answer objects to their spawn points
        for(int i = 0; i < 3; i++)
        {
            jobAnswerSpawn.GetChild(i).position = answerSpawnPointsPos[i];
            jobAnswerSpawn.GetChild(i).eulerAngles = answerSpawnPointsRot[i];
        }
	}

    private void UpdateAnswerObjects(int answer)
    {
        //Change the text of one of the answer objects to the correct answer
        int correctAnswerIndex = Random.Range(0, jobAnswersText.Length);
		jobAnswersText[correctAnswerIndex].text = answer.ToString();

        //Change the text of the other answer objects to random numbers
        for (int i = 0; i < jobAnswersText.Length; i++)
        {
			if (i != correctAnswerIndex)
            {
				jobAnswersText[i].text = Random.Range(-50, 50).ToString();
			}
		}
    }

    public void FailJob()
    {
        computerScreen.gameObject.SetActive(false);
		jobAnswerSpawn.gameObject.SetActive(false);

		playerHUD.DisplayJobFailedNotification();
        ResetJob();

        audioSource.clip = jobFailSFX;
        audioSource.Play();

        timerStarted = false;

        playerStats.OnJob = false;

		//Reenable the job pickup
		jobPickup.GetComponent<MeshRenderer>().enabled = true;
		transform.GetComponent<BoxCollider>().enabled = true;
    }

    private void ResetJob()
    {
        waveTimeLimit = 150;
        bonusPay = 0;
        jobWave = 1;
        playerHUD.DisableTimerText();
    }

    public bool CheckAnswer(int answer)
    {
		if (answer == this.answer)
        {
			return true;
		}
		else
        {
			return false;
		}
	}

    public void TaskComplete()
    {
        taskPay = Random.Range(0.1f, 0.26f);
        bonusPay = basePay * (jobWave * 0.1) + Random.Range(0.1f, 1f);
        playerStats.AddMoney(bonusPay + taskPay);
        playerHUD.UpdateMoney();
        playerStats.DecreaseEnergy();

        jobWave++;

        //Decrease the time limit by 25%
        waveTimeLimit *= 0.75f;

        //Play the job complete SFX
        audioSource.clip = jobCompleteSFX;
        audioSource.Play();

        //Generate a new math equation
        GenerateMathEquation();
    }

	public override void Interact()
	{
        AcceptJob();
	}
}
