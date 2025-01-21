using UnityEngine;
using UnityEngine.UI;

public class FoodIceCreamGoal : FoodController
{
    [SerializeField] private ValueDisplay valueDisplayPrefab;
    [SerializeField] private IceCreamSO[] iceCreams;
    [SerializeField] private Image imageDisplay;
    private IceCreamSO iceCream;
    public int goalSpawner;
    public int iceCreamIndex = -1;

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

        SelectIceCreamGoal();

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

    public void SetGoalSpawnerAndIceCreamGoalIndex(int goalSpawnerInt, int iceCreamGoalIndex)
    {
        goalSpawner = goalSpawnerInt;
        iceCreamIndex = iceCreamGoalIndex;
    }

    private void SelectIceCreamGoal()
    {
        if (iceCreamIndex != -1)
        {
            iceCream = iceCreams[iceCreamIndex];
        }
        else
        {
            // Debug.Log($"Error. Setup Ice Cream Goal Index: {iceCreamIndex}");
            return;
        }
    }
}
