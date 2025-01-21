using UnityEngine;

public class BindWithGoogle : SettingsBindOption
{
    void Start()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            gameObject.SetActive(false);
        }   
    }
}
