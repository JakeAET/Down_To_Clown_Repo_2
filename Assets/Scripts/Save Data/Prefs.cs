using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefs : MonoBehaviour
{
    //public PlayerMovement_Jake stuff;
    public void SaveGame()
    {
        //PlayerPrefs.SetInt("SavedInteger", stuff.gems);
        PlayerPrefs.Save();
        Debug.Log("Game data saved!");
    }
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("SavedInteger"))
        {
            //stuff.gems = PlayerPrefs.GetInt("SavedInteger");
            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
    }
}