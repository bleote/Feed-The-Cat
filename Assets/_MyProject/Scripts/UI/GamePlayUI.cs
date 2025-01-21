using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    public static GamePlayUI Instance;
    [SerializeField] private Button pauseButton;
    [SerializeField] private PauseHandler pauseHandler;
    [SerializeField] private ContinueHandler continueHandler;
    [SerializeField] private TextMeshProUGUI highScoreDisplay;
    [SerializeField] private TextMeshProUGUI rewardingIceCreamTimerDisplay;

    [Header("Relax and Level Mode UI Setup")]
    [SerializeField] private GameObject relaxModeScores;
    [SerializeField] private GameObject levelModeGoal;
    [SerializeField] private GameObject healthBarNormal;
    [SerializeField] private GameObject goalSpawner;
    [SerializeField] private Image topHudImage;
    [SerializeField] private GameObject topHudBoss;
    [SerializeField] private GameObject[] goalGameObjects;
    [SerializeField] private GameObject[] startLevelGoalGameObjects;
    [SerializeField] private Sprite[] firstGoalSprites;
    [SerializeField] private Sprite[] secondGoalSprites;
    [SerializeField] private Sprite[] thirdGoalSprites;
    [SerializeField] private Sprite[] fourthGoalSprites;

    [Header("Goal Displays")]
    [SerializeField] private TextMeshProUGUI firstGoalDisplay;
    [SerializeField] private TextMeshProUGUI secondGoalDisplay;
    [SerializeField] private TextMeshProUGUI thirdGoalDisplay;
    [SerializeField] private TextMeshProUGUI fourthGoalDisplay;
    [SerializeField] private GameObject firstCheckmark;
    [SerializeField] private GameObject secondCheckmark;
    [SerializeField] private GameObject thirdCheckmark;
    [SerializeField] private GameObject fourthCheckmark;
    [SerializeField] private GameObject explosionPrefab;

    [Header("Boss Displays")]
    [SerializeField] private TextMeshProUGUI[] bossGoalDisplays;
    [SerializeField] private GameObject bossFirstCheckmark;
    [SerializeField] private GameObject bossSecondCheckmark;

    private int firstGoalTarget;
    private int secondGoalTarget;
    private int thirdGoalTarget;
    private int fourthGoalTarget;

    private int[] singleLevelGoals = { 1, 1, 1, 1, 1 }; // Goal from levels 1-5

    private int[,] doubleLevelGoals = // Goal from levels 6-12
    {
        { 5, 5 }, { 6, 6 }, { 10, 6 }, { 6, 10 }, { 9, 9 }, { 14, 6 }, { 10, 10 }
    };

    private int[,] tripleLevelGoals = // Goal from levels 13-21
    {
        { 12, 7, 5 }, { 12, 7, 5 }, { 14, 8, 6 }, { 14, 8, 6 }, { 15, 9, 6 }, { 16, 10, 6 }, { 18, 11, 7 }, { 20, 12, 8 }, { 21, 13, 8 }

    };

    private int[,] quadrupleLevelGoals = // Goal from levels 22-46
    {
        { 18, 14, 9, 5 }, { 19, 14, 10, 5 }, { 20, 15, 10, 5 }, { 22, 16, 11, 5 }, { 22, 17, 11, 6 }, { 24, 18, 12, 6 }, { 25, 19, 12, 6 },
        { 26, 19, 13, 6 }, { 26, 19, 13, 6 }, { 26, 20, 13, 7 }, { 28, 21, 14, 7 }, { 30, 22, 15, 7 }, { 30, 23, 15, 8 }, { 30, 23, 15, 8 },
        { 31, 23, 16, 8 }, { 31, 23, 16, 8 }, { 32, 24, 16, 8 }, { 32, 24, 16, 8 }, { 34, 25, 17, 8 }, { 34, 25, 17, 8 }, { 36, 27, 18, 9 },
        { 36, 27, 18, 9 }, { 38, 29, 19, 10 }, { 38, 29, 19, 10 }, { 0, 0, 40, 60 }
    };

    private int currentLevel;

    private void Awake()
    {
        Instance = this;

        MapLevelGoals();
    }

    private void Start()
    {
        currentLevel = GamePlayManager.currentLevel;

        if (!GamePlayManager.levelModeOn)
        {
            relaxModeScores.SetActive(true);
            levelModeGoal.SetActive(false);
            goalSpawner.SetActive(false);
            ShowHighScore();
        }
        else
        {
            if (GamePlayManager.currentLevel <= 45)
            {
                relaxModeScores.SetActive(false);
                levelModeGoal.SetActive(true);
            }

            if (GamePlayManager.Instance.bossLevel)
            {
                topHudImage.enabled = false;
                relaxModeScores.SetActive(false);
                levelModeGoal.SetActive(false);
                healthBarNormal.SetActive(false);
                topHudBoss.SetActive(true);
            }

            SetLevelGoal();
            goalSpawner.SetActive(true);
        }
    }

    private void MapLevelGoals()
    {
        var configData = FirebaseRemoteConfigManager.Instance.levelGoalsConfigData;

        // Map Single Level Goals (Levels 1-5)
        singleLevelGoals = new int[]
        {
            configData.Level01Goals[0],
            configData.Level02Goals[0],
            configData.Level03Goals[0],
            configData.Level04Goals[0],
            configData.Level05Goals[0]
        };


        //Map Double Level Goals(Levels 6 - 12)
        doubleLevelGoals = new int[7, 2]; // 7 levels, 2 goals each

        doubleLevelGoals[0, 0] = GetGoalArray(configData.Level06Goals, 2)[0];
        doubleLevelGoals[0, 1] = GetGoalArray(configData.Level06Goals, 2)[1];
        doubleLevelGoals[1, 0] = GetGoalArray(configData.Level07Goals, 2)[0];
        doubleLevelGoals[1, 1] = GetGoalArray(configData.Level07Goals, 2)[1];
        doubleLevelGoals[2, 0] = GetGoalArray(configData.Level08Goals, 2)[0];
        doubleLevelGoals[2, 1] = GetGoalArray(configData.Level08Goals, 2)[1];
        doubleLevelGoals[3, 0] = GetGoalArray(configData.Level09Goals, 2)[0];
        doubleLevelGoals[3, 1] = GetGoalArray(configData.Level09Goals, 2)[1];
        doubleLevelGoals[4, 0] = GetGoalArray(configData.Level10Goals, 2)[0];
        doubleLevelGoals[4, 1] = GetGoalArray(configData.Level10Goals, 2)[1];
        doubleLevelGoals[5, 0] = GetGoalArray(configData.Level11Goals, 2)[0];
        doubleLevelGoals[5, 1] = GetGoalArray(configData.Level11Goals, 2)[1];
        doubleLevelGoals[6, 0] = GetGoalArray(configData.Level12Goals, 2)[0];
        doubleLevelGoals[6, 1] = GetGoalArray(configData.Level12Goals, 2)[1];


        // Map Triple Level Goals (Levels 13-21)
        tripleLevelGoals = new int[9, 3]; // 9 levels, 3 goals each

        tripleLevelGoals[0, 0] = GetGoalArray(configData.Level13Goals, 3)[0];
        tripleLevelGoals[0, 1] = GetGoalArray(configData.Level13Goals, 3)[1];
        tripleLevelGoals[0, 2] = GetGoalArray(configData.Level13Goals, 3)[2];
        tripleLevelGoals[1, 0] = GetGoalArray(configData.Level14Goals, 3)[0];
        tripleLevelGoals[1, 1] = GetGoalArray(configData.Level14Goals, 3)[1];
        tripleLevelGoals[1, 2] = GetGoalArray(configData.Level14Goals, 3)[2];
        tripleLevelGoals[2, 0] = GetGoalArray(configData.Level15Goals, 3)[0];
        tripleLevelGoals[2, 1] = GetGoalArray(configData.Level15Goals, 3)[1];
        tripleLevelGoals[2, 2] = GetGoalArray(configData.Level15Goals, 3)[2];
        tripleLevelGoals[3, 0] = GetGoalArray(configData.Level16Goals, 3)[0];
        tripleLevelGoals[3, 1] = GetGoalArray(configData.Level16Goals, 3)[1];
        tripleLevelGoals[3, 2] = GetGoalArray(configData.Level16Goals, 3)[2];
        tripleLevelGoals[4, 0] = GetGoalArray(configData.Level17Goals, 3)[0];
        tripleLevelGoals[4, 1] = GetGoalArray(configData.Level17Goals, 3)[1];
        tripleLevelGoals[4, 2] = GetGoalArray(configData.Level17Goals, 3)[2];
        tripleLevelGoals[5, 0] = GetGoalArray(configData.Level18Goals, 3)[0];
        tripleLevelGoals[5, 1] = GetGoalArray(configData.Level18Goals, 3)[1];
        tripleLevelGoals[5, 2] = GetGoalArray(configData.Level18Goals, 3)[2];
        tripleLevelGoals[6, 0] = GetGoalArray(configData.Level19Goals, 3)[0];
        tripleLevelGoals[6, 1] = GetGoalArray(configData.Level19Goals, 3)[1];
        tripleLevelGoals[6, 2] = GetGoalArray(configData.Level19Goals, 3)[2];
        tripleLevelGoals[7, 0] = GetGoalArray(configData.Level20Goals, 3)[0];
        tripleLevelGoals[7, 1] = GetGoalArray(configData.Level20Goals, 3)[1];
        tripleLevelGoals[7, 2] = GetGoalArray(configData.Level20Goals, 3)[2];
        tripleLevelGoals[8, 0] = GetGoalArray(configData.Level21Goals, 3)[0];
        tripleLevelGoals[8, 1] = GetGoalArray(configData.Level21Goals, 3)[1];
        tripleLevelGoals[8, 2] = GetGoalArray(configData.Level21Goals, 3)[2];


        // Map Quadruple Level Goals (Levels 22-46)
        quadrupleLevelGoals = new int[25, 4]; // 25 levels, 4 goals each

        quadrupleLevelGoals[0, 0] = GetGoalArray(configData.Level22Goals, 4)[0];
        quadrupleLevelGoals[0, 1] = GetGoalArray(configData.Level22Goals, 4)[1];
        quadrupleLevelGoals[0, 2] = GetGoalArray(configData.Level22Goals, 4)[2];
        quadrupleLevelGoals[0, 3] = GetGoalArray(configData.Level22Goals, 4)[3];
        quadrupleLevelGoals[1, 0] = GetGoalArray(configData.Level23Goals, 4)[0];
        quadrupleLevelGoals[1, 1] = GetGoalArray(configData.Level23Goals, 4)[1];
        quadrupleLevelGoals[1, 2] = GetGoalArray(configData.Level23Goals, 4)[2];
        quadrupleLevelGoals[1, 3] = GetGoalArray(configData.Level23Goals, 4)[3];
        quadrupleLevelGoals[2, 0] = GetGoalArray(configData.Level24Goals, 4)[0];
        quadrupleLevelGoals[2, 1] = GetGoalArray(configData.Level24Goals, 4)[1];
        quadrupleLevelGoals[2, 2] = GetGoalArray(configData.Level24Goals, 4)[2];
        quadrupleLevelGoals[2, 3] = GetGoalArray(configData.Level24Goals, 4)[3];
        quadrupleLevelGoals[3, 0] = GetGoalArray(configData.Level25Goals, 4)[0];
        quadrupleLevelGoals[3, 1] = GetGoalArray(configData.Level25Goals, 4)[1];
        quadrupleLevelGoals[3, 2] = GetGoalArray(configData.Level25Goals, 4)[2];
        quadrupleLevelGoals[3, 3] = GetGoalArray(configData.Level25Goals, 4)[3];
        quadrupleLevelGoals[4, 0] = GetGoalArray(configData.Level26Goals, 4)[0];
        quadrupleLevelGoals[4, 1] = GetGoalArray(configData.Level26Goals, 4)[1];
        quadrupleLevelGoals[4, 2] = GetGoalArray(configData.Level26Goals, 4)[2];
        quadrupleLevelGoals[4, 3] = GetGoalArray(configData.Level26Goals, 4)[3];
        quadrupleLevelGoals[5, 0] = GetGoalArray(configData.Level27Goals, 4)[0];
        quadrupleLevelGoals[5, 1] = GetGoalArray(configData.Level27Goals, 4)[1];
        quadrupleLevelGoals[5, 2] = GetGoalArray(configData.Level27Goals, 4)[2];
        quadrupleLevelGoals[5, 3] = GetGoalArray(configData.Level27Goals, 4)[3];
        quadrupleLevelGoals[6, 0] = GetGoalArray(configData.Level28Goals, 4)[0];
        quadrupleLevelGoals[6, 1] = GetGoalArray(configData.Level28Goals, 4)[1];
        quadrupleLevelGoals[6, 2] = GetGoalArray(configData.Level28Goals, 4)[2];
        quadrupleLevelGoals[6, 3] = GetGoalArray(configData.Level28Goals, 4)[3];
        quadrupleLevelGoals[7, 0] = GetGoalArray(configData.Level29Goals, 4)[0];
        quadrupleLevelGoals[7, 1] = GetGoalArray(configData.Level29Goals, 4)[1];
        quadrupleLevelGoals[7, 2] = GetGoalArray(configData.Level29Goals, 4)[2];
        quadrupleLevelGoals[7, 3] = GetGoalArray(configData.Level29Goals, 4)[3];
        quadrupleLevelGoals[8, 0] = GetGoalArray(configData.Level30Goals, 4)[0];
        quadrupleLevelGoals[8, 1] = GetGoalArray(configData.Level30Goals, 4)[1];
        quadrupleLevelGoals[8, 2] = GetGoalArray(configData.Level30Goals, 4)[2];
        quadrupleLevelGoals[8, 3] = GetGoalArray(configData.Level30Goals, 4)[3];
        quadrupleLevelGoals[9, 0] = GetGoalArray(configData.Level31Goals, 4)[0];
        quadrupleLevelGoals[9, 1] = GetGoalArray(configData.Level31Goals, 4)[1];
        quadrupleLevelGoals[9, 2] = GetGoalArray(configData.Level31Goals, 4)[2];
        quadrupleLevelGoals[9, 3] = GetGoalArray(configData.Level31Goals, 4)[3];
        quadrupleLevelGoals[10, 0] = GetGoalArray(configData.Level32Goals, 4)[0];
        quadrupleLevelGoals[10, 1] = GetGoalArray(configData.Level32Goals, 4)[1];
        quadrupleLevelGoals[10, 2] = GetGoalArray(configData.Level32Goals, 4)[2];
        quadrupleLevelGoals[10, 3] = GetGoalArray(configData.Level32Goals, 4)[3];
        quadrupleLevelGoals[11, 0] = GetGoalArray(configData.Level33Goals, 4)[0];
        quadrupleLevelGoals[11, 1] = GetGoalArray(configData.Level33Goals, 4)[1];
        quadrupleLevelGoals[11, 2] = GetGoalArray(configData.Level33Goals, 4)[2];
        quadrupleLevelGoals[11, 3] = GetGoalArray(configData.Level33Goals, 4)[3];
        quadrupleLevelGoals[12, 0] = GetGoalArray(configData.Level34Goals, 4)[0];
        quadrupleLevelGoals[12, 1] = GetGoalArray(configData.Level34Goals, 4)[1];
        quadrupleLevelGoals[12, 2] = GetGoalArray(configData.Level34Goals, 4)[2];
        quadrupleLevelGoals[12, 3] = GetGoalArray(configData.Level34Goals, 4)[3];
        quadrupleLevelGoals[13, 0] = GetGoalArray(configData.Level35Goals, 4)[0];
        quadrupleLevelGoals[13, 1] = GetGoalArray(configData.Level35Goals, 4)[1];
        quadrupleLevelGoals[13, 2] = GetGoalArray(configData.Level35Goals, 4)[2];
        quadrupleLevelGoals[13, 3] = GetGoalArray(configData.Level35Goals, 4)[3];
        quadrupleLevelGoals[14, 0] = GetGoalArray(configData.Level36Goals, 4)[0];
        quadrupleLevelGoals[14, 1] = GetGoalArray(configData.Level36Goals, 4)[1];
        quadrupleLevelGoals[14, 2] = GetGoalArray(configData.Level36Goals, 4)[2];
        quadrupleLevelGoals[14, 3] = GetGoalArray(configData.Level36Goals, 4)[3];
        quadrupleLevelGoals[15, 0] = GetGoalArray(configData.Level37Goals, 4)[0];
        quadrupleLevelGoals[15, 1] = GetGoalArray(configData.Level37Goals, 4)[1];
        quadrupleLevelGoals[15, 2] = GetGoalArray(configData.Level37Goals, 4)[2];
        quadrupleLevelGoals[15, 3] = GetGoalArray(configData.Level37Goals, 4)[3];
        quadrupleLevelGoals[16, 0] = GetGoalArray(configData.Level38Goals, 4)[0];
        quadrupleLevelGoals[16, 1] = GetGoalArray(configData.Level38Goals, 4)[1];
        quadrupleLevelGoals[16, 2] = GetGoalArray(configData.Level38Goals, 4)[2];
        quadrupleLevelGoals[16, 3] = GetGoalArray(configData.Level38Goals, 4)[3];
        quadrupleLevelGoals[17, 0] = GetGoalArray(configData.Level39Goals, 4)[0];
        quadrupleLevelGoals[17, 1] = GetGoalArray(configData.Level39Goals, 4)[1];
        quadrupleLevelGoals[17, 2] = GetGoalArray(configData.Level39Goals, 4)[2];
        quadrupleLevelGoals[17, 3] = GetGoalArray(configData.Level39Goals, 4)[3];
        quadrupleLevelGoals[18, 0] = GetGoalArray(configData.Level40Goals, 4)[0];
        quadrupleLevelGoals[18, 1] = GetGoalArray(configData.Level40Goals, 4)[1];
        quadrupleLevelGoals[18, 2] = GetGoalArray(configData.Level40Goals, 4)[2];
        quadrupleLevelGoals[18, 3] = GetGoalArray(configData.Level40Goals, 4)[3];
        quadrupleLevelGoals[19, 0] = GetGoalArray(configData.Level41Goals, 4)[0];
        quadrupleLevelGoals[19, 1] = GetGoalArray(configData.Level41Goals, 4)[1];
        quadrupleLevelGoals[19, 2] = GetGoalArray(configData.Level41Goals, 4)[2];
        quadrupleLevelGoals[19, 3] = GetGoalArray(configData.Level41Goals, 4)[3];
        quadrupleLevelGoals[20, 0] = GetGoalArray(configData.Level42Goals, 4)[0];
        quadrupleLevelGoals[20, 1] = GetGoalArray(configData.Level42Goals, 4)[1];
        quadrupleLevelGoals[20, 2] = GetGoalArray(configData.Level42Goals, 4)[2];
        quadrupleLevelGoals[20, 3] = GetGoalArray(configData.Level42Goals, 4)[3];
        quadrupleLevelGoals[21, 0] = GetGoalArray(configData.Level43Goals, 4)[0];
        quadrupleLevelGoals[21, 1] = GetGoalArray(configData.Level43Goals, 4)[1];
        quadrupleLevelGoals[21, 2] = GetGoalArray(configData.Level43Goals, 4)[2];
        quadrupleLevelGoals[21, 3] = GetGoalArray(configData.Level43Goals, 4)[3];
        quadrupleLevelGoals[22, 0] = GetGoalArray(configData.Level44Goals, 4)[0];
        quadrupleLevelGoals[22, 1] = GetGoalArray(configData.Level44Goals, 4)[1];
        quadrupleLevelGoals[22, 2] = GetGoalArray(configData.Level44Goals, 4)[2];
        quadrupleLevelGoals[22, 3] = GetGoalArray(configData.Level44Goals, 4)[3];
        quadrupleLevelGoals[23, 0] = GetGoalArray(configData.Level45Goals, 4)[0];
        quadrupleLevelGoals[23, 1] = GetGoalArray(configData.Level45Goals, 4)[1];
        quadrupleLevelGoals[23, 2] = GetGoalArray(configData.Level45Goals, 4)[2];
        quadrupleLevelGoals[23, 3] = GetGoalArray(configData.Level45Goals, 4)[3];
        quadrupleLevelGoals[24, 0] = GetGoalArray(configData.Level46Goals, 4)[0];
        quadrupleLevelGoals[24, 1] = GetGoalArray(configData.Level46Goals, 4)[1];
        quadrupleLevelGoals[24, 2] = GetGoalArray(configData.Level46Goals, 4)[2];
        quadrupleLevelGoals[24, 3] = GetGoalArray(configData.Level46Goals, 4)[3];
    }

    private int[] GetGoalArray(List<int> goals, int expectedLength)
    {
        int[] result = new int[expectedLength];
        for (int i = 0; i < expectedLength; i++)
        {
            if (i < goals.Count)
            {
                result[i] = goals[i];
            }
            else
            {
                result[i] = 0; // Default to 0 if there are fewer goals than expected
            }
        }
        return result;
    }

    private void OnEnable()
    {
        GamePlayManager.GameEnded += HandleGameEnded;
        pauseButton.onClick.AddListener(Pause);
    }

    private void OnDisable()
    {
        GamePlayManager.GameEnded -= HandleGameEnded;
        pauseButton.onClick.RemoveListener(Pause);
    }

    private void Pause()
    {
        if (!GamePlayManager.isPaused)
        {
            pauseHandler.Setup();
        }
        else if (TutorialGameplay.Instance.togglePauseTutorial)
        {
            pauseHandler.TutorialFromSetup();
        }
    }

    private void HandleGameEnded(bool _result)
    {
        if (_result)
        {
            LevelCompletedHandler.Instance.Setup();
        }
        else
        {
            continueHandler.Setup();
        }
    }

    private void ShowHighScore()
    {
        highScoreDisplay.text = GamePlayManager.ConvertIntegerToString(DataManager.Instance.PlayerData.HighScore);
    }

    public void SetTimer(float _time)
    {
        rewardingIceCreamTimerDisplay.text = "00:" + _time.ToString(("00"));
    }

    private void SetLevelGoal()
    {
        if (currentLevel >= 2 && currentLevel <= 5)
        {
            SetSingleGoal(currentLevel - 1); // Levels 2 to 5 are indices 0 to 4 in singleLevelGoals
            SetFirstGoalImage(goalGameObjects[0], startLevelGoalGameObjects[0]);
        }
        else if (currentLevel >= 6 && currentLevel <= 12)
        {
            SetDoubleGoals(currentLevel - 6); // Levels 6 to 12 are indices 0 to 6 in doubleLevelGoals
            SetFirstGoalImage(goalGameObjects[0], startLevelGoalGameObjects[0]);
            SetSecondGoalImage(goalGameObjects[1], startLevelGoalGameObjects[1]);
        }
        else if (currentLevel >= 13 && currentLevel <= 21)
        {
            SetTripleGoals(currentLevel - 13); // Levels 13 to 21 are indices 0 to 8 in tripleLevelGoals
            SetFirstGoalImage(goalGameObjects[0], startLevelGoalGameObjects[0]);
            SetSecondGoalImage(goalGameObjects[1], startLevelGoalGameObjects[1]);
            SetThirdGoalImage(goalGameObjects[2], startLevelGoalGameObjects[2]);
        }
        else if (currentLevel >= 22 && currentLevel <= 45)
        {
            SetQuadrupleGoals(currentLevel - 22); // Levels 22 to 45 are indices 0 to 23 in quadrupleLevelGoals
            SetFirstGoalImage(goalGameObjects[0], startLevelGoalGameObjects[0]);
            SetSecondGoalImage(goalGameObjects[1], startLevelGoalGameObjects[1]);
            SetThirdGoalImage(goalGameObjects[2], startLevelGoalGameObjects[2]);
            SetFourthGoalImage(goalGameObjects[3], startLevelGoalGameObjects[3]);
        }
        else if (currentLevel >= 46)
        {
            SetBossGoals(currentLevel - 22); // Level 46 is indice 24 in quadrupleLevelGoals
            SetThirdGoalImage(goalGameObjects[2], startLevelGoalGameObjects[2]);
            SetFourthGoalImage(goalGameObjects[3], startLevelGoalGameObjects[3]);
        }
        else
        {
            SetSingleGoal(0); // Default goal for level 1
            SetFirstGoalImage(goalGameObjects[0], startLevelGoalGameObjects[0]);
        }
    }

    private void SetSingleGoal(int goalIndex)
    {
        goalGameObjects[0].SetActive(true);
        startLevelGoalGameObjects[0].SetActive(true);
        SetGoalQuantity(goalGameObjects[0], startLevelGoalGameObjects[0], singleLevelGoals[goalIndex]);

        var levelCompletion = LevelCompletedHandler.Instance;
        levelCompletion.firstGoalCompleted = false;
        levelCompletion.secondGoalCompleted = true;
        levelCompletion.thirdGoalCompleted = true;
        levelCompletion.fourthGoalCompleted = true;
    }

    private void SetDoubleGoals(int goalIndex)
    {
        for (int i = 0; i < doubleLevelGoals.GetLength(1); i++)
        {
            goalGameObjects[i].SetActive(true);
            startLevelGoalGameObjects[i].SetActive(true);
            SetGoalQuantity(goalGameObjects[i], startLevelGoalGameObjects[i], doubleLevelGoals[goalIndex, i]);
        }

        var levelCompletion = LevelCompletedHandler.Instance;
        levelCompletion.firstGoalCompleted = false;
        levelCompletion.secondGoalCompleted = false;
        levelCompletion.thirdGoalCompleted = true;
        levelCompletion.fourthGoalCompleted = true;
    }

    private void SetTripleGoals(int goalIndex)
    {
        for (int i = 0; i < tripleLevelGoals.GetLength(1); i++)
        {
            goalGameObjects[i].SetActive(true);
            startLevelGoalGameObjects[i].SetActive(true);
            SetGoalQuantity(goalGameObjects[i], startLevelGoalGameObjects[i], tripleLevelGoals[goalIndex, i]);
        }

        var levelCompletion = LevelCompletedHandler.Instance;
        levelCompletion.firstGoalCompleted = false;
        levelCompletion.secondGoalCompleted = false;
        levelCompletion.thirdGoalCompleted = false;
        levelCompletion.fourthGoalCompleted = true;
    }

    private void SetQuadrupleGoals(int goalIndex)
    {
        for (int i = 0; i < quadrupleLevelGoals.GetLength(1); i++)
        {
            goalGameObjects[i].SetActive(true);
            startLevelGoalGameObjects[i].SetActive(true);
            SetGoalQuantity(goalGameObjects[i], startLevelGoalGameObjects[i], quadrupleLevelGoals[goalIndex, i]);
        }

        var levelCompletion = LevelCompletedHandler.Instance;
        levelCompletion.firstGoalCompleted = false;
        levelCompletion.secondGoalCompleted = false;
        levelCompletion.thirdGoalCompleted = false;
        levelCompletion.fourthGoalCompleted = false;
    }

    private void SetGoalQuantity(GameObject goalGameObject, GameObject startLevelGoalGameObject, int goalAmount)
    {
        TextMeshProUGUI goalDisplay = goalGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI startGoalDisplay = startLevelGoalGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        goalDisplay.text = $"x{goalAmount}";
        startGoalDisplay.text = $"x{goalAmount}";

        SetLevelTargets(goalGameObject, goalAmount);
    }

    private void SetFirstGoalImage(GameObject goalGameObject, GameObject startLevelGoalGameObject)
    {
        Image goalImage = goalGameObject.transform.GetChild(0).GetComponent<Image>();
        Image goalStartImage = startLevelGoalGameObject.transform.GetChild(0).GetComponent<Image>();

        int goalSpriteIndex = GamePlayManager.firstIceCreamGoalIndex;

        goalImage.sprite = firstGoalSprites[goalSpriteIndex];
        goalStartImage.sprite = firstGoalSprites[goalSpriteIndex];
    }

    private void SetSecondGoalImage(GameObject goalGameObject, GameObject startLevelGoalGameObject)
    {
        Image goalImage = goalGameObject.transform.GetChild(0).GetComponent<Image>();
        Image goalStartImage = startLevelGoalGameObject.transform.GetChild(0).GetComponent<Image>();

        int goalSpriteIndex = GamePlayManager.secondIceCreamGoalIndex;

        goalImage.sprite = secondGoalSprites[goalSpriteIndex];
        goalStartImage.sprite = secondGoalSprites[goalSpriteIndex];
    }

    private void SetThirdGoalImage(GameObject goalGameObject, GameObject startLevelGoalGameObject)
    {
        Image goalImage = goalGameObject.transform.GetChild(0).GetComponent<Image>();
        Image goalStartImage = startLevelGoalGameObject.transform.GetChild(0).GetComponent<Image>();

        int goalSpriteIndex = GamePlayManager.thirdIceCreamGoalIndex;

        goalImage.sprite = thirdGoalSprites[goalSpriteIndex];
        goalStartImage.sprite = thirdGoalSprites[goalSpriteIndex];
    }

    private void SetFourthGoalImage(GameObject goalGameObject, GameObject startLevelGoalGameObject)
    {
        Image goalImage = goalGameObject.transform.GetChild(0).GetComponent<Image>();
        Image goalStartImage = startLevelGoalGameObject.transform.GetChild(0).GetComponent<Image>();

        int goalSpriteIndex = GamePlayManager.fourthIceCreamGoalIndex;

        goalImage.sprite = fourthGoalSprites[goalSpriteIndex];
        goalStartImage.sprite = fourthGoalSprites[goalSpriteIndex];
    }

    private void SetBossGoals(int goalIndex)
    {
        for (int i = 2; i < quadrupleLevelGoals.GetLength(1); i++)
        {
            startLevelGoalGameObjects[i].SetActive(true);
            SetBossGoalQuantity(bossGoalDisplays[i - 2], startLevelGoalGameObjects[i], quadrupleLevelGoals[goalIndex, i]);
        }

        SetBossLevelTargets(quadrupleLevelGoals[goalIndex, 2], quadrupleLevelGoals[goalIndex, 3]);

        var levelCompletion = LevelCompletedHandler.Instance;
        levelCompletion.firstGoalCompleted = true;
        levelCompletion.secondGoalCompleted = true;
        levelCompletion.thirdGoalCompleted = false;
        levelCompletion.fourthGoalCompleted = false;
    }

    private void SetBossGoalQuantity(TextMeshProUGUI bossGoalDisplay, GameObject startLevelGoalGameObject, int goalAmount)
    {
        TextMeshProUGUI startGoalDisplay = startLevelGoalGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        bossGoalDisplay.text = $"x{goalAmount}";
        startGoalDisplay.text = $"x{goalAmount}";
    }

    private void SetBossLevelTargets(int firstGoalAmount, int secondGoalAmount)
    {
        thirdGoalTarget = firstGoalAmount;
        fourthGoalTarget = secondGoalAmount;
    }

    private void SetLevelTargets(GameObject goalGameObject, int goalAmount)
    {
        if (goalGameObject == goalGameObjects[0])
        {
            firstGoalTarget = goalAmount;
        }
        else if (goalGameObject == goalGameObjects[1])
        {
            secondGoalTarget = goalAmount;
        }
        else if (goalGameObject == goalGameObjects[2])
        {
            thirdGoalTarget = goalAmount;
        }
        else if (goalGameObject == goalGameObjects[3])
        {
            fourthGoalTarget = goalAmount;
        }
        else
        {
            // Debug.Log($"Couldn't find a perfect match for {goalGameObject} and {goalAmount}");
            return;
        }
    }

    public void UpdateGoalDisplay(int goalSpawner, int amount)
    {
        if (GamePlayManager.Instance.bossLevel)
        {
            BossGoalDisplayUpdate(goalSpawner, amount);
        }
        else
        {
            NormalGoalDisplayUpdate(goalSpawner, amount);
        }

        CheckRemainingGoalIceCreams();
    }

    private void NormalGoalDisplayUpdate(int goalSpawner, int amount)
    {
        if (goalSpawner == 1)
        {
            if (firstGoalTarget > 0)
            {
                Instantiate(explosionPrefab, goalGameObjects[0].transform);
                firstGoalTarget -= amount;
            }
            else
            {
                return;
            }

            if (firstGoalTarget >= 1)
            {
                firstGoalDisplay.text = $"x{firstGoalTarget}";
            }
            else if (firstGoalTarget <= 0)
            {
                firstGoalDisplay.gameObject.SetActive(false);
                firstCheckmark.SetActive(true);
                LevelCompletedHandler.Instance.firstGoalCompleted = true;
                GoalSpawnerHandler.Instance.firstSpawnerAvailable = false;
            }
        }
        else if (goalSpawner == 2)
        {
            if (secondGoalTarget > 0)
            {
                Instantiate(explosionPrefab, goalGameObjects[1].transform);
                secondGoalTarget -= amount;
            }
            else
            {
                return;
            }

            if (secondGoalTarget >= 1)
            {
                secondGoalDisplay.text = $"x{secondGoalTarget}";
            }
            else if (secondGoalTarget <= 0)
            {
                secondGoalDisplay.gameObject.SetActive(false);
                secondCheckmark.SetActive(true);
                LevelCompletedHandler.Instance.secondGoalCompleted = true;
                GoalSpawnerHandler.Instance.secondSpawnerAvailable = false;
            }
        }
        else if (goalSpawner == 3)
        {
            if (thirdGoalTarget > 0)
            {
                Instantiate(explosionPrefab, goalGameObjects[2].transform);
                thirdGoalTarget -= amount;
            }
            else
            {
                return;
            }

            if (thirdGoalTarget >= 1)
            {
                thirdGoalDisplay.text = $"x{thirdGoalTarget}";
            }
            else if (thirdGoalTarget <= 0)
            {
                thirdGoalDisplay.gameObject.SetActive(false);
                thirdCheckmark.SetActive(true);
                LevelCompletedHandler.Instance.thirdGoalCompleted = true;
                GoalSpawnerHandler.Instance.thirdSpawnerAvailable = false;
            }
        }
        else if (goalSpawner == 4)
        {
            if (fourthGoalTarget > 0)
            {
                Instantiate(explosionPrefab, goalGameObjects[3].transform);
                fourthGoalTarget -= amount;
            }
            else
            {
                return;
            }

            if (fourthGoalTarget >= 1)
            {
                fourthGoalDisplay.text = $"x{fourthGoalTarget}";
            }
            else if (fourthGoalTarget <= 0)
            {
                fourthGoalDisplay.gameObject.SetActive(false);
                fourthCheckmark.SetActive(true);
                LevelCompletedHandler.Instance.fourthGoalCompleted = true;
                GoalSpawnerHandler.Instance.fourthSpawnerAvailable = false;
            }
        }
    }

    private void BossGoalDisplayUpdate(int goalSpawner, int amount)
    {
        if (goalSpawner == 3)
        {
            if (thirdGoalTarget > 0)
            {
                thirdGoalTarget -= amount;
            }
            else
            {
                return;
            }

            if (thirdGoalTarget >= 1)
            {
                bossGoalDisplays[0].text = $"x{thirdGoalTarget}";
            }
            else if (thirdGoalTarget <= 0)
            {
                bossGoalDisplays[0].gameObject.SetActive(false);
                bossFirstCheckmark.SetActive(true);
                LevelCompletedHandler.Instance.thirdGoalCompleted = true;
                GoalSpawnerHandler.Instance.thirdSpawnerAvailable = false;
            }
        }
        else if (goalSpawner == 4)
        {
            if (fourthGoalTarget > 0)
            {
                fourthGoalTarget -= amount;
            }
            else
            {
                return;
            }

            if (fourthGoalTarget >= 1)
            {
                bossGoalDisplays[1].text = $"x{fourthGoalTarget}";
            }
            else if (fourthGoalTarget <= 0)
            {
                bossGoalDisplays[1].gameObject.SetActive(false);
                bossSecondCheckmark.SetActive(true);
                LevelCompletedHandler.Instance.fourthGoalCompleted = true;
                GoalSpawnerHandler.Instance.fourthSpawnerAvailable = false;
            }
        }
    }

    private void CheckRemainingGoalIceCreams()
    {
        // Array to hold the target values
        int[] targets = { firstGoalTarget, secondGoalTarget, thirdGoalTarget, fourthGoalTarget };

        // Count how many targets are exactly 1
        int nearCompletionCount = 0;

        foreach (int target in targets)
        {
            if (target == 1)
            {
                nearCompletionCount++;
            }

            if (nearCompletionCount > 1)
            {
                return;
            }
        }

        // Set holdTutorials to true only if exactly one target is 1
        TutorialGameplay.Instance.holdTutorials = (nearCompletionCount == 1);
    }
}
