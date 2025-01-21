using Firebase.Analytics;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardedAdHandler : MonoBehaviour
{
    public static RewardedAdHandler Instance;

    [SerializeField] private GameObject holder;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button reduceMeltedIceButton;
    [SerializeField] private Button increaseHealthButton;
    [SerializeField] private Button getMultiplierButton;
    [SerializeField] private Button getMoreRewardsButton;

    [Header("Skip Ad")]
    [SerializeField] private GameObject skipAdsCostGO;
    [SerializeField] private TextMeshProUGUI skipAdCostDisplay;
    [SerializeField] private Button skipAdButton;
    [SerializeField] private GameObject skipAdButton1;
    [SerializeField] private Animator skipAdButton1Animator;
    [SerializeField] private GameObject skipAdButton2;
    [SerializeField] private Animator skipAdButton2Animator;
    [SerializeField] private GameObject skipAdButton3;
    [SerializeField] private Animator skipAdButton3Animator;
    [SerializeField] private GameObject skipAdButton4;
    [SerializeField] private int skipCost;
    [SerializeField] private GameObject coinSpentAnimation;
    [SerializeField] private CoinsDisplay coinsHolder;
    private bool skipAdThisTime = false;

    private const string BUTTON_DISAPPEAR = "buttonDisappear";

    [Space()]
    [SerializeField]
    private MeltedIceCreamHandler meltedIceCreamHandler;

    private AdRewardType selectedRewardType;

    private void Awake()
    {
        Instance = this;

        skipCost = FirebaseRemoteConfigManager.Instance.adsConfigData.skipAdCost;
    }

    private void OnEnable()
    {
        closeButton.onClick.AddListener(Close);
        reduceMeltedIceButton.onClick.AddListener(ReduceMeltedLevels);
        increaseHealthButton.onClick.AddListener(RecoverHealth);
        getMultiplierButton.onClick.AddListener(ScoreMultiplier);
        //getMoreRewardsButton.onClick.AddListener(LuckyWheel);
        skipAdButton.onClick.AddListener(SkipAds);

        skipAdCostDisplay.text = skipCost.ToString();
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(Close);
        reduceMeltedIceButton.onClick.RemoveListener(ReduceMeltedLevels);
        increaseHealthButton.onClick.RemoveListener(RecoverHealth);
        getMultiplierButton.onClick.RemoveListener(ScoreMultiplier);
        //getMoreRewardsButton.onClick.RemoveListener(LuckyWheel);
        skipAdButton.onClick.RemoveListener(SkipAds);
    }

    public void Setup()
    {
        GamePlayManager.Pause();

        TutorialGameplay.Instance.TurnOffAllTutorialMasksAtGameplay();

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
        coinsHolder.gameObject.SetActive(true);
        coinsHolder.ShowCoins();
        holder.SetActive(true);
    }

    private void Close()
    {
        coinSpentAnimation.SetActive(false);
        holder.SetActive(false);
        ResumeHandler.Instance.Resume();

        FirebaseAnalytics.LogEvent("rv_ice_cream_canceled");
    }

    private void ReduceMeltedLevels()
    {
        selectedRewardType = AdRewardType.ReduceMeltedLevels;

        GamePlayManager.destroyMeltedIceCreams = true;

        HandleRewardCollection(selectedRewardType);
    }

    private void RecoverHealth()
    {
        selectedRewardType = AdRewardType.RecoverHealth;

        HandleRewardCollection(selectedRewardType);
    }

    private void ScoreMultiplier()
    {
        selectedRewardType = AdRewardType.ScoreMultiplier;

        HandleRewardCollection(selectedRewardType);
    }

    private void LuckyWheel()
    {
        selectedRewardType = AdRewardType.LuckyWheel;

        HandleRewardCollection(selectedRewardType);
    }

    private void HandleRewardCollection(AdRewardType adRewardType)
    {
        if (!skipAdThisTime)
        {
            PlayRewardedAd(adRewardType);
        }
        else
        {
            ActivateAdButtons();
            CollectReward(adRewardType);
        }
    }

    private void SkipAds()
    {
        if (DataManager.Instance.PlayerData.Coins >= skipCost)
        {
            DataManager.Instance.PlayerData.Coins -= skipCost;
            DeactivateAdButtons();
        }
    }

    private void PlayRewardedAd(AdRewardType adRewardType)
    {
        if (!GoogleMobileAdsManager.Instance.IsRewardedAdAvailable)
        {
            UIManager.Instance.OkDialog.Show("Ad is not ready");
            UIManager.Instance.OkDialog.OkPressed.AddListener(ResumeHandler.Instance.Resume);
            return;
        }

        GamePlayManager.waitingToResume = true;

        coinsHolder.gameObject.SetActive(false);

        GoogleMobileAdsManager.Instance.ShowRewardedAd(adRewardType);
    }

    public void CollectReward(AdRewardType adRewardType)
    {
        switch (adRewardType)
        {
            case AdRewardType.ReduceMeltedLevels:
                meltedIceCreamHandler.SetToZero();
                break;

            case AdRewardType.RecoverHealth:
                GamePlayManager.Instance.RecoverToFullHealth();
                break;

            case AdRewardType.ScoreMultiplier:
                GamePlayManager.Instance.IncreaseMultiplier();
                break;

            case AdRewardType.LuckyWheel:
                Debug.Log("Can't collect lucky wheel at this time");
                break;

            default:
                throw new System.Exception("Can't collect this reward");
        }

        coinSpentAnimation.SetActive(false);
        holder.SetActive(false);
        ResumeHandler.Instance.Resume();
    }

    private void ActivateCoinSpentAnimation()
    {
        TextMeshProUGUI coinSpentAnimationText = coinSpentAnimation.GetComponent<TextMeshProUGUI>();
        coinSpentAnimationText.text = $"-{skipCost}";
        coinSpentAnimation.SetActive(true);
        AudioManager.Instance.Play(AudioManager.COIN_SPENT_SOUND);
    }

    private void DeactivateAdButtons()
    {
        ActivateCoinSpentAnimation();
        StartCoroutine(DelayButtonDeactivation());
        skipAdButton1Animator.SetTrigger(BUTTON_DISAPPEAR);
        skipAdButton2Animator.SetTrigger(BUTTON_DISAPPEAR);
        skipAdButton3Animator.SetTrigger(BUTTON_DISAPPEAR);
        skipAdButton.gameObject.SetActive(false);
        skipAdsCostGO.SetActive(false);
        skipAdThisTime = true;
    }

    private IEnumerator DelayButtonDeactivation()
    {
        closeButton.gameObject.SetActive(false);

        yield return new WaitForSecondsRealtime(1);

        skipAdButton1.SetActive(false);
        skipAdButton2.SetActive(false);
        skipAdButton3.SetActive(false);
        //skipAdButton4.SetActive(false);//INCLUDE WHEN THE FORTUNE WHEEL IS READY
    }

    private void ActivateAdButtons()
    {
        closeButton.gameObject.SetActive(true);
        skipAdButton1.SetActive(true);
        skipAdButton2.SetActive(true);
        skipAdButton3.SetActive(true);
        //skipAdButton4.SetActive(true);//INCLUDE WHEN THE FORTUNE WHEEL IS READY
        skipAdButton.gameObject.SetActive(true);
        skipAdsCostGO.SetActive(true);
        skipAdThisTime = false;
    }
}
