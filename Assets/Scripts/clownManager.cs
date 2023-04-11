using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clownManager : MonoBehaviour
{
    public GameObject[] clownSprites;
    private infoManager infoMang;

    private void Awake()
    {
        infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();

        int i = 0;

        foreach (Clown c in infoMang.clowns)
        {
            if (c.unlocked)
            {
                clownSprites[i].SetActive(true);
            }
            else
            {
                clownSprites[i].SetActive(false);
            }

            i++;
        }
    }
}
