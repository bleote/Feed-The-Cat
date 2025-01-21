using System;
using System.Collections.Generic;
using System.Linq;

public static class LeaderBoardManager
{
    private static List<LeaderBoardEntry> entries = new List<LeaderBoardEntry>();
    private static Action<List<LeaderBoardEntry>> callback;

    public static void GetEntries(Action<List<LeaderBoardEntry>> _callback)
    {
        callback = _callback;
        if (entries.Count != 0)
        {
            callback.Invoke(entries);
            return;
        }

        FirebaseManager.Instance.GetLeaderboardEntries(SetEntries);
    }

    private static void SetEntries(List<LeaderBoardEntry> _entries)
    {
        entries = _entries.OrderByDescending(_element => _element.Score).ToList();
        for (int _i = 0; _i < _entries.Count; _i++)
        {
            entries[_i].Place = _i + 1;
        }

        callback?.Invoke(entries);
    }
}
