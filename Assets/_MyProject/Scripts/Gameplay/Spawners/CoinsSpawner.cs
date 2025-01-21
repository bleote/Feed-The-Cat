using UnityEngine;

public class CoinsSpawner : MonoBehaviour
{
    public static CoinsSpawner Instance;

    [SerializeField] private FoodController prefab;
    [SerializeField] private Transform foodHolder;

    [SerializeField] private float minCooldown;
    [SerializeField] private float maxCooldown;

    private float counter;

    public bool spawnCoin;

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
        SetCooldownRange();

        counter = minCooldown;
        spawnCoin = false;
    }

    private void Update()
    {
        if (!GamePlayManager.levelModeOn)
        {
            counter -= Time.deltaTime;

            if (counter <= 0)
            {
                spawnCoin = true;
                counter = Random.Range(minCooldown, maxCooldown);
            }
        }
    }

    private void SetCooldownRange()
    {
        var configData = FirebaseRemoteConfigManager.Instance.generalConfigData;
        minCooldown = configData.coinMinCooldown;
        maxCooldown = configData.coinMaxCooldown;
    }
}
