using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuCoinSpent : MonoBehaviour
{
    [SerializeField] private int coinSpentValue;
    [SerializeField] private TextMeshProUGUI coinSpentText;

    public void Start()
    {
        coinSpentValue = FirebaseRemoteConfigManager.Instance.adsConfigData.extraHeartCost;
        coinSpentText.text = $"-{coinSpentValue}";
        StartCoroutine(SpentCoinAnimation());
    }

    private IEnumerator SpentCoinAnimation()
    {
        yield return new WaitForSeconds(0.2f);

        Vector3 startPosition = coinSpentText.transform.position;
        Vector3 endPosition = new(coinSpentText.transform.position.x, coinSpentText.transform.position.y - 100, coinSpentText.transform.position.z);
        float elapsedTime1 = 0;
        float elapsedTime2 = 0;

        while (elapsedTime1 < 0.66f)
        {
            coinSpentText.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime1 / 0.66f);
            elapsedTime1 += Time.deltaTime;
            yield return null;
        }

        coinSpentText.transform.position = endPosition;

        while (elapsedTime2 < 0.33f)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime2 / 0.33f);
            coinSpentText.color = new Color(1, 0, 0, alpha);

            yield return null;

            elapsedTime2 += Time.deltaTime;
        }

        coinSpentText.color = new Color(1, 0, 0, 0);

        Destroy(gameObject);
    }
}
