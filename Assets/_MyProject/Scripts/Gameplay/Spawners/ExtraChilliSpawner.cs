using UnityEngine;

public class ExtraChilliSpawner : MonoBehaviour
{
    public static ExtraChilliSpawner Instance;

    [SerializeField] private FoodController[] extraChilliPrefabs;
    [SerializeField] private Transform foodHolder;

    [SerializeField] private float minCooldown;
    [SerializeField] private float maxCooldown;

    private float counter;

    public bool holdExtraChilliSpawn;

    private bool firstDrop;
    private bool secondDrop;

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
        if (GamePlayManager.currentLevel < FirebaseRemoteConfigManager.Instance.generalConfigData.activateExtraChilliSpawnerAtLevel || GamePlayManager.Instance.bossLevel)
        {
            enabled = false;
        }
        else
        {
            SetCooldownRange();

            counter = minCooldown;
            holdExtraChilliSpawn = false;

            if (GamePlayManager.currentLevel == 5)
            {
                firstDrop = true;
                secondDrop = false;
            }
        }
    }

    private void Update()
    {
        counter -= Time.deltaTime;

        if (counter <= 0 && !holdExtraChilliSpawn)
        {
            counter = Random.Range(minCooldown, maxCooldown);

            FoodController _foodController;

            if (GamePlayManager.currentLevel >= 3 && GamePlayManager.currentLevel <= 4) //This condition sets only the green chilli available for levels 3 and 4. Bomb chilli stays inactive.
            {
                _foodController = Instantiate(extraChilliPrefabs[1], foodHolder);
            }
            else if (GamePlayManager.currentLevel == 5) //For tutorial purposes, at level 5, this condition makes sure the bomb chilli will start at the second drop and be available from there.
            {
                if (firstDrop)
                {
                    firstDrop = false;
                    secondDrop = true;

                    _foodController = Instantiate(extraChilliPrefabs[1], foodHolder);
                }
                else if (secondDrop)
                {
                    secondDrop = false;
                    _foodController = Instantiate(extraChilliPrefabs[2], foodHolder);
                }
                else
                {
                    int randomNumber = Random.Range(0, 3);

                    if (randomNumber <= 1)
                    {
                        _foodController = Instantiate(extraChilliPrefabs[1], foodHolder);
                    }
                    else
                    {
                        _foodController = Instantiate(extraChilliPrefabs[2], foodHolder);
                    }

                }
            }
            else
            {
                if (!GamePlayManager.Instance.useBombRainEffect) //If bomb rain isn't active, there's a 66% of chance for Green Chilli to drop against 33% of chance for Bomb Chilli.
                {
                    int randomNumber = Random.Range(0, 3);

                    if (randomNumber <= 1)
                    {
                        _foodController = Instantiate(extraChilliPrefabs[1], foodHolder);
                    }
                    else
                    {
                        _foodController = Instantiate(extraChilliPrefabs[2], foodHolder);
                    }
                }
                else //If bomb rain is active, there's a 66% of chance for Red Chilli to drop against 33% of chance for Green Chilli.
                {
                    int randomNumber = Random.Range(0, 3);

                    if (randomNumber <= 1)
                    {
                        _foodController = Instantiate(extraChilliPrefabs[0], foodHolder);
                    }
                    else
                    {
                        _foodController = Instantiate(extraChilliPrefabs[1], foodHolder);
                    }
                }
            }

            Vector3 _spawnPosition = new Vector3();
            _spawnPosition.x = Random.Range(Screen.width / 12f, Screen.width - Screen.width / 12f);
            _spawnPosition.y = Screen.height + (Screen.height * 0.05f);
            _foodController.transform.position = _spawnPosition;

            _foodController.Setup();
        }
    }

    private void SetCooldownRange()
    {
        var configData = FirebaseRemoteConfigManager.Instance.generalConfigData;
        minCooldown = configData.extraChilliMinCooldown;
        maxCooldown = configData.extraChilliMaxCooldown;
    }
}
