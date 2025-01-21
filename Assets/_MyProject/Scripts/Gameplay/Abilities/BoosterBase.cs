using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterBase : MonoBehaviour
{
    [SerializeField] protected Image displayImage;
    [SerializeField] protected BoosterSO boosterSO;
    [SerializeField] protected GameObject amountHolder;
    [SerializeField] protected TextMeshProUGUI amountDisplay;
    [SerializeField] protected Image coolDownDisplay;
    [SerializeField] private GameObject lockedHolder;

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    protected virtual void SubscribeEvents()
    {
        throw new Exception("Subscribe events must be implemented");
    }

    protected virtual void UnregisterEvents()
    {
        throw new Exception("Unregister events must be implemented");
    }

    protected virtual void Use()
    {
        throw new Exception("Use must be implemented");
    }

    protected virtual void Start()
    {
        if (!DataManager.Instance.PlayerData.UnlockedBoosters.Contains(boosterSO.Id))
        {
            Color _color = displayImage.color;
            //_color.a = 0;
            displayImage.color = _color;
            //lockedHolder.SetActive(true);
        }
        ShowGraphics();
    }

    protected virtual void ShowGraphics()
    {
        throw new Exception("Show graphics must be implemented");
    }
    
    protected IEnumerator DurationBasedBooster(float _duration)
    {
        amountHolder.SetActive(false);
        coolDownDisplay.gameObject.SetActive(true);
        float _waitTimeCounter = _duration;
        while (_waitTimeCounter >= 0)
        {
            _waitTimeCounter -= Time.deltaTime;
            coolDownDisplay.fillAmount = _waitTimeCounter / _duration;
            yield return null;
        }
        coolDownDisplay.gameObject.SetActive(false);
        amountHolder.SetActive(true);
        FinishedDurationBasedBoost();
    }

    protected virtual void FinishedDurationBasedBoost()
    {
        throw new Exception("Looks like you forgot to implement me");
    }
}
