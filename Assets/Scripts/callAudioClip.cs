using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class callAudioClip : MonoBehaviour
{
    public void playClip(string name)
    {
        FindObjectOfType<audioManager>().Play(name);
    }
}
