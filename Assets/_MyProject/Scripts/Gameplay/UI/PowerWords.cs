using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PowerWords : MonoBehaviour
{
    public static PowerWords Instance;

    [SerializeField] private GameObject powerWordsDisplay;
    [SerializeField] private Sprite[] powerWords;
    [SerializeField] private float fadeTime = 0.15f;

    private Coroutine showPowerWordCoroutine;

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

    public void ActivatePowerWord()
    {
        Image image = null;
        Sprite selectedSprite = null;

        image = powerWordsDisplay.GetComponent<Image>();
        int randomNumber = Random.Range(0, powerWords.Length);
        selectedSprite = powerWords[randomNumber];
        image.sprite = selectedSprite;

        if (selectedSprite != null)
        {
            if (showPowerWordCoroutine != null)
            {
                StopCoroutine(showPowerWordCoroutine);
            }

            showPowerWordCoroutine = StartCoroutine(ShowPowerWord(image));
        }
    }

    private IEnumerator ShowPowerWord(Image image)
    {
        image.color = new Color(1, 1, 1, 0);

        powerWordsDisplay.SetActive(true);
        
        float elapsedTimeA = 0;
        float elapsedTimeB = 0;

        // Fade in
        while (elapsedTimeA < fadeTime)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTimeA / fadeTime);
            image.color = new Color(1, 1, 1, alpha);

            yield return null;

            elapsedTimeA += Time.deltaTime;
        }

        image.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(0.9f);

        // Fade out
        while (elapsedTimeB < fadeTime)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTimeB / fadeTime);
            image.color = new Color(1, 1, 1, alpha);

            yield return null;

            elapsedTimeB += Time.deltaTime;
        }

        image.color = new Color(1, 1, 1, 0);
    }
}
