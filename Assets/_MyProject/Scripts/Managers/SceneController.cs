using UnityEngine;

public class SceneController
{
    private const string MAIN_MENU = "MainMenu";
    private const string GAMEPLAY = "Gameplay";
    private const string LEADERBOARD = "LeaderBoard";
    private const string DATA_COLLECTOR = "DataCollector";
    private const string INTRO = "Intro";
    private const string SHOP = "Shop";
    private const string CATS = "CatsScreen";
    public static bool IsDataCollectorScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == DATA_COLLECTOR;
    public static AsyncOperation loadingScene;
    
    public static void LoadMainMenu()
    {
        LoadScene(MAIN_MENU);
    }

    public static void LoadCats()
    {
        // Debug.Log("Loading cats");
        LoadScene(CATS);
    }

    public static void LoadGamePlay()
    {
        LoadScene(GAMEPLAY);
    }

    public static void LoadLeaderBoard()
    {
        LoadScene(LEADERBOARD);
    }

    public static void LoadShop()
    {
        LoadScene(SHOP);
    }

    public static void LoadDataCollector()
    {
        LoadScene(DATA_COLLECTOR);
    }

    public static void LoadIntro()
    {
        LoadScene(INTRO);
    }

    private static void LoadScene(string _key)
    {
        loadingScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_key);
    }
}
