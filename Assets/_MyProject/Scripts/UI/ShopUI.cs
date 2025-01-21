using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    [SerializeField] private Button extraCoinsButton;
    [SerializeField] private Button extraLifeButton;
    [SerializeField] private CoinsDisplay coinsDisplay;
    [SerializeField] private Sprite[] extraCoinsSprites;
    [SerializeField] private Image extraCoinsImage;

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
    }

    private void Start()
    {
        extraCoinsImage.sprite = extraCoinsSprites[FirebaseRemoteConfigManager.Instance.adsConfigData.extraCoinsSpriteIndex];

        FirebaseAnalytics.LogEvent("shop_visit");
    }

    private void OnEnable()
    {
        extraCoinsButton.onClick.AddListener(PlayAdForCoins);
        extraLifeButton.onClick.AddListener(PlayAdForHeart);
    }

    private void OnDisable()
    {
        extraCoinsButton.onClick.RemoveListener(PlayAdForCoins);
        extraLifeButton.onClick.RemoveListener(PlayAdForHeart);
    }

    private void PlayAdForCoins()
    {
        if (!GoogleMobileAdsManager.Instance.IsRewardedAdAvailable)
        {
            UIManager.Instance.OkDialog.Show("Ad is not ready");
            return;
        }

        GoogleMobileAdsManager.Instance.ShowRewardedAd(AdRewardType.ExtraCoins);
    }

    private void PlayAdForHeart()
    {
        if (!HeartsManager.Instance.IsFull)
        {
            if (!GoogleMobileAdsManager.Instance.IsRewardedAdAvailable)
            {
                UIManager.Instance.OkDialog.Show("Ad is not ready");
                return;
            }

            GoogleMobileAdsManager.Instance.ShowRewardedAd(AdRewardType.ExtraHeart);
        }
        else
        {
            UIManager.Instance.OkDialog.Show("Your hearts are already full!");
        }
    }

    public void ExtraCoinsFromRewardedAd()
    {
        coinsDisplay.AddCoins(FirebaseRemoteConfigManager.Instance.adsConfigData.extraCoinsPerRewardedAd);
        coinsDisplay.CoinsExplosionOnTopHUD();
        coinsDisplay.ShowCoins();
    }
}
