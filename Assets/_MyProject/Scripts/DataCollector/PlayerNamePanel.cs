using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerNamePanel : MonoBehaviour
{
    public static Action SavedName;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button confirmButton;

    public void Setup()
    {
        //nameInput.text = string.Empty;
        //gameObject.SetActive(true);

        //     TEMPORARILY Automatically set the name to "username"
        // Debug.Log("Setting new username");
        nameInput.text = "username";
        gameObject.SetActive(true);
        SetName(); // TEMPORARILY Automatically call SetName to bypass the user input
    }

    private void OnEnable()
    {
        confirmButton.onClick.AddListener(SetName);
    }

    private void OnDisable()
    {
        confirmButton.onClick.RemoveListener(SetName);
    }

    private void SetName()
    {
        string _name = nameInput.text;
        if (IsNameValid(_name))
        {
            UIManager.Instance.WaitPanel.Show("Setting up data");
            confirmButton.interactable = false;
            DataManager.Instance.CreateNewPlayer();
            DataManager.Instance.PlayerData.UserName = _name;
            DataManager.Instance.PlayerData.PlaySound = true;
            DataManager.Instance.PlayerData.PlayMusic = true;
            FirebaseManager.Instance.SaveEverything(SaveHandler);

            UIManager.Instance.finishedPlayerDataLoading = true;
        }
    }

    private void SaveHandler(bool _status)
    {
        UIManager.Instance.WaitPanel.Hide();
        if (_status)
        {
            SavedName?.Invoke();
        }
        else
        {
            confirmButton.interactable = true;
            UIManager.Instance.OkDialog.Show("Something went wrong, please try another name or try again later. If this keeps occuring please contact support");
        }
    }

    private bool IsNameValid(string _name)
    {
        if (string.IsNullOrEmpty(_name))
        {
            UIManager.Instance.OkDialog.Show("Please enter your name");
            return false;
        }
        int _length = _name.Trim().Length;

        if (_length < 4)
        {
            UIManager.Instance.OkDialog.Show("Name must be at least 4 characters");
            return false;
        }

        if (_length > 10)
        {
            UIManager.Instance.OkDialog.Show("Name must be at most 10 characters");
            return false;
        }

        return true;
    }
}
