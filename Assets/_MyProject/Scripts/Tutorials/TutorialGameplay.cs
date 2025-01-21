using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FirebaseRemoteConfigManager;

public class TutorialGameplay : MonoBehaviour
{
    public static TutorialGameplay Instance;

    [Header("Gameplay References")]
    [SerializeField] public GameObject catMovementTutorialGroup;
    [SerializeField] public RectTransform catMovementTutorialMask;
    [SerializeField] public GameObject catMovementTutorialHUDMasks;
    [SerializeField] private GameObject tutorialActionBlocker;
    [SerializeField] private GameObject pauseTutorialActionBlocker;
    [SerializeField] private GameObject pawTutorialActionBlocker;
    [SerializeField] private GameObject iceButtonTutorialActionBlocker;
    [SerializeField] private TextMeshProUGUI tutorialMessageText;
    [SerializeField] public RectTransform tutorialGameplayMask;
    [SerializeField] private Sprite[] tutorialGameplayMaskShapes;

    [Header("Tutorial Focus Elements")]
    [SerializeField] private RectTransform catFocus;
    [SerializeField] private RectTransform goalFocus;
    [SerializeField] private RectTransform pauseFocus;
    [SerializeField] private RectTransform pawFocus;
    [SerializeField] private RectTransform iceButtonFocus;
    [SerializeField] private RectTransform meltedIceCreamsFocus;

    private float scalingDir = .1f;

    public bool toggleCatMovementTutorial;
    public bool toggleRelaxGameTutorial;
    public bool toggleGoalTutorial;
    public bool toggleFoodTutorial;
    public bool togglePauseTutorial;
    public bool togglePawTutorial;
    public bool toggleIceButtonTutorial;
    public bool toggleMeltedLevelsTutorial;
    public int meltedLevelsTutorialCounter;

    //Tutorial Timers
    private bool tutorialDelayRolling;
    private int tutorialSetupDelay;
    public float tutorialDelayTimer;
    private float tutorialDisplayTimer;
    private bool tutorialDisplayTimerFinished;
    private float tutorialDisplayMinInterval;

    // Boolean for Pause Tutorial deactivation when a player pause the game before seeing the tutorial
    public bool deactivatePauseTutorialSetupDelay;

    // Boolean to avoid bomb chilli tutorial duplication
    public bool calledChilliBombTutorial;

    // Boolean to hold all tutorials
    public bool holdTutorials;

    private TutorialConfigData configData;

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
        configData = FirebaseRemoteConfigManager.Instance.tutorialConfigData;

        tutorialDisplayMinInterval = configData.tutorialDisplayMinInterval;
        tutorialDisplayTimer = 0;
        tutorialDisplayTimerFinished = false;
        tutorialDelayRolling = TutorialsWithDelayCheck();

        if (TutorialManager.Instance.meltedLevelsTutorialActive && PlayerPrefs.GetInt(TutorialManager.MELTED_LEVELS_TUTORIAL, -1) == -1)
        {
            meltedLevelsTutorialCounter = 0;
        }
    }

    private bool TutorialsWithDelayCheck()
    {
        var _tutorial = TutorialManager.Instance;
        tutorialSetupDelay = configData.tutorialSetupDelay;
        tutorialDelayTimer = 0;

        if (GamePlayManager.currentLevel == configData.goalTutorialGameLevel && _tutorial.goalTutorialActive && PlayerPrefs.GetInt(TutorialManager.GOAL_TUTORIAL, -1) == -1 ||
            GamePlayManager.currentLevel == configData.pauseTutorialGameLevel && _tutorial.pauseTutorialActive && PlayerPrefs.GetInt(TutorialManager.PAUSE_TUTORIAL, -1) == -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Update()
    {
        if (catMovementTutorialMask != null && catMovementTutorialMask.gameObject.activeInHierarchy)
        {
            TutorialMaskExpandAndContractMovement(catMovementTutorialMask);
        }

        if (tutorialGameplayMask != null && tutorialGameplayMask.gameObject.activeInHierarchy)
        {
            TutorialMaskExpandAndContractMovement(tutorialGameplayMask);
        }

        if (!holdTutorials)
        {
            if (togglePawTutorial && !tutorialDisplayTimerFinished)
            {
                TutorialMinDisplayTimer(TutorialManager.PAW_TUTORIAL);
            }

            if (toggleIceButtonTutorial && !tutorialDisplayTimerFinished)
            {
                TutorialMinDisplayTimer(TutorialManager.ICE_BUTTON_TUTORIAL);
            }

            if (toggleMeltedLevelsTutorial && !tutorialDisplayTimerFinished)
            {
                TutorialMinDisplayTimer(TutorialManager.MELTED_LEVELS_TUTORIAL);
            }

            if (toggleGoalTutorial && !tutorialDisplayTimerFinished)
            {
                TutorialMinDisplayTimer(TutorialManager.GOAL_TUTORIAL);
            }

            if (togglePauseTutorial && !tutorialDisplayTimerFinished)
            {
                TutorialMinDisplayTimer(TutorialManager.PAUSE_TUTORIAL);
            }

            HandleTutorialSetupDelay();
        }
    }

    private void HandleTutorialSetupDelay()
    {
        if (GamePlayManager.currentLevel == configData.goalTutorialGameLevel && configData.goalTutorialActive && tutorialDelayRolling)
        {
            if (!GamePlayManager.isPaused && !toggleGoalTutorial)
            {
                tutorialDelayTimer += Time.deltaTime;
            }

            if (tutorialDelayTimer >= tutorialSetupDelay)
            {
                GoalTutorialSetup();
            }
        }
        else if (GamePlayManager.currentLevel == configData.pauseTutorialGameLevel && configData.pauseTutorialActive && tutorialDelayRolling && !deactivatePauseTutorialSetupDelay)
        {
            if (!GamePlayManager.isPaused && !togglePauseTutorial)
            {
                tutorialDelayTimer += Time.deltaTime;
            }

            if (tutorialDelayTimer >= tutorialSetupDelay)
            {
                PauseTutorialSetup();
            }
        }
    }

    private void TutorialMinDisplayTimer(string tutorialString)
    {
        tutorialDisplayTimer += Time.deltaTime;

        if (tutorialDisplayTimer >= tutorialDisplayMinInterval)
        {
            tutorialDisplayTimer = 0;
            PlayerPrefs.SetInt(tutorialString, 1);

            tutorialDisplayTimerFinished = true;
        }
    }

    private void BaseTutorialSetup(Transform focusPosition, string tutorialText, ref bool toggleTutorial)
    {
        if (holdTutorials) { return; }
        
        tutorialDisplayTimer = 0;
        toggleTutorial = true;
        tutorialDisplayTimerFinished = false;

        Transform maskToUse = toggleTutorial == toggleCatMovementTutorial
        ? catMovementTutorialMask
        : tutorialGameplayMask;

        maskToUse.position = focusPosition.position;

        tutorialMessageText.text = tutorialText;

        TutorialMaskAndMessageSwitch(true);
    }

    public void CatMovementTutorialSetup()
    {
        BaseTutorialSetup(
        catFocus,
        FirebaseRemoteConfigManager.Instance.tutorialConfigData.catMovementTutorialText,
        ref toggleCatMovementTutorial
        );
    }

    public void RelaxGameTutorialSetup()
    {
        BaseTutorialSetup(
        catFocus,
        FirebaseRemoteConfigManager.Instance.tutorialConfigData.relaxGameTutorialText,
        ref toggleRelaxGameTutorial
        );
    }

    private void GoalTutorialSetup()
    {
        BaseTutorialSetup(
        goalFocus,
        FirebaseRemoteConfigManager.Instance.tutorialConfigData.goalTutorialText,
        ref toggleGoalTutorial
        );

        tutorialDelayRolling = false;
        tutorialDelayTimer = 0;
    }

    private void PauseTutorialSetup()
    {
        BaseTutorialSetup(
        pauseFocus,
        FirebaseRemoteConfigManager.Instance.tutorialConfigData.pauseTutorialText,
        ref togglePauseTutorial
        );

        tutorialDelayRolling = false;
        tutorialDelayTimer = 0;
    }

    public void CutPauseTutorial()
    {
        PlayerPrefs.SetInt(TutorialManager.PAUSE_TUTORIAL, 1);

        if (GamePlayManager.currentLevel == configData.pauseTutorialGameLevel && configData.pauseTutorialActive && tutorialDelayRolling && !deactivatePauseTutorialSetupDelay)
        {
            deactivatePauseTutorialSetupDelay = true;
            tutorialDelayRolling = false;
            tutorialDelayTimer = 0;
        }
    }

    public void PawTutorialSetup()
    {
        BaseTutorialSetup(
        pawFocus,
        FirebaseRemoteConfigManager.Instance.tutorialConfigData.pawTutorialText,
        ref togglePawTutorial
        );
    }

    public void IceButtonTutorialSetup()
    {
        BaseTutorialSetup(
        iceButtonFocus,
        FirebaseRemoteConfigManager.Instance.tutorialConfigData.iceButtonTutorialText,
        ref toggleIceButtonTutorial
        );
    }

    public void MeltedLevelsTutorialSetup()
    {
        BaseTutorialSetup(
        meltedIceCreamsFocus,
        FirebaseRemoteConfigManager.Instance.tutorialConfigData.meltedLevelsTutorialText,
        ref toggleMeltedLevelsTutorial
        );
    }

    public void SwitchMaskSpriteForMeltedTutorial(int _val)
    {
        Image gameplayMask = tutorialGameplayMask.gameObject.GetComponent<Image>();

        if (gameplayMask == null)
        {
            Debug.Log("No SpriteRenderer found on the game object!");
            return;
        }

        if (_val == 1)
        {
            gameplayMask.sprite = tutorialGameplayMaskShapes[_val];
        }
        else
        {
            gameplayMask.sprite = tutorialGameplayMaskShapes[0];
        }
    }

    public void TurnOffAllTutorialMasksAtGameplay()
    {
        // Debug.Log("All game masks off!");
        tutorialActionBlocker.SetActive(false);
        pauseTutorialActionBlocker.SetActive(false);
        pawTutorialActionBlocker.SetActive(false);
        iceButtonTutorialActionBlocker.SetActive(false);
        catMovementTutorialHUDMasks.SetActive(false);
        catMovementTutorialGroup.SetActive(false);
        tutorialGameplayMask.gameObject.SetActive(false);
        tutorialMessageText.gameObject.SetActive(false);
    }

    private void TutorialMaskExpandAndContractMovement(Transform mask)
    {
        // Define the scaling boundaries and speed
        const float maxScale = 1.02f;
        const float minScale = 0.98f;
        const float scaleSpeed = 0.1f;

        // Ensure the mask exists and is active
        if (mask == null || !mask.gameObject.activeInHierarchy)
            return;

        // Retrieve current scale
        var currentScale = mask.localScale;

        // Adjust scaling direction based on bounds
        if (currentScale.x > maxScale)
        {
            scalingDir = -scaleSpeed;
        }
        else if (currentScale.x < minScale)
        {
            scalingDir = scaleSpeed;
        }

        // Apply scaling change
        mask.localScale += scalingDir * Time.deltaTime * Vector3.one;
    }


    public IEnumerator ShowInstruction(GameObject obj, string msg, string tutorialKey)
    {
        // Exit immediately if holdTutorials is true
        if (TutorialGameplay.Instance.holdTutorials)
        {
            yield break; // Exit the coroutine
        }

        // Wait until the condition is met or holdTutorials becomes true
        yield return new WaitUntil(() =>
        {
            return obj.transform.position.y < Screen.height / 1.5f || TutorialGameplay.Instance.holdTutorials;
        });

        // Exit if holdTutorials became true while waiting
        if (TutorialGameplay.Instance.holdTutorials)
        {
            yield break; // Exit the coroutine
        }

        tutorialMessageText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150);
        CharacterMover.Instance.OnPointerUp(null);
        tutorialGameplayMask.position = obj.transform.position;
        tutorialMessageText.text = msg;
        TutorialMaskAndMessageSwitch(true);
        PlayerPrefs.SetInt(tutorialKey, 1);
    }

    public void TutorialMaskAndMessageSwitch(bool _val)
    {
        if (toggleCatMovementTutorial)
        {
            catMovementTutorialHUDMasks.SetActive(_val);
            catMovementTutorialGroup.SetActive(_val);

            if (_val == false)
            {
                toggleCatMovementTutorial = false;
            }
        }
        else
        {
            tutorialGameplayMask.gameObject.SetActive(_val);
        }

        if (togglePawTutorial || toggleIceButtonTutorial || togglePauseTutorial)
        {
            if (togglePawTutorial)
            {
                pawTutorialActionBlocker.SetActive(_val);
                if (!_val) togglePawTutorial = false;
            }

            if (toggleIceButtonTutorial)
            {
                iceButtonTutorialActionBlocker.SetActive(_val);
                if (!_val) toggleIceButtonTutorial = false;
            }

            if (togglePauseTutorial)
            {
                pauseTutorialActionBlocker.SetActive(_val);
            }
        }
        else
        {
            tutorialActionBlocker.SetActive(_val);
        }

        if (!_val)
        {
            if (toggleRelaxGameTutorial)
            {
                toggleRelaxGameTutorial = false;
            }
            else if (toggleMeltedLevelsTutorial)
            {
                toggleMeltedLevelsTutorial = false;
            }
            else if (toggleFoodTutorial)
            {
                toggleFoodTutorial = false;
            }
            else if (toggleGoalTutorial)
            {
                toggleGoalTutorial = false;
            }
        }

        tutorialMessageText.gameObject.SetActive(_val);

        // Pause or play the game based on _val
        if (_val == true)
        {
            GamePlayManager.Pause();
        }
        else
        {
            GamePlayManager.Play();
        }
    }
}
