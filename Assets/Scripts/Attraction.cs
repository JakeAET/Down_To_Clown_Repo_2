using UnityEngine;

[System.Serializable]
public class Attraction
{
    // Scripting Info
    public string name;
    public string targetClown;
    public bool unlocked;
    public bool inUse;
    public int level = 1;
    public float increaseRate;
    public int unlockTicketCost;
    public int unlockCoinCost;
    public int upgradeCost;
    public int ticketProduction;
    public int whimsyMeterProgress;
    public int whimsyProduction;
    public bool ticketBonus = false;
}
