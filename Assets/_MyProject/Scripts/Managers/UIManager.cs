using Firebase.Analytics;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image loadingBar;
    [SerializeField] private AccountManager accountHandler;
    
    public static UIManager Instance;
    
    [field: SerializeField] public OkDialog OkDialog { get; private set; }
    [field: SerializeField] public WaitingPanel WaitPanel { get; private set; }

    public int loadingPhase = 0;

    //phase 2 variables
    private bool initiatedPlayerDataLoading = false;
    public bool finishedPlayerDataLoading = false;

    //phase 3 variables
    private bool initiatedGameDataLoading = false;
    public bool finishedGameDataLoading = false;

    //finish game loading
    private bool finishedGameLoading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        loadingPanel.SetActive(true);

        LoadingInitialPhaseProgress();
    }

    private void Update()
    {
        if (!finishedGameLoading)
        {
            if (loadingPhase == 2 && !initiatedPlayerDataLoading)
            {
                initiatedPlayerDataLoading = true;
                LoadingPlayerDataProgress();
            }

            if (loadingPhase == 3 && finishedPlayerDataLoading && !initiatedGameDataLoading)
            {
                initiatedGameDataLoading = true;
                LoadingGameDataProgress();
            }

            if (loadingPhase == 4 && finishedGameDataLoading && !finishedGameLoading)
            {
                finishedGameLoading = true;

                if (PlayerPrefs.GetInt(DataManager.FINISHED_FIRST_GAME_LOADING, -1) == -1)
                {
                    GoogleMobileAdsManager.Instance.AdsFirstLoad();
                }

                FinishLoadingScreen();
            }
        }
    }

    private void LoadingInitialPhaseProgress()
    {
        loadingPhase = 1;

        // Start loading bar progression
        Routine.LerpConstant(loadingBar.fillAmount, 0.4f, 0.008f, (fill) => loadingBar.fillAmount = fill, () => { loadingBar.fillAmount = 0.4f; accountHandler.LoginAsGuest(); loadingPhase = 2; });
    }

    private void LoadingPlayerDataProgress()
    {
        loadingBar.fillAmount = 0.5f;

        Routine.LerpConstant(loadingBar.fillAmount, 0.8f, 0.02f, (fill) => loadingBar.fillAmount = fill, () => { loadingBar.fillAmount = 0.8f; loadingPhase = 3; });
    }

    public void LoadingGameDataProgress()
    {
        loadingBar.fillAmount = 0.9f;

        Routine.LerpConstant(loadingBar.fillAmount, 1, 0.002f, (fill) => loadingBar.fillAmount = fill, () => { loadingBar.fillAmount = 1; loadingPhase = 4; });
    }

    private void FinishLoadingScreen()
    {
        TutorialManager.Instance.enabled = true;

        StartCoroutine(RunFinishLoadingScreen());
    }

    private IEnumerator RunFinishLoadingScreen()
    {
        loadingBar.fillAmount = 1f;

        yield return new WaitForSeconds(0.5f);

        loadingPanel.SetActive(false);

        PlayerPrefs.SetInt(DataManager.OPENED_MAIN_MENU_AFTER_LOADING, 1);
        
        FirebaseAnalytics.LogEvent("game_loading_complete");
        
        if (PlayerPrefs.GetInt(DataManager.FINISHED_FIRST_GAME_LOADING, -1) == -1)
        {
            FirstGameplaySetup();
        }
        else
        {
            SceneController.LoadMainMenu();
        }
    }

    private void FirstGameplaySetup()
    {
        PlayerPrefs.SetInt(DataManager.FINISHED_FIRST_GAME_LOADING, 1);

        FirebaseAnalytics.LogEvent("game_loading_complete_first_time");

        AudioManager.Instance.Play(AudioManager.CAT_SELECT_SOUND);
        CatSO.SelectedCat = CatSO.Get(DataManager.Instance.PlayerData.SelectedCat);
        GamePlayManager.levelModeOn = true;
        SceneController.LoadGamePlay();
    }
}
