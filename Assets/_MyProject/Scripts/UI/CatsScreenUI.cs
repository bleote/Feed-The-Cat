using Firebase.Analytics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FirebaseRemoteConfigManager;

public class CatsScreenUI : MonoBehaviour
{
    public static CatsScreenUI Instance;

    [SerializeField] private Button characterOne;
    [SerializeField] private Button characterTwo;
    [SerializeField] private Button characterThree;
    [SerializeField] private Button characterFour;
    [SerializeField] private GameObject characterDescriptionBox;
    [SerializeField] private TextMeshProUGUI characterHeader;
    [SerializeField] private TextMeshProUGUI characterText;
    [SerializeField] private Image characterImage;
    [SerializeField] private Button closeDescription;
    [SerializeField] private Sprite[] characterSprites;
    [SerializeField] private ScrollRect scrollRect;

    public const string SCOOP_DESCRIPTION = "ScoopDescription";
    public const string FROSTBITE_DESCRIPTION = "FrostbiteDescription";
    public const string SPLASH_DESCRIPTION = "SplashDescription";
    public const string PEBBLE_DESCRIPTION = "PebbleDescription";

    public bool scoopActive;
    public bool frostbiteActive;
    public bool splashActive;
    public bool pebbleActive;

    private CharactersConfigData configData;

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
    private void Start()
    {
        configData = FirebaseRemoteConfigManager.Instance.charactersConfigData;

        CheckActiveCharacterDescriptions();

        ActivateCharacterButtons();

        FirebaseAnalytics.LogEvent("characters_section");
    }

    private void OnEnable()
    {
        characterOne.onClick.AddListener(CharacterOneDescription);
        characterTwo.onClick.AddListener(CharacterTwoDescription);
        characterThree.onClick.AddListener(CharacterThreeDescription);
        characterFour.onClick.AddListener(CharacterFourDescription);
        closeDescription.onClick.AddListener(CloseCharacterDescriptionBox);
    }

    private void OnDisable()
    {
        characterOne.onClick.RemoveListener(CharacterOneDescription);
        characterTwo.onClick.RemoveListener(CharacterTwoDescription);
        characterThree.onClick.RemoveListener(CharacterThreeDescription);
        characterFour.onClick.RemoveListener(CharacterFourDescription);
        closeDescription.onClick.RemoveListener(CloseCharacterDescriptionBox);
    }

    private void CharacterOneDescription()
    {
        characterHeader.text = configData.characterOneName;
        characterText.text = configData.characterOneText;
        characterImage.sprite = characterSprites[0];
        ActivateDescriptionBox();

        FirebaseManager.Instance.LogCharacterDescriptionView("Scoop");
    }

    private void CharacterTwoDescription()
    {
        characterHeader.text = configData.characterTwoName;
        characterText.text = configData.characterTwoText;
        characterImage.sprite = characterSprites[1];
        ActivateDescriptionBox();

        FirebaseManager.Instance.LogCharacterDescriptionView("Frostbite");
    }

    private void CharacterThreeDescription()
    {
        characterHeader.text = configData.characterThreeName;
        characterText.text = configData.characterThreeText;
        characterImage.sprite = characterSprites[2];
        ActivateDescriptionBox();

        FirebaseManager.Instance.LogCharacterDescriptionView("Splash");
    }

    private void CharacterFourDescription()
    {
        characterHeader.text = configData.characterFourName;
        characterText.text = configData.characterFourText;
        characterImage.sprite = characterSprites[3];
        ActivateDescriptionBox();

        FirebaseManager.Instance.LogCharacterDescriptionView("Pebble");
    }

    private void ActivateDescriptionBox()
    {
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        characterDescriptionBox.SetActive(true);
        DeactivateCharacterButtons();
    }

    private void CloseCharacterDescriptionBox()
    {
        characterDescriptionBox.SetActive(false);
        ActivateCharacterButtons();
    }

    private void CheckActiveCharacterDescriptions()
    {
        scoopActive = true;

        if (PlayerPrefs.GetInt(FROSTBITE_DESCRIPTION, -1) != -1)
        {
            frostbiteActive = true;
        }

        if (PlayerPrefs.GetInt(SPLASH_DESCRIPTION, -1) != -1)
        {
            splashActive = true;
        }

        if (PlayerPrefs.GetInt(PEBBLE_DESCRIPTION, -1) != -1)
        {
            pebbleActive = true;
        }
    }

    private void ActivateCharacterButtons()
    {
        if (scoopActive)
        {
            CharacterButton _scoop = characterOne.GetComponent<CharacterButton>();
            _scoop.Setup();
        }

        if (frostbiteActive)
        {
            CharacterButton _frostbite = characterTwo.GetComponent<CharacterButton>();
            _frostbite.Setup();
        }

        if (splashActive)
        {
            CharacterButton _splash = characterThree.GetComponent<CharacterButton>();
            _splash.Setup();
        }

        if (pebbleActive)
        {
            CharacterButton _pebble = characterFour.GetComponent<CharacterButton>();
            _pebble.Setup();
        }
    }

    private void DeactivateCharacterButtons()
    {
        if (scoopActive)
        {
            CharacterButton _scoop = characterOne.GetComponent<CharacterButton>();
            _scoop.InactiveButtonSettings();
        }

        if (frostbiteActive)
        {
            CharacterButton _frostbite = characterTwo.GetComponent<CharacterButton>();
            _frostbite.InactiveButtonSettings();
        }

        if (splashActive)
        {
            CharacterButton _splash = characterThree.GetComponent<CharacterButton>();
            _splash.InactiveButtonSettings();
        }

        if (pebbleActive)
        {
            CharacterButton _pebble = characterFour.GetComponent<CharacterButton>();
            _pebble.InactiveButtonSettings();
        }
    }
}
