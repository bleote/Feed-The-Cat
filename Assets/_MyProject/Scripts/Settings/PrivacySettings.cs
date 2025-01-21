using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrivacySettings : MonoBehaviour
{
    [SerializeField] private Button gearButton;
    [SerializeField] private Button privacySettings;
    [SerializeField] private Transform privacySettingsPositionReference;
    [SerializeField] private float motionSpeed;

    private float PrivacySettingsButtonInitialPositionY;
    private float PrivacySettingsButtonFinalPositionY;

    private bool PrivacySettingsButtonEnabled = false;

    private void Start()
    {
        GoogleMobileAdsConsentController.Instance.FindPrivacyButton();

        PrivacySettingsButtonInitialPositionY = privacySettings.transform.position.y;
        PrivacySettingsButtonFinalPositionY = privacySettingsPositionReference.position.y;
    }

    private void OnEnable()
    {
        privacySettings.onClick.AddListener(CallPrivacyOptions);
        gearButton.onClick.AddListener(SwitchPrivacySettingsButon);
    }

    private void OnDisable()
    {
        privacySettings.onClick.RemoveListener(CallPrivacyOptions);
        gearButton.onClick.RemoveListener(SwitchPrivacySettingsButon);
    }

    private void CallPrivacyOptions()
    {
        // Call ShowPrivacyOptionsForm without needing to handle any results (pass null)
        GoogleMobileAdsConsentController.Instance.ShowPrivacyOptionsForm(null);
    }

    private void SwitchPrivacySettingsButon()
    {
        if (!PrivacySettingsButtonEnabled)
        {
            StartCoroutine(MovePrivacyButtonIn());
        }
        else
        {
            StartCoroutine(MovePrivacyButtonOut());
        }
    }

    private IEnumerator MovePrivacyButtonIn()
    {
        PrivacySettingsButtonEnabled = true;

        while (privacySettings.transform.position.y > PrivacySettingsButtonFinalPositionY)
        {
            float newY = Mathf.MoveTowards(privacySettings.transform.position.y, PrivacySettingsButtonFinalPositionY, Time.deltaTime * motionSpeed);

            privacySettings.transform.position = new Vector2(privacySettings.transform.position.x, newY);

            yield return null;
        }

        privacySettings.transform.position = new Vector2(privacySettings.transform.position.x, PrivacySettingsButtonFinalPositionY);
    }

    private IEnumerator MovePrivacyButtonOut()
    {
        PrivacySettingsButtonEnabled = false;

        while (privacySettings.transform.position.y < PrivacySettingsButtonInitialPositionY)
        {
            float newY = Mathf.MoveTowards(privacySettings.transform.position.y, PrivacySettingsButtonInitialPositionY, Time.deltaTime * motionSpeed);

            privacySettings.transform.position = new Vector2(privacySettings.transform.position.x, newY);

            yield return null;
        }

        privacySettings.transform.position = new Vector2(privacySettings.transform.position.x, PrivacySettingsButtonInitialPositionY);
    }
}
