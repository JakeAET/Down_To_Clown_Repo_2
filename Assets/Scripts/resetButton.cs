using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetButton : MonoBehaviour
{
    public void resetGame()
    {
        FindObjectOfType<saveManager>().wipeSave();
        FindObjectOfType<saveManager>().sceneChanger("Start Screen");
    }
}
