using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class infoManager : MonoBehaviour
{
    public static infoManager Instance;

    //Information to store:

    //Clowns
    public Clown[] clowns;

    //Attractions
    public Attraction[] attractions;

    //Currency
    public int funnyMoney;
    public int clownCoins;
    public int whimsy;

    public int rareClowns;
    public int uncommonClowns;
    public int commonClowns;

    private bool canFlex = true;
    private float flexTimer;

    public long lastLoginTime;

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

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            funnyMoney += 100000000;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            clownCoins += 1;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            whimsy += 100;
        }

        if (!canFlex)
        {
            flexTimer += Time.deltaTime;
            if (flexTimer >= 0.2f)
            {
                canFlex = true;
                flexTimer = 0f;
            }
        }
    }

    public void ticketIncrease(int num)
    {
        funnyMoney += num;
    }

    public void slowTicketIncrease(int num)
    {
        StartCoroutine(slowUpdateTickets(0.001f, num));
        GameObject ticketCounter = GameObject.FindGameObjectWithTag("ticketCounter");
        LeanTween.scale(ticketCounter, new Vector3(1.1f, 1.1f, 1.1f), 0.1f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.scale(ticketCounter, new Vector3(1f, 1f, 1f), 0.3f).setEase(LeanTweenType.easeInOutCubic).setDelay(0.1f);
    }

    public void clickerTicketFlex()
    {
        if (canFlex)
        {
            canFlex = false;
            GameObject ticketCounter = GameObject.FindGameObjectWithTag("ticketCounter");
            LeanTween.scale(ticketCounter, new Vector3(1.1f, 1.1f, 1.1f), 0.1f).setEase(LeanTweenType.easeInOutCubic);
            LeanTween.scale(ticketCounter, new Vector3(1f, 1f, 1f), 0.3f).setEase(LeanTweenType.easeInOutCubic).setDelay(0.1f);
        }
    }

    public void duplicateClown(int num)
    {
        clownCoins += num;
    }

    public void whimsyIncrease(int num)
    {
        GameObject whimsyCounter = GameObject.FindGameObjectWithTag("whimsyCounter");
        LeanTween.scale(whimsyCounter, new Vector3(1.2f, 1.2f, 1.2f), 0.3f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.scale(whimsyCounter, new Vector3(1f, 1f, 1f), 0.3f).setEase(LeanTweenType.easeInOutCubic).setDelay(0.3f);
        StartCoroutine(slowUpdateWhimsy(0.01f, num));
    }

    public void initializeClowns()
    {
        int i = 0;

        foreach (Clown c in clowns)
        {

            if (c.rarity == "rare")
            {
                rareClowns++;
                c.ID = "R" + rareClowns;
            }
            else if (c.rarity == "uncommon")
            {
                uncommonClowns++;
                c.ID = "U" + uncommonClowns;
            }
            else if (c.rarity == "common")
            {
                commonClowns++;
                c.ID = "C" + commonClowns;
            }

            i++;
        }
    }

    private IEnumerator slowUpdateWhimsy(float waitTime, float num)
    {
        for (int i = 0; i < num; i++)
        {
            yield return new WaitForSeconds(waitTime);
            whimsy++;
        }
    }

    private IEnumerator slowUpdateTickets(float waitTime, float num)
    {
        for (int i = 0; i < num; i++)
        {
            //yield return new WaitForSeconds(waitTime);
            yield return null;
            funnyMoney++;
        }
    }
}
