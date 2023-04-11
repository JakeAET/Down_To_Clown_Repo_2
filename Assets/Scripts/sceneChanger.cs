using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneChanger : MonoBehaviour
{
    public void changeScene(string sceneName)
    {
        FindObjectOfType<saveManager>().sceneChanger(sceneName);
    }
}
