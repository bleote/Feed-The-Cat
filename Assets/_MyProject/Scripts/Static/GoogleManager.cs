using Firebase.Auth;
using Firebase.Extensions;
using Google;
using System;
using System.Threading.Tasks;

public static class GoogleManager
{
    private static readonly string GOOGLE_WEB_API = "441623435735-3cj7aeb68osm35ajugrv61omkqiqot87.apps.googleusercontent.com";
    private static GoogleSignInConfiguration configuration;
    private static bool isInit;

    public static void Init(Action _callBack)
    {
        if (isInit)
        {
            _callBack?.Invoke();
            return;
        }
        
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GOOGLE_WEB_API,
            RequestIdToken = true
        };
        isInit = true;
        _callBack?.Invoke();
    }

    public static void Login(Action<Credential> _loginSuccess, Action<string> _loginFailed)
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn()
            .ContinueWithOnMainThread((_task) => OnGoogleAuthenticationFinished(_task, _loginSuccess, _loginFailed));
    }

    private static void OnGoogleAuthenticationFinished(Task<GoogleSignInUser> _task, Action<Credential> _loginSuccess, Action<string> _loginFailed)
    {
        if (_task.IsFaulted)
        {
            _loginFailed?.Invoke("There was a fault, if this keeps happening please contact support:");
        }
        else if (_task.IsCanceled)
        {
            _loginFailed?.Invoke("Looks like you have canceled signin with google");
        }
        else
        {
            string _token = _task.Result.IdToken;
            var _credential = GoogleAuthProvider.GetCredential(_token, null);
            _loginSuccess?.Invoke(_credential);
        }
    }
}
