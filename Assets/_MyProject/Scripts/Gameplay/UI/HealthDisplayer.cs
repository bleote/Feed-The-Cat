using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayer : MonoBehaviour
{
    [SerializeField] private Image healthDisplay;

    private void OnEnable()
    {
        GamePlayManager.UpdatedCurrentHP += ShowHealth;
    }

    private void OnDisable()
    {
        GamePlayManager.UpdatedCurrentHP -= ShowHealth;
    }

    private void ShowHealth()
    {
        Routine.Lerp(healthDisplay.fillAmount, (float)GamePlayManager.Instance.CurrentHP / GamePlayManager.Instance.MaxHP, .2f, (fill) => healthDisplay.fillAmount = fill);
        //healthDisplay.fillAmount =  (float)GamePlayManager.Instance.CurrentAmountOfLives / GamePlayManager.Instance.MaxAmountOfLives;
    }
}
