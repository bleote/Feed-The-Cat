using UnityEngine;
using UnityEngine.UI;


public class PauseHandler : MonoBehaviour
{
    public static PauseHandler Instance;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;

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
        resumeButton.onClick.AddListener(Resume);
    }

    private void OnDisable()
    {
        resumeButton.onClick.RemoveListener(Resume);
    }

    public void Setup()
    {
        pausePanel.SetActive(true);

        if (PlayerPrefs.GetInt(TutorialManager.PAUSE_TUTORIAL, -1) == -1)
        {
            TutorialGameplay.Instance.CutPauseTutorial();
        }

        GamePlayManager.Pause();
    }

    public void TutorialFromSetup()
    {
        pausePanel.SetActive(true);

        TutorialGameplay.Instance.TurnOffAllTutorialMasksAtGameplay();
        TutorialGameplay.Instance.togglePauseTutorial = false;
    }

    private void Resume()
    {
        ResumeHandler.Instance.Resume();
        pausePanel.SetActive(false);
    }

    private void Restart()
    {
        SceneController.LoadGamePlay();
    }

    private void MainMenu()
    {
        SceneController.LoadMainMenu();
    }
}
