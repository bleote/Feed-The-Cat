using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class Explosion : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(DestroyRoutine());
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(0.8f);

        Destroy(gameObject);
    }
}
