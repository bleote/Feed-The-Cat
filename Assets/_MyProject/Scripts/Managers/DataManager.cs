using UnityEngine;
using Newtonsoft.Json;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    //Strings used for PlayerPrefs
    public const string OPENED_MAIN_MENU = "OpenedMainMenu";
    public const string PLAYER_FIRST_RETURN = "PlayerFirstReturn";
    public const string UNLOCKED_LEVELS = "UnlockedLevels";
    public const string PREVIOUSLY_UNLOCKED_LEVEL = "PreviouslyUnlockedLevel";
    public const string PIN_MOVEMENT = "PinMovement";

    //Keys used to store player's info on Firebase Database
    public const string PAWS_KEY = "Paws";
    public const string HIGH_SCORE_KEY = "HighScore";
    public const string EXP_KEY = "Exp";
    public const string GAMEPLAY_LEVEL_KEY = "GameplayLevel";
    public const string USER_NAME_KEY = "UserName";
    public const string ELIXIRS_KEY = "Elixirs";
    public const string BISCUITS_KEY = "Biscuits";
    public const string EXTRA_LIVES_KEY = "ExtraLivesPackage";
    public const string OWNED_CATS_KEY = "OwnedCatIds";
    public const string HEARTS_KEY = "Hearts";
    public const string KEYS_KEY = "Keys";
    public const string LAST_TIME_CLOSED_APP_KEY = "LastTimeClosedApp";
    public const string SECONDS_LEFT_FOR_ANOTHER_HEART_KEY = "SecondsLeftForAnotherHeart";
    public const string COINS_KEY = "Coins";
    public const string COINS_SPENT_KEY = "CoinsSpent";
    private const string VIBRATION_KEY = "Vibration";
    private const string PLAY_SOUND_KEY = "PlaySound";
    private const string PLAY_MUSIC_KEY = "PlayMusic";
    private const string NOTIFICATIONS_KEY = "Notifications";
    private const string REWARDED_AD_WATCH_KEY = "RewardedAdWatch";

    //First time finish loading the game
    public const string FINISHED_FIRST_GAME_LOADING = "FinishedFirstGameLoading";

    //Reached Main Menu after laoding screen
    public const string OPENED_MAIN_MENU_AFTER_LOADING = "OpenedMainMenuAfterLoading";

    public bool actionPlayerFirstReturn;

    private PlayerData playerData;
    private GameData gameData;

    public PlayerData PlayerData => playerData;
    public GameData GameData => gameData;

    private int highScore;

    public event Action UpdatedHighScore;

    public int HighScore
    {
        get => highScore;
        set
        {
            if (highScore != value)
            {
                highScore = value;
                UpdatedHighScore?.Invoke();
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Routine.Initialize(this);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerData(string _playerData)
    {
        playerData = JsonConvert.DeserializeObject<PlayerData>(_playerData);

        // Debug.Log("Player data correctly deserialized");

        SubscribePlayerDataEvents();
    }

    public void SetGameData(string _gameData)
    {
        gameData = JsonConvert.DeserializeObject<GameData>(_gameData);
    }

    public void CreateNewPlayer()
    {
        playerData = new PlayerData();
        playerData.AddOwnedCat(0);
        playerData.LastTimeClosedApp = DateTime.UtcNow;
        playerData.SecondsLeftForAnotherHeart = 0;
        playerData.ChangeHearts(5);
        playerData.Paws = 5;
        playerData.ExtraLivesPackage = 5;
        playerData.Biscuits = 5;
        playerData.Elixirs = 5;
    }

    public void CreateNewGameData()
    {
        gameData = new GameData();
        GameData.MaxAmountOfHearts = 5;
        GameData.ReduceHearts = true;
    }

    private void SubscribePlayerDataEvents()
    {
        playerData.UpdatedPaws += SavePaws;
        playerData.UpdatedElixirs += SaveElixir;
        playerData.UpdatedBiscuits += SaveBiscuit;
        playerData.UpdatedHighScore += SaveHighScore;
        playerData.UpdatedExtraLives += SaveExtraLives;
        playerData.UpdatedUserName += SaveUserName;
        playerData.UpdatedOwnedCatsList += SaveOwnedCats;
        playerData.UpdatedKeys += SaveKeys;
        playerData.UpdatedHearts += SaveHearts;
        playerData.UpdatedCoins += SaveCoins;
        playerData.SpentCoins += SaveTotalCoinsSpent;
        PlayerData.UpdatedSounds += SaveSound;
        PlayerData.UpdatedMusic += SaveMusic;
        playerData.UpdatedVibration += SaveVibration;
        playerData.UpdatedNotifications += SaveNotifications;
        playerData.WatchedRewardedAd += SaveTotalRewardedAdsWatched;
    }

    private void SavePaws()
    {
        FirebaseManager.Instance.SaveValue(PAWS_KEY, playerData.Paws);
    }

    private void SaveElixir()
    {
        FirebaseManager.Instance.SaveValue(ELIXIRS_KEY, playerData.Elixirs);
    }

    private void SaveBiscuit()
    {
        FirebaseManager.Instance.SaveValue(BISCUITS_KEY, playerData.Biscuits);
    }

    private void SaveHighScore()
    {
        FirebaseManager.Instance.SaveValue(HIGH_SCORE_KEY, playerData.HighScore);
    }

    private void SaveExtraLives()
    {
        FirebaseManager.Instance.SaveValue(EXTRA_LIVES_KEY, playerData.ExtraLivesPackage);
    }

    private void SaveUserName()
    {
        FirebaseManager.Instance.SaveValue(USER_NAME_KEY, playerData.UserName);
    }

    private void SaveOwnedCats()
    {
        FirebaseManager.Instance.SaveJsonValue(OWNED_CATS_KEY, JsonConvert.SerializeObject(playerData.OwnedCatIds));
    }

    private void SaveKeys()
    {
        FirebaseManager.Instance.SaveValue(KEYS_KEY, playerData.Keys);
    }

    private void SaveHearts()
    {
        FirebaseManager.Instance.SaveValue(HEARTS_KEY, playerData.Hearts);
    }

    private void SaveLastTimeClosedApp()
    {
        FirebaseManager.Instance.SaveValue(LAST_TIME_CLOSED_APP_KEY, DateToString(PlayerData.LastTimeClosedApp));
        FirebaseManager.Instance.SaveValue(SECONDS_LEFT_FOR_ANOTHER_HEART_KEY, PlayerData.SecondsLeftForAnotherHeart);
    }

    private void SaveCoins()
    {
        FirebaseManager.Instance.SaveValue(COINS_KEY,PlayerData.Coins);
    }

    private void SaveTotalCoinsSpent()
    {
        FirebaseManager.Instance.SaveValue(COINS_SPENT_KEY, PlayerData.TotalCoinsSpent);
    }

    private void SaveVibration()
    {
        FirebaseManager.Instance.SaveValue(VIBRATION_KEY,PlayerData.Vibration);
    }

    private void SaveSound()
    {
        FirebaseManager.Instance.SaveValue(PLAY_SOUND_KEY,playerData.PlaySound);
    }

    private void SaveMusic()
    {
        FirebaseManager.Instance.SaveValue(PLAY_MUSIC_KEY,playerData.PlayMusic);
    }

    private void SaveNotifications()
    {
        FirebaseManager.Instance.SaveValue(NOTIFICATIONS_KEY,playerData.Notifications);
    }

    private void SaveTotalRewardedAdsWatched()
    {
        FirebaseManager.Instance.SaveValue(REWARDED_AD_WATCH_KEY, playerData.TotalRewardedAdsWatched);
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void UnsubscribeEvents()
    {
        playerData.UpdatedPaws -= SavePaws;
        playerData.UpdatedElixirs -= SaveElixir;
        playerData.UpdatedBiscuits -= SaveBiscuit;
        playerData.UpdatedHighScore -= SaveHighScore;
        playerData.UpdatedExtraLives -= SaveExtraLives;
        playerData.UpdatedUserName -= SaveUserName;
        playerData.UpdatedOwnedCatsList -= SaveOwnedCats;
        playerData.UpdatedKeys -= SaveKeys;
        playerData.UpdatedHearts -= SaveHearts;
        playerData.UpdatedCoins -= SaveCoins;
        playerData.SpentCoins -= SaveTotalCoinsSpent;
        PlayerData.UpdatedSounds -= SaveSound;
        PlayerData.UpdatedMusic -= SaveMusic;
        playerData.UpdatedVibration -= SaveVibration;
        playerData.UpdatedNotifications -= SaveNotifications;
        playerData.WatchedRewardedAd -= SaveTotalRewardedAdsWatched;
    }

    private void OnApplicationPause(bool _pause)
    {
        if (PlayerData == null)
        {
            return;
        }

        if (_pause)
        {
            PlayerData.LastTimeClosedApp = DateTime.UtcNow;
            PlayerData.SecondsLeftForAnotherHeart = HeartsManager.Instance.SecondsLeftForAnotherHeart;
            SaveLastTimeClosedApp();
        }
        else
        {
            HeartsManager.Instance.CalculateHearts();
        }
    }

    private void OnApplicationQuit()
    {
        if (PlayerData == null)
        {
            return;
        }

        PlayerData.LastTimeClosedApp = DateTime.UtcNow;
        PlayerData.SecondsLeftForAnotherHeart = HeartsManager.Instance.SecondsLeftForAnotherHeart;
        SaveLastTimeClosedApp();
    }

    private string DateToString(DateTime _dateTime)
    {
        string _month = _dateTime.Month < 10 ? "0" + _dateTime.Month : "" + _dateTime.Month;
        string _day = _dateTime.Day < 10 ? "0" + _dateTime.Day : "" + _dateTime.Day;
        string _hour = _dateTime.Hour < 10 ? "0" + _dateTime.Hour : "" + _dateTime.Hour;
        string _minute = _dateTime.Minute < 10 ? "0" + _dateTime.Minute : "" + _dateTime.Minute;
        string _seconds = _dateTime.Second < 10 ? "0" + _dateTime.Second : "" + _dateTime.Second;
        return $"{_dateTime.Year}-{_month}-{_day}T{_hour}:{_minute}:{_seconds}";
    }
}
