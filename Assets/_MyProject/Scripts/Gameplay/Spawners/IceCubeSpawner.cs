using UnityEngine;

public class IceCubeSpawner : MonoBehaviour
{
    public static IceCubeSpawner Instance;

    [SerializeField] private FoodController prefab;
    [SerializeField] private Transform foodHolder;

    [SerializeField] private float minCooldown;
    [SerializeField] private float maxCooldown;

    private float counter;

    public bool spawnIceCube;

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
        spawnIceCube = false;
    }

    private void Update()
    {
        if (GamePlayManager.currentLevel > 2 || !GamePlayManager.levelModeOn)
        {
            counter -= Time.deltaTime;

            if (counter <= 0)
            {
                spawnIceCube = true;
                counter = Random.Range(minCooldown, maxCooldown);
            }
        }
    }

    private void SetCooldownRange()
    {
        var configData = FirebaseRemoteConfigManager.Instance.generalConfigData;
        minCooldown = configData.iceCubeMinCooldown;
        maxCooldown = configData.iceCubeMaxCooldown;
    }
}
