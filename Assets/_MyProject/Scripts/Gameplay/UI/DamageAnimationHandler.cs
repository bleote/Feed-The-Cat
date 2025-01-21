using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageAnimationHandler : MonoBehaviour
{
    public static DamageAnimationHandler Instance;

    [SerializeField] private Image redFlash;
    [SerializeField] private float flashTime;
    [SerializeField] private int maxFlashes;

    private bool isFlashing;
    private int flashCount;

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

    public void InitiateDamageFlashCoroutine()
    {
        if (!isFlashing)
        {
            StartCoroutine(DamageFlashCoroutine());
        }
    }

    private IEnumerator DamageFlashCoroutine()
    {
        //Vibration.Vibrate(50);
        Vibration.VibrateNope();

        isFlashing = true;
        flashCount = 0;

        while (flashCount < maxFlashes && !GamePlayManager.continueProcess)
        {
            // Fade in
            float elapsedTimeA = 0;
            while (elapsedTimeA < flashTime && !GamePlayManager.continueProcess)
            {
                float alpha = Mathf.Lerp(0, 0.8f, elapsedTimeA / flashTime);
                redFlash.color = new Color(0.7f, 0, 0, alpha);

                yield return null;
                elapsedTimeA += Time.deltaTime;
            }

            if (GamePlayManager.continueProcess) break;

            redFlash.color = new Color(0.7f, 0, 0, 0.8f);

            // Fade out
            float elapsedTimeB = 0;
            while (elapsedTimeB < flashTime && !GamePlayManager.continueProcess)
            {
                float alpha = Mathf.Lerp(0.8f, 0, elapsedTimeB / flashTime);
                redFlash.color = new Color(0.7f, 0, 0, alpha);

                yield return null;
                elapsedTimeB += Time.deltaTime;
            }

            if (GamePlayManager.continueProcess) break;

            redFlash.color = new Color(0.7f, 0, 0, 0);
            flashCount++;
        }

        isFlashing = false;

        // Ensure flash ends cleanly if interrupted
        if (GamePlayManager.continueProcess)
        {
            redFlash.color = new Color(0.7f, 0, 0, 0); // Fully transparent
        }
    }
}
