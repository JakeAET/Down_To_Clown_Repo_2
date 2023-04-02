using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class uiManager : MonoBehaviour
{
    public static uiManager Instance;

    private clownManager clownMang;
    private gachaManager gachaMang;
    private tutorialManager tutorialMang;
    private touchCam tCam;

    [SerializeField] GameObject bottomUI;
    [SerializeField] GameObject topUI;
    [SerializeField] GameObject summonUI;
    [SerializeField] GameObject settingsUI;

    // Summon Elements
    [SerializeField] GameObject[] dupeSprite;
    private bool summoning;
    [SerializeField] Button summonButton;
    [SerializeField] GameObject leftCurtain;
    [SerializeField] GameObject rightCurtain;
    [SerializeField] GameObject drumCombo;
    [SerializeField] GameObject drum;
    [SerializeField] GameObject drumStick1;
    [SerializeField] GameObject drumStick2;
    [SerializeField] GameObject clownCoin;
    [SerializeField] GameObject clownCoinCounter;
    private GameObject targetClown;
    private GameObject targetDupe;
    private float summonTimer;
    private bool currentDuplicate;
    private bool firstSummon = false;

    private bool settingsActive = false;

    // Settings Buttons
    private bool sfxMute;
    private bool musicMute;
    [SerializeField] Image sfxIcon;
    [SerializeField] Image musicIcon;
    [SerializeField] Sprite sfxOn;
    [SerializeField] Sprite sfxOff;
    [SerializeField] Sprite musicOn;
    [SerializeField] Sprite musicOff;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        clownMang = GameObject.FindGameObjectWithTag("clownManager").GetComponent<clownManager>();
        gachaMang = GameObject.FindGameObjectWithTag("gachaManager").GetComponent<gachaManager>();
        tutorialMang = GameObject.FindGameObjectWithTag("tutorialManager").GetComponent<tutorialManager>();
        tCam = GameObject.FindGameObjectWithTag("touchCam").GetComponent<touchCam>();

        drumCombo.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);

        sfxMute = FindObjectOfType<audioManager>().sfxMute;
        musicMute = FindObjectOfType<audioManager>().musicMute;

        if (sfxMute)
        {
            sfxIcon.overrideSprite = sfxOff;
        }
        else
        {
            sfxIcon.overrideSprite = sfxOn;
        }

        if (musicMute)
        {
            musicIcon.overrideSprite = musicOn;
        }
        else
        {
            musicIcon.overrideSprite = musicOff;
        }
    }

    // Update is called once per frame
    void Update()
    {


        if (summoning)
        {
            summonTimer += Time.deltaTime;
        }

        if (summonTimer > 3f && firstSummon)
        {
            drumCombo.GetComponent<Animator>().SetTrigger("stopDrum");

            if (!currentDuplicate)
            {
                gachaMang.summonComplete(false);
                summonButton.interactable = true;
            }
            firstSummon = false;
        }

        if(summonTimer > 5f && !currentDuplicate)
        {
            targetClown.GetComponent<clownBehavior>().walkToPath = true;
            summoning = false;
            summonTimer = 0f;

            if (tutorialMang.step == tutorialManager.tutorial.Summon1 && !tutorialMang.summonClicked)
            {
                tutorialMang.summonClick();
            }
        }

        if(summonTimer > 6f && currentDuplicate)
        {
            summonButton.interactable = true;
            gachaMang.summonComplete(true);
            LeanTween.moveLocal(clownCoin, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
            clownCoin.transform.localScale = new Vector3(0, 0, 0);
            clownCoin.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            Destroy(targetDupe);
            summoning = false;
            summonTimer = 0f;

            if (tutorialMang.step == tutorialManager.tutorial.Summon1 && !tutorialMang.summonClicked)
            {
                tutorialMang.summonClick();
            }
        }
    }

    public void book(bool active)
    {
        if (active)
        {
            
        }
        else
        {
            if (settingsActive)
            {
                settings(false);
            }
        }
    }

    public void bottomBar(bool active)
    {
        if (active)
        {
            LeanTween.moveLocal(bottomUI, new Vector3(0, -500, 0), 1f).setEase(LeanTweenType.easeOutCubic);
        }
        else
        {
            LeanTween.moveLocal(bottomUI, new Vector3(0, -1036, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
        }
    }

    public void summon(bool active)
    {
        if (active)
        {
            if (tutorialMang.step == tutorialManager.tutorial.Whimsy && !tutorialMang.tentClicked)
            {
                tutorialMang.tentClick();
            }

            tCam.zoomLockCam("summon");

            LeanTween.moveLocal(bottomUI, new Vector3(0, -1036, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
            LeanTween.moveLocal(summonUI, new Vector3(0, -564, 0), 1f).setEase(LeanTweenType.easeInOutCubic);

            LeanTween.scale(drumCombo, new Vector3(0.7f, 0.7f, 0.7f), 0.5f).setEase(LeanTweenType.easeInBounce).setDelay(0.5f);
            LeanTween.alpha(drum.GetComponent<RectTransform>(), 1, 0.5f).setDelay(0.5f);
            LeanTween.alpha(drumStick1.GetComponent<RectTransform>(), 1, 0.5f).setDelay(0.5f);
            LeanTween.alpha(drumStick2.GetComponent<RectTransform>(), 1, 0.5f).setDelay(0.5f);
        }
        else
        {
            tCam.zoomLockCam("default");

            LeanTween.moveLocal(bottomUI, new Vector3(0, -500, 0), 1f).setEase(LeanTweenType.easeOutCubic);
            LeanTween.moveLocal(summonUI, new Vector3(0, -1100, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);

            LeanTween.scale(drumCombo, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutBounce);
            LeanTween.alpha(drum.GetComponent<RectTransform>(), 0, 0.5f);
            LeanTween.alpha(drumStick1.GetComponent<RectTransform>(), 0, 0.5f);
            LeanTween.alpha(drumStick2.GetComponent<RectTransform>(), 0, 0.5f);

            if (settingsActive)
            {
                settings(false);
            }
        }
    }

    public void build(bool active)
    {
        if (active)
        {

        }
        else
        {

            if (settingsActive)
            {
                settings(false);
            }
        }
    }

    public void advertisement(bool active)
    {
        if (active)
        {

        }
        else
        {

            if (settingsActive)
            {
                settings(false);
            }
        }
    }

    public void settings(bool active)
    {
        if (active)
        {
            LeanTween.moveLocal(settingsUI, new Vector3(0, -295, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
            LeanTween.moveLocal(topUI, new Vector3(0, 295, 0), 0.5f).setEase(LeanTweenType.easeOutCubic);
            settingsActive = true;
        }
        else
        {
            LeanTween.moveLocal(settingsUI, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeOutCubic);
            LeanTween.moveLocal(topUI, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
            settingsActive = false;
        }
    }

    public void summonSequence(int clownIndex, string name, string rarity, bool duplicate)
    {
        summoning = true;
        firstSummon = true;
        summonButton.interactable = false;
        drumCombo.GetComponent<Animator>().SetTrigger("startDrum");
        targetClown = clownMang.clownSprites[clownIndex];
        currentDuplicate = duplicate;

        if (!duplicate)
        {
            targetClown.SetActive(true);
            targetClown.GetComponent<clownBehavior>().summoning = true;
            targetClown.transform.localScale = new Vector3(0, 0, 0);
            targetClown.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

            LeanTween.scale(targetClown, new Vector3(3, 3, 3), 0.5f).setEase(LeanTweenType.easeOutElastic).setDelay(3f);
            LeanTween.scale(targetClown, new Vector3(1, 1, 1), 0.5f).setEase(LeanTweenType.easeInCirc).setDelay(4f);
            LeanTween.alpha(targetClown.GetComponent<clownBehavior>().frontView, 1f, 0.5f).setEase(LeanTweenType.easeOutCirc).setDelay(3f);
        }
        else
        {
            var tempClown = Instantiate(dupeSprite[clownIndex], transform);
            targetDupe = tempClown;
            targetDupe.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            LeanTween.scale(targetDupe, new Vector3(3, 3, 3), 0.5f).setEase(LeanTweenType.easeOutElastic).setDelay(3f);
            LeanTween.alpha(targetDupe.GetComponent<RectTransform>(), 1f, 0.5f).setEase(LeanTweenType.easeOutCirc).setDelay(3f);

            LeanTween.scale(targetDupe, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic).setDelay(5f);
            LeanTween.alpha(targetDupe.GetComponent<RectTransform>(), 0, 0.5f).setEase(LeanTweenType.easeInOutCubic).setDelay(5f);

            LeanTween.scale(clownCoin, new Vector3(2, 2, 2), 0.5f).setEase(LeanTweenType.easeInOutCubic).setDelay(5f);
            LeanTween.alpha(clownCoin.GetComponent<RectTransform>(), 1f, 0.5f).setEase(LeanTweenType.easeInOutCubic).setDelay(5f);
            LeanTween.moveLocal(clownCoin, new Vector3(0, 800, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic).setDelay(5.5f);

            LeanTween.scale(clownCoinCounter, new Vector3(1.2f, 1.2f, 1.2f), 0.3f).setEase(LeanTweenType.easeInOutCubic).setDelay(5.5f);
            LeanTween.scale(clownCoinCounter, new Vector3(1f, 1f, 1f), 0.3f).setEase(LeanTweenType.easeInOutCubic).setDelay(5.8f);
        }

        LeanTween.moveLocal(drumCombo, new Vector3(0, 800, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic).setDelay(2f);
        LeanTween.alpha(drum.GetComponent<RectTransform>(), 0, 0.5f).setDelay(2f);
        LeanTween.alpha(drumStick1.GetComponent<RectTransform>(), 0, 0.5f).setDelay(2f);
        LeanTween.alpha(drumStick2.GetComponent<RectTransform>(), 0, 0.5f).setDelay(2f);
        LeanTween.alpha(drum.GetComponent<RectTransform>(), 1, 0.5f).setDelay(5f);
        LeanTween.alpha(drumStick1.GetComponent<RectTransform>(), 1, 0.5f).setDelay(5f);
        LeanTween.alpha(drumStick2.GetComponent<RectTransform>(), 1, 0.5f).setDelay(5f);
        LeanTween.moveLocal(drumCombo, new Vector3(0, -385, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic).setDelay(4f);
    }

    public void sfxButton()
    {
        if (sfxMute)
        {
            sfxIcon.overrideSprite = sfxOn;
            FindObjectOfType<audioManager>().muteSFX(false);
            sfxMute = false;
        }
        else
        {
            sfxIcon.overrideSprite = sfxOff;
            FindObjectOfType<audioManager>().muteSFX(true);
            sfxMute = true;
        }
    }

    public void musicButton()
    {
        if (musicMute)
        {
            musicIcon.overrideSprite = musicOn;
            FindObjectOfType<audioManager>().muteMusic(false);
            musicMute = false;
        }
        else
        {
            musicIcon.overrideSprite = musicOff;
            FindObjectOfType<audioManager>().muteMusic(true);
            musicMute = true;
        }
    }
}
