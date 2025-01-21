using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StartupClearConsole : MonoBehaviour
{
    void Start()
    {
        ClearConsole();
    }

    // Method to clear the console using Unity's internal LogEntries class
    private static void ClearConsole()
    {
        // Get the LogEntries class through reflection
        var logEntries = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries?.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
        clearMethod?.Invoke(null, null); // Call the clear method if available
    }
}
