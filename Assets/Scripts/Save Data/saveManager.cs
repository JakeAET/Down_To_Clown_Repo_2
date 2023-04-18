using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// following tutorial: https://youtu.be/LBs6qOgCDOY

public class saveManager : MonoBehaviour
{
    public static saveManager Instance { set; get; }
    public saveState state;
    private infoManager infoMang;
    private currencyManager currencyMang;
    private tutorialManager tutorialMang;
    private bool initialized = false;
    private bool managerCheck = false;

    [SerializeField] Clown[] defaultClowns;
    [SerializeField] Attraction[] defaultAttractions;

    private void Awake()
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
        infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();
        Load();

        Debug.Log(saveAssist.Serialize<saveState>(state));
    }

    private void Update()
    {
        // manual save
        if (Input.GetKeyDown("s"))
        {
            Save();
        }

        //manual save
        if (Input.GetKeyDown("r"))
        {
            // reset save file
            wipeSave();
        }

        if (managerCheck && GameObject.FindGameObjectWithTag("currManager") != null && GameObject.FindGameObjectWithTag("tutorialManager") != null)
        {
            currencyMang = GameObject.FindGameObjectWithTag("currManager").GetComponent<currencyManager>();
            tutorialMang = GameObject.FindGameObjectWithTag("tutorialManager").GetComponent<tutorialManager>();

            long period = 10L * 60L * 1000L * 10000L;
            long timeStamp = System.DateTime.Now.Ticks + period;
            currencyMang.idleCurrency(state.lastLoginTime, timeStamp);
            state.lastLoginTime = timeStamp;

            infoMang.initializeClowns();

            managerCheck = false;
        }
    }

    // Save the state to player prefs
    public void Save()
    {
        int i = 0;

        foreach (Attraction a in infoMang.attractions)
        {
            state.attractionData[i] = a;
            i++;
        }

        i = 0;

        foreach (Clown c in infoMang.clowns)
        {
            state.clownData[i] = c;
            i++;
        }

        state.funnyMoney = infoMang.funnyMoney;
        state.clownCoins = infoMang.clownCoins;
        state.whimsy = infoMang.whimsy;

        if (initialized)
        {
            state.tutorialStep = tutorialMang.tutorialStep;
        }

        long period = 10L * 60L * 1000L * 10000L;
        long timeStamp = System.DateTime.Now.Ticks + period;
        state.lastLoginTime = timeStamp;

        PlayerPrefs.SetString("save", saveAssist.Serialize<saveState>(state));
        Debug.Log(saveAssist.Serialize<saveState>(state));
    }

    //Load previous save state from player prefs
    public void Load()
    {
        if (PlayerPrefs.HasKey("save"))
        {
            state = saveAssist.Deserialize<saveState>(PlayerPrefs.GetString("save"));

            int i = 0;

            foreach (Attraction a in state.attractionData) // Ensure all attraction data aligns with the default attractions
            {
                a.increaseRate = defaultAttractions[i].increaseRate;
                a.unlockTicketCost = defaultAttractions[i].unlockTicketCost;
                a.unlockCoinCost = defaultAttractions[i].unlockCoinCost;
                a.ticketProduction = defaultAttractions[i].ticketProduction;
                a.whimsyProduction = defaultAttractions[i].whimsyProduction;
                i++;
            }

            i = 0;

            foreach (Attraction a in state.attractionData)
            {
                infoMang.attractions[i] = a;
                i++;
            }

            i = 0;

            foreach (Clown c in state.clownData)
            {
                infoMang.clowns[i] = c;
                i++;
            }

            infoMang.funnyMoney = state.funnyMoney;
            infoMang.clownCoins = state.clownCoins;
            infoMang.whimsy = state.whimsy;
        }
        else
        {
            state = new saveState();

            List<Attraction> tempAttraction = new List<Attraction>();
            List<Clown> tempClown = new List<Clown>();


            foreach (Attraction a in infoMang.attractions)
            {
                //Debug.Log(a.name);
                tempAttraction.Add(a);
            }

            state.attractionData = tempAttraction;

            foreach (Clown c in infoMang.clowns)
            {
                //Debug.Log(c.name);
                tempClown.Add(c);
            }

            state.clownData = tempClown;

            Save();
            Debug.Log("no save file found, creating a new one");
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }


    public void sceneChanger(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

        if (sceneName == "Game Screen" && !initialized)
        {
            managerCheck = true;
            initialized = true;
        }
    }

    public void wipeSave()
    {
        // reset save file
        infoMang.funnyMoney = 0;
        infoMang.clownCoins = 0;
        infoMang.whimsy = 0;
        state.funnyMoney = 0;
        state.clownCoins = 0;
        state.whimsy = 0;
        tutorialMang.tutorialStep = 0;
        state.lastAdWatched = 0;
        state.lastLoginTime = 0;
        state.musicMute = false;
        state.sfxMute = false;

        foreach (Attraction a in infoMang.attractions)
        {
            a.unlocked = false;
            a.inUse = false;
            a.level = 1;
        }

        foreach (Clown c in infoMang.clowns)
        {
            c.unlocked = false;
            c.working = false;
            c.targetAttraction = Vector3.zero;
        }

        Save();
    }
}