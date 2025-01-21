using System.Collections;
using TMPro;
using UnityEngine;

public class ValueDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountDisplay;

    public void Setup(int _amount)
    {
        amountDisplay.text = "+" + _amount;
        amountDisplay.alpha = 0;
        StartCoroutine(AmountDisplayAnimation());
    }

    public void DamageSetup(int _amount)
    {
        amountDisplay.text = "-" + _amount;
        StartCoroutine(DamageDestroyRoutine());
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private IEnumerator DamageDestroyRoutine()
    {
        while (!GamePlayManager.continueProcess)
        {
            for (int i = 0; i < 3; i++)
            {
                if (GamePlayManager.continueProcess) break; // Check if need to exit early

                yield return new WaitForSeconds(0.1f);

                amountDisplay.color = new Color(0, 0, 0, 0);

                if (GamePlayManager.continueProcess) break; // Check again to exit early

                yield return new WaitForSeconds(0.1f);

                amountDisplay.color = Color.red;
            }

            if (GamePlayManager.continueProcess) break; // Check after the loop

            float waitTime = 0.7f;
            float elapsedTime = 0;
            while (elapsedTime < waitTime && !GamePlayManager.continueProcess)
            {
                yield return null; 
                elapsedTime += Time.deltaTime;
            }

            if (GamePlayManager.continueProcess) break; // Final check after the wait

            Destroy(gameObject);
            yield break;
        }

        Destroy(gameObject);
    }

    private IEnumerator AmountDisplayAnimation()
    {
        Vector3 startPosition = amountDisplay.transform.localPosition;
        Vector3 endPosition = new(amountDisplay.transform.localPosition.x, amountDisplay.transform.localPosition.y + 30, amountDisplay.transform.localPosition.z);
        float duration = 0.3f;
        float fadeDuration = 0.25f;
        float elapsedTime = 0;

        // Move text up with fade-in
        while (elapsedTime < duration)
        {
            // Calculate progress for movement
            float progress = Mathf.Clamp01(elapsedTime / duration);
            amountDisplay.transform.localPosition = Vector3.Lerp(startPosition, endPosition, progress);

            // Calculate and set alpha for fade-in
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            amountDisplay.alpha = alpha;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        amountDisplay.alpha = 1;
        amountDisplay.transform.localPosition = endPosition;

        // Short pause displaying the amount
        yield return new WaitForSeconds(0.2f);

        // text fade-out
        elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            amountDisplay.alpha = alpha;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        amountDisplay.alpha = 0;

        // Short pause with no amount display before destroying the object.
        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }
}
