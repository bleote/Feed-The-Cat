using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacks : MonoBehaviour
{
    public void ResetTutorialRecord()
    {
        PlayerPrefs.SetInt(TutorialManager.LEVEL_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.CAT_MOVEMENT_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.GOAL_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.ICECREAM_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.SUN_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.CHILLI_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.GREEN_CHILLI_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.BOMB_CHILLI_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.PAUSE_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.RELAX_MENU_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.RELAX_GAME_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.FIRST_TIME_PLAYED_RELAX_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.PLAY_BOTH_MODES_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.COIN_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.REWARDING_ICE_CREAM_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.PAW_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.ICE_BUTTON_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.MELTED_LEVELS_TUTORIAL, -1);
        PlayerPrefs.SetInt(TutorialManager.DEFEATED_BOSS_BOOLEAN, -1);
        PlayerPrefs.SetInt(TutorialManager.DEFEATED_BOSS, -1);

        PlayerPrefs.SetInt(DataManager.UNLOCKED_LEVELS, 1);
        PlayerPrefs.SetInt(DataManager.PREVIOUSLY_UNLOCKED_LEVEL, 1);
        PlayerPrefs.SetInt(DataManager.FINISHED_FIRST_GAME_LOADING, -1);

        Debug.Log("Tutorial Reset Done!");
        
        SceneController.LoadMainMenu();
    }

    public void ResetLevels()
    {
        DataManager.Instance.PlayerData.Coins = 0;
        DataManager.Instance.PlayerData.Keys = 0;
        PlayerPrefs.SetInt(DataManager.UNLOCKED_LEVELS, 1);
        PlayerPrefs.SetInt(DataManager.PREVIOUSLY_UNLOCKED_LEVEL, 1);
        PlayerPrefs.SetInt(TutorialManager.DEFEATED_BOSS_BOOLEAN, -1);
        PlayerPrefs.SetInt(TutorialManager.DEFEATED_BOSS, -1);

        SceneController.LoadMainMenu();
    }

    public void BossLevel()
    {
        PlayerPrefs.SetInt(TutorialManager.LEVEL_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.CAT_MOVEMENT_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.GOAL_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.ICECREAM_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.SUN_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.CHILLI_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.PAUSE_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.GREEN_CHILLI_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.BOMB_CHILLI_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.RELAX_MENU_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.RELAX_GAME_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.FIRST_TIME_PLAYED_RELAX_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.PLAY_BOTH_MODES_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.COIN_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.REWARDING_ICE_CREAM_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.PAW_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.ICE_BUTTON_TUTORIAL, 1);
        PlayerPrefs.SetInt(TutorialManager.MELTED_LEVELS_TUTORIAL, 1);

        PlayerPrefs.SetInt(DataManager.UNLOCKED_LEVELS, 46);
        PlayerPrefs.SetInt(DataManager.PREVIOUSLY_UNLOCKED_LEVEL, 46);

        SceneController.LoadMainMenu();
    }

    public void AddFiveHundredCoins()
    {
        if (DataManager.Instance.PlayerData.Coins < 3000)
        {
            DataManager.Instance.PlayerData.Coins += 500;
        }

        SceneController.LoadMainMenu();
    }

    public void RefillHeartsHack()
    {
        DataManager.Instance.PlayerData.Hearts = 5;

        SceneController.LoadMainMenu();
    }
}
