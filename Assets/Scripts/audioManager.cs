using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public string copyPaste = "FindObjectOfType<audioManager>().Play('sound name');"; // line to paste in other scripts to call a sound

    public Sound[] sounds; // sound class containing name, volume, pitch, loop bool, music bool, and the actual audio clip

    public static audioManager Instance;
    private saveManager saveMang;

    [SerializeField] bool home;
    [SerializeField] bool main;

    public bool sfxMute;
    public bool musicMute;

    void Awake()
    {

        if (Instance != null && Instance != this)
        {

            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        // adds an audio source in the inspector for every audio source in the array, allowing you to manipulate each sound from the manager
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        saveMang = GameObject.FindGameObjectWithTag("saveManager").GetComponent<saveManager>();
        sfxMute = saveMang.state.sfxMute;
        musicMute = saveMang.state.musicMute;

        if (musicMute)
        {
            muteMusic(true);
        }
        else
        {
            if (home)
            {
                Play("home screen");
            }
            if (main)
            {
                Play("main screen");
            }
        }

        if (sfxMute)
        {
            muteSFX(true);
        }
    }

    private void Update()
    {

    }

    // method to call for playing a certain sound
    public void Play(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
    // method to call for pausing a certain sound
    public void Pause(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        s.source.Pause();
    }

    public void muteMusic(bool mute)
    {
        foreach (Sound s in sounds)
        {
            if(s.music == true)
            {
                s.source.mute = mute;
            }
        }

        musicMute = mute;
        saveMang.state.musicMute = musicMute;
    }

    public void muteSFX(bool mute)
    {
        foreach (Sound s in sounds)
        {
            if (s.sfx == true)
            {
                s.source.mute = mute;
            }
        }

        sfxMute = mute;
        saveMang.state.sfxMute = sfxMute;
    }
}
