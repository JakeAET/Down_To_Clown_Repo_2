using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneChanger : MonoBehaviour
{
    public void changeScene(string sceneName)
    {
        if(sceneName == "Start Screen")
        {
            FindObjectOfType<saveManager>().Save();
        }
        FindObjectOfType<saveManager>().sceneChanger(sceneName);
    }
}
