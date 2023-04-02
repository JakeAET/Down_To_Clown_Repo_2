using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    //By Jake Gollub

    public string name; // this will be used for calling specific sounds

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;
    public bool loop;

    // type of sound
    public bool music;
    public bool sfx;

    [HideInInspector]
    public AudioSource source;
}