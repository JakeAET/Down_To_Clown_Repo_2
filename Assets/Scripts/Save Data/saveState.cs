using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class saveState
{
    //Clowns
    public List<Clown> clownData;

    //Attractions
    public List<Attraction> attractionData;

    //Currency
    public int funnyMoney;
    public int clownCoins;
    public int whimsy;

    //Audio Manager
    public bool sfxMute = false;
    public bool musicMute = false;

    //Tutorial
    public int tutorialStep;

    //Time
    public long lastLoginTime;
    public long lastAdWatched;
}