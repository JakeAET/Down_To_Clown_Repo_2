using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bookCards : MonoBehaviour
{
    private infoManager infoMang;
    [SerializeField] Button[] cards;
    [SerializeField] GameObject[] locks;


    // Start is called before the first frame update
    void Start()
    {
        infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();
        updateCardInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if(infoMang == null)
        {
            infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();
        }
    }

    public void updateCardInfo()
    {
        if(infoMang != null)
        {
            int i = 0;

            foreach (Clown c in infoMang.clowns)
            {
                if (c.unlocked)
                {
                    cards[i].interactable = true;
                    locks[i].SetActive(false);
                }
                else
                {
                    cards[i].interactable = false;
                    locks[i].SetActive(true);
                }
                i++;
            }
        }
    }
}
