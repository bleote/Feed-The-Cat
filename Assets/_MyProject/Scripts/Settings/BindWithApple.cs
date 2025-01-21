using UnityEngine;

public class BindWithApple : MonoBehaviour
{
    void Start()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            gameObject.SetActive(false);
        }
    }
}
