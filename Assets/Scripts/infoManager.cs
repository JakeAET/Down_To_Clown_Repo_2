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
            funnyMoney += 500;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            clownCoins += 1;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            whimsy += 100;
        }
    }

    public void ticketIncrease(int num)
    {
        funnyMoney += num;
    }

    public void duplicateClown(int num)
    {
        clownCoins += num;
    }

    public void whimsyIncrease(int num)
    {
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

            if (c.unlocked)
            {
                GameObject.FindGameObjectWithTag("clownManager").GetComponent<clownManager>().clownSprites[i].SetActive(true);
            }
            else
            {
                GameObject.FindGameObjectWithTag("clownManager").GetComponent<clownManager>().clownSprites[i].SetActive(false);
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
}
