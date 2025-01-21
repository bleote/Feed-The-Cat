using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroLogoFade : MonoBehaviour
{
    [SerializeField] private Image darkScreen;
    [SerializeField] private float fadeTime;

    void Start()
    {
        StartCoroutine(FadeAnimation());
    }

    private IEnumerator FadeAnimation()
    {
        float startTimeA = Time.time;
        float elapsedTimeA = 0;

        while (elapsedTimeA < fadeTime)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTimeA / fadeTime);
            darkScreen.color = new Color(0, 0, 0, alpha);

            yield return null;

            elapsedTimeA = Time.time - startTimeA;
        }

        darkScreen.color = new Color(0, 0, 0, 0);

        yield return new WaitForSeconds(1);

        float startTimeB = Time.time;
        float elapsedTimeB = 0;

        while (elapsedTimeB < fadeTime)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTimeB / fadeTime);
            darkScreen.color = new Color(0, 0, 0, alpha);

            yield return null;

            elapsedTimeB = Time.time - startTimeB;
        }

        darkScreen.color = new Color(0, 0, 0, 1);

        SceneController.LoadDataCollector();
    }
}
