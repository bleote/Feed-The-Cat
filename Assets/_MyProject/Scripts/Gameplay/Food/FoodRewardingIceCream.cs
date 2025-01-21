using UnityEngine;

public class FoodRewardingIceCream : FoodController
{
    [SerializeField] private IceCreamSO iceCream;

    private Coroutine meltCoroutine;

    protected override bool AllowAutomaticEating()
    {
        return false;
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
