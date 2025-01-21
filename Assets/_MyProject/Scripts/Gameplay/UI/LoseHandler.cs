using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoseHandler : MonoBehaviour
{
    public static LoseHandler Instance;

    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button doubleScoreButton;
    [SerializeField] private NewHighScoreDisplay newHighScoreDisplay;
    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private TextMeshProUGUI highScoreDisplay;
    [SerializeField] private LevelUpHandler levelUpHandler;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject relaxModeResults;
    [SerializeField] private GameObject levelModeResults;
    [SerializeField] private GameObject explosionPrefab;
    public int gameplayInterstitialCallTime;

    private bool doubleScore;
    private bool calledInterstitial;
    public bool waitingToLoadPlayAgain;
    public bool waitingToLoadMainMenu;
    public bool waitingHighScoreDisplay;

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

        gameplayInterstitialCallTime = FirebaseRemoteConfigManager.Instance.generalConfigData.gameplayInterstitialCallTime;
    }

    public void Setup()
    {
        GamePlayManager.Pause();

        AudioManager.Instance.Stop(AudioManager.CLOCK_TICKING_SOUND);
        AudioManager.Instance.Stop(AudioManager.ALARM_SOUND);

        bool isFirstReturn = PlayerPrefs.GetInt(DataManager.PLAYER_FIRST_RETURN, -1) == -1;
        bool heartsReduced = DataManager.Instance.PlayerData.ReduceHearts();

        FirebaseManager.Instance.LogLevelFail();

        if (GamePlayManager.levelModeOn)
        {
            if (isFirstReturn)
            {
                if (heartsReduced)
                {
                    PlayerPrefs.SetInt(DataManager.HEARTS_KEY, DataManager.Instance.PlayerData.Hearts);
                }

                PlayerPrefs.SetInt(DataManager.COINS_KEY, DataManager.Instance.PlayerData.Coins);
            }
        }
        else
        {
            if (GamePlayManager.Instance.Score == 0)
            {
                doubleScoreButton.gameObject.SetActive(false);
            }
            else if (GamePlayManager.Instance.Score > DataManager.Instance.PlayerData.HighScore)
            {
                DataManager.Instance.PlayerData.HighScore = GamePlayManager.Instance.Score;
                waitingHighScoreDisplay = true;
                newHighScoreDisplay.Setup();

                if (isFirstReturn)
                {
                    PlayerPrefs.SetInt(DataManager.HIGH_SCORE_KEY, DataManager.Instance.PlayerData.HighScore);
                }
            }
        }


        if (isFirstReturn)
        {
            PlayerPrefs.Save();
        }

        // Update UI elements
        scoreDisplay.text = GamePlayManager.Instance.Score.ToString();
        highScoreDisplay.text = DataManager.Instance.PlayerData.HighScore.ToString();
        //bonusPawsDisplay.text = "+" + levelUpHandler.PawsEarned;
        //bonusElixirsDisplay.text = "+" + levelUpHandler.ElixirsEarned;
        //bonusExtraLivesDisplay.text = "+"+levelUpHandler.ExtraLivesEarned;
        //bonusBiscuitsDisplay.text = "+" + levelUpHandler.BiscuitsEarned;

        //levelUpHandler.Check();

        if (GamePlayManager.levelModeOn)
        {
            relaxModeResults.SetActive(false);
            AudioManager.Instance.Play(AudioManager.YOU_LOST_SOUND);
        }
        else
        {
            levelModeResults.SetActive(false);
        }

        gameObject.SetActive(true);

        PlayAgainButtonSetup();

        mainMenuButton.onClick.AddListener(MainMenu);
        doubleScoreButton.onClick.AddListener(PlayRewardedAdForDoubleScore);

        if (FirebaseRemoteConfigManager.Instance.adsConfigData.interstitialAdsActive && InterstitialAdTimer.Instance.callInterstitial)
        {
            if (!waitingHighScoreDisplay)
            {
                StartCoroutine(InterstitialCallRoutine());
            }
        }
    }

    private void OnDisable()
    {
        mainMenuButton.onClick.RemoveListener(MainMenu);
        playAgainButton.onClick.RemoveListener(PlayAgain);
        doubleScoreButton.onClick.RemoveListener(PlayRewardedAdForDoubleScore);

    }

    private void PlayAgainButtonSetup()
    {
        if (!GamePlayManager.levelModeOn && PlayerPrefs.GetInt(TutorialManager.FIRST_TIME_PLAYED_RELAX_TUTORIAL, -1) == -1)
        {
            RegisterFirstTimePlayedRelax();
        }

        playAgainButton.onClick.AddListener(PlayAgain);

    }

    private void RegisterFirstTimePlayedRelax()
    {
        PlayerPrefs.SetInt(TutorialManager.FIRST_TIME_PLAYED_RELAX_TUTORIAL, 1);
    }

    private void PlayAgain()
    {
        if (FirebaseRemoteConfigManager.Instance.adsConfigData.interstitialAdsActive && InterstitialAdTimer.Instance.callInterstitial && !doubleScore && !calledInterstitial)
        {
            calledInterstitial = true;
            GoogleMobileAdsManager.Instance.loseHandlerRequest = true;
            waitingToLoadPlayAgain = true;
            content.SetActive(false);
            InterstitialAdTimer.Instance.InterstitialAdCall();
        }
        else
        {
            SceneController.LoadGamePlay();
        }
    }

    private void MainMenu()
    {
        if (FirebaseRemoteConfigManager.Instance.adsConfigData.interstitialAdsActive && InterstitialAdTimer.Instance.callInterstitial && !doubleScore && !calledInterstitial)
        {
            calledInterstitial = true;
            GoogleMobileAdsManager.Instance.loseHandlerRequest = true;
            waitingToLoadMainMenu = true;
            content.SetActive(false);
            InterstitialAdTimer.Instance.InterstitialAdCall();
        }
        else
        {
            SceneController.LoadMainMenu();
        }
    }

    public void InitiateInterstitialRoutine()
    {
        if (!waitingHighScoreDisplay)
        {
            StartCoroutine(InterstitialCallRoutine());
        }
    }

    public void InterstitialConclusionHandler()
    {
        if (waitingToLoadPlayAgain)
        {
            waitingToLoadPlayAgain = false;
            SceneController.LoadGamePlay();
        }
        else if (waitingToLoadMainMenu)
        {
            waitingToLoadMainMenu = false;
            SceneController.LoadMainMenu();
        }
    }

    private void PlayRewardedAdForDoubleScore()
    {
        if (!GoogleMobileAdsManager.Instance.IsRewardedAdAvailable)
        {
            UIManager.Instance.OkDialog.Show("Ad is not ready");
            UIManager.Instance.OkDialog.OkPressed.AddListener(ResumeHandler.Instance.Resume);
            return;
        }

        GoogleMobileAdsManager.Instance.ShowDoubleScoreRewardedAd();
    }

    public void DoubleTheScore()
    {
        doubleScoreButton.gameObject.SetActive(false);
        doubleScore = true;
        GamePlayManager.Instance.Score = GamePlayManager.Instance.Score * 2;
        Instantiate(explosionPrefab, scoreDisplay.transform);
        scoreDisplay.text = GamePlayManager.Instance.Score.ToString();
        AudioManager.Instance.Play(AudioManager.BONUS_ACTIVATED_SOUND);

        if (GamePlayManager.Instance.Score > DataManager.Instance.PlayerData.HighScore)
        {
            DataManager.Instance.PlayerData.HighScore = GamePlayManager.Instance.Score;

            Instantiate(explosionPrefab, highScoreDisplay.transform);
            highScoreDisplay.text = DataManager.Instance.PlayerData.HighScore.ToString();
        }

        //Save it to Player Prefs if necessary
        bool isFirstReturn = PlayerPrefs.GetInt(DataManager.PLAYER_FIRST_RETURN, -1) == -1;

        if (isFirstReturn)
        {
            PlayerPrefs.SetInt(DataManager.HIGH_SCORE_KEY, DataManager.Instance.PlayerData.HighScore);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator InterstitialCallRoutine()
    {
        float interstitialTimer = 0;

        if (GamePlayManager.levelModeOn)
        {
            gameplayInterstitialCallTime -= 1;
        }

        while (!doubleScore && !calledInterstitial)
        {
            if (interstitialTimer >= gameplayInterstitialCallTime)
            {
                calledInterstitial = true;
                InterstitialAdTimer.Instance.InterstitialAdCall();
                yield break;
            }

            interstitialTimer += Time.deltaTime;

            yield return null; // Wait until the next frame
        }
    }
}
