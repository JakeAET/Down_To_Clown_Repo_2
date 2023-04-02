using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class sceneManager : MonoBehaviour
{
    private saveManager saveMang;

    // Start is called before the first frame update
    void Start()
    {
        saveMang = GameObject.FindGameObjectWithTag("saveManager").GetComponent<saveManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void sceneChanger (string sceneName)
    {
        saveMang = GameObject.FindGameObjectWithTag("saveManager").GetComponent<saveManager>();
        SceneManager.LoadScene(sceneName);
    }
}
