using UnityEngine;
using TMPro;

public class KeysDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keysDisplay;

    private void OnEnable()
    {
        DataManager.Instance.PlayerData.UpdatedKeys += ShowKeys;
    }

    private void OnDisable()
    {
        DataManager.Instance.PlayerData.UpdatedKeys -= ShowKeys;
    }

    private void Start()
    {
        ShowKeys();
    }

    public void ShowKeys()
    {
        keysDisplay.text = DataManager.Instance.PlayerData.Keys.ToString();
    }
}
