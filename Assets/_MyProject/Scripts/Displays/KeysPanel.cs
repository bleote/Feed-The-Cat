using UnityEngine;
using UnityEngine.UI;

public class KeysPanel : MonoBehaviour
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

    void Close()
    {
        gameObject.SetActive(false);
    }
}
