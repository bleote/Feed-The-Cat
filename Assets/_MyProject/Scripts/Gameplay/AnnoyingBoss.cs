using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnnoyingBoss : MonoBehaviour
{
    public static AnnoyingBoss Instance;

    [SerializeField] private GameObject leftSideWarning;
    [SerializeField] private GameObject leftPunchArea;
    [SerializeField] private GameObject leftPunchImage;
    [SerializeField] private GameObject rightSideWarning;
    [SerializeField] private GameObject rightPunchArea;
    [SerializeField] private GameObject rightPunchImage;
    [SerializeField] private float annoyingBossCoolDownMin;
    [SerializeField] private float annoyingBossCoolDownMax;
    [SerializeField] private float warningTime;
    [SerializeField] private float annoyingBossWaitMin;
    [SerializeField] private float annoyingBossWaitMax;
    [SerializeField] private Sprite[] frostbiteFaceSprites;

    private Image rightSideWarningImage;
    private Image leftSideWarningImage;
    private float annoyingBossCoolDown;
    private float annoyingBossTimer;
    private float annoyingBossWait;

    public bool bossCanPunch;
    public bool punchActive;
    public int punchLevel;

    private float sideFloat;
    public bool punchFromRightSide;

    RectTransform punchRectTransform;
    Vector2 punchPosition;

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
        rightSideWarningImage = rightSideWarning.GetComponent<Image>();
        leftSideWarningImage = leftSideWarning.GetComponent<Image>();

        annoyingBossCoolDown = annoyingBossCoolDownMin;
        annoyingBossTimer = 0;
        punchActive = false;
        ChooseRandomPunchSide();

        if (!GamePlayManager.Instance.useAnnoyingBossEffect)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!GamePlayManager.Instance.useAnnoyingBossEffect) { return; }

        if (!punchActive)
        {
            if (!GamePlayManager.isPaused)
            {
                annoyingBossTimer += Time.deltaTime;
            }

            if (annoyingBossTimer >= annoyingBossCoolDown)
            {
                annoyingBossCoolDown = Random.Range(annoyingBossCoolDownMin, annoyingBossCoolDownMax);
                annoyingBossTimer = 0;
                punchActive = true;
            
                if (punchFromRightSide)
                {
                    StartCoroutine(RightSideWarning());
                }
                else
                {
                    StartCoroutine(LeftSideWarning());
                }
            }
        }
    }

    private IEnumerator RightSideWarning()
    {
        RectTransform rectTransform = rightSideWarning.GetComponent<RectTransform>();

        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 targetXPosition = new Vector2(startPosition.x - 175, startPosition.y); // Adjust based on desired movement in UI

        float elapsedTime1 = 0;
        float elapsedTime2 = 0;

        // Wiggle parameters
        float wiggleAmount = 2.5f; // Amount of left-right movement
        int wiggleCount = 2;       // Number of wiggles
        float wiggleSpeed = 15f;    // Speed of the wiggle

        // Move to the target position
        while (elapsedTime1 < warningTime)
        {
            if (!GamePlayManager.isPaused)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetXPosition, elapsedTime1 / warningTime);
                elapsedTime1 += Time.deltaTime;
            }
            
            yield return null;
        }

        rectTransform.anchoredPosition = targetXPosition;

        // Frostbite blinks once
        if (!GamePlayManager.isPaused)
        {
            rightSideWarningImage.sprite = frostbiteFaceSprites[1];
            yield return new WaitForSeconds(0.1f);
            rightSideWarningImage.sprite = frostbiteFaceSprites[0];
        }

        // Wiggle effect
        for (int i = 0; i < wiggleCount; i++)
        {
            float wiggleElapsedTime = 0;
            while (wiggleElapsedTime < Mathf.PI * 2 / wiggleSpeed)
            {
                if (!GamePlayManager.isPaused)
                {
                    float offset = Mathf.Sin(wiggleElapsedTime * wiggleSpeed) * wiggleAmount;
                    rectTransform.anchoredPosition = new Vector2(targetXPosition.x + offset, targetXPosition.y);
                    wiggleElapsedTime += Time.deltaTime;
                }
                
                yield return null;
            }
        }

        rectTransform.anchoredPosition = targetXPosition;

        // Return to the starting position
        while (elapsedTime2 < warningTime)
        {
            if (!GamePlayManager.isPaused)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(targetXPosition, startPosition, elapsedTime2 / warningTime);
                elapsedTime2 += Time.deltaTime;
            }
            
            yield return null;
        }


        rectTransform.anchoredPosition = startPosition;


        // Set Punch vertical position
        punchRectTransform = rightPunchArea.GetComponent<RectTransform>();
        punchPosition = punchRectTransform.anchoredPosition;
        SetPunchLevel();
        punchRectTransform.anchoredPosition = punchPosition;

        // Set the wait time before the punch
        annoyingBossWait = Random.Range(annoyingBossWaitMin, annoyingBossWaitMax);
        while (GamePlayManager.isPaused) yield return null; // Wait if the game is paused
        yield return new WaitForSeconds(annoyingBossWait);

        bossCanPunch = true;
    }

    private IEnumerator LeftSideWarning()
    {
        RectTransform rectTransform = leftSideWarning.GetComponent<RectTransform>();

        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 targetXPosition = new Vector2(startPosition.x + 175, startPosition.y); // Adjust based on desired movement in UI

        float elapsedTime1 = 0;
        float elapsedTime2 = 0;

        // Wiggle parameters
        float wiggleAmount = 2.5f; // Amount of left-right movement
        int wiggleCount = 2;       // Number of wiggles
        float wiggleSpeed = 15f;    // Speed of the wiggle

        // Move to the target position
        while (elapsedTime1 < warningTime)
        {
            if (!GamePlayManager.isPaused)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetXPosition, elapsedTime1 / warningTime);
                elapsedTime1 += Time.deltaTime;
            }
            yield return null;
        }

        rectTransform.anchoredPosition = targetXPosition;

        // Frostbite blinks once
        if (!GamePlayManager.isPaused)
        {
            leftSideWarningImage.sprite = frostbiteFaceSprites[1];
            yield return new WaitForSeconds(0.1f);
            leftSideWarningImage.sprite = frostbiteFaceSprites[0];
        }

        // Wiggle effect
        for (int i = 0; i < wiggleCount; i++)
        {
            float wiggleElapsedTime = 0;
            while (wiggleElapsedTime < Mathf.PI * 2 / wiggleSpeed)
            {
                if (!GamePlayManager.isPaused)
                {
                    float offset = Mathf.Sin(wiggleElapsedTime * wiggleSpeed) * wiggleAmount;
                    rectTransform.anchoredPosition = new Vector2(targetXPosition.x + offset, targetXPosition.y);
                    wiggleElapsedTime += Time.deltaTime;
                }
                yield return null;
            }
        }

        rectTransform.anchoredPosition = targetXPosition;

        // Return to the starting position
        while (elapsedTime2 < warningTime)
        {
            if (!GamePlayManager.isPaused)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(targetXPosition, startPosition, elapsedTime2 / warningTime);
                elapsedTime2 += Time.deltaTime;
            }
            yield return null;
        }


        rectTransform.anchoredPosition = startPosition;


        // Set Punch vertical position
        punchRectTransform = leftPunchArea.GetComponent<RectTransform>();
        punchPosition = punchRectTransform.anchoredPosition;
        SetPunchLevel();
        punchRectTransform.anchoredPosition = punchPosition;

        // Set the wait time before the punch
        annoyingBossWait = Random.Range(annoyingBossWaitMin, annoyingBossWaitMax);
        yield return new WaitForSeconds(annoyingBossWait);

        bossCanPunch = true;
    }

    private void SetPunchLevel()
    {
        punchLevel = Random.Range(1, 4);

        switch (punchLevel)
        {
            case 1:
                punchPosition = new(punchPosition.x, 150);
                break;

            case 2:
                punchPosition = new(punchPosition.x, 0);
                break;

            case 3:
                punchPosition = new(punchPosition.x, -150);
                break;

            default:
                punchPosition = new(punchPosition.x, 0);
                break;
        }
    }

    public void CallPunchRoutine()
    {
        if (punchFromRightSide)
        {
            StartCoroutine(RightSidePunch());
        }
        else
        {
            StartCoroutine(LeftSidePunch());
        }
    }

    private IEnumerator RightSidePunch()
    {
        RectTransform rectTransform = rightPunchImage.GetComponent<RectTransform>();

        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 targetXPosition = new Vector2(startPosition.x - 265, startPosition.y); // Adjust 265 based on UI layout

        float elapsedTime1 = 0;
        float elapsedTime2 = 0;

        // Wiggle parameters
        float wiggleAmount = 5f; // Amount of left-right movement
        int wiggleCount = 2;     // Number of wiggles
        float wiggleSpeed = 100f;  // Speed of the wiggle

        AudioManager.Instance.Play(AudioManager.WHOOSH_SOUND);

        // Move to the target position
        while (elapsedTime1 < 0.1f)
        {
            if (!GamePlayManager.isPaused)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetXPosition, elapsedTime1 / 0.1f);
                elapsedTime1 += Time.deltaTime;
            }
            
            yield return null;
        }

        rectTransform.anchoredPosition = targetXPosition;

        // Wiggle effect
        for (int i = 0; i < wiggleCount; i++)
        {
            float wiggleElapsedTime = 0;
            while (wiggleElapsedTime < Mathf.PI * 2 / wiggleSpeed)
            {
                if (!GamePlayManager.isPaused)
                {
                    float offset = Mathf.Sin(wiggleElapsedTime * wiggleSpeed) * wiggleAmount;
                    rectTransform.anchoredPosition = new Vector2(targetXPosition.x + offset, targetXPosition.y);
                    wiggleElapsedTime += Time.deltaTime;
                }
                
                yield return null;
            }
        }

        // Pause while waiting to resume, if necessary
        while (GamePlayManager.isPaused) yield return null;

        yield return new WaitForSeconds(0.3f);

        // Return to the starting position
        while (elapsedTime2 < 0.5f)
        {
            if (!GamePlayManager.isPaused)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(targetXPosition, startPosition, elapsedTime2 / 0.5f);
                elapsedTime2 += Time.deltaTime;
            }
            
            yield return null;
        }

        rectTransform.anchoredPosition = startPosition;

        punchActive = false;
        ChooseRandomPunchSide();
    }

    private IEnumerator LeftSidePunch()
    {
        RectTransform rectTransform = leftPunchImage.GetComponent<RectTransform>();

        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 targetXPosition = new Vector2(startPosition.x + 265, startPosition.y); // Adjust 265 based on UI layout

        float elapsedTime1 = 0;
        float elapsedTime2 = 0;

        // Wiggle parameters
        float wiggleAmount = 5f; // Amount of left-right movement
        int wiggleCount = 2;     // Number of wiggles
        float wiggleSpeed = 100f;  // Speed of the wiggle

        AudioManager.Instance.Play(AudioManager.WHOOSH_SOUND);

        // Move to the target position
        while (elapsedTime1 < 0.1f)
        {
            if (!GamePlayManager.isPaused)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetXPosition, elapsedTime1 / 0.1f);
                elapsedTime1 += Time.deltaTime;
            }
            
            yield return null;
        }

        rectTransform.anchoredPosition = targetXPosition;

        // Wiggle effect
        for (int i = 0; i < wiggleCount; i++)
        {
            float wiggleElapsedTime = 0;
            while (wiggleElapsedTime < Mathf.PI * 2 / wiggleSpeed)
            {
                if (!GamePlayManager.isPaused)
                {
                    float offset = Mathf.Sin(wiggleElapsedTime * wiggleSpeed) * wiggleAmount;
                    rectTransform.anchoredPosition = new Vector2(targetXPosition.x + offset, targetXPosition.y);
                    wiggleElapsedTime += Time.deltaTime;
                }
                
                yield return null;
            }
        }

        // Pause while waiting to resume, if necessary
        while (GamePlayManager.isPaused) yield return null;

        yield return new WaitForSeconds(0.3f);

        // Return to the starting position
        while (elapsedTime2 < 0.5f)
        {
            if (!GamePlayManager.isPaused)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(targetXPosition, startPosition, elapsedTime2 / 0.5f);
                elapsedTime2 += Time.deltaTime;
            }
            
            yield return null;
        }

        rectTransform.anchoredPosition = startPosition;

        punchActive = false;
        ChooseRandomPunchSide();
    }

    private void ChooseRandomPunchSide()
    {
        sideFloat = Random.value;

        if (sideFloat > 0.5f)
        {
            punchFromRightSide = true;
        }
        else
        {
            punchFromRightSide = false;
        }
    }
}
