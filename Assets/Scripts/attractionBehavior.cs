using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

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
    [SerializeField] TextMeshProUGUI unlockCostText;
    [SerializeField] TextMeshProUGUI coinCostText;
    [SerializeField] TextMeshProUGUI upgradeCostText;
    [SerializeField] TextMeshProUGUI levelText;

    private bool unlocked = false;
    public bool inUse = false;
    private int level = 1;
    public float ticketTimer;
    private int upgradeTicketCost;
    private int unlockTicketCost;
    private int unlockCoinCost;
    private float increaseRate;
    private int ticketProduction;

    // Start is called before the first frame update
    void Start()
    {

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
        level = thisAttraction.level;
        upgradeTicketCost = thisAttraction.upgradeCost;
        unlockTicketCost = thisAttraction.unlockTicketCost;
        unlockCoinCost = thisAttraction.unlockCoinCost;
        increaseRate = thisAttraction.increaseRate;
        ticketProduction = thisAttraction.ticketProduction;
        unlockCostText.text = "" + unlockTicketCost;
        coinCostText.text = "" + unlockCoinCost;
        upgradeCostText.text = "" + upgradeCost(level);
        levelText.text = "" + level;

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
                ticketTimer = 0;
            }

            if(workingClown == null || !thisAttraction.inUse)
            {
                inUse = false;
                thisAttraction.inUse = false;
            }
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
            updateAttraction();
            gameObject.tag = "attraction";
            infoMang.funnyMoney -= unlockTicketCost;
            infoMang.clownCoins -= unlockCoinCost;
            unlockPrompt.SetActive(false);
            upgradePrompt.SetActive(true);
            FindObjectOfType<audioManager>().Play("unlock");
        }
        else
        {
            FindObjectOfType<audioManager>().Play("invalid");
        }
    }

    public void upgradeAttraction()
    {
        if(infoMang.funnyMoney >= upgradeCost(level))
        {
            infoMang.funnyMoney -= upgradeCost(level);
            level++;
            upgradeCostText.text = "" + upgradeCost(level);
            levelText.text = "" + level;

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
        int newOutput = (level * ticketProduction);

        return newOutput;
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
            if (!inUse)
            {
                workingClown = clown;
                inUse = true;
                thisAttraction.inUse = true;
            }
        }
        else
        {
            workingClown = null;
            inUse = false;
            thisAttraction.inUse = false;
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
                return;
            }
        }
    }
}
