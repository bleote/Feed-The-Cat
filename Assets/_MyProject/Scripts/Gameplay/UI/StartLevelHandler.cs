using Firebase.Analytics;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartLevelHandler : MonoBehaviour
{
    public static StartLevelHandler Instance;

    [SerializeField] private GameObject startLevelPanel;
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI levelDisplay;
    [SerializeField] private TextMeshProUGUI automaticPlayText;
    [SerializeField] private Image automaticPlayBarProgress;
    private float automaticPlayTime = 3;

    private int currentLevel;

    private float counter;
    private Coroutine automaticPlayCoroutine;

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

    private void OnEnable()
    {
        playButton.onClick.AddListener(Play);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(Play);
    }

    public void Setup()
    {
        GamePlayManager.Pause();
        currentLevel = GamePlayManager.currentLevel;
        SetLevelDisplay();
        counter = 0;
        
        if (automaticPlayCoroutine != null)
        {
            StopCoroutine(automaticPlayCoroutine);
            automaticPlayCoroutine = null;
        }

        automaticPlayCoroutine = StartCoroutine(AutomaticPlayTimerRoutine());
        
        startLevelPanel.SetActive(true);
    }

    private void Play()
    {
        counter = 0;

        startLevelPanel.SetActive(false);

        if (automaticPlayCoroutine != null)
        {
            StopCoroutine(automaticPlayCoroutine);
            automaticPlayCoroutine = null;
        }

        GamePlayManager.Play();
    }

    private void SetLevelDisplay()
    {
        levelDisplay.text = currentLevel == 46 ? "FINAL LEVEL" : $"LEVEL {currentLevel}";
    }

    private IEnumerator AutomaticPlayTimerRoutine()
    {
        while (counter < automaticPlayTime)
        {
            counter += Time.deltaTime;

            // Update countdown text
            int remainingTime = Mathf.CeilToInt(automaticPlayTime - counter);
            automaticPlayText.text = $"Automatically starting in {remainingTime}...";

            // Update progress bar
            automaticPlayBarProgress.fillAmount = counter / automaticPlayTime;

            yield return null;
        }

        Play(); // Automatically start when the timer finishes
    }
}
