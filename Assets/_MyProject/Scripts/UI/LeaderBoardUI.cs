using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardUI : MonoBehaviour
{
    [SerializeField] private Button leaveButton;
    [SerializeField] private Transform entriesHolder;
    [SerializeField] private EntrieDisplay entryPrefab;

    private readonly List<EntrieDisplay> displayedEntries = new List<EntrieDisplay>();

    private void OnEnable()
    {
        leaveButton.onClick.AddListener(ShowMainMenu);
    }

    private void OnDisable()
    {
        leaveButton.onClick.RemoveListener(ShowMainMenu);
        ClearShownItems();
    }

    private void ClearShownItems()
    {
        foreach (var _entire in displayedEntries)
        {
            Destroy(_entire);
        }

        displayedEntries.Clear();
    }

    private void ShowMainMenu()
    {
        SceneController.LoadMainMenu();
    }

    private void Start()
    {
        UIManager.Instance.WaitPanel.Show("Loading leaderboard...");
        LeaderBoardManager.GetEntries(ShowEntries);
    }

    private void ShowEntries(List<LeaderBoardEntry> _entries)
    {
        int _counter = 0;
        foreach (var _entry in _entries)
        {
            EntrieDisplay _entryDisplay = Instantiate(entryPrefab, entriesHolder);
            _entryDisplay.Setup(_entry, _counter % 2 == 0);
            displayedEntries.Add(_entryDisplay);
            _counter++;
        }

        UIManager.Instance.WaitPanel.Hide();
    }
}
