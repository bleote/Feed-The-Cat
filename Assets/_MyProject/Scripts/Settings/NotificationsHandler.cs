using UnityEngine;

public class NotificationsHandler : SettingsToggleHandler
{
    protected override void Toggle()
    {
        DataManager.Instance.PlayerData.Notifications = !DataManager.Instance.PlayerData.Notifications;
        Show();
    }

    protected override void Show()
    {
        Transform _parent = DataManager.Instance.PlayerData.Notifications ? onHolder : offHolder;
        Transform _toggleDisplayTransform=toggleDisplay.transform;
        _toggleDisplayTransform.SetParent(_parent);
        _toggleDisplayTransform.localPosition = Vector3.zero;
    }
}
