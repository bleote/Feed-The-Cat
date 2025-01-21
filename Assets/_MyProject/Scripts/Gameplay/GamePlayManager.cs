using Firebase.Analytics;
using System;
using System.Collections;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager Instance;

    private int maxHP;
    private int score;
    private int currentHP;

    public int Multiplier { get; private set; } = 1;
    public Transform foodHolder;

    public static Action UpdatedScore;
    public static Action UpdatedCurrentHP;
    public static Action<bool> GameEnded;
    public static Action UpdatedMultiplier;
    public static InputSystem input;

    public static bool isPaused;
    public static bool waitingInterstitialAdConclusion;
    public static bool waitingToResume;
    public static bool continueProcess;
    public static bool destroyMeltedIceCreams;

    [SerializeField] private GameObject firstGoalSpawner;
    [SerializeField] private GameObject secondGoalSpawner;
    [SerializeField] private GameObject thirdGoalSpawner;
    [SerializeField] private GameObject fourthGoalSpawner;

    public static int firstIceCreamGoalIndex;
    public static int secondIceCreamGoalIndex;
    public static int thirdIceCreamGoalIndex;
    public static int fourthIceCreamGoalIndex;

    public static bool levelModeOn;

    public static int currentLevel;

    public bool useWaterfallEffect = false; // Toggle based on level

    public bool useBombRainEffect = false; // Toggle based on level

    public bool useAnnoyingBossEffect = false; // Toggle based on level

    public bool bossLevel = false; // Toggle based on level

    public int Score
    {
        get => score;
        set
        {
            score = value;
            UpdatedScore?.Invoke();
        }
    }

    public int MaxHP => maxHP;

    public int CurrentHP => currentHP;

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

        InitialStatus();

        SetLevelType();
    }

    private void InitialStatus()
    {
        currentLevel = PlayerPrefs.GetInt(DataManager.UNLOCKED_LEVELS, 1);
        isPaused = false;
        waitingToResume = false;
        waitingInterstitialAdConclusion = false;
        continueProcess = false;
        destroyMeltedIceCreams = false;
    }

    private void SetLevelType()
    {
        if (levelModeOn)
        {
            // Determine if the waterfall effect should be used by the spawners based on the current level
            useWaterfallEffect = DetermineWaterfallEffect(currentLevel);
            useBombRainEffect = DetermineBombRainEffect(currentLevel);
            useAnnoyingBossEffect = DetermineAnnoyingBossEffect(currentLevel);
            bossLevel = CheckForBossLevel(currentLevel);
        }
    }

    private void Start()
    {
        input = new InputStandalone();
        Score = 0;
        maxHP = 100;
        currentHP = maxHP;
        Vibration.Init();

        if (DataManager.Instance.PlayerData.Hearts <= 0)
        {
            SceneController.LoadMainMenu();
            UIManager.Instance.OkDialog.Show("You're out of hearts! Wait to refill or watch an ad to keep playing!");
            return;
        }

        if (levelModeOn)
        {
            FirebaseManager.Instance.LogLevelStart();

            if (bossLevel)
            {
                firstGoalSpawner.SetActive(false);
                secondGoalSpawner.SetActive(false);
                thirdIceCreamGoalIndex = SetThirdIceCreamGoalIndex();
                thirdGoalSpawner.SetActive(true);
                fourthIceCreamGoalIndex = SetFourthIceCreamGoalIndex();
                fourthGoalSpawner.SetActive(true);
            }
            else
            {
                firstIceCreamGoalIndex = SetFirstIceCreamGoalIndex();

                if (currentLevel >= 6)
                {
                    secondIceCreamGoalIndex = SetSecondIceCreamGoalIndex();
                    secondGoalSpawner.SetActive(true);
                }

                if (currentLevel >= 13)
                {
                    thirdIceCreamGoalIndex = SetThirdIceCreamGoalIndex();
                    thirdGoalSpawner.SetActive(true);
                }

                if (currentLevel >= 22)
                {
                    fourthIceCreamGoalIndex = SetFourthIceCreamGoalIndex();
                    fourthGoalSpawner.SetActive(true);
                }
            }

            if (currentLevel == 1 &&
                TutorialManager.Instance.catMovementTutorialActive &&
                PlayerPrefs.GetInt(TutorialManager.CAT_MOVEMENT_TUTORIAL, -1) == -1)
            {
                TutorialGameplay.Instance.CatMovementTutorialSetup();
            }
            else
            {
                StartLevelHandler.Instance.Setup();
            }
        }
        else
        {
            FirebaseAnalytics.LogEvent("relax_mode_start");

            if (TutorialManager.Instance.relaxGameTutorialActive && PlayerPrefs.GetInt(TutorialManager.RELAX_GAME_TUTORIAL, -1) == -1)
            {
                TutorialGameplay.Instance.RelaxGameTutorialSetup();
            }
        }

        if (bossLevel)
        {
            AudioManager.Instance.PlayBackgroundMusic(AudioManager.BOSS_THEME_MUSIC);
        }
        else
        {
            AudioManager.Instance.PlayBackgroundMusic(AudioManager.GAMEPLAY_MUSIC);
        }
    }

    private void OnDestroy()
    {
        isPaused = false;
    }

    public static void Pause()
    {
        isPaused = true;

        FoodSpawner.Instance.enabled = false;
        if (currentLevel > 2)
        {
            ExtraChilliSpawner.Instance.enabled = false;
        }
        IceCubeSpawner.Instance.enabled = false;
        CoinsSpawner.Instance.enabled = false;
        RewardingIceCreamSpawner.Instance.enabled = false;
    }

    public static void Play()
    {
        if (destroyMeltedIceCreams)
        {
            destroyMeltedIceCreams = false;
        }

        if (waitingToResume)
        {
            waitingToResume = false;
        }

        isPaused = false;

        FoodSpawner.Instance.enabled = true;
        if (currentLevel > 2)
        {
            ExtraChilliSpawner.Instance.enabled = true;
        }
        IceCubeSpawner.Instance.enabled = true;
        CoinsSpawner.Instance.enabled = true;
        RewardingIceCreamSpawner.Instance.enabled = true;
    }

    public void RecoverToFullHealth()
    {
        currentHP = maxHP;
        UpdatedCurrentHP?.Invoke();
    }

    public void TakeDamage(int _amount, FoodType chilliType)
    {
        DamageAnimationHandler.Instance.InitiateDamageFlashCoroutine();

        currentHP -= _amount;
        UpdatedCurrentHP?.Invoke();

        if (currentHP <= 0)
        {
            GameEnded?.Invoke(false);
            FirebaseManager.Instance.LogDeathByChilly(chilliType);
        }
    }

    public void Drown()
    {
        GameEnded?.Invoke(false);
        FirebaseAnalytics.LogEvent("failed_by_flood");
    }

    public void LevelCompleted()
    {
        GameEnded?.Invoke(true);
    }

    public void IncreaseMultiplier()
    {
        Multiplier++;
        UpdatedMultiplier?.Invoke();
    }

    public static string ConvertIntegerToString(long _score)
    {
        if (_score >= 1_000_000_000)
        {
            return (_score / 1_000_000_000f).ToString("0.00") + "B";
        }
        else if (_score >= 1_000_000)
        {
            return (_score / 1_000_000f).ToString("0.00") + "M";
        }
        else if (_score >= 1_000)
        {
            return (_score / 1_000f).ToString("0.00") + "k";
        }
        else
        {
            return _score.ToString("0");
        }
    }

    private int SetFirstIceCreamGoalIndex()
    {
        int chosenIndex;

        switch (currentLevel)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                chosenIndex = currentLevel - 1; // Adjusted to be zero-indexed
                break;

            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
                chosenIndex = currentLevel - 7; // Reset and zero-indexed
                break;

            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
            case 18:
                chosenIndex = currentLevel - 13; // Reset and zero-indexed
                break;

            case 19:
            case 20:
            case 21:
            case 22:
            case 23:
            case 24:
                chosenIndex = currentLevel - 19; // Reset and zero-indexed
                break;

            case 25:
            case 26:
            case 27:
            case 28:
            case 29:
            case 30:
                chosenIndex = currentLevel - 25; // Reset and zero-indexed
                break;

            case 31:
            case 32:
            case 33:
            case 34:
            case 35:
            case 36:
                chosenIndex = currentLevel - 31; // Reset and zero-indexed
                break;

            case 37:
            case 38:
            case 39:
            case 40:
            case 41:
            case 42:
                chosenIndex = currentLevel - 37; // Reset and zero-indexed
                break;

            case 43:
            case 44:
            case 45:
            case 46:
                chosenIndex = currentLevel - 43; // Reset and zero-indexed
                break;

            default:
                chosenIndex = 0; // Default to the first sprite in case of an unexpected level
                break;
        }

        return chosenIndex;
    }

    private int SetSecondIceCreamGoalIndex()
    {
        int chosenIndex;

        switch (currentLevel)
        {
            case 6:
            case 7:
            case 8:
                chosenIndex = currentLevel - 6; // Adjusted to be zero-indexed
                break;

            case 9:
            case 10:
            case 11:
                chosenIndex = currentLevel - 9; // Reset and zero-indexed
                break;

            case 12:
            case 13:
            case 14:
                chosenIndex = currentLevel - 12; // Reset and zero-indexed
                break;

            case 15:
            case 16:
            case 17:
                chosenIndex = currentLevel - 15; // Reset and zero-indexed
                break;

            case 18:
            case 19:
            case 20:
                chosenIndex = currentLevel - 18; // Adjusted to be zero-indexed
                break;

            case 21:
            case 22:
            case 23:
                chosenIndex = currentLevel - 21; // Reset and zero-indexed
                break;

            case 24:
            case 25:
            case 26:
                chosenIndex = currentLevel - 24; // Reset and zero-indexed
                break;

            case 27:
            case 28:
            case 29:
                chosenIndex = currentLevel - 27; // Reset and zero-indexed
                break;

            case 30:
            case 31:
            case 32:
                chosenIndex = currentLevel - 30; // Adjusted to be zero-indexed
                break;

            case 33:
            case 34:
            case 35:
                chosenIndex = currentLevel - 33; // Reset and zero-indexed
                break;

            case 36:
            case 37:
            case 38:
                chosenIndex = currentLevel - 36; // Reset and zero-indexed
                break;

            case 39:
            case 40:
            case 41:
                chosenIndex = currentLevel - 39; // Reset and zero-indexed
                break;

            case 42:
            case 43:
            case 44:
                chosenIndex = currentLevel - 42; // Reset and zero-indexed
                break;

            case 45:
            case 46:
                chosenIndex = currentLevel - 45; // Reset and zero-indexed
                break;

            default:
                chosenIndex = 0; // Default to the first sprite in case of an unexpected level
                break;
        }

        return chosenIndex;
    }

    private int SetThirdIceCreamGoalIndex()
    {
        int chosenIndex;

        switch (currentLevel)
        {
            case 13:
            case 14:
            case 15:
                chosenIndex = currentLevel - 13; // Reset and zero-indexed
                break;

            case 16:
            case 17:
            case 18:
                chosenIndex = currentLevel - 16; // Reset and zero-indexed
                break;

            case 19:
            case 20:
            case 21:
                chosenIndex = currentLevel - 19; // Adjusted to be zero-indexed
                break;

            case 22:
            case 23:
            case 24:
                chosenIndex = currentLevel - 22; // Reset and zero-indexed
                break;

            case 25:
            case 26:
            case 27:
                chosenIndex = currentLevel - 25; // Reset and zero-indexed
                break;

            case 28:
            case 29:
            case 30:
                chosenIndex = currentLevel - 28; // Reset and zero-indexed
                break;

            case 31:
            case 32:
            case 33:
                chosenIndex = currentLevel - 31; // Adjusted to be zero-indexed
                break;

            case 34:
            case 35:
            case 36:
                chosenIndex = currentLevel - 34; // Reset and zero-indexed
                break;

            case 37:
            case 38:
            case 39:
                chosenIndex = currentLevel - 37; // Reset and zero-indexed
                break;

            case 40:
            case 41:
            case 42:
                chosenIndex = currentLevel - 40; // Reset and zero-indexed
                break;

            case 43:
            case 44:
            case 45:
                chosenIndex = currentLevel - 43; // Reset and zero-indexed
                break;

            case 46:
                chosenIndex = currentLevel - 46; // Reset and zero-indexed
                break;

            default:
                chosenIndex = 0; // Default to the first sprite in case of an unexpected level
                break;
        }

        return chosenIndex;
    }

    private int SetFourthIceCreamGoalIndex()
    {
        int chosenIndex;

        switch (currentLevel)
        {
            default:
                chosenIndex = 0; // Default to the first sprite because there's only one Unique Ice Cream
                break;
        }

        return chosenIndex;
    }

    private bool DetermineWaterfallEffect(int level)
    {
        // Define the levels where the waterfall effect should be active
        int[] waterfallLevels = { 3, 8, 12, 16, 20, 24, 27, 28, 32, 33, 36, 38, 40, 44 }; // Waterfall levels
        foreach (int l in waterfallLevels)
        {
            if (level == l)
            {
                return true;
            }
        }
        return false;
    }

    private bool DetermineBombRainEffect(int level)
    {
        // Define the levels where the bomb rain effect should be active
        int[] bombRainLevels = { 6, 9, 11, 15, 19, 23, 26, 30, 34, 35, 37, 39, 41, 43, 45 }; // Bomb Rain levels
        foreach (int l in bombRainLevels)
        {
            if (level == l)
            {
                return true;
            }
        }
        return false;
    }

    private bool DetermineAnnoyingBossEffect(int level)
    {
        // Define the levels where the annoying boss effect should be active
        int[] annoyingBossLevels = { 4, 7, 13, 18, 20, 21, 25, 26, 28, 30, 33, 35, 36, 38, 39, 40, 42, 43, 44 }; // Annoying Boss levels
        foreach (int l in annoyingBossLevels)
        {
            if (level == l)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckForBossLevel(int level)
    {
        if (level == 46)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnApplicationQuit()
    {
        FirebaseManager.Instance.LogGameplayQuit();

        StartCoroutine(QuitAfterDelay());
    }

    private IEnumerator QuitAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
    }
}
