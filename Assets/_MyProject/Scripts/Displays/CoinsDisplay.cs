using UnityEngine;
using TMPro;
using System.Collections;

public class CoinsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsDisplay;
    [SerializeField] private GameObject explosionPrefab;

    private void OnEnable()
    {
        DataManager.Instance.PlayerData.UpdatedCoins += ShowCoins;
    }

    private void OnDisable()
    {
        DataManager.Instance.PlayerData.UpdatedCoins -= ShowCoins;
    }

    private void Start()
    {
        ShowCoins();
    }

    public void ShowCoins()
    {
       //coinsDisplay.text = DataManager.Instance.PlayerData.Coins.ToString();
       coinsDisplay.text = GamePlayManager.ConvertIntegerToString(DataManager.Instance.PlayerData.Coins);
    }

    public void AddCoins(int amountOfCoins)
    {
        DataManager.Instance.PlayerData.Coins += amountOfCoins;
    }

    public void SubtractCoins(int amountOfCoins)
    {
        DataManager.Instance.PlayerData.Coins -= amountOfCoins;
    }

    public void CoinsExplosionOnTopHUD()
    {
        Instantiate(explosionPrefab, coinsDisplay.transform);
        StartCoroutine(ExtraCoinSoundRoutine());
    }

    private IEnumerator ExtraCoinSoundRoutine()
    {
        AudioManager.Instance.Play(AudioManager.GIFT_COLLECT_SOUND);
        AudioManager.Instance.Play(AudioManager.COIN_COLLECT_SOUND);

        yield return new WaitForSeconds(0.25f);

        AudioManager.Instance.Play(AudioManager.COIN_COLLECT_SOUND);
    }
}
