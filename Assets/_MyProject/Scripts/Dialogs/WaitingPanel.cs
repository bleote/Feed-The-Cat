using UnityEngine;
using TMPro;


public class WaitingPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageDisplay;

    public void Show(string _message)
    {
        messageDisplay.text = _message;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
