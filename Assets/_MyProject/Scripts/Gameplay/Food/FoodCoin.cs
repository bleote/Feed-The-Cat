using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FoodCoin : FoodController
{
    [SerializeField] private ValueDisplay valueDisplayPrefab;

    public void CoinValueDisplay(int amount)
    {
        ValueDisplay _textObj = Instantiate(valueDisplayPrefab, GameObject.FindGameObjectWithTag("FoodHolder").transform);
        _textObj.transform.localPosition = transform.localPosition;
        _textObj.Setup(amount);
    }
}
