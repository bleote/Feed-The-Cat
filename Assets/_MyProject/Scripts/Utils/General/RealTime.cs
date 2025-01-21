using System;
using UnityEngine;


/// <summary>
/// This is a class containing generic Real Time functions
/// <remarks></remarks>
/// </summary>
public class RealTime {
    public static Vector3Int GetTimeFromSeconds (int seconds) {
        return new Vector3Int (Mathf.FloorToInt(seconds / 3600), Mathf.FloorToInt(seconds % 3600 / 60), Mathf.FloorToInt(seconds % 60));
    }

    public static int GetSecondsFromDays (int days) {
        return days * 24 * 60 * 60;
    }

    public static int GetSecondsFromHours (int hours) {
        return hours * 60 * 60;
    }

    public static int GetSecondsFromMinutes (int seconds) {
        return seconds * 24 * 60 * 60;
    }

    public static DateTime GetTimeFromPlayerprefs(string key) {
        return DateTime.FromBinary(long.Parse(PlayerPrefs.GetString(key, "0")));
    }

    public static void SaveTimeToPlayerprefs(string key, DateTime time) {
        PlayerPrefs.SetString(key, time.ToBinary().ToString());
    }

    public static DateTime GetTimeNow() {
        return DateTime.Now;
    }

    public static int GetTimeDiff(DateTime time1, DateTime time2) {
        return (int) (time1 - time2).TotalSeconds;
    }

    public static int GetTimeDiffFromNow(DateTime time) {
        return (int) (DateTime.Now - time).TotalSeconds;
    }
}
