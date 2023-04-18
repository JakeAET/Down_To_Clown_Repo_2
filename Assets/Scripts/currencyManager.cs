using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;


public class currencyManager : MonoBehaviour
{
    public static currencyManager Instance;

    private infoManager infoMang;
    private tutorialManager tutorialMang;
    private attractionManager attractionMang;

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

    // Idle Currency
    [SerializeField] GameObject welcomePopup;
    [SerializeField] Text welcomeText;
    private int ticketsToClaim;

    private RenderTexture renderTexture;

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

        welcomePopup.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();
        tutorialMang = GameObject.FindGameObjectWithTag("tutorialManager").GetComponent<tutorialManager>();
        attractionMang = GameObject.FindGameObjectWithTag("attractionManager").GetComponent<attractionManager>();
        tapMeterUI.maxValue = tapGoal;
    }

    // Update is called once per frame
    void Update()
    {
        ticketText.text = "" + formatNumber(infoMang.funnyMoney);
        coinText.text = "" + formatNumber(infoMang.clownCoins);
        whimsyText.text = "" + formatNumber(infoMang.whimsy);

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
        infoMang.clickerTicketFlex();

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
        int randSide = UnityEngine.Random.Range(0,2);
        int side = edgeX;

        if(randSide == 0)
        {
            side = edgeX * -1;
        }

        int randHeight = UnityEngine.Random.Range(minY, maxY);

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
                if (a.ticketBonus)
                {
                    totalTickets += (Mathf.RoundToInt(a.level * a.ticketProduction * 1.2f) * secondsPassed);
                }
                else
                {
                    totalTickets += (a.level * a.ticketProduction * secondsPassed);
                }

                foreach (GameObject g in attractionMang.attractionSprites)
                {
                    attractionBehavior script = g.GetComponent<attractionBehavior>();
                    if (script.thisAttraction.name == a.name)
                    {
                        script.idleWhimsyProgress(secondsCounted);
                    }
                }
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

        if(totalTickets > 0)
        {
            welcomeText.text = "Your clowns earned <i><color=white>" + formatNumber(totalTickets) + " tickets</color></i> while you were away!";
            welcomePopup.SetActive(true);
        }
        else
        {
            welcomePopup.SetActive(false);
        }

        ticketsToClaim = totalTickets;
    }

    public void claimTickets()
    {
        infoMang.ticketIncrease(ticketsToClaim);
        ticketsToClaim = 0;
        welcomePopup.SetActive(false);
    }

    private string formatTime(float rawTime) // function called to convert timer's raw int value into usable timer strings
    {

        string hours = Mathf.Floor((rawTime % 216000) / 3600).ToString("0");
        string minutes = Mathf.Floor((rawTime % 3600) / 60).ToString("0");
        string seconds = Mathf.Floor(rawTime % 60).ToString("0");

        string output = hours + " hour(s), " + minutes + " minute(s), " + seconds + " second(s)";

        return output;
    }

    private string formatNumber(float rawNum)
    {
        double dNum = (double)rawNum;

        if (dNum > 999999999999999 || dNum < -999999999999999)
        {
            return dNum.ToString("0,,,,,.#####Q", CultureInfo.InvariantCulture);
        }
        else if (dNum > 999999999999 || dNum < -999999999999)
        {
            return dNum.ToString("0,,,,.####T", CultureInfo.InvariantCulture);
        }
        else if (dNum > 999999999 || dNum < -999999999)
        {
            return dNum.ToString("0,,,.###B", CultureInfo.InvariantCulture);
        }
        else if (dNum > 999999 || dNum < -999999)
        {
            return dNum.ToString("0,,.##M", CultureInfo.InvariantCulture);
        }
        else if (rawNum == 0)
        {
            return "0";
        }
        else
        {
            return dNum.ToString("#,#", CultureInfo.InvariantCulture);
        }
    }
}
