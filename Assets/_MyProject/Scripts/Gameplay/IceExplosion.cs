using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceExplosion : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DestroyRoutine());
    }
    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
