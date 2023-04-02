using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gachaManager : MonoBehaviour
{
    public static gachaManager Instance;

    private infoManager infoMang;
    private currencyManager currMang;
    private uiManager uiMang;
    private clownManager clownMang;

    [SerializeField] private int gachaCost;


    [SerializeField] private float commonHatchRate;
    [SerializeField] private float uncommonHatchRate;
    [SerializeField] private float rareHatchRate;

    private string clownPulled;

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
        currMang = GameObject.FindGameObjectWithTag("currManager").GetComponent<currencyManager>();
        uiMang = GameObject.FindGameObjectWithTag("uiManager").GetComponent<uiManager>();
        clownMang = GameObject.FindGameObjectWithTag("clownManager").GetComponent<clownManager>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void gachaFunction()
    {
        string clownID = "";

        if(infoMang.whimsy >= gachaCost)
        {
            infoMang.whimsy -= gachaCost;
            // pull random clown

            float randRarity = Random.Range(1, 101);

            //Debug.Log(randRarity);

            float rareMax = 100;
            float rareMin = 100 - rareHatchRate + 1;
            float uncommonMax = 100 - rareHatchRate;
            float uncommonMin = rareMin - uncommonHatchRate;
            float commonMax = uncommonMax - uncommonHatchRate;
            float commonMin = 1;

            if (randRarity >= rareMin && randRarity <= rareMax) // Rare Summon
            {
                clownID = "R" + Random.Range(1, infoMang.rareClowns + 1);
            }
            else if(randRarity >= uncommonMin && randRarity <= uncommonMax) // Uncommon Summon
            {
                clownID = "U" + Random.Range(1, infoMang.uncommonClowns + 1);
            }
            else if (randRarity >= commonMin && randRarity <= commonMax)// Common Summon
            {
                clownID = "C" + Random.Range(1, infoMang.commonClowns + 1);

            }

            Debug.Log(rareMin + ", " + rareMax);
            Debug.Log(uncommonMin + ", " + uncommonMax);
            Debug.Log(commonMin + ", " + commonMax);

            //Debug.Log(clownID);

            GameObject targetSprite = null;
            int i = 0;

            foreach (Clown c in infoMang.clowns)
            {
                if(c.ID == clownID)
                {
                    clownPulled = c.name;
                    targetSprite = clownMang.clownSprites[i];

                    if (!c.unlocked)
                    {
                        c.unlocked = true;
                        //Debug.Log(targetSprite.name + " = " + clownPulled);
                        uiMang.summonSequence(i, clownPulled, c.rarity, false);
                        Debug.Log("You got " + clownPulled + ", a " + c.rarity + " clown!");
                    }
                    else
                    {
                        //Debug.Log(targetSprite.name + " = " + clownPulled);
                        uiMang.summonSequence(i, clownPulled, c.rarity, true);
                        Debug.Log("You already have " + clownPulled + "! They've been converted into 1 clown coin.");
                    }
                }
                i++;
            }

        }
        else
        {
            Debug.Log("insufficient whimsy");
        }
    }

    public void summonComplete(bool duplicate)
    {
        if (duplicate)
        {
            currMang.clownCoinIncrease(1);
        }
    }
}
