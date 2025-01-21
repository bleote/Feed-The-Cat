using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] frozenFriendPrefabs;
    [SerializeField] private GameObject catPin;
    [SerializeField] private GameObject scrollBlocker;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private TextMeshProUGUI levelDisplay;

    public GameObject levelPrefab;
    public List<GameObject> levelGameObjects;
    public int unlockedLevels;
    private int unlockedLevelIndex;

    public const int FrozenFriend1 = 11 - 1; //Frozen Friend Level Indexes
    private const int FrozenFriend2 = 23 - 1;

    public float catMovementDuration;
    private int latestUnlockedLevel;

    private bool pinMovement = false;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        SetCurrentLevel();

        HandleMapDisplaySetup();

        ComingFromLoadingSceneCheck();

        SunTutorialCheck();

        RelaxMenuTutorialCheck();

        DefeatedBossCheck();
    }

    private void Update()
    {
        CheckForPinMovement();

        if (pinMovement)
        {
            pinMovement = false;
            PlayerPrefs.SetInt(DataManager.PIN_MOVEMENT, 0);
            IncreaseLevelsUnlocked();
            CheckForUnlockedLevels();
        }
    }

    private void SetCurrentLevel()
    {
        unlockedLevels = PlayerPrefs.GetInt(DataManager.UNLOCKED_LEVELS, 1);

        latestUnlockedLevel = PlayerPrefs.GetInt(DataManager.PREVIOUSLY_UNLOCKED_LEVEL, 1);

        unlockedLevelIndex = unlockedLevels - 1;

        FirebaseAnalytics.LogEvent("main_menu_total");

        SetLevelButtonDisplayText();
    }

    private void SetLevelButtonDisplayText()
    {
        if (unlockedLevels == 46)
        {
            levelDisplay.text = $"Final Level";
        }
        else
        {
            levelDisplay.text = $"Level {unlockedLevels}";
        }
    }

    private void HandleMapDisplaySetup()
    {
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Image levelPositiononMap = transform.GetChild(i).GetComponent<Image>();
            levelPositiononMap.enabled = false;//levelPositionMap is only used as a visual reference in the editor.

            GameObject levelGO = Instantiate(levelPrefab, transform.GetChild(i).transform);

            Image levelImage = levelGO.GetComponent<Image>();

            SetLevelNumber(i, levelGO);

            SetLevelColor(i, levelImage);

            // When the boss is defeated, we need to guarantee gifts and friends won't reappear when the player loads an initial level
            if (PlayerPrefs.GetInt(TutorialManager.DEFEATED_BOSS, -1) == -1)
            {
                SetFrozenFriendLevel(i, levelGO);
            }

            SetCatPinInitialPosition(i, levelGO, levelImage);

            levelGO.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }
    }

    private void SetLevelNumber(int i, GameObject levelGO)
    {
        LevelSelect levelSelect = levelGO.GetComponent<LevelSelect>();
        levelSelect.levelNumber = i + 1;
    }

    private void SetLevelColor(int i, Image levelImage)
    {
        if (PlayerPrefs.GetInt(TutorialManager.DEFEATED_BOSS, -1) == -1)
        {
            if (i < unlockedLevelIndex)
            {
                levelImage.color = new Color(0, 0.9f, 0, 0.8f);//green
            }
            else if (i == unlockedLevelIndex)
            {
                levelImage.color = new Color(1, 0.9f, 0, 0.8f);//yellow
            }
            else
            {
                levelImage.color = new Color(1, 0, 0, 0.8f);//red
            }
        }
        else
        {
            levelImage.color = new Color(0, 0.9f, 0, 0.8f);//green
        }
    }

    private void SetCatPinInitialPosition(int i, GameObject levelGO, Image levelImage)
    {
        if (i == unlockedLevelIndex)
        {
            SetPlayerPresence(levelGO);
        }

        if (i < unlockedLevelIndex)
        {
            TurnOffPlayerPresence(levelGO);
        }
    }

    private void SetPlayerPresence(GameObject levelGO)
    {
        levelGO.transform.GetChild(4).gameObject.SetActive(true);
    }

    private void TurnOffPlayerPresence(GameObject levelGO)
    {
        levelGO.transform.GetChild(4).gameObject.SetActive(false);
    }

    private void SetFrozenFriendLevel(int i, GameObject levelGO)
    {
        switch (i)
        {
            case FrozenFriend1:
                if (unlockedLevelIndex <= i)
                {
                    Instantiate(frozenFriendPrefabs[0], levelGO.transform.GetChild(3).transform);
                }
                break;
            case FrozenFriend2:
                if (unlockedLevelIndex < i)
                {
                    Instantiate(frozenFriendPrefabs[1], levelGO.transform.GetChild(3).transform);
                }
                break;
            default:
                break;
        }
    }

    private void CheckForPinMovement()
    {
        int pinMove = PlayerPrefs.GetInt(DataManager.PIN_MOVEMENT, 0);

        if (pinMove != 0)
        {
            pinMovement = true;
        }
    }

    public void IncreaseLevelsUnlocked()
    {
        if (unlockedLevels < 46)
        {
            unlockedLevels++;
        }

        PlayerPrefs.SetInt(DataManager.UNLOCKED_LEVELS, unlockedLevels);
    }

    private void CheckForUnlockedLevels()
    {
        if (PlayerPrefs.GetInt(TutorialManager.DEFEATED_BOSS, -1) != -1)
        {
            return;
        }
        else
        {
            if (latestUnlockedLevel != unlockedLevels)
            {
                latestUnlockedLevel = unlockedLevels;
                PlayerPrefs.SetInt(DataManager.PREVIOUSLY_UNLOCKED_LEVEL, latestUnlockedLevel);
                unlockedLevelIndex = unlockedLevels - 1;
                MoveCatPin(levelGameObjects[unlockedLevelIndex - 1].transform.GetChild(0).gameObject, levelGameObjects[unlockedLevelIndex].transform.GetChild(0).gameObject, catMovementDuration);
            }
        }
    }

    private void MoveCatPin(GameObject fromLevelGO, GameObject toLevelGO, float duration)
    {
        StartCoroutine(MoveCatPinCoroutine(fromLevelGO, toLevelGO, duration));
    }

    private IEnumerator MoveCatPinCoroutine(GameObject fromLevelGO, GameObject toLevelGO, float duration)
    {
        Vector3 startPosition = fromLevelGO.transform.GetChild(0).transform.position;
        Vector3 endPosition = toLevelGO.transform.GetChild(0).transform.position;
        float elapsedTime = 0;

        scrollBlocker.SetActive(true);

        yield return new WaitForSeconds(1);

        AudioManager.Instance.Play(AudioManager.LEVEL_UNLOCK_SOUND);

        if (unlockedLevels == 46)
        {
            levelDisplay.text = $"Final Level";
        }
        else
        {
            levelDisplay.text = $"Level {unlockedLevels}";
        }

        ExplosionsAndLevelColorUpdate(fromLevelGO, toLevelGO);

        yield return new WaitForSeconds(1);

        TurnOffPlayerPresence(fromLevelGO);

        catPin.SetActive(true);
        catPin.transform.position = startPosition;

        while (elapsedTime < duration)
        {
            catPin.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        catPin.transform.position = endPosition;

        catPin.SetActive(false);

        SetPlayerPresence(toLevelGO);

        scrollBlocker.SetActive(false);

        SunTutorialCheck();

        RelaxMenuTutorialCheck();
    }

    private void ExplosionsAndLevelColorUpdate(GameObject fromLevelGO, GameObject toLevelGO)
    {
        Transform fromLevelGOChild = fromLevelGO.transform.GetChild(3);
        Transform toLevelGOChild = toLevelGO.transform.GetChild(3);
        Instantiate(explosionPrefab, fromLevelGOChild.transform);
        Instantiate(explosionPrefab, toLevelGOChild.transform);

        Image fromLevelGOImage = fromLevelGO.GetComponent<Image>();
        fromLevelGOImage.color = new Color(0, 0.9f, 0, 0.8f);
        Image toLevelGOImage = toLevelGO.GetComponent<Image>();
        toLevelGOImage.color = new Color(1, 0.9f, 0, 0.8f);

        // Destroy frozen friend on fromLevelGO, if it exists
        Transform frozenFriend = fromLevelGOChild.Cast<Transform>()
        .FirstOrDefault(child => child.CompareTag("Splash") || child.CompareTag("Pebble"));

        if (frozenFriend != null)
        {
            Destroy(frozenFriend.gameObject);
        }
    }

    private void ComingFromLoadingSceneCheck()
    {
        if (PlayerPrefs.GetInt(DataManager.OPENED_MAIN_MENU_AFTER_LOADING, -1) == 1)
        {
            PlayerPrefs.SetInt(DataManager.OPENED_MAIN_MENU_AFTER_LOADING, -1);

            FirebaseAnalytics.LogEvent("main_menu_after_game_loading");
        }

        FirstTimeOpenedMainMenuCheck();

        FirstTimePlayerReturnCheck();
    }

    private void FirstTimeOpenedMainMenuCheck()
    {
        if (PlayerPrefs.GetInt(DataManager.OPENED_MAIN_MENU, -1) == -1)
        {
            PlayerPrefs.SetInt(DataManager.OPENED_MAIN_MENU, 1);

            FirebaseAnalytics.LogEvent("main_menu_first_time");
        }
    }

    private void FirstTimePlayerReturnCheck()
    {
        if (PlayerPrefs.GetInt(DataManager.OPENED_MAIN_MENU, 1) == 1 && DataManager.Instance.actionPlayerFirstReturn)
        {
            DataManager.Instance.actionPlayerFirstReturn = false;

            PlayerPrefs.SetInt(DataManager.PLAYER_FIRST_RETURN, 1);

            DataManager.Instance.PlayerData.Keys += PlayerPrefs.GetInt(DataManager.KEYS_KEY, 0);
            DataManager.Instance.PlayerData.Coins += PlayerPrefs.GetInt(DataManager.COINS_KEY, 0);
            DataManager.Instance.PlayerData.HighScore += PlayerPrefs.GetInt(DataManager.HIGH_SCORE_KEY, 0);

            int heartsDifference = DataManager.Instance.PlayerData.Hearts - PlayerPrefs.GetInt(DataManager.HEARTS_KEY, 0);

            if (heartsDifference >= 2)
            {
                heartsDifference = 2;
            }

            DataManager.Instance.PlayerData.ChangeHearts(-heartsDifference);

            SceneController.LoadMainMenu();
        }
    }

    private void SunTutorialCheck()
    {
        if (unlockedLevels == 2 && TutorialManager.Instance.sunTutorialActive && PlayerPrefs.GetInt(TutorialManager.SUN_TUTORIAL, -1) == -1)
        {
            TutorialMainMenu.Instance.SunTutorialSetup();
        }
    }

    private void RelaxMenuTutorialCheck()
    {
        if (unlockedLevels == 6)
        {
            //TutorialMainMenu.Instance.EnableRelaxModeButton();

            if (TutorialManager.Instance.relaxMenuTutorialActive && PlayerPrefs.GetInt(TutorialManager.RELAX_MENU_TUTORIAL, -1) == -1)
            {
                TutorialMainMenu.Instance.RelaxMenuTutorialSetup();
            }
        }
    }

    private void DefeatedBossCheck()
    {
        if (unlockedLevels >= 46 && TutorialManager.Instance.defeatedBossTutorialActive && PlayerPrefs.GetInt(TutorialManager.DEFEATED_BOSS_BOOLEAN, -1) != -1 && PlayerPrefs.GetInt(TutorialManager.DEFEATED_BOSS, -1) == -1)
        {
            TutorialMainMenu.Instance.DefeatedBossTutorialSetup();
        }
    }

    public void LevelUpShortCut()
    {
        if (unlockedLevels < 46)
        {
            unlockedLevels++;
        }

        DataManager.Instance.PlayerData.Keys += 1;
        PlayerPrefs.SetInt(DataManager.UNLOCKED_LEVELS, unlockedLevels);

        SceneController.LoadMainMenu();
    }
}
