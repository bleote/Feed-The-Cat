using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class OkDialog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageDisplay;
    [SerializeField] private Button okButton;

    [HideInInspector] public UnityEvent OkPressed;

    public void Show(string _message)
    {
        messageDisplay.text = _message;
        gameObject.SetActive(true);
        okButton.onClick.AddListener(OkButtonPressed);
    }

    private void OnDisable()
    {
        okButton.onClick.RemoveListener(OkButtonPressed);
    }

    private void OkButtonPressed()
    {
        OkPressed?.Invoke();
        OkPressed?.RemoveAllListeners();
        gameObject.SetActive(false);
    }
}
