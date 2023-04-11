using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attractionManager : MonoBehaviour
{
    public GameObject[] attractionSprites;
    private infoManager infoMang;

    private void Awake()
    {
        infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();
    }
}
