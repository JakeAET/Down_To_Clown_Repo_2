using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class currencyManager : MonoBehaviour
{
    public static currencyManager Instance;

    private infoManager infoMang;
    private tutorialManager tutorialMang;

    [SerializeField] TextMeshProUGUI ticketText;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshProUGUI whimsyText;
    [SerializeField] Slider tapMeterUI;

    //Continuous tapping
    private int multiplyer = 1;
    private bool multiplyerOn = false;
    private float multiplyerTimer;
    [SerializeField] float multiplyerDuration;
    public float tapTimer = 0;
    public float downtimeTimer = 0;
    private int tapMeter;
    [SerializeField] private int tapGoal;

    //Balloon Spawn
    [SerializeField] private GameObject balloon;
    [SerializeField] int maxY;
    [SerializeField] int minY;
    [SerializeField] int edgeX;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {

            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();
        tutorialMang = GameObject.FindGameObjectWithTag("tutorialManager").GetComponent<tutorialManager>();
        tapMeterUI.maxValue = tapGoal;
    }

    // Update is called once per frame
    void Update()
    {
        ticketText.text = "" + infoMang.funnyMoney;
        coinText.text = "" + infoMang.clownCoins;
        whimsyText.text = "" + infoMang.whimsy;

        if (tapTimer < 2)
        {
            tapTimer += Time.deltaTime;
        }
        else if (tapTimer >= 2 && (tapMeter - Mathf.CeilToInt(Time.deltaTime) > 0))
        {
            downtimeTimer += Time.deltaTime;

            if(downtimeTimer >= 0.1f)
            {
                tapMeter--;
                downtimeTimer = 0;
            }
        }

        if (multiplyerOn)
        {
            multiplyerTimer += Time.deltaTime;

            if(multiplyerTimer >= multiplyerDuration)
            {
                multiplyerOn = false;
                multiplyerTimer = 0;
                multiplyer = 1;
            }
        }

        tapMeterUI.value = tapMeter;
    }
    
    public void hornClick()
    {
        infoMang.ticketIncrease(10 * multiplyer);

        if(tapMeter < tapGoal)
        {
            tapMeter++;
        }

        if (tapMeter == (tapGoal / 2) && tutorialMang.step == tutorialManager.tutorial.Clicker2 && tutorialMang.halfTicketGoal == false)
        {
            tutorialMang.halfwayTicketGoal();
            Debug.Log("halfway goal");
        }

        if (tapMeter == tapGoal)
        {
            tapGoalReached();
        }

        tapTimer = 0;

        Debug.Log(tapMeter);
    }

    public void clownCoinIncrease(int num)
    {
        infoMang.duplicateClown(num);
        coinText.text = "" + infoMang.clownCoins;
    }

    public void whimsyIncrease(int num)
    {
        infoMang.whimsyIncrease(num);
        whimsyText.text = "" + infoMang.whimsy;
    }

    public void multiplyerActive(int num)
    {
        multiplyer = num;
        multiplyerOn = true;
    }

    private void tapGoalReached()
    {
        int randSide = Random.Range(0,2);
        int side = edgeX;

        if(randSide == 0)
        {
            side = edgeX * -1;
        }

        int randHeight = Random.Range(minY, maxY);

        Vector3 spawnPoint = new Vector3(side, randHeight, 0);

        if (tutorialMang.step == tutorialManager.tutorial.Clicker2)
        {
            tutorialMang.ticketGoal();
        }

        Instantiate(balloon, spawnPoint, Quaternion.identity.normalized);
        //Debug.Log("Goal Reached");
        tapMeter = 0;
    }

    public void idleCurrency(long lastLogin, long currentTime)
    {
        infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();

        int totalTickets = 0;
        int secondsPassed = (int)((int)(currentTime / 10000000) - (lastLogin / 10000000));
        int secondsCounted = 0;
        bool overCapLimit = false;

        if (secondsPassed > 86400)
        {
            overCapLimit = true;
            secondsCounted = 86400;
        }
        else
        {
            secondsCounted = secondsPassed;

        }

        foreach (Attraction a in infoMang.attractions)
        {
            if (a.inUse)
            {
                totalTickets += (a.level * a.ticketProduction * secondsPassed);
            }
        }

        if (overCapLimit)
        {
            Debug.Log("Time difference since last login: " + formatTime(secondsPassed) + ", but was capped at 24hrs. Tickets gained: " + totalTickets);
        }
        else
        {
            Debug.Log("Time difference since last login: " + formatTime(secondsPassed) + ". Tickets gained: " + totalTickets);
        }

        infoMang.ticketIncrease(totalTickets);
    }

    private string formatTime(float rawTime) // function called to convert timer's raw int value into usable timer strings
    {

        string hours = Mathf.Floor((rawTime % 216000) / 3600).ToString("0");
        string minutes = Mathf.Floor((rawTime % 3600) / 60).ToString("0");
        string seconds = Mathf.Floor(rawTime % 60).ToString("0");

        string output = hours + " hour(s), " + minutes + " minute(s), " + seconds + " second(s)";

        return output;
    }

}
