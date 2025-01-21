public class MusicHandler : SettingsButtonOption
{
    protected  override  void Toggle()
    {
        DataManager.Instance.PlayerData.PlayMusic = !DataManager.Instance.PlayerData.PlayMusic;
        Show();
    }

    protected override void Show()
    {
        display.sprite = DataManager.Instance.PlayerData.PlayMusic ? on : off;
    }
}
