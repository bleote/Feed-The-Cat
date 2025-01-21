using UnityEngine;
using TMPro;

public class MultiplierHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI multiplierDisplay;

    private void OnEnable()
    {
        GamePlayManager.UpdatedMultiplier += ShowMultiplier;
    }

    private void OnDisable()
    {
        GamePlayManager.UpdatedMultiplier -= ShowMultiplier;
    }

    private void ShowMultiplier()
    {
        multiplierDisplay.text = "x" + GamePlayManager.Instance.Multiplier;
    }
}
