using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    private const string USERS_KEY = "users";
    private const string GAME_DATA_KEY = "gameData";

    private FirebaseAuth auth;
    private DatabaseReference database;
    private FirebaseUser firebaseUser;

    public string PlayerId => firebaseUser.UserId;

    public bool IsLoggedIn => firebaseUser != null;

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

    public void Init(Action _callBack)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(_result =>
        {
            if (_result.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance.RootReference;
                _callBack?.Invoke();
            }
            else
            {
                throw new Exception("Couldn't fix dependencies in FireBaseManager.cs");
            }
        });
    }

    public void Login(Credential _credential, Action<bool, string> _loginCallback)
    {
        auth.SignInWithCredentialAsync(_credential).ContinueWithOnMainThread(_result =>
        {
            if (_result.Exception != null)
            {
                _loginCallback?.Invoke(false, "Failed to login: " + _result.Exception.Message);
            }
            else
            {
                firebaseUser = _result.Result;
                DecideIfThisIsNewUser(_loginCallback);
            }
        });
    }

    public void LoginAnonymous(Action<bool, string> _loginCallback)
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(_task =>
        {
            if (_task.IsCanceled)
            {
                _loginCallback(false, "SignIn was canceled.");
                return;
            }

            if (_task.IsFaulted)
            {
                _loginCallback?.Invoke(false, "SignIn encountered an error: " + _task.Exception);
                return;
            }

            // Extract FirebaseUser from AuthResult
            AuthResult result = _task.Result;
            firebaseUser = result.User;

            DecideIfThisIsNewUser(_loginCallback);
        });
    }

    private void DecideIfThisIsNewUser(Action<bool, string> _callback)
    {
        database.Child(USERS_KEY).Child(firebaseUser.UserId).GetValueAsync().ContinueWithOnMainThread(_task =>
        {
            if (_task.IsFaulted)
            {
                UIManager.Instance.OkDialog.Show("Error while checking if it is new user. Please contact support");
            }
            else if (_task.IsCompleted)
            {
                DataSnapshot _snapshot = _task.Result;
                bool _newUser = _snapshot.Child(DataManager.PAWS_KEY).Exists == false;
                string _isNewAccountKey = string.Empty;
                if (_newUser)
                {
                    _isNewAccountKey = AccountManager.NEW_ACC_KEY;
                }
                _callback?.Invoke(true, _isNewAccountKey);
            }
        });
    }

    public void LoadPlayerData(Action<string> _callBack)
    {
        database.Child(USERS_KEY).Child(firebaseUser.UserId).GetValueAsync().ContinueWithOnMainThread(_result =>
        {
            if (_result.IsFaulted)
            {
                Debug.LogError("Error loading player data: " + _result.Exception);
                _callBack?.Invoke(null);
                return;
            }
            if (!_result.IsCompleted)
            {
                Debug.LogWarning("LoadPlayerData not completed");
                return;
            }

            DataSnapshot _snapshot = _result.Result;
            string _jsonData = _snapshot.GetRawJsonValue();
            // Debug.Log("Player data loaded: " + _jsonData);
            _callBack?.Invoke(_jsonData);
        });
    }

    public void LoadGameData(Action<string> _callBack)
    {
        // Debug.Log("----- Sending request to Load Game Data");
        database.Child(GAME_DATA_KEY).GetValueAsync().ContinueWithOnMainThread(_result =>
        {
            if (_result.IsFaulted)
            {
                Debug.LogError("Error loading game data: " + _result.Exception);
                _callBack?.Invoke(null);
                return;
            }

            if (!_result.IsCompleted || _result.Result == null || !_result.Result.Exists)
            {
                Debug.LogWarning("Game data not found, initializing new game data.");
                DataManager.Instance.CreateNewGameData();
                _callBack?.Invoke(null);
                return;
            }

            DataSnapshot _snapshot = _result.Result;
            string _jsonData = _snapshot.GetRawJsonValue();
            // Debug.Log("----- Got response: " + _jsonData);
            _callBack?.Invoke(_jsonData);
        });
    }

    public void SaveValue<T>(string _path, T _value)
    {
        database.Child(USERS_KEY)
            .Child(firebaseUser.UserId)
            .Child(_path)
            .SetValueAsync(_value);
    }

    public void SaveJsonValue(string _path, string _json)
    {
        database.Child(USERS_KEY)
            .Child(firebaseUser.UserId)
            .Child(_path)
            .SetRawJsonValueAsync(_json);
    }

    public void SaveEverything(Action<bool> _callback)
    {
        string _data = JsonConvert.SerializeObject(DataManager.Instance.PlayerData);
        database.Child(USERS_KEY).Child(firebaseUser.UserId).SetRawJsonValueAsync(_data).ContinueWithOnMainThread(_result =>
        {
            if (_result.IsFaulted)
            {
                _callback.Invoke(false);
            }
            else if (_result.IsCompleted)
            {
                _callback?.Invoke(true);
            }
        });
    }

    public void GetLeaderboardEntries(Action<List<LeaderBoardEntry>> _callback)
    {
        database.Child(USERS_KEY).GetValueAsync().ContinueWithOnMainThread(_result =>
        {
            DataSnapshot _snapshot = _result.Result;
            List<LeaderBoardEntry> _entries = new();
            foreach (DataSnapshot _childSnapshot in _snapshot.Children)
            {
                string _name = _childSnapshot.Child(DataManager.USER_NAME_KEY).Value.ToString();
                int _score = Convert.ToInt32(_childSnapshot.Child(DataManager.HIGH_SCORE_KEY).Value.ToString());
                _entries.Add(new LeaderBoardEntry() { Nickname = _name, Score = _score });
            }
            _callback?.Invoke(_entries);
        });
    }

    public void LogAdRewardSelection(AdRewardType adRewardType)
    {
        string selectedReward;
        bool alreadyLogged = false;

        switch (adRewardType)
        {
            case AdRewardType.ExtraCoins:
                selectedReward = "Coins";
                break;

            case AdRewardType.ExtraHeart:
                selectedReward = "Extra Heart";
                break;

            case AdRewardType.ContinueGame:
                selectedReward = "Continue Game";
                alreadyLogged = true;
                break;

            case AdRewardType.ReduceMeltedLevels:
                selectedReward = "Reduce Melted Ice Level";
                break;

            case AdRewardType.RecoverHealth:
                selectedReward = "Recover To Full Health";
                break;

            case AdRewardType.ScoreMultiplier:
                selectedReward = "Increase Multiplier Points";
                break;

            case AdRewardType.LuckyWheel:
                selectedReward = "Spin Lucky Wheel";
                break;

            default:
                throw new System.Exception("Don`t know how to log this reward");
        }

        if (!alreadyLogged)
        {
            Parameter[] parameters = {
                new ("reward_name", $"Reward: {selectedReward}"),
            };

            FirebaseAnalytics.LogEvent("collected_reward", parameters);
        }
    }

    public void LogLevelStart()
    {
        Parameter[] parameters = {
            new ("level_name", $"Level {GamePlayManager.currentLevel}"),
        };

        FirebaseAnalytics.LogEvent("level_start", parameters);

        if (GamePlayManager.currentLevel == 3)
        {
            FirebaseAnalytics.LogEvent("milestone_level_03");
        }
        else if (GamePlayManager.currentLevel == 5)
        {
            FirebaseAnalytics.LogEvent("milestone_level_05");
        }
        else if (GamePlayManager.currentLevel == 10)
        {
            FirebaseAnalytics.LogEvent("milestone_level_10");
        }
    }

    public void LevelCompleteLog()
    {
        Parameter[] parameters = {
            new ("level_name", $"Level {GamePlayManager.currentLevel}"),
        };

        FirebaseAnalytics.LogEvent("level_complete", parameters);
    }

    public void LogLevelFail()
    {
        if (GamePlayManager.levelModeOn)
        {
            Parameter[] parameters = {
                new ("level_name", $"Level {GamePlayManager.currentLevel}"),
            };

            FirebaseAnalytics.LogEvent("level_fail", parameters);
        }
        else
        {
            FirebaseAnalytics.LogEvent("relax_mode_finish");
        }
    }

    public void LogDeathByChilly(FoodType chilliType)
    {
        switch (chilliType)
        {
            case FoodType.Chilli:
            case FoodType.ChilliGreen:
                FirebaseAnalytics.LogEvent("failed_by_pepper");
                break;

            case FoodType.ChilliBomb:
                FirebaseAnalytics.LogEvent("failed_by_bomb");
                break;

        }
    }

    public void LogPawButtonUsage()
    {
        FirebaseAnalytics.LogEvent("paw_button_used");

        if (!PlayerPrefs.HasKey("PawButtonFirstTimeUsage"))
        {
            PlayerPrefs.SetInt("PawButtonFirstTimeUsage", 1);

            FirebaseAnalytics.LogEvent("paw_button_first_time_usage");
        }
    }
    public void LogIceButtonUsage()
    {
        FirebaseAnalytics.LogEvent("ice_button_used");

        if (!PlayerPrefs.HasKey("IceButtonFirstTimeUsage"))
        {
            PlayerPrefs.SetInt("IceButtonFirstTimeUsage", 1);

            FirebaseAnalytics.LogEvent("ice_button_first_time_usage");
        }
    }

    public void LogCoinsSpent(int amountSpent)
    {
        Parameter[] parameters = {
            new ("coins_spent", amountSpent) // Log only the amount spent
        };

        FirebaseAnalytics.LogEvent("coins_spent", parameters);
    }

    public void LogRelaxModeTimeSpent(float timeSpent)
    {
        Parameter[] parameters = {
            new ("relax_mode_time_spent", timeSpent)
        };

        FirebaseAnalytics.LogEvent("relax_mode_duration", parameters);
    }

    public void LogGameplayQuit()
    {
        if (GamePlayManager.levelModeOn)
        {
            Parameter[] parameters = {
                new ("level_name", $"Level {GamePlayManager.currentLevel}"),
            };

            FirebaseAnalytics.LogEvent("level_quit", parameters);
        }
        else
        {
            FirebaseAnalytics.LogEvent("relax_mode_quit");
        }
    }

    public void LogCharacterDiscovery(string characterName)
    {
        Parameter[] parameters = {
            new ("character_name", characterName)
        };

        FirebaseAnalytics.LogEvent("character_discovery", parameters);
    }

    public void LogCharacterDescriptionView(string characterName)
    {
        Parameter[] parameters = {
            new ("character_name", characterName)
        };

        FirebaseAnalytics.LogEvent("character_description_view", parameters);
    }

}
