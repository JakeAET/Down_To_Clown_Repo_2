using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;
using System.Globalization;

public class attractionBehavior : MonoBehaviour
{
    private infoManager infoMang;

    [SerializeField] string attractionName;
    public Attraction thisAttraction;

    public GameObject clownPos;
    public GameObject workingClown;
    public GameObject attractionWalkPoint;
    public GameObject activeNPC;

    [SerializeField] GameObject lockIcon;
    [SerializeField] GameObject unlockPrompt;
    [SerializeField] GameObject upgradePrompt;
    [SerializeField] Button upgradeButton;
    [SerializeField] TextMeshProUGUI unlockCostText;
    [SerializeField] TextMeshProUGUI coinCostText;
    [SerializeField] TextMeshProUGUI upgradeCostText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI ticketOutputText;

    private bool unlocked = false;
    public bool inUse = false;
    private int level = 1;
    public float ticketTimer;
    private int upgradeTicketCost;
    private int unlockTicketCost;
    private int unlockCoinCost;
    private float increaseRate;
    private int ticketProduction;
    private float clownBonusMultiplyer = 1.2f;
    private int currentTicketOutput;

    [SerializeField] private SpriteRenderer attractionSprite;
    [SerializeField] private SpriteRenderer boothSprite;
    [SerializeField] private GameObject levelIcon;
    [SerializeField] private GameObject ticketOutputIcon;
    [SerializeField] private GameObject ticketEffect;
    [SerializeField] private GameObject bonusTicketEffect;

    // Whimsy Meter
    [SerializeField] Slider whimsyMeter;
    [SerializeField] GameObject sliderHandle;
    [SerializeField] GameObject whimsyMeterObj;
    [SerializeField] int whimsyMeterGoal; // in seconds
    [SerializeField] GameObject whimsyCollect;
    private int whimsyMeterNum;
    private bool whimsyMeterFull = false;

    // Start is called before the first frame update
    void Start()
    {
        whimsyCollect.SetActive(false);
        infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();
        foreach (Attraction a in infoMang.attractions)
        {
            if (a.name == attractionName)
            {
                thisAttraction = a;
            }
        }

        waypointManager waypointMang = GameObject.FindGameObjectWithTag("waypointManager").GetComponent<waypointManager>();
        foreach (GameObject a in waypointMang.attractionPoints)
        {
            if (a.GetComponent<waypoint>().targetAttractionName == attractionName)
            {
                attractionWalkPoint = a;
            }
        }

        unlocked = thisAttraction.unlocked;
        inUse = thisAttraction.inUse;
        if(thisAttraction.level > 999)
        {
            thisAttraction.level = 1000;
        }
        level = thisAttraction.level;
        upgradeTicketCost = thisAttraction.upgradeCost;
        unlockTicketCost = thisAttraction.unlockTicketCost;
        unlockCoinCost = thisAttraction.unlockCoinCost;
        increaseRate = thisAttraction.increaseRate;
        ticketProduction = thisAttraction.ticketProduction;
        unlockCostText.text = formatNumber(unlockTicketCost);
        coinCostText.text = formatNumber(unlockCoinCost);
        upgradeCostText.text = formatNumber(upgradeCost(level));
        ticketOutputText.text = "0/s";
        if (level > 999)
        {
            levelText.text = "lvl MAX";
            upgradeButton.interactable = false;
        }
        else
        {
            levelText.text = "lvl " + level;
        }
        whimsyMeterNum = thisAttraction.whimsyMeterProgress;
        whimsyMeter.maxValue = whimsyMeterGoal;
        whimsyMeterObj.SetActive(false);

        if (unlocked)
        {
            unlockPrompt.SetActive(false);
            upgradePrompt.SetActive(true);
            gameObject.tag = "attraction";
            lockIcon.SetActive(false);
        }
        else
        {
            unlockPrompt.SetActive(true);
            upgradePrompt.SetActive(false);
            gameObject.tag = "disabled";
            lockIcon.SetActive(true);
            levelIcon.SetActive(false);
            ticketOutputIcon.SetActive(false);
            boothSprite.material.color = new Color(0.6f, 0.6f, 0.6f, 0.6f);
            attractionSprite.material.color = new Color(0.6f, 0.6f, 0.6f, 0.6f);
        }

        if (inUse)
        {
            whimsyMeterObj.SetActive(true);
            ticketEffect.SetActive(true);

            if(workingClown != null)
            {
                if (thisAttraction.targetClown == workingClown.name)
                {
                    bonusTicketEffect.SetActive(true);
                }
            }

            currentTicketOutput = ticketOutput(level);
            ticketOutputText.text = currentTicketOutput + "/s";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inUse)
        {
            ticketTimer += Time.deltaTime;

            if(ticketTimer >= 1f)
            {
                infoMang.ticketIncrease(ticketOutput(level));

                if(whimsyMeterNum < whimsyMeterGoal)
                {
                    whimsyMeterNum++;
                    thisAttraction.whimsyMeterProgress = (int)whimsyMeterNum;
                }

                ticketTimer = 0;
            }

            if(workingClown == null || !thisAttraction.inUse)
            {
                inUse = false;
                thisAttraction.inUse = false;
            }

            if(whimsyMeterNum == whimsyMeterGoal && !whimsyMeterFull)
            {
                whimsyMeterFull = true;
                whimsyMeterMaxed();
            }
            else
            {
                sliderHandle.SetActive(true);
            }

            if (ticketOutputText.text == "0/s")
            {
                ticketOutputText.text = currentTicketOutput + "/s";
            }

            if (workingClown != null)
            {
                if (thisAttraction.targetClown == workingClown.name && !bonusTicketEffect.activeInHierarchy)
                {
                    bonusTicketEffect.SetActive(true);
                }
            }

            whimsyMeter.value = whimsyMeterNum;
        }
    }

    //private bool checkClown(GameObject clown)
    //{
        //string name = clown.GetComponent<clownBehavior>().thisClown.name;

        //if(name == thisAttraction.targetClown)
        //{
            //return true;
        //}
        //else
        //{
            //return false;
        //}
    //}

    public void unlockAttraction()
    {
        if (infoMang.funnyMoney >= unlockTicketCost && infoMang.clownCoins >= unlockCoinCost)
        {
            unlocked = true;
            lockIcon.SetActive(false);
            levelIcon.SetActive(true);
            ticketOutputIcon.SetActive(false);
            updateAttraction();
            gameObject.tag = "attraction";
            infoMang.funnyMoney -= unlockTicketCost;
            infoMang.clownCoins -= unlockCoinCost;
            unlockPrompt.SetActive(false);
            upgradePrompt.SetActive(true);
            boothSprite.material.color = Color.white;
            attractionSprite.material.color = Color.white;
            whimsyMeterObj.SetActive(true);
            FindObjectOfType<audioManager>().Play("unlock");
        }
        else
        {
            FindObjectOfType<audioManager>().Play("invalid");
        }
    }

    public void upgradeAttraction()
    {
        if(infoMang.funnyMoney >= upgradeCost(level) && level < 1000)
        {
            infoMang.funnyMoney -= upgradeCost(level);
            level++;
            upgradeCostText.text = formatNumber(upgradeCost(level));

            if(level > 999)
            {
                levelText.text = "lvl MAX";
                upgradeButton.interactable = false;
            }
            else
            {
                levelText.text = "lvl " + level;
            }

            currentTicketOutput = ticketOutput(level);
            ticketOutputText.text = currentTicketOutput + "/s";
            updateAttraction();

            FindObjectOfType<audioManager>().Play("upgrade");
        }
        else
        {
            FindObjectOfType<audioManager>().Play("invalid");
        }
    }

    private int ticketOutput(int level)
    {
        int output = 0;

        if(workingClown != null)
        {
            if (thisAttraction.targetClown == workingClown.name) // target clown bonus
            {
                currentTicketOutput = output = Mathf.RoundToInt(level * ticketProduction * clownBonusMultiplyer);
            }
        }

        currentTicketOutput = output = (level * ticketProduction);
        return output;
    }

    private int upgradeCost(int level)
    {
        int newOutput = Mathf.RoundToInt(upgradeTicketCost * Mathf.Pow(level,increaseRate));

        newOutput = (Mathf.CeilToInt(newOutput / 100)) * 100;

        return newOutput;
    }

    public void clownWorking(GameObject clown, bool isWorking)
    {
        if (isWorking)
        {
            workingClown = clown;
            inUse = true;
            thisAttraction.inUse = true;
            whimsyMeterObj.SetActive(true);
            ticketEffect.SetActive(true);
            if (thisAttraction.targetClown == workingClown.name)
            {
                bonusTicketEffect.SetActive(true);
            }

            currentTicketOutput = ticketOutput(level);
            ticketOutputText.text = currentTicketOutput + "/s";
        }
        else
        {
            workingClown = null;
            inUse = false;
            thisAttraction.inUse = false;
            whimsyMeterObj.SetActive(false);
            ticketEffect.SetActive(false);
            bonusTicketEffect.SetActive(false);
            ticketOutputText.text = "0/s";
        }

        updateAttraction();
    }

    void updateAttraction()
    {
        foreach (Attraction a in infoMang.attractions)
        {
            if (a.name == attractionName)
            {
                a.level = level;
                a.unlocked = unlocked;
                a.inUse = inUse;

                if(workingClown != null)
                {
                    if (a.targetClown == workingClown.name)
                    {
                        a.ticketBonus = true;
                    }
                }

                return;
            }
        }
    }

    public void idleWhimsyProgress(int secondsPassed)
    {
        whimsyMeterNum = thisAttraction.whimsyMeterProgress;

        if(secondsPassed >= secondsPassed + whimsyMeterNum)
        {
            whimsyCollect.SetActive(true);
        }
        else
        {
            if(whimsyMeterNum + secondsPassed > whimsyMeterGoal)
            {
                whimsyMeterNum = whimsyMeterGoal;
            }
            else
            {
                whimsyMeterNum += secondsPassed;
            }
        }
    }

    void whimsyMeterMaxed()
    {
        sliderHandle.SetActive(false);
        whimsyCollect.SetActive(true);
        whimsyMeterFull = false;
    }

    public void resetWhimsyMeter()
    {
        infoMang.whimsyIncrease(thisAttraction.whimsyProduction);
        whimsyMeterNum = 0;
        foreach (Attraction a in infoMang.attractions)
        {
            if (a.name == attractionName)
            {
                a.whimsyMeterProgress = 0;
                return;
            }
        }
        whimsyCollect.SetActive(false);
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
        else if (rawNum > 999 || rawNum < -999)
        {
            return dNum.ToString("0,.#K", CultureInfo.InvariantCulture);
        }
        else if(rawNum == 0)
        {
            return "0";
        }
        else
        {
            return dNum.ToString("#,#", CultureInfo.InvariantCulture);
        }
    }
}
