using UnityEngine;

public class VibrationOld
{
#if UNITY_ANDROID
    private static AndroidJavaObject vibrator = null;
    private static AndroidJavaObject currentActivity = null;
#endif

    public static void Vibrate(long milliseconds)
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                if (vibrator == null)
                {
                    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                }

                // Check if the vibrator is available and has permission
                if (vibrator != null && vibrator.Call<bool>("hasVibrator"))
                {
                    vibrator.Call("vibrate", milliseconds);
                }
                else
                {
                    Debug.LogWarning("Vibration not supported or permission missing.");
                }
            }
            catch (AndroidJavaException e)
            {
                Debug.LogError("Vibration failed: " + e.Message);
            }
        }
#else
        Debug.LogWarning("Current vibration setup is only supported on Android devices.");
#endif
    }
}
