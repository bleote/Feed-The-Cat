using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EntrieDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI placeDisplay;
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private Image backgroundDisplay;
    [SerializeField] private Sprite[] backgrounds;
    [SerializeField] private Sprite[] crowns;
    [SerializeField] private Image crownDisplay;

    public void Setup(LeaderBoardEntry _entry, bool _isOd)
    {
        placeDisplay.text = _entry.Place.ToString();
        nameDisplay.text = _entry.Nickname;
        scoreDisplay.text = _entry.Score.ToString();

        if (_entry.Place <= 3)
        {
            crownDisplay.sprite = crowns[_entry.Place - 1];
        }
        else
        {
            crownDisplay.gameObject.SetActive(false);
        }

        backgroundDisplay.sprite = _isOd ? backgrounds[0] : backgrounds[1];
    }
}
