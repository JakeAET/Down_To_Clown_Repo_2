using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class bookPage : MonoBehaviour
{
    private infoManager infoMang;
    [SerializeField] string clownName;
    [SerializeField] Button[] chapters;
    [SerializeField] GameObject[] locks;
    [SerializeField] GameObject purchasePrompt;
    [SerializeField] GameObject chapterPurchase;
    [SerializeField] Button chapterPurchaseButton;
    [SerializeField] TMP_Text chapterPurchaseText;
    private Clown targetClown;
    // Start is called before the first frame update
    void Start()
    {
        infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();
        foreach (Clown c in infoMang.clowns)
        {
            if (c.name == clownName)
            {
                targetClown = c;
            }
        }

        updateInfo();

        if (targetClown.unlockedInfo + 1f == chapters.Length)
        {
            chapterPurchase.SetActive(false);
        }
        else
        {
            chapterPurchase.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void unlockNextChapter()
    {
        if(infoMang.clownCoins >= 1f)
        {
            infoMang.clownCoins--;
            foreach (Clown c in infoMang.clowns)
            {
                if (c.name == clownName)
                {
                    c.unlockedInfo++;

                    if(c.unlockedInfo + 1f == chapters.Length)
                    {
                        chapterPurchase.SetActive(false);
                    }
                    else
                    {
                        chapterPurchase.SetActive(true);
                    }
                }
            }
            updateInfo();
            purchasePrompt.SetActive(false);
        }
    }

    void updateInfo()
    {
        int i = 0;

        foreach (Button c in chapters)
        {
            if (i <= targetClown.unlockedInfo)
            {
                c.interactable = true;
            }
            else
            {
                c.interactable = false;
            }

            i++;
        }

        i = 0;

        foreach (GameObject l in locks)
        {
            if (i <= targetClown.unlockedInfo)
            {
                l.SetActive(false);
            }
            else
            {
                l.SetActive(true);
            }

            i++;
        }

        if (infoMang.clownCoins == 0)
        {
            chapterPurchaseText.text = "Insufficient Coins to Unlock";
            chapterPurchaseButton.interactable = false;
        }
        else
        {
            chapterPurchaseText.text = "Unlock Next Chapter";
            chapterPurchaseButton.interactable = true;
        }
    }
}
