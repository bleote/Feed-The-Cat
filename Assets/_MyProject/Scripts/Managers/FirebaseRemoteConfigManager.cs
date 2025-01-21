using Firebase.Extensions;
using Firebase.RemoteConfig;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseRemoteConfigManager : MonoBehaviour
{
    public static FirebaseRemoteConfigManager Instance;

    [Serializable]
    public class GeneralConfigData
    {
        // Hearts
        public float amountOfSecondsForNextHeart = 900;

        //Map
        public bool skipMap = true;

        //Level Completed
        public int nextLevelAutomaticTrigger = 7;
        public int gameplayInterstitialCallTime = 3;
        public float characterDiscoverCallTime = 1.5f;

        //Food Speed
        public bool variableFoodSpeed = false;
        public int foodMinSpeed = 600;
        public int foodMaxSpeed = 800;
        public int chilliSpeedDifference = -100;
        public int iceCubeSpeedDifference = 0;
        public int coinSpeedDifference = 0;
        public int rewardingIceCreamSpeedDifference = -200;
        public float foodVector3SpeedMultiplier = 9.0f;
        public float foodVector3SpeedMultiplierIncrease = 0.5f;

        //Spawners
        public float spawnCooldownReducer = 0.1f;
        public float minSpawnCooldown = 0.4f;
        public float foodSpawnCooldownRelax = 1.0f;
        public float foodSpawnCooldownIntro = 1.0f;
        public float foodSpawnCooldownEasy = 0.9f;
        public float foodSpawnCooldownMedium = 0.8f;
        public float foodSpawnCooldownHard = 0.7f;
        public float foodSpawnCooldownBoss = 0.7f;
        public int reduceSpawnCooldownTargetRelax = 50;
        public int reduceSpawnCooldownTargetIntro = 50;
        public int reduceSpawnCooldownTargetEasy = 50;
        public int reduceSpawnCooldownTargetMedium = 50;
        public int reduceSpawnCooldownTargetHard = 50;
        public int reduceSpawnCooldownTargetBoss = 100;
        public int spawnGoalAtMinIntro = 4;
        public int spawnGoalAtMaxIntro = 6;
        public int spawnGoalAtMinEasy = 4;
        public int spawnGoalAtMaxEasy = 6;
        public int spawnGoalAtMinMedium = 4;
        public int spawnGoalAtMaxMedium = 6;
        public int spawnGoalAtMinHard = 4;
        public int spawnGoalAtMaxHard = 6;
        public int spawnGoalAtMinBoss = 4;
        public int spawnGoalAtMaxBoss = 6;
        public float extraChilliMinCooldown = 10.0f;
        public float extraChilliMaxCooldown = 15.0f;
        public float iceCubeMinCooldown = 15.0f;
        public float iceCubeMaxCooldown = 20.0f;
        public float coinMinCooldown = 60.0f;
        public float coinMaxCooldown = 80.0f;
        public int activateExtraChilliSpawnerAtLevel = 3;
        public float waterfallFoodSpawnCoolDownIntro = 0.6f;
        public float waterfallFoodSpawnCoolDownEasy = 0.55f;
        public float waterfallFoodSpawnCoolDownMedium = 0.5f;
        public float waterfallFoodSpawnCoolDownHard = 0.4f;
        public float waterfallFoodSpawnCoolDownBoss = 0.4f;
    }

    public GeneralConfigData generalConfigData;

    [Serializable]
    public class AdsConfigData
    {
        public bool interstitialAdsActive = false;
        public int interstitialFrequencyInSeconds = 90;
        public int skipAdCost = 500;
        public int rewardingIceCreamCoolDownPeriod = 45;
        public int extraCoinsPerRewardedAd = 100;
        public int extraCoinsSpriteIndex = 0;
        public int extraHeartCost = 300;
        public int coinsToBecomeMVPBronze = 1000;
        public int coinsToBecomeMVPSilver = 2000;
        public int coinsToBecomeMVPGold = 3000;
        public int adAmountToBecomeMVPBronze = 5;
        public int adAmountToBecomeMVPSilver = 10;
        public int adAmountToBecomeMVPGold = 15;
    }

    public AdsConfigData adsConfigData;

    [Serializable]
    public class TutorialConfigData
    {
        public bool levelTutorialActive = false;
        public bool catMovementTutorialActive = true;
        public bool goalTutorialActive = false;
        public bool iceCreamTutorialActive = false;
        public bool sunTutorialActive = false;
        public bool chilliTutorialActive = false;
        public bool chilliGreenTutorialActive = false;
        public bool chilliBombTutorialActive = true;
        public bool pauseTutorialActive = true;
        public bool relaxMenuTutorialActive = false;
        public bool relaxGameTutorialActive = true;
        public bool playBothModesTutorialActive = false;
        public bool coinTutorialActive = false;
        public bool rewardingIceCreamTutorialActive = true;
        public bool pawTutorialActive = true;
        public bool iceButtonTutorialActive = true;
        public bool meltedLevelsTutorialActive = true;
        public bool defeatedBossTutorialActive = true;

        public string catMovementTutorialText = "Swipe LEFT or RIGHT\r\nto guide our hero.\r\n\r\nRemember to eat ALL ice creams to avoid floods!";
        public string relaxGameTutorialText = "Play endlessly in RELAX mode! No levels, just you, the ice creams, and your best score!";
        public string goalTutorialText = "This is your ice cream goal to complete this level!";
        public string chilliBombTutorialText = "One bite of a bomb chili and it's game over for your hero!";
        public string pauseTutorialText = "Tap Scoop's profile to pause the game at anytime.";
        public string coinTutorialText = "Collect coins and use them to buy items or skip ads!";
        public string rewardingIceCreamTutorialText = "Eat the reward ice cream for special boosts to help you complete the level!";
        public string pawTutorialText = "Unleash the PAW Button to eat all ice creams and eliminate all peppers instantly!";
        public string iceButtonTutorialText = "Activate the Ice Button to freeze all peppers for a while!";
        public string meltedLevelsTutorialText = "Be careful! Miss too many ice creams, and it will be game over!";

        public int goalTutorialGameLevel = 1;
        public int pauseTutorialGameLevel = 15;
        public int tutorialSetupDelay = 10;
        public float tutorialDisplayMinInterval = 1;
    }

    public TutorialConfigData tutorialConfigData;

    [Serializable]
    public class PawButtonConfigData
    {
        public int pawButtonTargetRelax = 40;
        public int pawButtonTargetIntro = 25;
        public int pawButtonTargetEasy = 35;
        public int pawButtonTargetMedium = 45;
        public int pawButtonTargetHard = 55;
        public int pawButtonTargetBoss = 10;
    }

    public PawButtonConfigData pawButtonConfigData;

    [Serializable]
    public class IceButtonConfigData
    {
        public int iceButtonTargetRelax = 4;
        public int iceButtonTargetIntro = 2;
        public int iceButtonTargetEasy = 3;
        public int iceButtonTargetMedium = 4;
        public int iceButtonTargetHard = 5;
        public int iceButtonTargetBoss = 6;
        public float freezeTime = 5;
    }

    public IceButtonConfigData iceButtonConfigData;

    [Serializable]
    public class LevelGoalsConfigData
    {
        public List<int> Level01Goals = new() { 2 };
        public List<int> Level02Goals = new() { 4 };
        public List<int> Level03Goals = new() { 6 };
        public List<int> Level04Goals = new() { 6 };
        public List<int> Level05Goals = new() { 8 };
        public List<int> Level06Goals = new() { 5, 5 };
        public List<int> Level07Goals = new() { 6, 6 };
        public List<int> Level08Goals = new() { 10, 6 };
        public List<int> Level09Goals = new() { 6, 10 };
        public List<int> Level10Goals = new() { 9, 9 };
        public List<int> Level11Goals = new() { 14, 6 };
        public List<int> Level12Goals = new() { 10, 10 };
        public List<int> Level13Goals = new() { 12, 7, 5 };
        public List<int> Level14Goals = new() { 12, 7, 5 };
        public List<int> Level15Goals = new() { 14, 8, 6 };
        public List<int> Level16Goals = new() { 14, 8, 6 };
        public List<int> Level17Goals = new() { 15, 9, 6 };
        public List<int> Level18Goals = new() { 16, 10, 6 };
        public List<int> Level19Goals = new() { 18, 11, 7 };
        public List<int> Level20Goals = new() { 20, 12, 8 };
        public List<int> Level21Goals = new() { 21, 13, 8 };
        public List<int> Level22Goals = new() { 18, 14, 9, 5 };
        public List<int> Level23Goals = new() { 19, 14, 10, 5 };
        public List<int> Level24Goals = new() { 20, 15, 10, 5 };
        public List<int> Level25Goals = new() { 22, 16, 11, 5 };
        public List<int> Level26Goals = new() { 22, 17, 11, 6 };
        public List<int> Level27Goals = new() { 24, 18, 12, 6 };
        public List<int> Level28Goals = new() { 25, 19, 12, 6 };
        public List<int> Level29Goals = new() { 26, 19, 13, 6 };
        public List<int> Level30Goals = new() { 26, 19, 13, 6 };
        public List<int> Level31Goals = new() { 26, 20, 13, 7 };
        public List<int> Level32Goals = new() { 28, 21, 14, 7 };
        public List<int> Level33Goals = new() { 30, 22, 15, 7 };
        public List<int> Level34Goals = new() { 30, 23, 15, 8 };
        public List<int> Level35Goals = new() { 30, 23, 15, 8 };
        public List<int> Level36Goals = new() { 31, 23, 16, 8 };
        public List<int> Level37Goals = new() { 31, 23, 16, 8 };
        public List<int> Level38Goals = new() { 32, 24, 16, 8 };
        public List<int> Level39Goals = new() { 32, 24, 16, 8 };
        public List<int> Level40Goals = new() { 34, 25, 17, 8 };
        public List<int> Level41Goals = new() { 34, 25, 17, 8 };
        public List<int> Level42Goals = new() { 36, 27, 18, 9 };
        public List<int> Level43Goals = new() { 36, 27, 18, 9 };
        public List<int> Level44Goals = new() { 38, 29, 19, 10 };
        public List<int> Level45Goals = new() { 38, 29, 19, 10 };
        public List<int> Level46Goals = new() { 0, 0, 40, 60 };
    }

    public LevelGoalsConfigData levelGoalsConfigData;

    [Serializable]
    public class CharactersConfigData
    {
        public string characterOneName = "SCOOP";
        public string characterOneText = "Scoop is the purrfect hero of Sundae Skies! Her love for ice cream and her boundless curiosity drive her to explore every sweet corner of this magical world. " +
            "But a chilling spell has turned her once-warm home into a frosty wasteland. With every ice cream she devours, Scoop melts away the cold, determined to restore Sundae Skies to its former glory. " +
            "Together with her friends (and you!), she’s ready to bring the sweetness back!";

        public string characterTwoName = "FROSTBITE";
        public string characterTwoText = "Once a part of Sundae Skies' joy-filled community, Frostbite grew cold—both inside and out. " +
            "Consumed by envy for the warmth and love that surrounded Scoop, he unleashed a bitter spell to freeze it all away. " +
            "But behind his frosty exterior lies a lonely soul longing for connection, even as he hides it beneath layers of ice. " +
            "Frostbite's icy kingdom is more than just a place—it’s a reflection of his frozen heart.";

        public string characterThreeName = "SPLASH";
        public string characterThreeText = "Splash is a bubbly seal with a heart of gold! Always up for a swim or a snack, Splash’s playful spirit " +
            "makes him the perfect companion on Scoop’s quest.";

        public string characterFourName = "PEBBLE";
        public string characterFourText = "Pebble is a clever penguin with a knack for solving icy puzzles! With a sharp mind, " +
            "Pebble keeps the team cool under pressure, always finding the smartest way to help Scoop.";
    }

    public CharactersConfigData charactersConfigData;

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

        //print("json:" + JsonUtility.ToJson(charactersConfigData));
        CheckRemoteConfigValues();
    }

    public Task CheckRemoteConfigValues()
    {
        // Debug.Log("Fetching Remote Config data...");
        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Remote Config Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task =>
            {
                // Debug.Log($"Remote Config data loaded and ready for use. Last fetch time {info.FetchTime}.");

                // Fetch and deserialize general config
                string generalConfigDataString = remoteConfig.GetValue("general_remote_config").StringValue;
                generalConfigData = JsonUtility.FromJson<GeneralConfigData>(generalConfigDataString);
                //Debug.Log("General remote config loaded successfully.");
                //print("json:" + JsonUtility.ToJson(generalConfigData));

                // Fetch and deserialize ads config
                string adsConfigDataString = remoteConfig.GetValue("ads_remote_config").StringValue;
                adsConfigData = JsonUtility.FromJson<AdsConfigData>(adsConfigDataString);

                // Fetch and deserialize tutorial config
                string tutorialConfigDataString = remoteConfig.GetValue("tutorial_remote_config").StringValue;
                tutorialConfigData = JsonUtility.FromJson<TutorialConfigData>(tutorialConfigDataString);

                // Fetch and deserialize paw button config
                string pawButtonConfigDataString = remoteConfig.GetValue("paw_button_remote_config").StringValue;
                pawButtonConfigData = JsonUtility.FromJson<PawButtonConfigData>(pawButtonConfigDataString);

                // Fetch and deserialize ice button config
                string iceButtonConfigDataString = remoteConfig.GetValue("ice_button_remote_config").StringValue;
                iceButtonConfigData = JsonUtility.FromJson<IceButtonConfigData>(iceButtonConfigDataString);

                // Fetch and deserialize level goals config
                string levelGoalsRemoteConfigDataString = remoteConfig.GetValue("level_goals_remote_config").StringValue;
                levelGoalsConfigData = JsonUtility.FromJson<LevelGoalsConfigData>(levelGoalsRemoteConfigDataString);

                // Fetch and deserialize the characters config
                string charactersRemoteConfigDataString = remoteConfig.GetValue("characters_remote_config").StringValue;
                charactersConfigData = JsonUtility.FromJson<CharactersConfigData>(charactersRemoteConfigDataString);

            });
    }
}
