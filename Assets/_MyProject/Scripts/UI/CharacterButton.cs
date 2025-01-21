using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    [SerializeField] private Button characterButton;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private string characterName;
    [SerializeField] private string inactiveNameText = "???";

    private Color inactiveColor;

    private void Start()
    {
        inactiveColor = characterImage.color;

        InactiveButtonSettings();
    }

    public void Setup()
    {
        characterButton.interactable = true;
        characterImage.color = Color.white;
        characterNameText.text = characterName;
    }

    public void InactiveButtonSettings()
    {
        characterButton.interactable = false;
        characterImage.color = inactiveColor;
        characterNameText.text = inactiveNameText;
    }
}
