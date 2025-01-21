using UnityEngine;
using static FirebaseRemoteConfigManager;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    //Show Tutorials Player Choice
    public const string ACTIVATE_TUTORIALS = "ActivateTutorials";

    //Tutorial Steps
    public const string LEVEL_TUTORIAL = "LevelTutorial";
    public const string CAT_MOVEMENT_TUTORIAL = "CatMovementTutorial";
    public const string GOAL_TUTORIAL = "GoalTutorial";
    public const string ICECREAM_TUTORIAL = "IceCreamTutorial";
    public const string SUN_TUTORIAL = "SunTutorial";
    public const string CHILLI_TUTORIAL = "ChilliTutorial";
    public const string GREEN_CHILLI_TUTORIAL = "GreenChilliTutorial";
    public const string BOMB_CHILLI_TUTORIAL = "BombChilliTutorial";
    public const string PAUSE_TUTORIAL = "PauseTutorial";
    public const string RELAX_MENU_TUTORIAL = "RelaxMenuTutorial";
    public const string RELAX_GAME_TUTORIAL = "RelaxGameTutorial";
    public const string FIRST_TIME_PLAYED_RELAX_TUTORIAL = "FirstTimePlayedRelaxTutorial";
    public const string PLAY_BOTH_MODES_TUTORIAL = "PlayBothModesTutorial";
    public const string COIN_TUTORIAL = "CoinTutorial";
    public const string REWARDING_ICE_CREAM_TUTORIAL = "RewardingIceCreamTutorial";
    public const string PAW_TUTORIAL = "PawTutorial";
    public const string ICE_BUTTON_TUTORIAL = "IceButtonTutorial";
    public const string MELTED_LEVELS_TUTORIAL = "MeltedLevelsTutorial";
    public const string DEFEATED_BOSS_BOOLEAN = "DefeatedBossBooleanTutorial"; // just a checkmark to trigger the defeated boss tutorial at the main menu
    public const string DEFEATED_BOSS = "DefeatedBossTutorial";

    //Tutorial Booleans
    public bool levelTutorialActive;
    public bool catMovementTutorialActive;
    public bool goalTutorialActive;
    public bool iceCreamTutorialActive;
    public bool sunTutorialActive;
    public bool chilliTutorialActive;
    public bool chilliGreenTutorialActive;
    public bool chilliBombTutorialActive;
    public bool pauseTutorialActive;
    public bool relaxMenuTutorialActive;
    public bool relaxGameTutorialActive;
    public bool playBothModesTutorialActive;
    public bool coinTutorialActive;
    public bool rewardingIceCreamTutorialActive;
    public bool pawTutorialActive;
    public bool iceButtonTutorialActive;
    public bool meltedLevelsTutorialActive;
    public bool defeatedBossTutorialActive;

    private TutorialConfigData configData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            enabled = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        configData = FirebaseRemoteConfigManager.Instance.tutorialConfigData;

        RemoteConfigTutorialSwitches();
    }

    private void RemoteConfigTutorialSwitches()
    {
        levelTutorialActive = configData.levelTutorialActive;
        catMovementTutorialActive = configData.catMovementTutorialActive;
        goalTutorialActive = configData.goalTutorialActive;
        iceCreamTutorialActive = configData.iceCreamTutorialActive;
        sunTutorialActive = configData.sunTutorialActive;
        chilliTutorialActive = configData.chilliTutorialActive;
        chilliGreenTutorialActive = configData.chilliGreenTutorialActive;
        chilliBombTutorialActive = configData.chilliBombTutorialActive;
        pauseTutorialActive = configData.pauseTutorialActive;
        relaxMenuTutorialActive = configData.relaxMenuTutorialActive;
        relaxGameTutorialActive = configData.relaxGameTutorialActive;
        playBothModesTutorialActive = configData.playBothModesTutorialActive;
        coinTutorialActive = configData.coinTutorialActive;
        rewardingIceCreamTutorialActive = configData.rewardingIceCreamTutorialActive;
        pawTutorialActive = configData.pawTutorialActive;
        iceButtonTutorialActive = configData.iceButtonTutorialActive;
        meltedLevelsTutorialActive = configData.meltedLevelsTutorialActive;
        defeatedBossTutorialActive = configData.defeatedBossTutorialActive;
    }
}
