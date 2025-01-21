using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;

    public void Setup()
    {
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        closeButton.onClick.AddListener(Close);
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(Close);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
