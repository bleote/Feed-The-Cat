using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotTaker : MonoBehaviour
{
    public string screenshotName = "screenshot";

    public void TakeScreenShot()
    {
            string path = screenshotName + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            ScreenCapture.CaptureScreenshot(path);
            // Debug.Log("Screenshot saved to: " + path);
    }
}
