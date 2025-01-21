using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsToggleHandler : MonoBehaviour
{
    [SerializeField] protected Transform onHolder;
    [SerializeField] protected Transform offHolder;
    [SerializeField] protected Transform toggleDisplay;
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(Toggle);
        Show();
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(Toggle);
    }

    protected virtual void Show()
    {
        throw new Exception("Show must be implemented");
    }

    protected virtual void Toggle()
    {
        throw new Exception("Toggle must be implemented");
    }
}
