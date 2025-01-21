using Firebase.Analytics;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static FirebaseRemoteConfigManager;

public class IceButtonController : MonoBehaviour
{
    [Header("Ice Button")]
    [SerializeField] private Image iceButtonBarFill;
    [SerializeField] private Button iceButton;
    [SerializeField] private Image IceButtonFlash;
    [SerializeField] private Image IceCubeImage;
    [SerializeField] private Image lockImage;
    [SerializeField] private float buttonEnabledFlashTime;
    [SerializeField] private int iceButtonTarget;
    private Color inactiveIceCubeColor;
    private Color inactiveIceButtonFlashColor;
    public bool icebuttonIsFlashing;
    private Coroutine buttonFlashCoroutine;

    [Header("Ice Blow Animation")]
    [SerializeField] private Animator iceBlowAnimation;

    private int iceButtonTargetRelax;
    private int iceButtonTargetIntro;
    private int iceButtonTargetEasy;
    private int iceButtonTargetMedium;
    private int iceButtonTargetHard;
    private int iceButtonTargetBoss;

    private bool IceBoosterActive;
    private int eatenIceCubes;

    private int currentLevel;

    private IceButtonConfigData configData;

    private void Start()
    {
        configData = Instance.iceButtonConfigData;
        IceBoosterActive = false;
        icebuttonIsFlashing = false;
        inactiveIceCubeColor = IceCubeImage.color;
        inactiveIceButtonFlashColor = IceButtonFlash.color;
        currentLevel = GamePlayManager.currentLevel;

        LoadIceButtonTargetRemoteConfig();

        AdjustIceButtonTarget();

        if (GamePlayManager.Instance.bossLevel)
        {
            IceCubeImage.gameObject.SetActive(false);
            lockImage.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        iceButton.onClick.AddListener(Use);
        MyCharacterController.OnEatenIceCube += EatenIceCube;
    }

    private void OnDisable()
    {
        iceButton.onClick.RemoveListener(Use);
        MyCharacterController.OnEatenIceCube -= EatenIceCube;
    }

    private void Use()
    {
        if (eatenIceCubes < iceButtonTarget) { return; }

        if (GamePlayManager.isPaused)
        {
            if (TutorialGameplay.Instance.toggleIceButtonTutorial)
            {
                TutorialGameplay.Instance.TurnOffAllTutorialMasksAtGameplay();
                TutorialGameplay.Instance.TutorialMaskAndMessageSwitch(false);
            }
            else
            {
                return;
            }
        }

        IceBoosterActive = true;
        eatenIceCubes = 0;
        FrozenChilliController.Instance.freezeChillies = true;

        StartCoroutine(BlowAnimationAutomaticSwitch());

        AudioManager.Instance.Play(AudioManager.ICE_BLOW_SOUND);

        //Vibration.Vibrate(100);
        Vibration.VibrateAndroid(500);

        // Stop the flashing when the ice blow booster is used
        icebuttonIsFlashing = false;
        if (buttonFlashCoroutine != null)
        {
            StopCoroutine(buttonFlashCoroutine);
            buttonFlashCoroutine = null;
        }

        IceCubeImage.color = inactiveIceCubeColor;
        IceButtonFlash.color = inactiveIceButtonFlashColor;

        FirebaseManager.Instance.LogIceButtonUsage();

        BarFillUpdate();
    }

    private void EatenIceCube()
    {
        if (!IceBoosterActive)
        {
            eatenIceCubes++;
        }

        if (eatenIceCubes >= iceButtonTarget)
        {
            iceButton.interactable = true;
            IceCubeImage.color = new Color(1, 1, 1, 1);

            if (!icebuttonIsFlashing)
            {
                BarFillUpdate();
                buttonFlashCoroutine = StartCoroutine(IceButtonFlashCoroutine());
            }

            return;
        }

        BarFillUpdate();
    }

    public IEnumerator IceButtonFlashCoroutine()
    {
        icebuttonIsFlashing = true;

        while (icebuttonIsFlashing)
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
                IceButtonFlash.color = new Color(0, 1, 1, alpha);

                yield return null; // Wait for the next frame
            }

            IceButtonFlash.color = new Color(0, 1, 1, 1); // Ensure fully visible

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
                IceButtonFlash.color = new Color(0, 1, 1, alpha);

                yield return null; // Wait for the next frame
            }

            IceButtonFlash.color = new Color(0, 1, 1, 0); // Ensure fully invisible
        }
    }

    private void BarFillUpdate()
    {
        float targetFillAmount = (float)eatenIceCubes / iceButtonTarget;

        Routine.LerpConstant(
            iceButtonBarFill.fillAmount,
            targetFillAmount,
            0.05f,
            (fill) =>
            {
                iceButtonBarFill.fillAmount = fill;
                IceButtonActivationCheck();
            },
            () =>
            {
                iceButtonBarFill.fillAmount = targetFillAmount;
                IceButtonActivationCheck();
            }
        );
    }

    private IEnumerator BlowAnimationAutomaticSwitch()
    {
        iceBlowAnimation.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.1f);

        iceBlowAnimation.gameObject.SetActive(false);
    }

    private void IceButtonActivationCheck()
    {
        if (iceButtonBarFill.fillAmount == 0)
        {
            IceBoosterActive = false;
            iceButton.interactable = false;
            IceCubeImage.color = inactiveIceCubeColor;
        }

        if (iceButtonBarFill.fillAmount == 1)
        {
            iceButton.interactable = true;
            IceCubeImage.color = new Color(1, 1, 1, 1);

            if (TutorialManager.Instance.iceButtonTutorialActive && PlayerPrefs.GetInt(TutorialManager.ICE_BUTTON_TUTORIAL, -1) == -1)
            {
                TutorialGameplay.Instance.IceButtonTutorialSetup();
            }
        }
    }

    private void LoadIceButtonTargetRemoteConfig()
    {
        iceButtonTargetRelax = configData.iceButtonTargetRelax;
        iceButtonTargetIntro = configData.iceButtonTargetIntro;
        iceButtonTargetEasy = configData.iceButtonTargetEasy;
        iceButtonTargetMedium = configData.iceButtonTargetMedium;
        iceButtonTargetHard = configData.iceButtonTargetHard;
        iceButtonTargetBoss = configData.iceButtonTargetBoss;
    }

    private void AdjustIceButtonTarget()
    {
        if (GamePlayManager.levelModeOn)
        {
            iceButtonTarget = SetIceButtonTarget();
        }
        else
        {
            iceButtonTarget = iceButtonTargetRelax;
        }
    }

    private int SetIceButtonTarget()
    {
        int _iceButtonTarget;

        switch (currentLevel)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                _iceButtonTarget = iceButtonTargetIntro;
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
                _iceButtonTarget = iceButtonTargetEasy;
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
                _iceButtonTarget = iceButtonTargetMedium;
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
                _iceButtonTarget = iceButtonTargetHard;
                break;

            case 46:
                _iceButtonTarget = iceButtonTargetBoss;
                break;

            default:
                _iceButtonTarget = 5;
                break;
        }

        return _iceButtonTarget;
    }
}
