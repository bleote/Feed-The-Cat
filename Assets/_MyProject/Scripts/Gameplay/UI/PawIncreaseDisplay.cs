using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PawIncreaseDisplay : MonoBehaviour
{
    [SerializeField] private Image pawIncreaseImage;
    [SerializeField] private float moveUpTime;
    [SerializeField] private float moveToPawButtonTime;

    public void Setup(int _amount)
    {
        StartCoroutine(DestroyRoutine());
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
