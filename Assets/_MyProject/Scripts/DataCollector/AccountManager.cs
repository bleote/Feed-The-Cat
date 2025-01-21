using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class AccountManager : MonoBehaviour
{
    public const string NEW_ACC_KEY = "NewAcc";
    public static bool isNewAccount;

    [SerializeField] private GameObject buttonsHolder;
    //[SerializeField] private Button loginWithFacebookButton;
    [SerializeField] private Button loginWithAppleButton;
    [SerializeField] private Button loginWithGoogleButton;
    [SerializeField] private Button loginAsGuestButton;
    [SerializeField] private PlayerNamePanel playerNamePanel;

    private void OnEnable()
    {
        PlayerNamePanel.SavedName += LoadGameData;
        //Initialization.Finished += CheckAccount;
        //loginWithFacebookButton.onClick.AddListener(LoginWithFacebook);
        loginWithAppleButton.onClick.AddListener(LoginWithApple);
        loginWithGoogleButton.onClick.AddListener(LoginWithGoogle);
        loginAsGuestButton.onClick.AddListener(LoginAsGuest);
    }

    private void OnDisable()
    {
        PlayerNamePanel.SavedName -= LoadGameData;
        //Initialization.Finished -= CheckAccount;
        //loginWithFacebookButton.onClick.RemoveListener(LoginWithFacebook);
        loginWithAppleButton.onClick.RemoveListener(LoginWithApple);
        loginWithGoogleButton.onClick.RemoveListener(LoginWithGoogle);
        loginAsGuestButton.onClick.RemoveListener(LoginAsGuest);
    }

    //private void CheckAccount()
    //{
    //    if (FirebaseManager.Instance.IsLoggedIn)
    //    {
    //        Debug.Log("User is logged in, loading player data...");
    //        FirebaseManager.Instance.LoadFirebasePlayerData(FinishLoadingPlayerData);
    //        return;
    //    }

    //    Debug.Log("User is not logged in, showing buttons...");
    //    buttonsHolder.SetActive(true);
    //}

    //private void LoginWithFacebook()
    //{
    //    FacebookManager.Init(Login);

    //    void Login()
    //    {
    //        ManageButtons(false);
    //        FacebookManager.Login(LoginSuccess, LoginFailed);
    //    }
    //}

    private void LoginWithGoogle()
    {
        GoogleManager.Init(Login);

        void Login()
        {
            ManageButtons(false);
            GoogleManager.Login(LoginSuccess, LoginFailed);
        }
    }

    private void LoginWithApple()
    {
        AppleManager.Init(Login);

        void Login()
        {
            ManageButtons(false);
            AppleManager.Login(LoginSuccess, LoginFailed);
        }
    }

    public void LoginAsGuest()
    {
        ManageButtons(false);
        FirebaseManager.Instance.LoginAnonymous(FirebaseLoginHandler);
    }

    private void LoginSuccess(Credential _credentials)
    {
        FirebaseManager.Instance.Login(_credentials, FirebaseLoginHandler);
    }

    private void FirebaseLoginHandler(bool _status, string _args)
    {
        if (_status)
        {
            // Debug.Log("Firebase login succeeded, args: " + _args);
            if (_args == NEW_ACC_KEY)
            {
                isNewAccount = true;
                ShowEnterName();
            }
            else
            {
                FirebaseManager.Instance.LoadPlayerData(FinishLoadingPlayerData);
            }
        }
        else
        {
            Debug.LogError("Firebase login failed, args: " + _args);
            ManageButtons(true);
            UIManager.Instance.OkDialog.Show(_args);
        }
    }

    private void FinishLoadingPlayerData(string _data)
    {
        // Debug.Log("Finished loading player data from Firebase: " + _data);
        DataManager.Instance.SetPlayerData(_data);

        UIManager.Instance.finishedPlayerDataLoading = true;

        if (PlayerPrefs.GetInt(DataManager.OPENED_MAIN_MENU, -1) == 1 && PlayerPrefs.GetInt(DataManager.PLAYER_FIRST_RETURN, -1) == -1)
        {
            DataManager.Instance.actionPlayerFirstReturn = true;
        }

        LoadGameData();
    }

    public void LoadGameData()
    {
        FirebaseManager.Instance.LoadGameData(FinishedLoadingGameData);
    }

    private void FinishedLoadingGameData(string _data)
    {
        if (string.IsNullOrEmpty(_data))
        {
            // Debug.Log("No game data found. Creating new game data.");
            DataManager.Instance.CreateNewGameData();
        }
        else
        {
            // Debug.Log("Finished loading game data. Deserializing it now...");
            DataManager.Instance.SetGameData(_data);
        }

        HeartsManager.Instance.Init();

        UIManager.Instance.finishedGameDataLoading = true;
    }

    private void LoginFailed(string _reason)
    {
        Debug.LogError("Login failed: " + _reason);
        UIManager.Instance.OkDialog.Show(_reason);
    }

    private void ShowEnterName()
    {
        playerNamePanel.Setup();
    }

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            loginWithGoogleButton.gameObject.SetActive(true);
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            loginWithAppleButton.gameObject.SetActive(true);
        }
    }

    void ManageButtons(bool _status)
    {
        loginAsGuestButton.interactable = _status;
        loginWithAppleButton.interactable = _status;
        //loginWithFacebookButton.interactable = _status;
        loginWithGoogleButton.interactable = _status;
    }

    //public IEnumerator Load()
    //{
    //    yield return new WaitUntil (() => SceneController.loadingScene.progress == 1);
    //    Routine.LerpConstant(loadingBar.fillAmount, 1, 0.02f, (fill) => loadingBar.fillAmount = fill, () => loadingBar.fillAmount = 1);
    //}
}