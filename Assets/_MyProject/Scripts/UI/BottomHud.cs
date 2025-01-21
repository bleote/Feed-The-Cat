using UnityEngine;
using UnityEngine.UI;

public class BottomHud : MonoBehaviour
{
    [SerializeField] private Button shopButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button catsButton;

    private void OnEnable()
    {
        mainMenuButton.onClick.AddListener(ShowMainMenu);
        catsButton.onClick.AddListener(ShowCats);
        shopButton.onClick.AddListener(ShowShop);
        //leaderboardButton.onClick.AddListener(ShowLeaderboard);
    }

    private void OnDisable()
    {
        mainMenuButton.onClick.RemoveListener(ShowMainMenu);
        catsButton.onClick.RemoveListener(ShowCats);
        shopButton.onClick.RemoveListener(ShowShop);
        //leaderboardButton.onClick.RemoveListener(ShowLeaderboard);
    }

    private void ShowMainMenu()
    {
        SceneController.LoadMainMenu();
    }

    private void ShowCats()
    {
        // Debug.Log("Should load cats");
        SceneController.LoadCats();
    }
    
    private void ShowShop()
    {
        SceneController.LoadShop();
    }
    
    private void ShowLeaderboard()
    {
        SceneController.LoadLeaderBoard();
    }
    
    
}
