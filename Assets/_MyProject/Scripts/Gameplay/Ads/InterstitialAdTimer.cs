using UnityEngine;
using static FirebaseRemoteConfigManager;

public class InterstitialAdTimer : MonoBehaviour
{
    public static InterstitialAdTimer Instance;

    public const string INTERSTITIAL_TIMER = "interstitialtimer";
    public const string INTERSTITIAL_CALLER = "interstitialcaller";

    private int interstitialFrequencyInSeconds;
    private float interstitialTimer;
    public bool callInterstitial;

    AdsConfigData configData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        configData = FirebaseRemoteConfigManager.Instance.adsConfigData;

        if (configData.interstitialAdsActive)
        {
            //Load Frequency from Firebase Remote Config
            interstitialFrequencyInSeconds = configData.interstitialFrequencyInSeconds;

            // Load the timer from PlayerPrefs
            interstitialTimer = PlayerPrefs.GetFloat(INTERSTITIAL_TIMER, 0);

            // Check if the interstitial ad should be called
            callInterstitial = PlayerPrefs.GetInt(INTERSTITIAL_CALLER, 0) == 1;
        }
        else
        {
            // Debug.Log("Interstitial Ads are Switched Off");
            return;
        }
    }

    private void Update()
    {
        if (configData.interstitialAdsActive)
        {
            if (!GamePlayManager.isPaused)
            {
                interstitialTimer += Time.deltaTime;
            }

            if (interstitialTimer >= interstitialFrequencyInSeconds && !callInterstitial)
            {
                callInterstitial = true;
                SaveState();
            }
        }
    }

    private void OnDestroy()
    {
        SaveState();
    }

    public void InterstitialAdCall()
    {
        if (!configData.interstitialAdsActive)
        {
            return;
        }

        if (GoogleMobileAdsManager.Instance != null && GoogleMobileAdsManager.Instance.IsInterstitialAdAvailable)
        {
            GamePlayManager.waitingInterstitialAdConclusion = true;
            GoogleMobileAdsManager.Instance.ShowInterstitialAd();
        }
        else
        {
            Debug.LogWarning("Interstitial Ad is not available.");
        }

        ResetTimer();
    }

    private void ResetTimer()
    {
        interstitialTimer = 0;
        callInterstitial = false;
        SaveState();
    }

    private void SaveState()
    {
        if (configData.interstitialAdsActive)
        {
            // Save the current timer and whether we need to call the interstitial
            PlayerPrefs.SetFloat(INTERSTITIAL_TIMER, interstitialTimer);
            PlayerPrefs.SetInt(INTERSTITIAL_CALLER, callInterstitial ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
