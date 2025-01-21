using Firebase.Analytics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RewardingIceCreamSpawner : MonoBehaviour
{
    public static RewardingIceCreamSpawner Instance;

    [SerializeField] private FoodController prefab;
    [SerializeField] private Transform foodHolder;

    [SerializeField] private float cooldownPeriod;
    [SerializeField] private Image rewardTimeBarFill;
    [SerializeField] private Animator rewardBarFillAnimator;
    [SerializeField] private Transform timerValueTransform;
    [SerializeField] private TextMeshProUGUI timerValueText;
    [SerializeField] private Animator timerValueAnimator;
    private bool timerValueIsPulsing;
    private bool clockTickingSound;
    private bool clockTickingSoundActive;
    private float counter;
    private float fullCount;
    private Color originalTimerValueTextColor;

    private const string IS_BLINKING = "isBlinking";
    private const string IS_PULSING = "isPulsing";

    public bool holdSpawn;

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
        holdSpawn = false;
        timerValueIsPulsing = false;
        clockTickingSound = false;
        clockTickingSoundActive = false;
        counter = 0;
        cooldownPeriod = FirebaseRemoteConfigManager.Instance.adsConfigData.rewardingIceCreamCoolDownPeriod;
        fullCount = cooldownPeriod;
        originalTimerValueTextColor = timerValueText.color;
    }

    private void Update()
    {
        counter += Time.deltaTime;
        rewardTimeBarFill.fillAmount = counter / fullCount;

        RewardBarFillUpdate();

        CheckForPulsingTimeValue();

        GamePlayUI.Instance.SetTimer((int)fullCount - counter);
        
        if (counter >= fullCount)
        {
            counter = 0;
            GamePlayUI.Instance.SetTimer((int)fullCount);
            FoodController _rewardingIceCream = Instantiate(prefab, foodHolder);
            AudioManager.Instance.Play(AudioManager.BUBBLE_POP_SOUND);

            _rewardingIceCream.transform.position = timerValueTransform.position;

            _rewardingIceCream.Setup(false);

            FirebaseAnalytics.LogEvent("rv_ice_cream_shown");
        }
    }

    private void RewardBarFillUpdate()
    {
        if (rewardTimeBarFill.fillAmount >= 0.75)
        {
            rewardBarFillAnimator.SetBool(IS_BLINKING, true);
        }
        else
        {
            rewardBarFillAnimator.SetBool(IS_BLINKING, false);
        }
    }

    private void CheckForPulsingTimeValue()
    {
        if (rewardTimeBarFill.fillAmount >= 1 - (60 / cooldownPeriod / 10))
        {
            timerValueIsPulsing = true;
            
            if (!clockTickingSoundActive)
            {
                clockTickingSoundActive = true;
                ActivateClockTickingSound();
            }
        }
        else
        {
            timerValueIsPulsing = false;
            clockTickingSoundActive = false;
            clockTickingSound = false;
        }

        if (timerValueIsPulsing)
        {
            timerValueAnimator.SetBool(IS_PULSING, true);
            timerValueText.color = new Color(0, 0.6f, 0, 1);
        }
        else
        {
            timerValueAnimator.SetBool(IS_PULSING, false);
            timerValueText.color = originalTimerValueTextColor;
        }
    }

    private void ActivateClockTickingSound()
    {
        if (!clockTickingSound && clockTickingSoundActive)
        {
            clockTickingSound = true;
            AudioManager.Instance.Play(AudioManager.CLOCK_TICKING_SOUND);
        }
    }
}
