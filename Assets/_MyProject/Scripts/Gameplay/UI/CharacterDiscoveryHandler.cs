using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDiscoveryHandler : MonoBehaviour
{
    public static CharacterDiscoveryHandler Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI characterDiscoveryHeader;
    [SerializeField] private TextMeshProUGUI learnMore;
    [SerializeField] private Image characterImage;
    [SerializeField] private Sprite[] characterSprites;
    [SerializeField] private Button tapToContinue;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Setup()
    {
        AudioManager.Instance.Play(AudioManager.CHARACTER_DISCOVERY_SOUND);
        SetDiscoveredCharacter();
        tapToContinue.onClick.AddListener(Continue);
        panel.SetActive(true);
    }

    private void SetDiscoveredCharacter()
    {
        if (PlayerPrefs.GetInt(CatsScreenUI.FROSTBITE_DESCRIPTION, -1) == -1 && GamePlayManager.Instance.useAnnoyingBossEffect)
        {
            PlayerPrefs.SetInt(CatsScreenUI.FROSTBITE_DESCRIPTION, 1);
            characterDiscoveryHeader.text = "YOU FACED\r\nFROSTBITE!";
            learnMore.text = "Learn more about Frostbite at the\r\nCharacter tab on the Main Menu.\r\n\r\n(Tap To Continue)";
            characterImage.sprite = characterSprites[0];

            FirebaseManager.Instance.LogCharacterDiscovery("Frostbite");
        }

        if (PlayerPrefs.GetInt(CatsScreenUI.SPLASH_DESCRIPTION, -1) == -1 && GamePlayManager.currentLevel == 11)
        {
            PlayerPrefs.SetInt(CatsScreenUI.SPLASH_DESCRIPTION, 1);
            characterDiscoveryHeader.text = "YOU SAVED\r\nSPLASH!";
            learnMore.text = "Learn more about Splash at the\r\nCharacter tab on the Main Menu.\r\n\r\n(Tap To Continue)";
            characterImage.sprite = characterSprites[1];

            FirebaseManager.Instance.LogCharacterDiscovery("Splash");
        }

        if (PlayerPrefs.GetInt(CatsScreenUI.PEBBLE_DESCRIPTION, -1) == -1 && GamePlayManager.currentLevel == 23)
        {
            PlayerPrefs.SetInt(CatsScreenUI.PEBBLE_DESCRIPTION, 1);
            characterDiscoveryHeader.text = "YOU SAVED\r\nPEBBLE!";
            learnMore.text = "Learn more about Pebble at the\r\nCharacter tab on the Main Menu.\r\n\r\n(Tap To Continue)";
            characterImage.sprite = characterSprites[2];

            FirebaseManager.Instance.LogCharacterDiscovery("Pebble");
        }
    }

    private void Continue()
    {
        LevelCompletedHandler.Instance.CharacterDiscoveryConclusionHandler();
        tapToContinue.onClick.RemoveListener(Continue);
        panel.SetActive(false);
    }
}
