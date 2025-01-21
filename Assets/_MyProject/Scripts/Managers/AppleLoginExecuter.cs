using UnityEngine;

public class AppleLoginExecuter : MonoBehaviour
{
    void Update()
    {
        if (AppleManager.IsInit)
        {
            AppleManager.Update();
        }
    }
}
