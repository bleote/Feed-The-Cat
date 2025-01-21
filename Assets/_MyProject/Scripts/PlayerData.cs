using Firebase.Analytics;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    private int paws;
    private int elixirs;
    private int biscuits;
    private int highScore;
    private int extraLivesPackage;
    private string userName = string.Empty;
    private int selectedCat;
    private List<int> ownedCatIds = new List<int>();
    private bool playMusic;
    private bool playSound;
    private int coins;
    private int keys;
    private int hearts;
    private DateTime lastTimeClosedApp;
    private int secondsLeftForAnotherHart;
    private List<int> unlockedBoosters = new List<int>();
    private bool vibration;
    private bool notifications;

    public Action UpdatedPaws;
    public Action UpdatedElixirs;
    public Action UpdatedBiscuits;
    public Action UpdatedHighScore;
    public Action UpdatedExp;
    public Action UpdatedGamePlayLevel;
    public Action UpdatedExtraLives;
    public Action UpdatedUserName;
    public Action UpdatedOwnedCatsList;
    public Action UpdatedCoins;
    public Action SpentCoins;
    public Action UpdatedKeys;
    public Action UpdatedHearts;
    public Action UpdatedVibration;
    public Action UpdatedNotifications;
    public static Action UpdatedMusic;
    public static Action UpdatedSounds;
    public Action WatchedRewardedAd;
    
    private int totalCoinsSpent = 0;
    private int totalRewardedAdsWatched = 0;

    private bool MVPSpentBronze = false;
    private bool MVPSpentSilver = false;
    private bool MVPSpentGold = false;
    private bool MVPRewardedAdsBronze = false;
    private bool MVPRewardedAdsSilver = false;
    private bool MVPRewardedAdsGold = false;

    public int Paws
    {
        get => paws;
        set
        {
            paws = value;
            UpdatedPaws?.Invoke();
        }
    }

    public int Elixirs
    {
        get => elixirs;
        set
        {
            elixirs = value;
            UpdatedElixirs?.Invoke();
        }
    }

    public int Biscuits
    {
        get => biscuits;
        set
        {
            biscuits = value;
            UpdatedBiscuits?.Invoke();
        }
    }

    public int HighScore
    {
        get => highScore;
        set
        {
            highScore = value;
            UpdatedHighScore?.Invoke();
        }
    }

    public int ExtraLivesPackage
    {
        get => extraLivesPackage;
        set
        {
            extraLivesPackage = value;
            UpdatedExtraLives?.Invoke();
        }
    }

    public string UserName
    {
        get => userName;
        set
        {
            userName = value;
            UpdatedUserName?.Invoke();
        }
    }

    public List<int> OwnedCatIds
    {
        get => ownedCatIds;
        set
        {
            ownedCatIds = value;
            UpdatedOwnedCatsList?.Invoke();
        }
    }

    public void AddOwnedCat(int _id)
    {
        ownedCatIds.Add(_id);
        UpdatedOwnedCatsList?.Invoke();
    }
    
    public bool PlayMusic
    {
        get => playMusic;
        set
        {
            playMusic = value;
            UpdatedMusic?.Invoke();
        }
    }

    public bool PlaySound
    {
        get => playSound;
        set
        {
            playSound = value;
            UpdatedSounds?.Invoke();
        }
    }

    public int Coins
    {
        get => coins;
        set
        {
            int change = value - coins; // Calculate the change in coins

            coins = value;
            UpdatedCoins?.Invoke();

            if (change < 0) // Check if coins were spent (negative change)
            {
                int amountSpent = -change; // Convert the amount value to positive
                TotalCoinsSpent += amountSpent; // Update total coins spent
                FirebaseManager.Instance.LogCoinsSpent(amountSpent); // Log to Firebase
            }
        }
    }

    public int TotalCoinsSpent
    {
        get => totalCoinsSpent;
        set
        {
            totalCoinsSpent = value; // Update the total coins spent
            SpentCoins?.Invoke();
            
            // Calculate MVP statuses
            UpdateMVPCoinsSpentStatus();
        }
    }

    public int Keys
    {
        get => keys;
        set
        {
            keys = value;
            UpdatedKeys?.Invoke();
        }
    }

    public int Hearts
    {
        get => hearts;
        set => ChangeHearts(value);
    }

    public DateTime LastTimeClosedApp
    {
        get => lastTimeClosedApp;
        set => lastTimeClosedApp = value;
    }

    public int SecondsLeftForAnotherHeart
    {
        get => secondsLeftForAnotherHart;
        set => secondsLeftForAnotherHart = value;
    }

    public bool ReduceHearts()
    {
        if (!DataManager.Instance.GameData.ReduceHearts)
        {
            return true;
        }

        if (hearts == 0)
        {
            return false;
        }

        ChangeHearts(-1);
        return true;
    }

    public int SelectedCat
    {
        get => selectedCat;
        set => selectedCat = value;
    }

    public List<int> UnlockedBoosters
    {
        get => unlockedBoosters;
        set => unlockedBoosters = value;
    }

    public void ChangeHearts(int _amount)
    {
        hearts += _amount;
        if (DataManager.Instance.GameData != null)
        {
            hearts = Math.Clamp(hearts, 0, DataManager.Instance.GameData.MaxAmountOfHearts);
        }
        UpdatedHearts?.Invoke();
    }

    public bool Vibration
    {
        get => vibration;
        set
        {
            vibration = value;
            UpdatedVibration?.Invoke();
        }
    }

    public bool Notifications
    {
        get => notifications;
        set
        {
            notifications = value;
            UpdatedNotifications?.Invoke();
        }
    }

    public int TotalRewardedAdsWatched
    {
        get => totalRewardedAdsWatched;
        set
        {
            totalRewardedAdsWatched = value; // Update the total coins spent
            WatchedRewardedAd?.Invoke();

            // Update MVP status for rewarded ads
            UpdateMVPAdWatchStatus();
        }
    }

    private void UpdateMVPCoinsSpentStatus()
    {
        var configData = FirebaseRemoteConfigManager.Instance.adsConfigData;
        
        // Check Bronze
        if (totalCoinsSpent >= configData.coinsToBecomeMVPBronze && !MVPSpentBronze)
        {
            MVPSpentBronze = true;
            FirebaseAnalytics.LogEvent("mvp_coins_spent_bronze");
        }
        else if (totalCoinsSpent < configData.coinsToBecomeMVPBronze && MVPSpentBronze)
        {
            MVPSpentBronze = false; // Unmark if the threshold is no longer met
        }

        // Check Silver
        if (totalCoinsSpent >= configData.coinsToBecomeMVPSilver && !MVPSpentSilver)
        {
            MVPSpentSilver = true;
            FirebaseAnalytics.LogEvent("mvp_coins_spent_silver");
        }
        else if (totalCoinsSpent < configData.coinsToBecomeMVPSilver && MVPSpentSilver)
        {
            MVPSpentSilver = false; // Unmark if the threshold is no longer met
        }

        // Check Gold
        if (totalCoinsSpent >= configData.coinsToBecomeMVPGold && !MVPSpentGold)
        {
            MVPSpentGold = true;
            FirebaseAnalytics.LogEvent("mvp_coins_spent_gold");
        }
        else if (totalCoinsSpent < configData.coinsToBecomeMVPGold && MVPSpentGold)
        {
            MVPSpentGold = false; // Unmark if the threshold is no longer met
        }
    }

    private void UpdateMVPAdWatchStatus()
    {
        var configData = FirebaseRemoteConfigManager.Instance.adsConfigData;
        
        // Check Bronze
        if (totalRewardedAdsWatched >= configData.adAmountToBecomeMVPBronze && !MVPRewardedAdsBronze)
        {
            MVPRewardedAdsBronze = true;
            FirebaseAnalytics.LogEvent("mvp_ad_watch_bronze");
        }
        else if (totalRewardedAdsWatched < configData.adAmountToBecomeMVPBronze && MVPRewardedAdsBronze)
        {
            MVPRewardedAdsBronze = false; // Unmark if the threshold is no longer met
        }

        // Check Silver
        if (totalRewardedAdsWatched >= configData.adAmountToBecomeMVPSilver && !MVPRewardedAdsSilver)
        {
            MVPRewardedAdsSilver = true;
            FirebaseAnalytics.LogEvent("mvp_ad_watch_silver");
        }
        else if (totalRewardedAdsWatched < configData.adAmountToBecomeMVPSilver && MVPRewardedAdsSilver)
        {
            MVPRewardedAdsSilver = false; // Unmark if the threshold is no longer met
        }

        // Check Gold
        if (totalRewardedAdsWatched >= configData.adAmountToBecomeMVPGold && !MVPRewardedAdsGold)
        {
            MVPRewardedAdsGold = true;
            FirebaseAnalytics.LogEvent("mvp_ad_watch_gold");
        }
        else if (totalRewardedAdsWatched < configData.adAmountToBecomeMVPGold && MVPRewardedAdsGold)
        {
            MVPRewardedAdsGold = false; // Unmark if the threshold is no longer met
        }
    }
}
