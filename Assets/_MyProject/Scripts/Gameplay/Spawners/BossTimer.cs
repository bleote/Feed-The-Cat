using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BossTimer : MonoBehaviour
{
    public static BossTimer Instance;

    [SerializeField] private GameObject bossGroup;
    [SerializeField] private float cooldownPeriod;
    [SerializeField] private Image bossTimerBarFill;
    [SerializeField] private TextMeshProUGUI bossTimerValueText;
    private float counter;
    private Coroutine blinkingCoroutine;

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
        if (GamePlayManager.Instance.bossLevel)
        {
            bossGroup.SetActive(true);
            counter = cooldownPeriod;
        }
    }

    private void Update()
    {
        if (!FoodSpawner.Instance.bossBombLineOn)
        {
            counter -= Time.deltaTime;

            if (counter <= 0)
            {
                counter = cooldownPeriod;
                FoodSpawner.Instance.bossBombLineOn = true;
            }

            BossBarFillUpdate();
            UpdateTimerText();

            if (blinkingCoroutine != null)
            {
                StopCoroutine(blinkingCoroutine);
                blinkingCoroutine = null;
                bossTimerBarFill.gameObject.SetActive(true);
            }
        }
        else
        {
            if (blinkingCoroutine == null)
            {
                blinkingCoroutine = StartCoroutine(BlinkingTimerBar());
            }
        }
    }

    private void BossBarFillUpdate()
    {
        bossTimerBarFill.fillAmount = counter / cooldownPeriod;
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(counter / 60);
        int seconds = Mathf.FloorToInt(counter % 60);
        bossTimerValueText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private IEnumerator BlinkingTimerBar()
    {
        bossTimerValueText.text = "00:00";
        AudioManager.Instance.Play(AudioManager.ALARM_SOUND);

        while (FoodSpawner.Instance.bossBombLineOn)
        {
            bossTimerBarFill.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            bossTimerBarFill.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);
        }
        bossTimerBarFill.gameObject.SetActive(true); // Ensure it's visible when stopping
    }
}
