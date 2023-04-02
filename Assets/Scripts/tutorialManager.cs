using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class tutorialManager : MonoBehaviour
{
    //Tutorial stages
    public enum tutorial
    {
        Intro,
        Clicker1,
        Clicker2,
        Whimsy,
        Summon1,
        Summon2,
        Working,
        Complete,
    }
    public tutorial step;

    public static tutorialManager Instance;
    private saveManager saveMang;
    private uiManager uiMang;
    private infoManager infoMang;
    private touchCam cam;

    [SerializeField] GameObject ringmasterUI;
    [SerializeField] Animator ringmasterAnimator;
    [SerializeField] tutorialClass[] tutorialPreset;
    private tutorialClass currentStep;
    [SerializeField] Text dialogueText;
    public int tutorialStep = 99;

    public bool initializeStep = true;
    public bool initializeTutorial = true;

    private string startText;
    private string currentText;
    [SerializeField] float typeSpeed = 0.1f;
    [SerializeField] float punctuationPause = 0.2f;

    // Progression Checks
    public bool halfTicketGoal = false;
    public bool ticketGoalReached = false;
    public bool balloonPopped = false;
    public bool tentClicked = false;
    public bool summonClicked = false;
    public bool clownDragged = false;

    public int dialogueStep = 0;
    public int camStep = 0;
    
    public bool textFinished = false;
    private int effectTextNum = 0;

    // Tap to Continue
    [SerializeField] Text contText;
    private bool waitingForTap = false;
    private bool fadeIn;
    private Color fadeColor;

    // Arrows
    private bool arrowActive = false;
    [SerializeField] GameObject unlockArrow;
    [SerializeField] GameObject tentArrow;
    [SerializeField] GameObject summonArrow;
    [SerializeField] GameObject placementArrow;
    private GameObject currentArrow;
    private bool grow;

    // Restricted Buttons
    [SerializeField] Button book;
    [SerializeField] Button tent;
    [SerializeField] Button build;
    [SerializeField] Button ads;
    [SerializeField] Button summon;
    [SerializeField] Button exitSummon;
    [SerializeField] Button settings;

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
        saveMang = GameObject.FindGameObjectWithTag("saveManager").GetComponent<saveManager>();
        uiMang = GameObject.FindGameObjectWithTag("uiManager").GetComponent<uiManager>();
        infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();
        cam = GameObject.FindGameObjectWithTag("touchCam").GetComponent<touchCam>();

        // Restricted button default state
        book.interactable = false;
        tent.interactable = false;
        settings.interactable = false;

        // Arrow default state
        unlockArrow.SetActive(false);
        tentArrow.SetActive(false);
        summonArrow.SetActive(false);
        placementArrow.SetActive(false);

        tutorialStep = saveMang.state.tutorialStep;
        determineState();

        currentStep = tutorialPreset[tutorialStep];
        startText = currentStep.dialogue[dialogueStep];
        StartCoroutine(Effect());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            advanceTutorial();
        }

        if (waitingForTap && textFinished)
        {
            if(fadeColor.a <= 0.2)
            {
                fadeIn = true;
            }

            if(fadeColor.a >= 0.8)
            {
                fadeIn = false;
            }

            if (fadeIn)
            {
                fadeColor = contText.color;
                fadeColor.a += Time.deltaTime * 0.5f;
                contText.color = fadeColor;
            }
            else
            {
                fadeColor = contText.color;
                fadeColor.a -= Time.deltaTime * 0.5f;
                contText.color = fadeColor;
            }


            if (Input.GetMouseButtonDown(0))
            {
                waitingForTap = false;
            }
        }
        else if (!textFinished)
        {
            fadeColor.a = 0;
            contText.color = fadeColor;
        }

        if (arrowActive && textFinished)
        {
            if (!currentArrow.activeInHierarchy)
            {
                currentArrow.SetActive(true);
            }

            if (currentArrow.transform.localScale.x <= 0.9)
            {
                grow = true;
            }

            if (currentArrow.transform.localScale.x >= 1)
            {
                grow = false;
            }

            if (grow)
            {
                currentArrow.transform.localScale += Vector3.one * 0.3f * Time.deltaTime;
            }
            else
            {
                currentArrow.transform.localScale -= Vector3.one * 0.3f * Time.deltaTime;
            }
        }

        #region State Actions
        switch (step)
        {
            case tutorial.Intro: // step 0 - Introduces circus
                if (initializeTutorial)
                {
                    waitingForTap = true;
                    uiMang.bottomBar(false);
                    initializeTutorial = false;
                }
                else if (initializeStep)
                {
                    waitingForTap = true;
                    uiMang.bottomBar(false);
                    initializeStep = false;
                }

                if (Input.GetMouseButtonDown(0) && textFinished)
                {
                    waitingForTap = true;
                    nextDialogue();
                }

                break;
            case tutorial.Clicker1: // step 1 - Click horn, earn tickets, buy attraction
                if (initializeTutorial)
                {
                    waitingForTap = true;
                    uiMang.bottomBar(false);
                    cam.zoomLockCam(currentStep.camPreset[0]);
                    initializeTutorial = false;
                }
                else if (initializeStep)
                {
                    waitingForTap = true;
                    cam.zoomLockCam(currentStep.camPreset[0]);
                    initializeStep = false;
                }

                if (Input.GetMouseButtonDown(0) && dialogueStep == 0 && textFinished)
                {
                    nextDialogue();
                    LeanTween.moveLocal(ringmasterUI, new Vector3(0, -149, 0), 0.5f).setEase(LeanTweenType.easeOutCubic);
                    uiMang.bottomBar(true);
                }
                if (dialogueStep == 1 && infoMang.funnyMoney >= 1000 && textFinished)
                {
                    nextDialogue();
                    LeanTween.moveLocal(ringmasterUI, new Vector3(0, -513, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
                    uiMang.bottomBar(false);
                    currentArrow = unlockArrow;
                    arrowActive = true;
                }
                if (infoMang.attractions[0].unlocked && dialogueStep == 2 && textFinished)
                {
                    currentArrow.SetActive(false);
                    arrowActive = false;
                    waitingForTap = true;
                    nextDialogue();
                }
                if (Input.GetMouseButtonDown(0) && dialogueStep == 3 && textFinished)
                {
                    waitingForTap = true;
                    nextDialogue();
                }
                if (Input.GetMouseButtonDown(0) && dialogueStep == 4 && textFinished)
                {
                    waitingForTap = true;
                    nextDialogue();
                }
                if (Input.GetMouseButtonDown(0) && dialogueStep == 5 && textFinished)
                {
                    nextDialogue();
                }
                break;
            case tutorial.Clicker2: // step 2 - Fill up the tap meter and pop the balloon
                if (initializeTutorial)
                {
                    waitingForTap = true;
                    uiMang.bottomBar(false);
                    cam.zoomLockCam(currentStep.camPreset[0]);
                    initializeTutorial = false;
                }
                else if (initializeStep)
                {
                    waitingForTap = true;
                    cam.zoomLockCam(currentStep.camPreset[0]);
                    initializeStep = false;
                }

                if (Input.GetMouseButtonDown(0) && dialogueStep == 0 && textFinished)
                {
                    LeanTween.moveLocal(ringmasterUI, new Vector3(0, -149, 0), 0.5f).setEase(LeanTweenType.easeOutCubic);
                    uiMang.bottomBar(true);
                    nextDialogue();
                }
                if (dialogueStep == 1 && halfTicketGoal && textFinished)
                {
                    nextDialogue();
                }
                if (dialogueStep == 2 && ticketGoalReached && textFinished)
                {
                    LeanTween.moveLocal(ringmasterUI, new Vector3(0, -513, 0), 0.5f).setEase(LeanTweenType.easeInOutCubic);
                    uiMang.bottomBar(false);
                    nextCam();
                    nextDialogue();
                }
                if (dialogueStep == 3 && balloonPopped && textFinished)
                {
                    nextDialogue();
                }
                break;
            case tutorial.Whimsy: // step 3 - Noting whimsy increase, tapping tent button to go to tent
                if (initializeTutorial)
                {
                    LeanTween.moveLocal(ringmasterUI, new Vector3(0, -149, 0), 0.1f).setEase(LeanTweenType.easeOutCubic);
                    currentArrow = tentArrow;
                    arrowActive = true;
                    initializeTutorial = false;
                }
                else if (initializeStep)
                {
                    LeanTween.moveLocal(ringmasterUI, new Vector3(0, -149, 0), 0.5f).setEase(LeanTweenType.easeOutCubic);
                    uiMang.bottomBar(true);
                    cam.zoomLockCam(currentStep.camPreset[0]);
                    currentArrow = tentArrow;
                    arrowActive = true;
                    initializeStep = false;
                }

                if(tentClicked && dialogueStep == 0 && textFinished)
                {
                    currentArrow.SetActive(false);
                    arrowActive = false;
                    nextDialogue();
                }
                break;
            case tutorial.Summon1: // step 4 - Tapping the summon button
                if (initializeTutorial)
                {
                    waitingForTap = true;
                    LeanTween.moveLocal(ringmasterUI, new Vector3(0, 387, 0), 0.1f).setEase(LeanTweenType.easeOutCubic);
                    uiMang.summon(true);
                    initializeTutorial = false;
                }
                else if (initializeStep)
                {
                    waitingForTap = true;
                    LeanTween.moveLocal(ringmasterUI, new Vector3(0, 387, 0), 0.5f).setEase(LeanTweenType.easeOutCubic);
                    uiMang.summon(true);
                    initializeStep = false;
                }
                if(Input.GetMouseButtonDown(0) && dialogueStep == 0 && textFinished)
                {
                    summon.interactable = true;
                    currentArrow = summonArrow;
                    arrowActive = true;
                    nextDialogue();
                }
                if (summonClicked && dialogueStep == 1 && textFinished)
                {
                    currentArrow.SetActive(false);
                    arrowActive = false;
                    nextDialogue();
                }

                break;
            case tutorial.Summon2: // step 5 - Following the clown with the camera
                if (initializeTutorial)
                {
                    waitingForTap = true;
                    LeanTween.moveLocal(ringmasterUI, new Vector3(0, 387, 0), 0.1f).setEase(LeanTweenType.easeOutCubic);
                    uiMang.bottomBar(false);
                    initializeTutorial = false;
                }
                else if (initializeStep)
                {
                    waitingForTap = true;
                    uiMang.summon(false);
                    uiMang.bottomBar(false);
                    initializeStep = false;
                }
                if(Input.GetMouseButtonDown(0) && dialogueStep == 0 && textFinished)
                {
                    waitingForTap = true;
                    cam.zoomLockCam(currentStep.camPreset[0]);
                    nextDialogue();
                }
                if (Input.GetMouseButtonDown(0) && dialogueStep == 1 && textFinished)
                {
                    nextDialogue();
                }

                break;
            case tutorial.Working: // step 6 - Drag clown to attraction;
                if (initializeTutorial)
                {
                    waitingForTap = true;
                    LeanTween.moveLocal(ringmasterUI, new Vector3(0, 387, 0), 0.1f).setEase(LeanTweenType.easeOutCubic);
                    cam.zoomLockCam(currentStep.camPreset[0]);
                    uiMang.bottomBar(false);
                    initializeTutorial = false;
                }
                else if (initializeStep)
                {
                    waitingForTap = true;
                    cam.zoomLockCam(currentStep.camPreset[0]);
                    initializeStep = false;
                }
                if (Input.GetMouseButtonDown(0) && dialogueStep == 0 && textFinished)
                {
                    nextDialogue();
                }
                if (clownDragged && dialogueStep == 1 && camStep == 0)
                {
                    currentArrow = placementArrow;
                    arrowActive = true;
                    nextCam();
                }
                if(infoMang.attractions[0].inUse && dialogueStep == 1 && textFinished)
                {
                    currentArrow.SetActive(false);
                    arrowActive = false;
                    waitingForTap = true;
                    nextDialogue();
                }
                if (Input.GetMouseButtonDown(0) && dialogueStep == 2 && textFinished)
                {
                    nextDialogue();
                }
                break;
            case tutorial.Complete: // step 7
                if (initializeStep)
                {
                    if (currentStep.camPreset != null && cam.currentPresetName != currentStep.camPreset[camStep])
                    {
                        cam.zoomLockCam(currentStep.camPreset[camStep]);
                    }

                    initializeStep = false;
                }
                break;
        }
        #endregion

        if (textFinished)
        {
            StopCoroutine(Effect());
        }

        ringmasterAnimator.SetBool("isTalking", !textFinished);
    }

    public void advanceTutorial()
    {
        StopCoroutine(Effect());
        dialogueText.text = "";
        textFinished = false;
        cam.presetZooming = false;
        initializeStep = true;
        dialogueStep = 0;
        camStep = 0;
        effectTextNum = 0;
        tutorialStep++;
        determineState();
        saveMang.state.tutorialStep = tutorialStep;
        currentStep = tutorialPreset[tutorialStep];
        startText = currentStep.dialogue[dialogueStep];
        StartCoroutine(Effect());
    }

    public void nextDialogue()
    {
        if (dialogueStep + 1 == currentStep.dialogue.Length)
        {
            advanceTutorial();
        }
        else
        {
            StopCoroutine(Effect());
            dialogueText.text = "";
            textFinished = false;
            effectTextNum = 0;
            dialogueStep++;
            startText = currentStep.dialogue[dialogueStep];
            //cam.zoomLockCam(currentStep.camPreset[0]);
            StartCoroutine(Effect());
        }
    }

    private void determineState()
    {
        if (tutorialStep == 0)
        {
            step = tutorial.Intro;
        }
        else if (tutorialStep == 1)
        {
            step = tutorial.Clicker1;
        }
        else if (tutorialStep == 2)
        {
            step = tutorial.Clicker2;
        }
        else if (tutorialStep == 3)
        {
            tent.interactable = true;
            step = tutorial.Whimsy;
        }
        else if (tutorialStep == 4)
        {
            tent.interactable = true;
            step = tutorial.Summon1;
        }
        else if (tutorialStep == 5)
        {
            tent.interactable = true;
            summon.interactable = true;
            step = tutorial.Summon2;
        }
        else if (tutorialStep == 6)
        {
            tent.interactable = true;
            summon.interactable = true;
            step = tutorial.Working;
        }
        else if (tutorialStep == 7)
        {
            book.interactable = true;
            tent.interactable = true;
            build.interactable = true;
            ads.interactable = true;
            summon.interactable = true;
            exitSummon.interactable = true;
            settings.interactable = true;
            ringmasterUI.SetActive(false);
            step = tutorial.Complete;
        }
    }

    public void nextCam()
    {
        camStep++;
        cam.presetZooming = false;
        cam.zoomLockCam(currentStep.camPreset[camStep]);
    }

    public void halfwayTicketGoal()
    {
        halfTicketGoal = true;
    }

    public void ticketGoal()
    {
        ticketGoalReached = true;
    }

    public void balloonPop()
    {
        balloonPopped = true;
    }

    public void tentClick()
    {
        tentClicked = true;
    }

    public void summonClick()
    {
        summonClicked = true;
    }

    public void clownDrag()
    {
        clownDragged = true;
    }

    IEnumerator Effect()
    {
        while (effectTextNum < startText.Length + 1)
        {
            currentText = startText.Substring(0, effectTextNum);
            dialogueText.text = currentText;

            if(startText == currentText)
            {
                textFinished = true;
                yield return null;
            }
            else if (startText[effectTextNum].ToString() == "." || startText[effectTextNum].ToString() == "!" || startText[effectTextNum].ToString() == "?")
            {
                yield return new WaitForSeconds(punctuationPause);
            }
            else
            {
                yield return new WaitForSeconds(typeSpeed);
            }

            effectTextNum++;
        }
    }
}
