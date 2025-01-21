using UnityEngine;
using UnityEngine.UI;

public class FoodIceCreamUnique : FoodController
{
    [SerializeField] private ValueDisplay valueDisplayPrefab;
    [SerializeField] private IceCreamSO[] iceCreams;
    [SerializeField] private Image imageDisplay;
    private IceCreamSO iceCream;

    private Coroutine meltCoroutine;

    public void IceCreamValueDisplay(int _amount)
    {
        ValueDisplay _textObj = Instantiate(valueDisplayPrefab, GameObject.FindGameObjectWithTag("FoodHolder").transform);
        _textObj.Setup(_amount);
        _textObj.transform.localPosition = transform.localPosition;
    }

    public override void Setup(bool _randomRotation = true)
    {
        base.Setup(_randomRotation);
        iceCream = iceCreams[Random.Range(0, iceCreams.Length)];
        imageDisplay.sprite = iceCream.Whole;
        imageDisplay.SetNativeSize();
        GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta * 1.1f;
    }

    protected override void HandleCollisionWithBorder()
    {
        OnReachedBorder?.Invoke(this);

        if (meltCoroutine != null)
        {
            StopCoroutine(meltCoroutine);
        }

        meltCoroutine = StartCoroutine(Melt(iceCream));
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (meltCoroutine != null)
        {
            StopCoroutine(meltCoroutine);
            meltCoroutine = null;
        }
    }
}
