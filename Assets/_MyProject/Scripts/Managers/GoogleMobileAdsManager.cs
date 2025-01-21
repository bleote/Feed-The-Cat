using Firebase.Analytics;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GoogleMobileAdsManager : MonoBehaviour
{
    public static GoogleMobileAdsManager Instance;

    public string appID = "ca-app-pub-9457767323824177~8537222418";

    private const string UNSUPPORTED_PLATFORM = "unused";

#if UNITY_ANDROID
    private string interstitialAdId = "ca-app-pub-9457767323824177/8871747412";
    private string rewardedAdId = "ca-app-pub-9457767323824177/3427849044";

#elif UNITY_EDITOR_WIN
    private string interstitialAdId = "ca-app-pub-9457767323824177/8871747412";
    private string rewardedAdId = "ca-app-pub-9457767323824177/3427849044";

#else
    private string interstitialAdId = UNSUPPORTED_PLATFORM;
    private string rewardedAdId = UNSUPPORTED_PLATFORM;

#endif

    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    public bool IsInterstitialAdAvailable;
    public bool IsRewardedAdAvailable;

    public bool consentGiven = false;

    // The Google Mobile Ads Unity plugin needs to be run only once.
    private static bool? _isInitialized;

    // Helper class that implements consent using the
    // Google User Messaging Platform (UMP) Unity plugin.
    [SerializeField, Tooltip("Controller for the Google User Messaging Platform (UMP) Unity plugin.")]
    private GoogleMobileAdsConsentController _consentController;

    // This variable holds the type of reward for the current ad display
    private AdRewardType currentRewardType;
    private bool mainMenuReward = false;
    private int pendingRewardAmount = 0;
    private string pendingRewardType = "";

    //variable to double the prize from completing a level
    private bool levelDoublePrize = false;

    //variable to double the score at the end of the Relax Mode
    private bool relaxDoubleScore = false;

    //variable to receive calls from Level Completed Handler
    public bool levelCompletedRequest;

    //variable to receive calls from Lose Handler
    public bool loseHandlerRequest;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (rewardedAdId == UNSUPPORTED_PLATFORM || interstitialAdId == UNSUPPORTED_PLATFORM)
        {
            return;
        }

        // On Android, Unity is paused when displaying interstitial or rewarded video.
        // This setting makes iOS behave consistently with Android.
        MobileAds.SetiOSAppPauseOnBackground(true);

        // When true all events raised by GoogleMobileAds will be raised
        // on the Unity main thread. The default value is false.
        // https://developers.google.com/admob/unity/quick-start#raise_ad_events_on_the_unity_main_thread
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        // Initialize Mobile Ads SDK
        InitializeGoogleMobileAds();
    }

    /// <summary>
    /// Initializes the Google Mobile Ads Unity plugin.
    /// </summary>
    private void InitializeGoogleMobileAds()
    {
        // The Google Mobile Ads Unity plugin needs to be run only once and before loading any ads.
        if (_isInitialized.HasValue)
        {
            return;
        }

        _isInitialized = false;

        // Initialize the Google Mobile Ads Unity plugin.
        // Debug.Log("Google Mobile Ads Initializing.");
        MobileAds.Initialize((InitializationStatus initstatus) =>
        {
            if (initstatus == null)
            {
                Debug.LogError("Google Mobile Ads initialization failed.");
                _isInitialized = null;
                return;
            }

            // Debug.Log("Google Mobile Ads initialization complete.");
            _isInitialized = true;
        });

        // Start consent gathering once initialization is complete
        InitializeGoogleMobileAdsConsent();
    }

    /// <summary>
    /// Ensures that privacy and consent information is up to date.
    /// </summary>
    private void InitializeGoogleMobileAdsConsent()
    {
        // Debug.Log("Google Mobile Ads gathering consent.");

        _consentController.GatherConsent((string error) =>
        {
            if (error != null)
            {
                //Debug.LogError("Failed to gather consent with error: " +
                //    error);
                consentGiven = false;
            }
            else
            {
                //Debug.Log("Google Mobile Ads consent updated: "
                //    + ConsentInformation.ConsentStatus);
                consentGiven = _consentController.CanRequestAds;
            }

            // After consent gathering, load ads based on user consent
            LoadAds();
        });
    }

    public void AdsFirstLoad()
    {
        LoadNonPersonalizedInterstitialAd(); // NPA
        LoadNonPersonalizedRewardedAd();     // NPA
    }

    /// <summary>
    /// Loads ads based on the user's consent status.
    /// </summary>
    private void LoadAds()
    {
        if (consentGiven)
        {
            // Debug.Log("Consent given. Loading personalized ads.");
            LoadInterstitialAd();
            LoadRewardedAd();
        }
        else
        {
            // Debug.Log("Consent not given. Loading non-personalized ads.");
            LoadNonPersonalizedInterstitialAd(); // NPA
            LoadNonPersonalizedRewardedAd();     // NPA
        }
    }

    /// <summary>
    /// Opens the privacy options form for the user.
    /// </summary>
    /// <remarks>
    /// Your app needs to allow the user to change their consent status at any time.
    /// </remarks>
    public void OpenPrivacyOptions()
    {
        _consentController.ShowPrivacyOptionsForm((string error) =>
        {
            if (error != null)
            {
                Debug.LogError("Failed to show consent privacy form with error: " +
                    error);
            }
            else
            {
                Debug.Log("Privacy form opened successfully.");
                return;
            }
        });
    }

    public void NPACheckAndLoadInterstitialAd()
    {
        if (consentGiven)
        {
            LoadInterstitialAd();
        }
        else
        {
            LoadNonPersonalizedInterstitialAd();
        }
    }

    public void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        InterstitialAd.Load(interstitialAdId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                IsInterstitialAdAvailable = false;
                return;
            }

            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                IsInterstitialAdAvailable = false;
                return;
            }

            //Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
            IsInterstitialAdAvailable = true;

            interstitialAd = ad;
            InterstitialAdEventHandler(interstitialAd);
        });
    }

    public void LoadNonPersonalizedInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        var adRequest = new AdRequest
        {
            Extras = new Dictionary<string, string> { { "npa", "1" } }  // Non-personalized ad
        };

        adRequest.Keywords.Add("unity-admob-sample");

        InterstitialAd.Load(interstitialAdId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Non-personalized interstitial ad failed to load with error: " + error);
                return;
            }

            interstitialAd = ad;
            IsInterstitialAdAvailable = true;
            //Debug.Log("Non-personalized Interstitial ad loaded.");
        });
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            //Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    public void InterstitialAdEventHandler(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            //Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
            //    adValue.Value,
            //    adValue.CurrencyCode));
        };

        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            //Debug.Log("Interstitial ad recorded an impression.");
        };

        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            //Debug.Log("Interstitial ad was clicked.");
        };

        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            //Debug.Log("Interstitial ad full screen content opened.");
        };

        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            //Debug.Log("Interstitial ad full screen content closed.");

            GamePlayManager.waitingInterstitialAdConclusion = false;

            CheckGameplayInterstitialStatus();

            NPACheckAndLoadInterstitialAd();
        };

        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            GamePlayManager.waitingInterstitialAdConclusion = false;

            CheckGameplayInterstitialStatus();

            NPACheckAndLoadInterstitialAd();
        };
    }

    private void CheckGameplayInterstitialStatus()
    {
        if (levelCompletedRequest)
        {
            levelCompletedRequest = false;
            LevelCompletedHandler.Instance.InterstitialConclusionHandler();
        }
        else if (loseHandlerRequest)
        {
            loseHandlerRequest = false;
            LoseHandler.Instance.InterstitialConclusionHandler();
        }
    }

    public void NPACheckAndLoadRewardedAd()
    {
        if (consentGiven)
        {
           LoadRewardedAd();
        }
        else
        {
            LoadNonPersonalizedRewardedAd();
        }
    }

    public void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        RewardedAd.Load(rewardedAdId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                IsRewardedAdAvailable = false;
                return;
            }

            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                IsRewardedAdAvailable = false;
                return;
            }

            //Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            IsRewardedAdAvailable = true;

            rewardedAd = ad;
            RewardedAdEventHandler(rewardedAd);
        });
    }

    public void LoadNonPersonalizedRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest
        {
            Extras = new Dictionary<string, string> { { "npa", "1" } }  // Non-personalized ad
        };

        adRequest.Keywords.Add("unity-admob-sample");

        RewardedAd.Load(rewardedAdId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Non-personalized rewarded ad failed to load with error: " + error);
                return;
            }

            rewardedAd = ad;
            IsRewardedAdAvailable = true;
            //Debug.Log("Non-personalized Rewarded ad loaded.");
        });
    }

    public void ShowRewardedAd(AdRewardType rewardType)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            mainMenuReward = false; // Reset before showing the ad
            currentRewardType = rewardType; // Set the current reward type

            pendingRewardAmount = GetRewardAmount(rewardType);
            pendingRewardType = GetRewardDescription(rewardType);

            // Temporarily store the main reward information, but do not give the reward yet
            if (currentRewardType == AdRewardType.ExtraCoins || currentRewardType == AdRewardType.ExtraHeart)
            {
                mainMenuReward = true;
            }

            rewardedAd.Show((Reward reward) =>
            {
                // If the rewarded ad was shown during gameplay, the Rewarded Ad Handler selects the reward effect on the game and resume to play.
            });
        }
    }

    public void ShowDoublePrizeRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            levelDoublePrize = true; 

            rewardedAd.Show((Reward reward) =>
            {
                // The double prize logic is handled in the Rewarded Ad Event Handler.
            });
        }
    }

    public void ShowDoubleScoreRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            relaxDoubleScore = true;

            rewardedAd.Show((Reward reward) =>
            {
                // The double score logic is handled in the Rewarded Ad Event Handler..
            });
        }
    }

    // Helper method to get the rewards selected at the main menu amount based on RewardType
    private int GetRewardAmount(AdRewardType rewardType)
    {
        switch (rewardType)
        {
            case AdRewardType.ExtraCoins:
                return 100;
            case AdRewardType.ExtraHeart:
                return 1;
            case AdRewardType.ContinueGame:
                return 1;
            case AdRewardType.ReduceMeltedLevels:
                return 1;
            case AdRewardType.RecoverHealth:
                return 1;
            case AdRewardType.ScoreMultiplier:
                return 1;
            case AdRewardType.LuckyWheel:
                return 1;
            default:
                return 0; // Default if unknown
        }
    }

    // Helper method to get the reward description based on RewardType
    private string GetRewardDescription(AdRewardType rewardType)
    {
        switch (rewardType)
        {
            case AdRewardType.ExtraCoins:
                return "coins";
            case AdRewardType.ExtraHeart:
                return "extra heart";
            case AdRewardType.ContinueGame:
                return "continue game";
            case AdRewardType.ReduceMeltedLevels:
                return "reduce melted levels";
            case AdRewardType.RecoverHealth:
                return "recover health";
            case AdRewardType.ScoreMultiplier:
                return "score multiplier";
            case AdRewardType.LuckyWheel:
                return "lucky wheel";
            default:
                return "unknown reward";
        }
    }

    public void RewardedAdEventHandler(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            //Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
            //    adValue.Value,
            //    adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            //Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            //Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            //Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            // Increment the total rewarded ads watched in player's data
            DataManager.Instance.PlayerData.TotalRewardedAdsWatched++;

            RewardDistribution();
            NPACheckAndLoadRewardedAd();
        };

        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            RewardDistribution();
            NPACheckAndLoadRewardedAd();
        };
    }

    private void RewardDistribution()
    {
        if (levelDoublePrize)
        {
            //Debug.Log("Double Prize Rewarded Ad");
            levelDoublePrize = false;
            LevelCompletedHandler.Instance.DoubleThePrize();
        }
        else if (relaxDoubleScore)
        {
            //Debug.Log("Relax Mode Double Score Rewarded Ad");
            relaxDoubleScore = false;
            LoseHandler.Instance.DoubleTheScore();
        }
        else
        {
            if (mainMenuReward)
            {
                GiveMenuRewardToUser(currentRewardType);
            }
            else if (GamePlayManager.continueProcess)
            {
                ContinueHandler.Instance.Continue();
                FirebaseAnalytics.LogEvent(GamePlayManager.levelModeOn ? "continued_game_level_ad" : "continued_game_relax_ad");
            }
            else
            {
                RewardedAdHandler.Instance.CollectReward(currentRewardType);
            }

            RewardColectionLocalLog(pendingRewardAmount, pendingRewardType);

            FirebaseManager.Instance.LogAdRewardSelection(currentRewardType);

            // Reset the reward flag and pending reward details
            mainMenuReward = false;
            pendingRewardAmount = 0;
            pendingRewardType = "";
        }
    }

    private void GiveMenuRewardToUser(AdRewardType adRewardType)
    {
        if (adRewardType != AdRewardType.ExtraCoins && adRewardType != AdRewardType.ExtraHeart)
        {
            //Debug.Log($"Can't handle the current reward type {currentRewardType}");
            return;
        }

        switch (currentRewardType)
        {
            case AdRewardType.ExtraCoins:
                if (MoreCoinsDisplay.Instance != null)
                {
                    MoreCoinsDisplay.Instance.ExtraCoinsFromRewardedAd();
                }
                else if (ShopUI.Instance != null)
                {
                    ShopUI.Instance.ExtraCoinsFromRewardedAd();
                }
                    break;

            case AdRewardType.ExtraHeart:
                HeartsManager.Instance.AddOneHeart();
                break;

            default:
                throw new System.Exception("Don`t know how to give this reward");
        }
    }

    private void RewardColectionLocalLog(int amount, string rewardType)
    {
        Debug.Log(string.Format("User has been rewarded with {0} {1}.", amount, rewardType));
    }
}
