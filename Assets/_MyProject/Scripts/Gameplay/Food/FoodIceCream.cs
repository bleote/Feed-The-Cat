using UnityEngine;
using UnityEngine.UI;

public class FoodIceCream : FoodController
{
    [SerializeField] private ValueDisplay valueDisplayPrefab;
    [SerializeField] private IceCreamSO[] iceCreams;
    [SerializeField] private Image imageDisplay;
    [SerializeField] private string prefabIdentifier;
    private IceCreamSO iceCream;

    private Coroutine meltCoroutine;

    private int currentLevel;
    private int includeIceCreamsFromIndex;
    private int excludeIceCreamsFromIndex;
    private int firstIceCreamGoalIndex;
    private int secondIceCreamGoalIndex;
    private int thirdIceCreamGoalIndex;
    private int fourthIceCreamGoalIndex;
    private int selectedRandomIceCreamIndex;

    public void IceCreamValueDisplay(int _amount)
    {
        ValueDisplay _textObj = Instantiate(valueDisplayPrefab, GameObject.FindGameObjectWithTag("FoodHolder").transform);
        _textObj.Setup(_amount);
        _textObj.transform.localPosition = transform.localPosition;
    }

    public override void Setup(bool _randomRotation = true)
    {
        base.Setup(_randomRotation);

        currentLevel = GamePlayManager.currentLevel;

        if (!GamePlayManager.levelModeOn)
        {
            iceCream = iceCreams[Random.Range(0, iceCreams.Length)];
        }
        else
        {
            SetIceCreamGoalIndexes();

            SelectIceCreamFromAvailableRange();
        }

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

    private void SetIceCreamGoalIndexes()
    {
        firstIceCreamGoalIndex = GamePlayManager.firstIceCreamGoalIndex;

        if (currentLevel >= 6)
        {
            secondIceCreamGoalIndex = GamePlayManager.secondIceCreamGoalIndex;
        }

        if (currentLevel >= 13)
        {
            thirdIceCreamGoalIndex = GamePlayManager.thirdIceCreamGoalIndex;
        }

        if (currentLevel >= 22)
        {
            fourthIceCreamGoalIndex = GamePlayManager.fourthIceCreamGoalIndex;
        }
    }

    private void SelectIceCreamFromAvailableRange()
    {
        if (currentLevel == 1)
        {
            includeIceCreamsFromIndex = 1;
        }
        else
        {
            includeIceCreamsFromIndex = 0;
        }

        switch (currentLevel)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                excludeIceCreamsFromIndex = 6;
                selectedRandomIceCreamIndex = GetRandomNumberExcludingIceCreamGoals(includeIceCreamsFromIndex, excludeIceCreamsFromIndex, firstIceCreamGoalIndex);
                break;

            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
                excludeIceCreamsFromIndex = 9;
                selectedRandomIceCreamIndex = GetRandomNumberExcludingIceCreamGoals(includeIceCreamsFromIndex, excludeIceCreamsFromIndex, firstIceCreamGoalIndex, secondIceCreamGoalIndex + 6);
                break;

            default:
                excludeIceCreamsFromIndex = 12;
                selectedRandomIceCreamIndex = GetRandomNumberExcludingIceCreamGoals(includeIceCreamsFromIndex, excludeIceCreamsFromIndex, firstIceCreamGoalIndex, secondIceCreamGoalIndex + 6, thirdIceCreamGoalIndex + 9);
                break;
        }

        iceCream = iceCreams[selectedRandomIceCreamIndex];
    }

    private int GetRandomNumberExcludingIceCreamGoals(int includeIceCreamsFromIndex, int excludeIceCreamsFromIndex, int excludedIceCreamGoalIndex1, int excludedIceCreamGoalIndex2 = -1, int excludedIceCreamGoalIndex3 = -1)
    {
        int randomNum = Random.Range(includeIceCreamsFromIndex, excludeIceCreamsFromIndex);

        for (int i = 0; i < 3; i++)
        {
            if (randomNum != excludedIceCreamGoalIndex1 && randomNum != excludedIceCreamGoalIndex2 && randomNum != excludedIceCreamGoalIndex3)
            {
                return randomNum;
            }

            randomNum = Random.Range(includeIceCreamsFromIndex, excludeIceCreamsFromIndex);
        }

        // Adjust if the random number is still one of the excluded indexes after 3 attempts
        if (randomNum == excludedIceCreamGoalIndex1 || randomNum == excludedIceCreamGoalIndex2 || randomNum == excludedIceCreamGoalIndex3)
        {
            randomNum = includeIceCreamsFromIndex;
        }

        return randomNum;
    }
}
