public class VibrationHandler :  SettingsButtonOption
{
    protected  override  void Toggle()
    {
        DataManager.Instance.PlayerData.Vibration = !DataManager.Instance.PlayerData.Vibration;
        Show();
    }

    protected override void Show()
    {
        display.sprite = DataManager.Instance.PlayerData.Vibration ? on : off;
    }
}
