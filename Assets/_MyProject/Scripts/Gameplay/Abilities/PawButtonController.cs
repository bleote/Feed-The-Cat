using Firebase.Analytics;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static FirebaseRemoteConfigManager;

public class PawButtonController : MonoBehaviour
{
    public static PawButtonController Instance;
    public static Action OnScratchThemAll;

    [Header("Paw Button")]
    [SerializeField] private Image pawButtonBarFill;
    [SerializeField] private Button pawButton;
    [SerializeField] private Image pawButtonImage;
    [SerializeField] private Image pawButtonFlash;
    [SerializeField] private float buttonEnabledFlashTime;
    [SerializeField] private int pawButtonTarget;
    private Color inactivePawColor;
    private Color inactivepawButtonFlashColor;

    [Header("Paw Animation")]
    [SerializeField] private Animator pawFlashAnimation;
    [SerializeField] private Animator pawScratchAnimation;
    private Coroutine flashCoroutine;
    private bool isFlashing;

    private const string SCRATCH = "Scratch";
    private const string FLASH = "Flash";

    private int pawButtonTargetRelax;
    private int pawButtonTargetIntro;
    private int pawButtonTargetEasy;
    private int pawButtonTargetMedium;
    private int pawButtonTargetHard;
    private int pawButtonTargetBoss;

    private bool pawBoosterActive;
    private int eatenIceCreams;

    private int currentLevel;

    private PawButtonConfigData configData;

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
        configData = FirebaseRemoteConfigManager.Instance.pawButtonConfigData;
        pawBoosterActive = false;
        isFlashing = false;
        inactivePawColor = pawButtonImage.color;
        inactivepawButtonFlashColor = pawButtonFlash.color;
        currentLevel = GamePlayManager.currentLevel;

        LoadPawButtonTargetRemoteConfig();

        AdjustPawButtonTarget();
    }

    private void OnEnable()
    {
        pawButton.onClick.AddListener(Use);
        MyCharacterController.OnEatenIceCream += EatenIceCream;
    }

    private void OnDisable()
    {
        pawButton.onClick.RemoveListener(Use);
        MyCharacterController.OnEatenIceCream -= EatenIceCream;
    }

    private void Use()
    {
        if (eatenIceCreams < pawButtonTarget) { return; }


        if (GamePlayManager.isPaused)
        {
            if (TutorialGameplay.Instance.togglePawTutorial)
            {
                TutorialGameplay.Instance.TurnOffAllTutorialMasksAtGameplay();
                TutorialGameplay.Instance.TutorialMaskAndMessageSwitch(false);
            }
            else
            {
                return;
            }
        }

        StartCoroutine(PauseSpawnersForScratchAnimations());
        ScratchAnimatons();
        OnScratchThemAll?.Invoke();

        //Vibration.Vibrate(100);
        Vibration.VibrateAndroid(500);

        pawBoosterActive = true;
        eatenIceCreams = 0;

        // Stop the flashing when the paw booster is used
        isFlashing = false;
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        pawButtonImage.color = inactivePawColor;
        pawButtonFlash.color = inactivepawButtonFlashColor;

        FirebaseManager.Instance.LogPawButtonUsage();

        BarFillUpdate();
    }

    private void EatenIceCream()
    {
        if (!pawBoosterActive)
        {
            eatenIceCreams++;
        }

        if (eatenIceCreams >= pawButtonTarget)
        {
            pawButton.interactable = true;
            pawButtonImage.color = new Color(1, 1, 1, 1);

            if (!isFlashing)
            {
                BarFillUpdate();
                flashCoroutine = StartCoroutine(PawFlashCoroutine());
            }

            return;
        }

        BarFillUpdate();
    }

    public IEnumerator PawFlashCoroutine()
    {
        isFlashing = true;

        while (isFlashing)
        {
            float elapsedTimeA = 0f;

            // Fade in
            while (elapsedTimeA < buttonEnabledFlashTime)
            {
                // Wait while the game is paused
                while (GamePlayManager.isPaused)
                {
                    yield return null; // Wait for the next frame
                }

                // Increment elapsed time
                elapsedTimeA += Time.deltaTime;

                // Calculate alpha and update the button's color
                float alpha = Mathf.Lerp(0, 1, elapsedTimeA / buttonEnabledFlashTime);
                pawButtonFlash.color = new Color(1, 1, 0, alpha);

                yield return null; // Wait for the next frame
            }

            pawButtonFlash.color = new Color(1, 1, 0, 1); // Ensure fully visible

            float elapsedTimeB = 0f;

            // Fade out
            while (elapsedTimeB < buttonEnabledFlashTime)
            {
                // Wait while the game is paused
                while (GamePlayManager.isPaused)
                {
                    yield return null; // Wait for the next frame
                }

                // Increment elapsed time
                elapsedTimeB += Time.deltaTime;

                // Calculate alpha and update the button's color
                float alpha = Mathf.Lerp(1, 0, elapsedTimeB / buttonEnabledFlashTime);
                pawButtonFlash.color = new Color(1, 1, 0, alpha);

                yield return null; // Wait for the next frame
            }

            pawButtonFlash.color = new Color(1, 1, 0, 0); // Ensure fully invisible
        }
    }

    private void BarFillUpdate()
    {
        float targetFillAmount = (float)eatenIceCreams / pawButtonTarget;

        Routine.LerpConstant(
            pawButtonBarFill.fillAmount,
            targetFillAmount,
            0.05f,
            (fill) =>
            {
                pawButtonBarFill.fillAmount = fill;
                PawButtonActivationCheck();
            },
            () =>
            {
                pawButtonBarFill.fillAmount = targetFillAmount;
                PawButtonActivationCheck();
            }
        );
    }

    private void PawButtonActivationCheck()
    {
        if (pawButtonBarFill.fillAmount == 0)
        {
            pawBoosterActive = false;
            pawButton.interactable = false;
            pawButtonImage.color = inactivePawColor;
        }

        if (pawButtonBarFill.fillAmount == 1)
        {
            pawButton.interactable = true;
            pawButtonImage.color = new Color(1, 1, 1, 1);

            if (TutorialManager.Instance.pawTutorialActive && PlayerPrefs.GetInt(TutorialManager.PAW_TUTORIAL, -1) == -1)
            {
                TutorialGameplay.Instance.PawTutorialSetup();
            }
        }
    }

    private IEnumerator PauseSpawnersForScratchAnimations()
    {
        FoodSpawner.Instance.holdRegularSpawn = true;
        FoodSpawner.Instance.holdSpecialSpawn = true;
        FoodSpawner.Instance.holdUniqueSpawn = true;
        GoalSpawnerHandler.Instance.holdGoalSpawn = true;
        RewardingIceCreamSpawner.Instance.holdSpawn = true;
        ExtraChilliSpawner.Instance.holdExtraChilliSpawn = true;

        yield return new WaitForSeconds(1.1f);

        pawScratchAnimation.gameObject.SetActive(false);
        pawFlashAnimation.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        FoodSpawner.Instance.holdRegularSpawn = false;
        FoodSpawner.Instance.holdSpecialSpawn = false;
        FoodSpawner.Instance.holdUniqueSpawn = false;
        GoalSpawnerHandler.Instance.holdGoalSpawn = false;
        RewardingIceCreamSpawner.Instance.holdSpawn = false;
        ExtraChilliSpawner.Instance.holdExtraChilliSpawn = false;
    }

    private void ScratchAnimatons()
    {
        AudioManager.Instance.Play(AudioManager.CAT_SCRATCH_SOUND);
        AudioManager.Instance.Play(AudioManager.ICE_CREAM_COLLECT_SOUND);
        pawScratchAnimation.gameObject.SetActive(true);
        pawFlashAnimation.gameObject.SetActive(true);
        pawScratchAnimation.SetTrigger(SCRATCH);
        pawFlashAnimation.SetTrigger(FLASH);
    }

    private void LoadPawButtonTargetRemoteConfig()
    {
        pawButtonTargetRelax = configData.pawButtonTargetRelax;
        pawButtonTargetIntro = configData.pawButtonTargetIntro;
        pawButtonTargetEasy = configData.pawButtonTargetEasy;
        pawButtonTargetMedium = configData.pawButtonTargetMedium;
        pawButtonTargetHard = configData.pawButtonTargetHard;
        pawButtonTargetBoss = configData.pawButtonTargetBoss;
    }

    private void AdjustPawButtonTarget()
    {
        if (GamePlayManager.levelModeOn)
        {
            pawButtonTarget = SetPawButtonTarget();
        }
        else
        {
            pawButtonTarget = pawButtonTargetRelax;
        }
    }

    private int SetPawButtonTarget()
    {
        int _pawButtonTarget;

        switch (currentLevel)
        {
            case 1:
                if (TutorialManager.Instance.pawTutorialActive && PlayerPrefs.GetInt(TutorialManager.PAW_TUTORIAL, -1) == -1)
                {
                    _pawButtonTarget = pawButtonTargetIntro * 3;
                }
                else
                {
                    _pawButtonTarget = pawButtonTargetIntro;
                }
                break;

            case 2:
            case 3:
            case 4:
            case 5:
                _pawButtonTarget = pawButtonTargetIntro;
                break;

            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
                _pawButtonTarget = pawButtonTargetEasy;
                break;

            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
            case 23:
            case 24:
            case 25:
            case 26:
            case 27:
            case 28:
            case 29:
            case 30:
            case 31:
            case 32:
                _pawButtonTarget = pawButtonTargetMedium;
                break;

            case 33:
            case 34:
            case 35:
            case 36:
            case 37:
            case 38:
            case 39:
            case 40:
            case 41:
            case 42:
            case 43:
            case 44:
            case 45:
                _pawButtonTarget = pawButtonTargetHard;
                break;

            case 46:
                _pawButtonTarget = pawButtonTargetBoss;
                break;

            default:
                _pawButtonTarget = 50;
                break;
        }

        return _pawButtonTarget;
    }
}
