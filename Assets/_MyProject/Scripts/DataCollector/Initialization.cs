using System;
using UnityEngine;

public class Initialization : MonoBehaviour
{
    public static Action Finished;

    private void Start()
    {
        InitSOs();
    }

    private void InitSOs()
    {
        CatSO.Init();
        InitFirebase();
    }

    private void InitFirebase()
    {
        FirebaseManager.Instance.Init(FinishInit);
    }

    private void FinishInit()
    {
        Finished?.Invoke();
    }
}
