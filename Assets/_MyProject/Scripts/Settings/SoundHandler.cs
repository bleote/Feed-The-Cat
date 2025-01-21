public class SoundHandler : SettingsButtonOption
{
    protected  override  void Toggle()
    {
        DataManager.Instance.PlayerData.PlaySound = !DataManager.Instance.PlayerData.PlaySound;
        Show();
    }

    protected override void Show()
    {
        display.sprite = DataManager.Instance.PlayerData.PlaySound ? on : off;
    }
}
