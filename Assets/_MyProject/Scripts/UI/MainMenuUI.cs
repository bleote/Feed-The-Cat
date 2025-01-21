using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Transform levelHolder;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button coinsButton;
    [SerializeField] private Button livesButton;
    [SerializeField] private Button keysButton;
    [SerializeField] private Button levelModeButton;
    [SerializeField] private Button relaxModeButton;
    
    [SerializeField] private MoreCoinsDisplay moreCoinsDisplay;
    [SerializeField] private MoreLivesDisplay moreLivesDisplay;
    [SerializeField] private KeysPanel keysDisplay;
    [SerializeField] private SettingsUI settingsUI;

    private bool loadGameplay;

    private void Start()
    {
        AudioManager.Instance.PlayBackgroundMusic(AudioManager.MAIN_THEME_MUSIC);
        var _levelHolderTransform = levelHolder.transform;
        _levelHolderTransform.localPosition = new Vector3(_levelHolderTransform.localPosition.x,10000, levelHolder.localPosition.z);
    }

    private void OnEnable()
    {
        //The lines below are comented off only for the MVP version
        //settingsButton.onClick.AddListener(ShowSettings);
        coinsButton.onClick.AddListener(ShowMoreCoins);
        livesButton.onClick.AddListener(ShowMoreLives);
        //keysButton.onClick.AddListener(ShowKeys);
        levelModeButton.onClick.AddListener(PlayLevelMode);
        relaxModeButton.onClick.AddListener(PlayRelaxMode);
    }

    private void OnDisable()
    {
        //The lines below are comented off only for the MVP version
        //settingsButton.onClick.RemoveListener(ShowSettings);
        coinsButton.onClick.RemoveListener(ShowMoreCoins);
        livesButton.onClick.RemoveListener(ShowMoreLives);
        //keysButton.onClick.RemoveListener(ShowKeys);
        levelModeButton.onClick.RemoveListener(PlayLevelMode);
        relaxModeButton.onClick.RemoveListener(PlayRelaxMode);
    }

    private void ShowSettings()
    {
        settingsUI.Setup();
    }

    private void ShowMoreCoins()
    {
        moreCoinsDisplay.Setup();
        moreLivesDisplay.Close();
    }

    private void ShowMoreLives()
    {
        moreLivesDisplay.Setup();
        moreCoinsDisplay.Close();
    }

    private void ShowKeys()
    {
        keysDisplay.Setup();
    }
    
    private void PlayRelaxMode()
    {
        AvailableHeartsCheck();

        if (loadGameplay)
        {
            AudioManager.Instance.Play(AudioManager.CAT_SELECT_SOUND);
            CatSO.SelectedCat = CatSO.Get(DataManager.Instance.PlayerData.SelectedCat);
            GamePlayManager.levelModeOn = false;
            SceneController.LoadGamePlay();
        }
    }

    private void PlayLevelMode()
    {
        AvailableHeartsCheck();

        if (loadGameplay)
        {
            if (TutorialManager.Instance.levelTutorialActive)
            {
                PlayerPrefs.SetInt(TutorialManager.LEVEL_TUTORIAL, 1);

                FirebaseAnalytics.LogEvent("tutorial_complete_level_start");
            }

            AudioManager.Instance.Play(AudioManager.CAT_SELECT_SOUND);
            CatSO.SelectedCat = CatSO.Get(DataManager.Instance.PlayerData.SelectedCat);
            GamePlayManager.levelModeOn = true;
            SceneController.LoadGamePlay();
        }
    }

    private void AvailableHeartsCheck()
    {
        if (DataManager.Instance.PlayerData.Hearts <= 0)
        {
            loadGameplay = false;
            UIManager.Instance.OkDialog.Show("You're out of hearts! Wait to refill or watch an ad to keep playing!");
            return;
        }
        else
        {
            loadGameplay = true;
        }
    }
}
