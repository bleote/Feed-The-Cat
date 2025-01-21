using UnityEngine;

public class BiscuitHandler : BoosterBase
{
    [SerializeField] private Transform hands;
    protected override void SubscribeEvents()
    {
        SuperPowersHandler.BiscuitsClicked += Use;
        DataManager.Instance.PlayerData.UpdatedBiscuits += ShowGraphics;
    }

    protected override void UnregisterEvents()
    {
        SuperPowersHandler.BiscuitsClicked -= Use;
        DataManager.Instance.PlayerData.UpdatedBiscuits -= ShowGraphics;
    }

    protected override void Use()
    {
        if (DataManager.Instance.PlayerData.Biscuits <= 0)
        {
            return;
        }
        AudioManager.Instance.Play(AudioManager.BONUS_ACTIVATED_SOUND);
        DataManager.Instance.PlayerData.Biscuits--;
        FoodController[] _foodObjects = GameObject.FindObjectsOfType<FoodController>();
        foreach (var _foodObject in _foodObjects)
        {
            if (_foodObject.transform.position.y<=hands.position.y)
            {
                Destroy(_foodObject.gameObject);
            }
        }
        MeltedIceCreamHandler.Instance.SetToZero();
        ShowGraphics();
    }

    protected override void ShowGraphics()
    {
        displayImage.sprite = DataManager.Instance.PlayerData.Biscuits > 0 ? boosterSO.AvailableSprite : boosterSO.NotAvailableSprite;
        amountDisplay.text = DataManager.Instance.PlayerData.Biscuits.ToString();
    }
}