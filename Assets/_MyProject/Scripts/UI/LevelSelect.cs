using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private Button levelButton;
    public int levelNumber;

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt(TutorialManager.DEFEATED_BOSS, -1) != -1)
        {
            levelButton.onClick.AddListener(PlayLevel);
        }
    }

    private void OnDisable()
    {
        if (PlayerPrefs.GetInt(TutorialManager.DEFEATED_BOSS, -1) != -1)
        {
            levelButton.onClick.RemoveListener(PlayLevel);
        }
    }

    private void PlayLevel()
    {
        if (DataManager.Instance.PlayerData.Hearts <= 0)
        {
            UIManager.Instance.OkDialog.Show("You're out of hearts! Wait to refill or watch an ad to keep playing!");
            return;
        }

        AudioManager.Instance.Play(AudioManager.CAT_SELECT_SOUND);
        CatSO.SelectedCat = CatSO.Get(DataManager.Instance.PlayerData.SelectedCat);
        PlayerPrefs.SetInt(DataManager.UNLOCKED_LEVELS, levelNumber);
        GamePlayManager.levelModeOn = true;
        SceneController.LoadGamePlay();
    }
}
