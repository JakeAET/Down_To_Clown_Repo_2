using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class RewardedAdsButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private bool canWatchAd;
    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";

    [SerializeField] Text promptText;
    private saveManager saveMang;
    private infoManager infoMang;
    public string _adUnitId = null; // This will remain null for unsupported platforms
    private bool adComplete;

    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif
        //Disable the button until the ad is ready to show:
        _showAdButton.interactable = false;
    }

    void Start()
    {
        saveMang = GameObject.FindGameObjectWithTag("saveManager").GetComponent<saveManager>();
        infoMang = GameObject.FindGameObjectWithTag("infoManager").GetComponent<infoManager>();
        checkAdCooldown();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            saveMang.state.lastAdWatched = 0;
            checkAdCooldown();
        }

        if (!canWatchAd)
        {
            long period = 10L * 60L * 1000L * 10000L;
            long timeStamp = System.DateTime.Now.Ticks + period;
            int secondsPassed = (int)((int)(timeStamp / 10000000) - (saveMang.state.lastAdWatched / 10000000));
            int timeRemaining = 7199 - secondsPassed;
            string hours = Mathf.Floor((timeRemaining % 216000) / 3600).ToString("0");
            string minutes = Mathf.Floor((timeRemaining % 3600) / 60).ToString("00");
            string output = "Next ad available in <color=white>" + hours + "h " + minutes + "m</color>";
            promptText.text = output;

            if (timeRemaining <= 0)
            {
                checkAdCooldown();
            }
        }
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
            // Configure the button to call the ShowAd() method when clicked:
            _showAdButton.onClick.AddListener(ShowAd);
            // Enable the button for users to click:
            _showAdButton.interactable = canWatchAd;
        }
    }

    // Implement a method to execute when the user clicks the button:
    public void ShowAd()
    {
        // Disable the button:
        _showAdButton.interactable = false;
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
        adComplete = false;
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED) && !adComplete)
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            infoMang.whimsyIncrease(50);

            long period = 10L * 60L * 1000L * 10000L;
            long timeStamp = System.DateTime.Now.Ticks + period;
            saveMang.state.lastAdWatched = timeStamp;
            checkAdCooldown();

            // Load another ad:
            Advertisement.Load(_adUnitId, this);
            adComplete = true;
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        // Clean up the button listeners:
        _showAdButton.onClick.RemoveAllListeners();
    }

    public void checkAdCooldown()
    {
        if(saveMang.state.lastAdWatched == 0)
        {
            canWatchAd = true;
            _showAdButton.interactable = true;
            promptText.text = "Watch an ad for <color=white><i>50 whimsy?</i></color>";
        }
        else
        {
            long period = 10L * 60L * 1000L * 10000L;
            long timeStamp = System.DateTime.Now.Ticks + period;
            int secondsPassed = (int)((int)(timeStamp / 10000000) - (saveMang.state.lastAdWatched / 10000000));
            canWatchAd = secondsPassed > 7200;
            _showAdButton.interactable = canWatchAd;

            if (canWatchAd)
            {
                promptText.text = "Watch an ad for <color=white><i>50 whimsy?</i></color>";
            }
        }
    }
}