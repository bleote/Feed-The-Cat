using Firebase.Analytics;
using System;
using UnityEngine;

public class RelaxModeTracker : MonoBehaviour
{
    private float startTime;
    private float endTime;
    private bool isTracking;

    void Start()
    {
        TryStartTracking();
    }

    private void OnDestroy()
    {
        if (isTracking)
        {
            StopTracking();
        }
    }

    private void OnApplicationPause(bool _pause)
    {
        if (_pause)
        {
            if (isTracking)
            {
                StopTracking();
            }
        }
        else
        {
            TryStartTracking();
        }
    }

    private void OnApplicationQuit()
    {
        if (isTracking)
        {
            StopTracking();
        }
    }

    private void TryStartTracking()
    {
        if (!GamePlayManager.levelModeOn && !isTracking)
        {
            StartTracking();
        }
    }

    private void StartTracking()
    {
        startTime = Time.time;
        isTracking = true;
    }

    private void StopTracking()
    {
        float endTime = Time.time;
        float timeSpent = endTime - startTime;
        isTracking = false;

        FirebaseManager.Instance.LogRelaxModeTimeSpent(timeSpent);
    }
}
