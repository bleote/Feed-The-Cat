using Firebase.Analytics;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContinueHandler : MonoBehaviour
{
    public static ContinueHandler Instance;

    [SerializeField] private GameObject continueBox;

    [SerializeField] private TextMeshProUGUI timerDisplay;
    [SerializeField] private Image timerBar;
    [SerializeField] private Button skipAdButton;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI skipAdCostDisplay;
    [SerializeField] private GameObject coinSpentAnimation;
    [SerializeField] private CoinsDisplay coinsHolder;

    [SerializeField] private LoseHandler loseHandler;
    [SerializeField] private int skipCost;
    [SerializeField] private int continueTime;

    private float counter;
    private bool hasUsed;
    private bool stopCounter;

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

        skipCost = FirebaseRemoteConfigManager.Instance.adsConfigData.skipAdCost;
    }

    public void Setup()
    {
        GamePlayManager.Pause();

        if (GamePlayManager.currentLevel < 5)
        {
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
        }

        if (DataManager.Instance.PlayerData.Coins < skipCost)
        {
            skipAdButton.interactable = false;
            TextMeshProUGUI skipAdButtonText = skipAdButton.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            skipAdButtonText.color = new(1, 1, 1, 0.1f);
        }
        else
        {
            skipAdButton.interactable = true;
            TextMeshProUGUI skipAdButtonText = skipAdButton.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            skipAdButtonText.color = new(1, 1, 1, 1);
        }

        counter = 0;
        coinsHolder.ShowCoins();
        gameObject.SetActive(true);
        coinsHolder.gameObject.SetActive(true);
        continueBox.SetActive(true);
        stopCounter = false;

        if (hasUsed)
        {
            Lose();
        }
    }

    private void OnEnable()
    {
        skipAdButton.onClick.AddListener(ContinueWithCoins);
        watchAdButton.onClick.AddListener(WatchAddToContinue);
        closeButton.onClick.AddListener(Lose);

        StartCoroutine(ContinueTimer());
        skipAdCostDisplay.text = skipCost.ToString();
    }

    private void OnDisable()
    {
        skipAdButton.onClick.RemoveListener(ContinueWithCoins);
        watchAdButton.onClick.RemoveListener(WatchAddToContinue);
        closeButton.onClick.RemoveListener(Lose);
    }

    private void ContinueWithCoins()
    {
        if (DataManager.Instance.PlayerData.Coins >= skipCost)
        {
            DataManager.Instance.PlayerData.Coins -= skipCost;
            skipAdButton.onClick.RemoveListener(ContinueWithCoins);
            ActivateCoinSpentAnimation();
            StartCoroutine(ContinueDelay());
        }

        FirebaseAnalytics.LogEvent(GamePlayManager.levelModeOn ? "continued_game_level_coins" : "continued_game_relax_coins");
    }

    private void WatchAddToContinue()
    {
        if (!GoogleMobileAdsManager.Instance.IsRewardedAdAvailable)
        {
            watchAdButton.interactable = false;
            return;
        }

        GamePlayManager.waitingToResume = true;

        GamePlayManager.continueProcess = true;

        GoogleMobileAdsManager.Instance.ShowRewardedAd(AdRewardType.ContinueGame);
    }

    private void ActivateCoinSpentAnimation()
    {
        stopCounter = true;
        TextMeshProUGUI coinSpentAnimationText = coinSpentAnimation.GetComponent<TextMeshProUGUI>();
        coinSpentAnimationText.text = $"-{skipCost}";
        coinSpentAnimation.SetActive(true);
        AudioManager.Instance.Play(AudioManager.COIN_SPENT_SOUND);
    }

    private IEnumerator ContinueDelay()
    {
        yield return new WaitForSecondsRealtime(1);

        Continue();
    }

    public void Continue()
    {
        hasUsed = true;
        MeltedIceCreamHandler.Instance.SetToZero();
        CharacterVisual.Instance.GetReadyToEat();
        GamePlayManager.continueProcess = false;
        GamePlayManager.Instance.RecoverToFullHealth();

        ResumeHandler.Instance.Resume();
        gameObject.SetActive(false);
    }

    private IEnumerator ContinueTimer()
    {
        while (gameObject.activeSelf)
        {
            if (counter >= continueTime)
            {
                Lose();
                break;
            }

            if (!stopCounter && !GamePlayManager.waitingToResume)
            {
                counter += Time.deltaTime;
            }

            timerDisplay.text = (int)(continueTime - counter) + "...";
            timerBar.fillAmount = counter / continueTime;
            yield return null;
        }
    }

    private void Lose()
    {
        loseHandler.Setup();
        gameObject.SetActive(false);
    }
}
