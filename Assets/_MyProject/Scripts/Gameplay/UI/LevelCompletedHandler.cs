using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletedHandler : MonoBehaviour
{
    public static LevelCompletedHandler Instance;

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button doublePrizeButton;
    [SerializeField] private Button tapToContinueButton;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject normalLevels;
    [SerializeField] private GameObject bossLevel;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int nextLevelAutomaticTrigger;
    [SerializeField] private int gameplayInterstitialCallTime;
    [SerializeField] private float characterDiscoverCallTime;
    [SerializeField] private Image nextLevelBarProgress;
    public int coinsPerLevel;
    public TextMeshProUGUI coinsPerLevelText;

    public bool firstGoalCompleted;
    public bool secondGoalCompleted;
    public bool thirdGoalCompleted;
    public bool fourthGoalCompleted;

    private float counter;
    private bool nextLevelTimer;

    private bool shouldShowCharacterDiscovery;
    private bool defeatedBoss;
    private bool doublePrize;
    private bool calledInterstitial;
    public bool waitingToLoadNextLevel;
    public bool waitingToLoadMainMenu;
    public bool waitingDiscovery;

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

        var configData = FirebaseRemoteConfigManager.Instance.generalConfigData;
        nextLevelAutomaticTrigger = configData.nextLevelAutomaticTrigger;
        gameplayInterstitialCallTime = configData.gameplayInterstitialCallTime;
        characterDiscoverCallTime = configData.characterDiscoverCallTime;
    }

    public void Setup()
    {
        GamePlayManager.Pause();

        if (ShouldShowCharacterDiscovery())
        {
            shouldShowCharacterDiscovery = true;
        }

        AudioManager.Instance.Stop(AudioManager.CLOCK_TICKING_SOUND);
        AudioManager.Instance.Stop(AudioManager.ALARM_SOUND);

        var t = TutorialGameplay.Instance;
        t.TurnOffAllTutorialMasksAtGameplay();
        t.toggleCatMovementTutorial = false;
        t.toggleRelaxGameTutorial = false;
        t.toggleGoalTutorial = false;
        t.toggleFoodTutorial = false;
        t.togglePauseTutorial = false;
        t.togglePawTutorial = false;
        t.toggleIceButtonTutorial = false;
        t.toggleMeltedLevelsTutorial = false;

        if (GamePlayManager.Instance.bossLevel)
        {
            normalLevels.SetActive(false);
            bossLevel.SetActive(true);
            PlayerPrefs.SetInt(TutorialManager.DEFEATED_BOSS_BOOLEAN, 1);
            defeatedBoss = true;
        }
        else
        {
            DataManager.Instance.PlayerData.Coins += coinsPerLevel;
            coinsPerLevelText.text = $"+{coinsPerLevel}";

            if (PlayerPrefs.GetInt(DataManager.PLAYER_FIRST_RETURN, -1) == -1)
            {
                PlayerPrefs.SetInt(DataManager.KEYS_KEY, DataManager.Instance.PlayerData.Keys);
                PlayerPrefs.SetInt(DataManager.COINS_KEY, DataManager.Instance.PlayerData.Coins);
                PlayerPrefs.SetInt(DataManager.HEARTS_KEY, DataManager.Instance.PlayerData.Hearts);
                PlayerPrefs.Save();
            }

            FirebaseManager.Instance.LevelCompleteLog();

            IncreaseGameLevelProgress();
        }

        if (!FirebaseRemoteConfigManager.Instance.generalConfigData.skipMap)
        {
            mainMenuButton.gameObject.SetActive(false);
            nextLevelButton.gameObject.SetActive(false);
            tapToContinueButton.gameObject.SetActive(true);
        }

        if (shouldShowCharacterDiscovery)
        {
            shouldShowCharacterDiscovery = false;
            mainMenuButton.gameObject.SetActive(false);
            nextLevelButton.gameObject.SetActive(false);
            doublePrizeButton.gameObject.SetActive(false);
            tapToContinueButton.gameObject.SetActive(false);
            panel.SetActive(true);
            AudioManager.Instance.Play(AudioManager.LEVEL_COMPLETE_SOUND);
            waitingDiscovery = true;
            StartCoroutine(CharacterDiscoveryRoutine());
        }
        else
        {
            counter = 0;
            nextLevelTimer = true;

            panel.SetActive(true);

            if (!defeatedBoss)
            {
                AudioManager.Instance.Play(AudioManager.LEVEL_COMPLETE_SOUND);
            }
            else
            {
                AudioManager.Instance.Play(AudioManager.VICTORY_SOUND);
            }

            StartCoroutine(NextLevelAutomaticTriggerRoutine());

            if (FirebaseRemoteConfigManager.Instance.adsConfigData.interstitialAdsActive && InterstitialAdTimer.Instance.callInterstitial)
            {
                StartCoroutine(InterstitialCallRoutine());
            }
        }

    }

    private bool ShouldShowCharacterDiscovery()
    {
        return (PlayerPrefs.GetInt(CatsScreenUI.FROSTBITE_DESCRIPTION, -1) == -1 && GamePlayManager.Instance.useAnnoyingBossEffect) ||
               (PlayerPrefs.GetInt(CatsScreenUI.SPLASH_DESCRIPTION, -1) == -1 && GamePlayManager.currentLevel == 11) ||
               (PlayerPrefs.GetInt(CatsScreenUI.PEBBLE_DESCRIPTION, -1) == -1 && GamePlayManager.currentLevel == 23);
    }

    private void OnEnable()
    {
        mainMenuButton.onClick.AddListener(MainMenu);
        nextLevelButton.onClick.AddListener(NextLevel);
        doublePrizeButton.onClick.AddListener(PlayRewardedAdForDoublePrize);
        tapToContinueButton.onClick.AddListener(MainMenu);
    }

    private void OnDisable()
    {
        mainMenuButton.onClick.RemoveListener(MainMenu);
        nextLevelButton.onClick.RemoveListener(NextLevel);
        doublePrizeButton.onClick.RemoveListener(PlayRewardedAdForDoublePrize);
        tapToContinueButton.onClick.RemoveListener(MainMenu);
    }

    private void IncreaseGameLevelProgress()
    {
        int nextLevel = GamePlayManager.currentLevel += 1;
        PlayerPrefs.SetInt(DataManager.UNLOCKED_LEVELS, nextLevel);
        PlayerPrefs.SetInt(DataManager.PREVIOUSLY_UNLOCKED_LEVEL, nextLevel);
    }

    private void DecreaseGameLevelProgress()
    {
        int nextLevel = GamePlayManager.currentLevel -= 1;
        PlayerPrefs.SetInt(DataManager.UNLOCKED_LEVELS, nextLevel);
        PlayerPrefs.SetInt(DataManager.PREVIOUSLY_UNLOCKED_LEVEL, nextLevel - 1);
    }

    public void CheckForLevelCompletion()
    {
        if (firstGoalCompleted && secondGoalCompleted && thirdGoalCompleted && fourthGoalCompleted)
        {
            StartCoroutine(LevelCompletionCoroutine());
        }
        else
        {
            return;
        }
    }

    private void TapToSkipToMainMenu()
    {
        if (FirebaseRemoteConfigManager.Instance.generalConfigData.skipMap)
        {
            return;
        }

        SetPinMovement();
        SceneController.LoadMainMenu();
    }

    private void MainMenu()
    {
        SetPinMovement();

        if (FirebaseRemoteConfigManager.Instance.adsConfigData.interstitialAdsActive && InterstitialAdTimer.Instance.callInterstitial && !doublePrize && !calledInterstitial)
        {
            calledInterstitial = true;
            GoogleMobileAdsManager.Instance.levelCompletedRequest = true;
            waitingToLoadMainMenu = true;
            content.SetActive(false);
            InterstitialAdTimer.Instance.InterstitialAdCall();
        }
        else
        {
            SceneController.LoadMainMenu();
        }
    }

    private void SetPinMovement()
    {
        DecreaseGameLevelProgress();

        PlayerPrefs.SetInt(DataManager.PIN_MOVEMENT, 1);
    }

    private void NextLevel()
    {
        if (FirebaseRemoteConfigManager.Instance.adsConfigData.interstitialAdsActive && InterstitialAdTimer.Instance.callInterstitial && !doublePrize && !calledInterstitial)
        {
            calledInterstitial = true;
            GoogleMobileAdsManager.Instance.levelCompletedRequest = true;
            waitingToLoadNextLevel = true;
            content.SetActive(false);
            InterstitialAdTimer.Instance.InterstitialAdCall();
        }
        else
        {
            SceneController.LoadGamePlay();
        }
    }

    public void InterstitialConclusionHandler()
    {
        if (waitingToLoadNextLevel)
        {
            waitingToLoadNextLevel = false;
            SceneController.LoadGamePlay();
        }
        else if (waitingToLoadMainMenu)
        {
            waitingToLoadMainMenu = false;
            SceneController.LoadMainMenu();
        }
    }

    public void CharacterDiscoveryConclusionHandler()
    {
        if (!FirebaseRemoteConfigManager.Instance.generalConfigData.skipMap)
        {
            tapToContinueButton.gameObject.SetActive(true);
        }
        else
        {
            mainMenuButton.gameObject.SetActive(true);
            nextLevelButton.gameObject.SetActive(true);
        }

        doublePrizeButton.gameObject.SetActive(true);
        
        explosion.SetActive(false);
        content.SetActive(true);
        
        counter = 0;
        nextLevelTimer = true;

        StartCoroutine(NextLevelAutomaticTriggerRoutine());

        if (FirebaseRemoteConfigManager.Instance.adsConfigData.interstitialAdsActive && InterstitialAdTimer.Instance.callInterstitial)
        {
            StartCoroutine(InterstitialCallRoutine());
        }
    }

    private void PlayRewardedAdForDoublePrize()
    {
        if (!GoogleMobileAdsManager.Instance.IsRewardedAdAvailable)
        {
            UIManager.Instance.OkDialog.Show("Ad is not ready");
            UIManager.Instance.OkDialog.OkPressed.AddListener(ResumeHandler.Instance.Resume);
            return;
        }

        GoogleMobileAdsManager.Instance.ShowDoublePrizeRewardedAd();
    }

    public void DoubleThePrize()
    {
        doublePrizeButton.gameObject.SetActive(false);
        doublePrize = true;
        Instantiate(explosionPrefab, coinsPerLevelText.transform);
        DataManager.Instance.PlayerData.Coins += coinsPerLevel;
        coinsPerLevelText.text = $"+{coinsPerLevel * 2}";
        StartCoroutine(DoubleCoinSound());
    }

    private IEnumerator DoubleCoinSound()
    {
        AudioManager.Instance.Play(AudioManager.COIN_COLLECT_SOUND);

        yield return new WaitForSecondsRealtime(0.25f);

        AudioManager.Instance.Play(AudioManager.COIN_COLLECT_SOUND);
    }

    private IEnumerator LevelCompletionCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        GamePlayManager.Instance.LevelCompleted();
    }

    private IEnumerator NextLevelAutomaticTriggerRoutine()
    {
        while (panel.activeSelf)
        {
            if (counter >= nextLevelAutomaticTrigger)
            {
                counter = nextLevelAutomaticTrigger;
                nextLevelTimer = false;
                NextLevel();
                break;
            }

            if (nextLevelTimer && !GamePlayManager.waitingToResume && !GamePlayManager.waitingInterstitialAdConclusion)
            {
                counter += Time.deltaTime;
            }

            nextLevelBarProgress.fillAmount = counter / nextLevelAutomaticTrigger;
            yield return null;
        }
    }

    private IEnumerator InterstitialCallRoutine()
    {
        float interstitialTimer = 0;

        while (!doublePrize && !calledInterstitial)
        {
            if (interstitialTimer >= gameplayInterstitialCallTime)
            {
                calledInterstitial = true;
                InterstitialAdTimer.Instance.InterstitialAdCall();
                counter = 0;
                yield break;
            }

            interstitialTimer += Time.deltaTime;

            yield return null; // Wait until the next frame
        }
    }

    private IEnumerator CharacterDiscoveryRoutine()
    {
        float characterTimer = 0;

        while (waitingDiscovery)
        {
            if (characterTimer >= characterDiscoverCallTime)
            {
                waitingDiscovery = false;
                content.SetActive(false);
                CharacterDiscoveryHandler.Instance.Setup();
                yield break;
            }

            characterTimer += Time.deltaTime;

            yield return null; // Wait until the next frame
        }
    }
}
