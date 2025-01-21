using UnityEngine;
using TMPro;
using System;

public class HeartsDisplay : MonoBehaviour
{
    public static HeartsDisplay Instance;

    [SerializeField] private TextMeshProUGUI heartsDisplay;
    [SerializeField] private TextMeshProUGUI timerDisplay;
    [SerializeField] private GameObject infiniteLivesHolder;
    [SerializeField] private GameObject explosionPrefab;

    private void OnEnable()
    {
        DataManager.Instance.PlayerData.UpdatedHearts += HeartExplosionOnTopHUD;
    }

    private void OnDisable()
    {
        DataManager.Instance.PlayerData.UpdatedHearts -= HeartExplosionOnTopHUD;
    }

    private void Start()
    {
        Instance = this;

        ShowHearts();

        if (!DataManager.Instance.GameData.ReduceHearts)
        {
            //infiniteLivesHolder.SetActive(true);
            heartsDisplay.text = "8";
            heartsDisplay.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
    }

    public void ShowHearts()
    {
        heartsDisplay.text = DataManager.Instance.PlayerData.Hearts.ToString();
    }

    private void Update()
    {
        ShowTimer();
    }

    private void ShowTimer()
    {
        if (HeartsManager.Instance.IsFull)
        {
            timerDisplay.text = "Full";
            return;
        }

        int _secondsForNext = HeartsManager.Instance.SecondsLeftForAnotherHeart;
        TimeSpan _timeForNextHeart = new TimeSpan(0, 0, _secondsForNext);
        // Display time in "mm:ss" format
        int minutes = _timeForNextHeart.Minutes;
        int seconds = _timeForNextHeart.Seconds;
        timerDisplay.text = $"{minutes:D2}:{seconds:D2}";

        //old version
        //timerDisplay.text = _timeForNextHeart.TotalMinutes > 1 ? $"{Convert.ToInt32(_timeForNextHeart.TotalMinutes):D2}:{Convert.ToInt32(_timeForNextHeart.Seconds):D2}" : $"{Convert.ToInt32(_timeForNextHeart.Seconds)}s";
    }

    public void HeartExplosionOnTopHUD()
    {
        AudioManager.Instance.Play(AudioManager.EXTRA_HEART_SOUND);
        AudioManager.Instance.Play(AudioManager.EXTRA_HEART_SOUND);
        Instantiate(explosionPrefab, heartsDisplay.transform);
        ShowHearts();
    }
}
