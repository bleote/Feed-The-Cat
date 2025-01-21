using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftCollectionHandler : MonoBehaviour
{
    [SerializeField] private GameObject closedGiftGroup;
    [SerializeField] private GameObject openGiftGroup;
    [SerializeField] private GameObject collectGiftCost;
    [SerializeField] private GameObject tapToContinue;
    [SerializeField] private Button panelButton;
    [SerializeField] private Button collectGiftButton;
    [SerializeField] private Button discardGiftButton;
    [SerializeField] Image flashScreen;
    [SerializeField] TextMeshProUGUI sunKeysSpent;
    [SerializeField] int giftCost;
    [SerializeField] KeysDisplay mainKeysHolder;
    [SerializeField] KeysDisplay secondKeysHolder;
    [SerializeField] CoinsDisplay coinsHolder;
    [SerializeField] int amountOfCoins;

    public void Setup()
    {
        panelButton.gameObject.SetActive(true);
        openGiftGroup.SetActive(false);
        closedGiftGroup.SetActive(true);
        collectGiftCost.SetActive(true);
        collectGiftButton.gameObject.SetActive(true);
        discardGiftButton.gameObject.SetActive(true);

        int currentLevel = PlayerPrefs.GetInt(DataManager.UNLOCKED_LEVELS, 1);

        //if (currentLevel == MapHandler.SpecialLevel1 + 1)
        //{
        //    discardGiftButton.interactable = false;
        //    discardGiftButton.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 0.2f;
        //} 
    }

    private void OnEnable()
    {
        panelButton.onClick.AddListener(CloseGiftPanel);
        collectGiftButton.onClick.AddListener(CollectGift);
        discardGiftButton.onClick.AddListener(CloseGiftPanel);
    }

    private void OnDisable()
    {
        panelButton.onClick.RemoveListener(CloseGiftPanel);
        collectGiftButton.onClick.RemoveListener(CollectGift);
        discardGiftButton.onClick.RemoveListener(CloseGiftPanel);
    }

    private void CloseGiftPanel()
    {
        tapToContinue.SetActive(false);
        panelButton.gameObject.SetActive(false);
    }

    private void CollectGift()
    {
        StartCoroutine(ScreenFlashToOpenGift());
        StartCoroutine(SunKeysSpentAnimation());
        AudioManager.Instance.Play(AudioManager.GIFT_COLLECT_SOUND);
    }

    private IEnumerator ScreenFlashToOpenGift()
    {
        flashScreen.gameObject.SetActive(true);
        float startTime = Time.deltaTime;
        float elapsedTime = 0;

        while (elapsedTime < 0.2f)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / 0.2f);
            flashScreen.color = new Color(1, 1, 1, alpha);

            yield return null;

            elapsedTime = Time.deltaTime - startTime;
        }

        flashScreen.color = new Color(1, 1, 1, 1);

        openGiftGroup.SetActive(true);
        closedGiftGroup.SetActive(false);
        collectGiftCost.SetActive(false);
        collectGiftButton.gameObject.SetActive(false);
        discardGiftButton.gameObject.SetActive(false);

        while (elapsedTime < 0.2f)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime / 0.2f);
            flashScreen.color = new Color(1, 1, 1, alpha);

            yield return null;

            elapsedTime = Time.deltaTime - startTime;
        }

        flashScreen.color = new Color(1, 1, 1, 0);

        flashScreen.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        tapToContinue.SetActive(true);

        panelButton.interactable = true;
    }

    private IEnumerator SunKeysSpentAnimation()
    {
        yield return new WaitForSeconds(0.2f);

        sunKeysSpent.gameObject.SetActive(true);
        sunKeysSpent.text = $"-{giftCost}";

        UpdateSunKeys();
        UpdateCoinsDisplay();

        Vector3 startPosition = sunKeysSpent.transform.position;
        Vector3 endPosition = new(sunKeysSpent.transform.position.x, sunKeysSpent.transform.position.y - 100, sunKeysSpent.transform.position.z);
        float elapsedTime1 = 0;
        float elapsedTime2 = 0;

        while (elapsedTime1 < 0.66f)
        {
            sunKeysSpent.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime1 / 0.66f);
            elapsedTime1 += Time.deltaTime;
            yield return null;
        }

        sunKeysSpent.transform.position = endPosition;

        while (elapsedTime2 < 0.33f)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime2 / 0.33f);
            sunKeysSpent.color = new Color(1, 0, 0, alpha);

            yield return null;

            elapsedTime2 += Time.deltaTime;
        }

        sunKeysSpent.color = new Color(1, 0, 0, 0);

        sunKeysSpent.gameObject.SetActive(false);
    }

    private void UpdateSunKeys()
    {
        DataManager.Instance.PlayerData.Keys -= giftCost;

        if (PlayerPrefs.GetInt(DataManager.PLAYER_FIRST_RETURN, -1) == -1)
        {
            PlayerPrefs.SetInt(DataManager.KEYS_KEY, DataManager.Instance.PlayerData.Keys);
            PlayerPrefs.Save();
        }

        if (mainKeysHolder != null)
        {
            mainKeysHolder.ShowKeys();
        }

        secondKeysHolder.ShowKeys();
    }

    private void UpdateCoinsDisplay()
    {
        if (coinsHolder != null)
        {
            coinsHolder.AddCoins(amountOfCoins);
            coinsHolder.ShowCoins();
        }

        if (PlayerPrefs.GetInt(DataManager.PLAYER_FIRST_RETURN, -1) == -1)
        {
            PlayerPrefs.SetInt(DataManager.COINS_KEY, DataManager.Instance.PlayerData.Coins);
            PlayerPrefs.Save();
        }
    }
}
