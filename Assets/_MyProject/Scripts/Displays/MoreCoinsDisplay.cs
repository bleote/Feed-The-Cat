using UnityEngine;
using UnityEngine.UI;

public class MoreCoinsDisplay : MonoBehaviour
{
    public static MoreCoinsDisplay Instance;

    [SerializeField] private Button closeButton;
    [SerializeField] private Button watchAdButton;
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
    }

    public void Setup()
    {
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        watchAdButton.onClick.AddListener(PlayAdForCoins);
        closeButton.onClick.AddListener(Close);
    }

    private void OnDisable()
    {
        watchAdButton.onClick.RemoveListener(PlayAdForCoins);
        closeButton.onClick.RemoveListener(Close);
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

    public void ExtraCoinsFromRewardedAd()
    {
        coinsDisplay.AddCoins(FirebaseRemoteConfigManager.Instance.adsConfigData.extraCoinsPerRewardedAd);
        coinsDisplay.CoinsExplosionOnTopHUD();
        coinsDisplay.ShowCoins();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
