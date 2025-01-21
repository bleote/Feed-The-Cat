public class ExtraLivesHandler : BoosterBase
{
    protected override void SubscribeEvents()
    {
        SuperPowersHandler.ExtraLivesClicked += Use;
        DataManager.Instance.PlayerData.UpdatedExtraLives += ShowGraphics;
    }

    protected override void UnregisterEvents()
    {
        SuperPowersHandler.ExtraLivesClicked -= Use;
        DataManager.Instance.PlayerData.UpdatedExtraLives -= ShowGraphics;
    }


    protected override void Use()
    {
        if (GamePlayManager.Instance.CurrentHP>=GamePlayManager.Instance.MaxHP)
        {
            return;
        }
        if (DataManager.Instance.PlayerData.ExtraLivesPackage > 0)
        {
            AudioManager.Instance.Play(AudioManager.BONUS_ACTIVATED_SOUND);
            DataManager.Instance.PlayerData.ExtraLivesPackage--;
            GamePlayManager.Instance.RecoverToFullHealth();
        }
        ShowGraphics();
    }

    protected override void ShowGraphics()
    {
        displayImage.sprite = DataManager.Instance.PlayerData.ExtraLivesPackage > 0 ? boosterSO.AvailableSprite : boosterSO.NotAvailableSprite;
        amountDisplay.text = DataManager.Instance.PlayerData.ExtraLivesPackage.ToString();
    }
}
