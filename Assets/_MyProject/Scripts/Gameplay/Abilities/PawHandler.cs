public class PawHandler : BoosterBase
{
    protected override void SubscribeEvents()
    {
        SuperPowersHandler.FireClicked += Use;
        DataManager.Instance.PlayerData.UpdatedPaws += ShowGraphics;
    }

    protected override void UnregisterEvents()
    {
        SuperPowersHandler.FireClicked -= Use;
        DataManager.Instance.PlayerData.UpdatedPaws -= ShowGraphics;
    }

    protected  override void Use()
    {
        
    }

    protected override void ShowGraphics()
    {
        displayImage.sprite = DataManager.Instance.PlayerData.Paws > 0 ? boosterSO.AvailableSprite : boosterSO.NotAvailableSprite;
        amountDisplay.text = DataManager.Instance.PlayerData.Paws.ToString();
    }
}
