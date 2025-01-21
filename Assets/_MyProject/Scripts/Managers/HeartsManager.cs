using System;
using UnityEngine;

public class HeartsManager : MonoBehaviour
{
    public static HeartsManager Instance;

    [SerializeField] private float amountOfSecondsForNextHeart;

    private bool isInit;
    private float counter;

    public int SecondsLeftForAnotherHeart => Mathf.FloorToInt(counter);
    public bool IsFull => DataManager.Instance.PlayerData.Hearts >= DataManager.Instance.GameData.MaxAmountOfHearts;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        if (!isInit)
        {
            amountOfSecondsForNextHeart = FirebaseRemoteConfigManager.Instance.generalConfigData.amountOfSecondsForNextHeart;
            CalculateHearts();
            isInit = true;
        }
    }

    public void CalculateHearts()
    {
        if (DataManager.Instance.PlayerData == null)
        {
            // Debug.Log("DataManager or PlayerData is null.");
            return;
        }

        if (DataManager.Instance.GameData == null)
        {
            // Debug.Log("DataManager or GameData is null.");
            return;
        }

        int secondsThatPassedSinceLastLogin = (int)(DateTime.UtcNow - DataManager.Instance.PlayerData.LastTimeClosedApp).TotalSeconds;
        counter = DataManager.Instance.PlayerData.SecondsLeftForAnotherHeart;

        if (secondsThatPassedSinceLastLogin >= counter)
        {
            secondsThatPassedSinceLastLogin -= (int)counter;
            DataManager.Instance.PlayerData.ChangeHearts(1);
            counter = amountOfSecondsForNextHeart;
        }
        else
        {
            counter -= secondsThatPassedSinceLastLogin;
        }

        while (secondsThatPassedSinceLastLogin >= amountOfSecondsForNextHeart)
        {
            if (IsFull)
            {
                break;
            }
            secondsThatPassedSinceLastLogin -= (int)amountOfSecondsForNextHeart;
            DataManager.Instance.PlayerData.ChangeHearts(1);
            counter = amountOfSecondsForNextHeart;
        }

        counter = Mathf.Clamp(counter, 0, amountOfSecondsForNextHeart);
        DataManager.Instance.PlayerData.SecondsLeftForAnotherHeart = (int)counter;
    }

    private void Update()
    {
        if (!isInit || IsFull)
        {
            return;
        }

        counter -= Time.deltaTime;
        if (counter <= 0)
        {
            if (!IsFull)
            {
                AddOneHeart();
            }
            else
            {
                counter = 0;
            }
        }
    }

    public void Refill()
    {
        if (DataManager.Instance.PlayerData == null || DataManager.Instance.GameData == null)
        {
            // Debug.Log("DataManager or PlayerData/GameData is null.");
            return;
        }

        int heartsToAdd = DataManager.Instance.GameData.MaxAmountOfHearts - DataManager.Instance.PlayerData.Hearts;
        DataManager.Instance.PlayerData.ChangeHearts(heartsToAdd);
    }

    public void AddOneHeart()
    {
        DataManager.Instance.PlayerData.ChangeHearts(1);
        counter = amountOfSecondsForNextHeart;
    }
}
