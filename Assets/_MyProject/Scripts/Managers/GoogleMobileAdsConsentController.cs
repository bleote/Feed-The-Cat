using Firebase.Analytics;
using GoogleMobileAds.Ump.Api;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GoogleMobileAdsConsentController : MonoBehaviour
{
    public static GoogleMobileAdsConsentController Instance;

    /// <summary>
    /// If true, it is safe to call MobileAds.Initialize() and load Ads.
    /// </summary>
    public bool CanRequestAds => ConsentInformation.CanRequestAds();

    [SerializeField, Tooltip("Button to show user consent and privacy settings.")]
    private Button _privacyButton;

    [SerializeField, Tooltip("GameObject with the error popup.")]
    private GameObject _errorPopup;

    [SerializeField, Tooltip("Error message for the error popup.")]
    private TextMeshProUGUI _errorText;

    [SerializeField, Tooltip("Button to show user consent and privacy settings.")]
    private Button _closeErrorPopupButton;

    private bool isConsentPopupActive = false; // Tracks if the consent popup is active

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Disable the error popup,
        if (_errorPopup != null)
        {
            _errorPopup.SetActive(false);
        }
    }

    /// <summary>
    /// Startup method for the Google User Messaging Platform (UMP) SDK
    /// which will run all startup logic including loading any required
    /// updates and displaying any required forms.
    /// </summary>
    public void GatherConsent(Action<string> onComplete)
    {
        // Debug.Log("Gathering consent.");

        var requestParameters = new ConsentRequestParameters
        {
            // False means users are not under age.
            TagForUnderAgeOfConsent = false
        };

        // Combine the callback with an error popup handler.
        onComplete = (onComplete == null)
            ? UpdateErrorPopup
            : onComplete + UpdateErrorPopup;

        // The Google Mobile Ads SDK provides the User Messaging Platform (Google's
        // IAB Certified consent management platform) as one solution to capture
        // consent for users in GDPR impacted countries. This is an example and
        // you can choose another consent management platform to capture consent.
        ConsentInformation.Update(requestParameters, (FormError updateError) =>
        {
            // Enable the change privacy settings button.
            UpdatePrivacyButton();

            if (updateError != null)
            {
                onComplete(updateError.Message);
                return;
            }

            // Determine the consent-related action to take based on the ConsentStatus.
            if (CanRequestAds)
            {
                // Consent has already been gathered or not required.
                // Return control back to the user.
                FirebaseAnalytics.LogEvent("consent_approved");
                onComplete(null);
                return;
            }

            // Consent not obtained and is required. Load and show the consent form.
            LoadAndShowConsentForm(onComplete);
        });
    }

    /// <summary>
    /// Loads and shows the initial consent form if required.
    /// </summary>
    private void LoadAndShowConsentForm(Action<string> onComplete)
    {
        ConsentForm.Load((ConsentForm form, FormError loadError) =>
        {
            if (loadError != null)
            {
                onComplete?.Invoke(loadError.Message);
                return;
            }

            isConsentPopupActive = true; // Mark popup as active

            // Show the consent form only if it was successfully loaded.
            form.Show((FormError showError) =>
            {
                UpdatePrivacyButton();
                isConsentPopupActive = false; // Mark popup as inactive

                if (showError != null)
                {
                    FirebaseAnalytics.LogEvent("consent_disapproved");
                    onComplete?.Invoke(showError.Message);
                }
                else
                {
                    if (ConsentInformation.ConsentStatus == GoogleMobileAds.Ump.Api.ConsentStatus.Required || !ConsentInformation.CanRequestAds())
                    {
                        FirebaseAnalytics.LogEvent("consent_disapproved");
                    }
                    else
                    {
                        FirebaseAnalytics.LogEvent("consent_approved");
                    }
                    onComplete?.Invoke(null); // Consent form shown successfully
                }
            });
        });
    }

    /// <summary>
    /// Shows the privacy options form to the user.
    /// </summary>
    /// <remarks>
    /// Your app needs to allow the user to change their consent status at any time.
    /// Load another form and store it to allow the user to change their consent status
    /// </remarks>
    public void ShowPrivacyOptionsForm(Action<string> onComplete)
    {
        // Debug.Log("Showing privacy options form.");

        // combine the callback with an error popup handler.
        onComplete = (onComplete == null)
            ? UpdateErrorPopup
            : onComplete + UpdateErrorPopup;

        // Load the privacy options form first, then display it if loaded.
        ConsentForm.Load((ConsentForm form, FormError loadError) =>
        {
            if (loadError != null)
            {
                onComplete?.Invoke("Error loading privacy options form: " + loadError.Message);
                return;
            }

            // Show the form if successfully loaded
            form.Show((FormError showError) =>
            {
                UpdatePrivacyButton();
                if (showError != null)
                {
                    onComplete?.Invoke("Error showing privacy options form: " + showError.Message);
                }
                else
                {
                    onComplete?.Invoke(null); // Privacy options form shown successfully
                }
            });
        });
    }

    /// <summary>
    /// Reset ConsentInformation for the user.
    /// </summary>
    public void ResetConsentInformation()
    {
        ConsentInformation.Reset();
        UpdatePrivacyButton();
    }

    private void OnApplicationQuit()
    {
        if (isConsentPopupActive)
        {
            FirebaseAnalytics.LogEvent("consent_quit"); // Log the consent_quit event
        }
    }

    void UpdatePrivacyButton()
    {
        if (_privacyButton != null)
        {
            PrivacyOptionsRequirementStatus status = ConsentInformation.PrivacyOptionsRequirementStatus;
            Debug.Log("PrivacyOptionsRequirementStatus: " + status);

            _privacyButton.interactable = (status == PrivacyOptionsRequirementStatus.Required);
        }
    }

    void UpdateErrorPopup(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        if (_errorText != null)
        {
            _errorText.text = message;
        }

        if (_errorPopup != null)
        {
            _errorPopup.SetActive(true);
        }
        if (_privacyButton != null)
        {
            _privacyButton.interactable = true;
        }
    }

    // This method will be called every time a new scene is loaded
    public void FindPrivacyButton()
    {
        // Try to find the button in the newly loaded scene
        if (_privacyButton == null)
        {
            _privacyButton = GameObject.FindWithTag("PrivacySettingsButton").GetComponent<Button>();

            if (_privacyButton != null)
            {
                //Debug.Log("The privacy settings button was found!");
                return;
            }
        }
    }

    private void OnEnable()
    {
        _closeErrorPopupButton.onClick.AddListener(ContinueAndCloseErrorPopUp);
    }

    private void OnDisable()
    {
        _closeErrorPopupButton.onClick.RemoveListener(ContinueAndCloseErrorPopUp);
    }

    public void ContinueAndCloseErrorPopUp()
    {
        _errorPopup.SetActive(false);
    }
}
