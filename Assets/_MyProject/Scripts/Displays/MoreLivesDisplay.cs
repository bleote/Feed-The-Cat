using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoreLivesDisplay : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button payForHeartButton;
    [SerializeField] private TextMeshProUGUI payForHeartButtonText;
    [SerializeField] private int extraHeartCost;
    [SerializeField] private Transform coinSpentDisplay;
    [SerializeField] private GameObject coinSpentPrefab;


    private void Start()
    {
        extraHeartCost = FirebaseRemoteConfigManager.Instance.adsConfigData.extraHeartCost;
        payForHeartButtonText.text = extraHeartCost.ToString();
    }

    public void Setup()
    {
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        watchAdButton.onClick.AddListener(PlayAdForHeart);
        payForHeartButton.onClick.AddListener(PayForHeart);
        closeButton.onClick.AddListener(Close);

        payForHeartButtonText.text = extraHeartCost.ToString();
    }

    private void OnDisable()
    {
        watchAdButton.onClick.RemoveListener(PlayAdForHeart);
        payForHeartButton.onClick.RemoveListener(PayForHeart);
        closeButton.onClick.RemoveListener(Close);
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
            Close();
        }
    }

    private void PayForHeart()
    {
        if (!HeartsManager.Instance.IsFull)
        {
            if (DataManager.Instance.PlayerData.Coins >= extraHeartCost)
            {

                Instantiate(coinSpentPrefab, coinSpentDisplay);
                DataManager.Instance.PlayerData.Coins -= extraHeartCost;
                AudioManager.Instance.Play(AudioManager.COIN_SPENT_SOUND);
                HeartsManager.Instance.AddOneHeart();
            }
            else
            {
                UIManager.Instance.OkDialog.Show("You don't have enough coins");
                Close();
            }
        }
        else
        {
            UIManager.Instance.OkDialog.Show("Your hearts are already full!");
            Close();
        }

    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
