using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMainMenu : MonoBehaviour
{
    public static TutorialMainMenu Instance;

    [Header("Main Menu References")]
    [SerializeField] private Button levelModeButton;
    [SerializeField] private Button relaxModeButton;
    [SerializeField] private GameObject mainMenuTopHUDBlocker;
    [SerializeField] private GameObject mainMenuBottomHUDBlocker;
    [SerializeField] private RectTransform tutorialMainMenuMask;
    [SerializeField] private GameObject tutorialMessageStructure;
    [SerializeField] private GameObject tapToContinue;
    [SerializeField] private TextMeshProUGUI tutorialMessageText;

    [Header("Tutorial Focus Elements")]
    [SerializeField] private RectTransform sunFocus;

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
        LevelTutorialCheck();
    }

    private void LevelTutorialCheck()
    {
        if (TutorialManager.Instance.levelTutorialActive && PlayerPrefs.GetInt(TutorialManager.LEVEL_TUTORIAL, -1) == -1)
        {
            tutorialMainMenuMask.position = levelModeButton.transform.position;
            tutorialMainMenuMask.gameObject.SetActive(true);
            mainMenuTopHUDBlocker.SetActive(true);
            mainMenuBottomHUDBlocker.SetActive(true);
            relaxModeButton.interactable = false;
        }
    }

    public void EnableRelaxModeButton()
    {
        if (relaxModeButton != null & relaxModeButton.interactable == false)
        {
            relaxModeButton.interactable = true;
            TextMeshProUGUI relaxModeButtonText = relaxModeButton.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            relaxModeButtonText.color = new(1, 1, 1, 1);
        }
    }

    public void RelaxMenuTutorialSetup()
    {
        PlayerPrefs.SetInt(TutorialManager.RELAX_MENU_TUTORIAL, 1);

        tutorialMainMenuMask.position = relaxModeButton.transform.position;
        tutorialMainMenuMask.gameObject.SetActive(true);
        mainMenuTopHUDBlocker.SetActive(true);
        mainMenuBottomHUDBlocker.SetActive(true);
        levelModeButton.interactable = false;
    }

    public void PlayBothModesTutorialSetup()
    {
        PlayerPrefs.SetInt(TutorialManager.PLAY_BOTH_MODES_TUTORIAL, 1);

        tutorialMessageText.text = "You can now play both game modes!\n\nChoose one and have fun!\n\n(Tap to continue)";
        tutorialMessageStructure.SetActive(true);
        tapToContinue.SetActive(true);
    }

    public void SunTutorialSetup()
    {
        PlayerPrefs.SetInt(TutorialManager.SUN_TUTORIAL, 1);

        tutorialMainMenuMask.position = sunFocus.position;
        tutorialMainMenuMask.gameObject.SetActive(true);
        mainMenuTopHUDBlocker.SetActive(true);
        mainMenuBottomHUDBlocker.SetActive(true);

        tutorialMessageText.text = "Earn a SUN for each\r\nlevel completed!\r\n\r\nSuns will help you\r\nalong your journey.\r\n\r\n(Tap to continue)";
        tutorialMessageStructure.SetActive(true);
        tapToContinue.SetActive(true);
    }

    public void DefeatedBossTutorialSetup()
    {
        PlayerPrefs.SetInt(TutorialManager.DEFEATED_BOSS, 1);

        tutorialMessageText.text = "Now you can play any levels you want! Just scroll the map and select one of them.\n\nYou can also enjoy Relax Mode anytime and try to beat your higscore!\n\n(Tap to continue)";
        tutorialMessageStructure.SetActive(true);
        tapToContinue.SetActive(true);
    }

    public void TurnOffAllTutorialMasksAtMenu()
    {
        mainMenuTopHUDBlocker.SetActive(false);
        mainMenuBottomHUDBlocker.SetActive(false);
        tutorialMainMenuMask.gameObject.SetActive(false);
        tutorialMessageStructure.SetActive(false);
        tapToContinue.SetActive(false);

        if (PlayerPrefs.GetInt(TutorialManager.DEFEATED_BOSS, -1) != -1)
        {
            SceneController.LoadMainMenu();
        }
    }
}

