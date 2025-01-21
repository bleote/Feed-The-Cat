using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewHighScoreDisplay : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private Button closeButton;

    public void Setup()
    {
        scoreDisplay.text = DataManager.Instance.PlayerData.HighScore.ToString();
        AudioManager.Instance.Play(AudioManager.HIGH_SCORE_SOUND);
        panel.gameObject.SetActive(true);
        closeButton.onClick.AddListener(Close);
    }

    void Close()
    {
        LoseHandler.Instance.waitingHighScoreDisplay = false;
        LoseHandler.Instance.InitiateInterstitialRoutine();
        panel.gameObject.SetActive(false);
        closeButton.onClick.RemoveListener(Close);
    }
}
