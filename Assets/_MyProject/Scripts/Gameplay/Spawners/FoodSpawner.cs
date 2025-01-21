using System.Collections;
using UnityEngine;
using static FirebaseRemoteConfigManager;

public class FoodSpawner : MonoBehaviour
{
    public static FoodSpawner Instance;

    [SerializeField] private Transform foodHolder;
    public Transform FoodHolder => foodHolder;

    [Header("Prefabs")]
    [SerializeField] private FoodChillies foodChilliPrefab;
    [SerializeField] private FoodIceCream foodIceCreamRelaxPrefab;
    [SerializeField] private FoodIceCreamSpecial foodIceCreamSpecialPrefab;
    [SerializeField] private FoodIceCreamUnique foodIceCreamUniquePrefab;
    [SerializeField] private FoodIceCream foodIceCreamLevelPrefab;
    [SerializeField] private FoodChilliBomb foodChilliBombPrefab;
    [SerializeField] private FoodIceCube foodIceCubePrefab;
    [SerializeField] private FoodCoin foodCoinPrefab;
    private FoodController foodPrefab;

    [Header("Spawn Regular Ice Cream Config")]
    [SerializeField] private float regularIceCreamSpawnTimer;
    public bool holdRegularSpawn;
    public int totalSpawns;

    [Header("Spawn Chilli Config")]
    [SerializeField] private int spawnChilliAfterMin;
    [SerializeField] private int spawnChilliAfterMax;
    [SerializeField] private int spawnChilliAt;
    public int maxChilliGroupSize = 2;
    public int chilliSpawnCount;

    [Header("Spawn Goal Ice Cream Config")]
    [SerializeField] private int spawnGoalAt;
    [SerializeField] private int spawnGoalAtMin;
    [SerializeField] private int spawnGoalAtMax;
    public int goalSpawnCount;
    public bool holdGoalSpawn;

    [Header("Spawn Special Ice Cream Config")]
    [SerializeField] private int spawnSpecialIceCreamAfterMin;
    [SerializeField] private int spawnSpecialIceCreamAfterMax;
    [SerializeField] private float specialIceCreamSpawnTimer;
    public bool holdSpecialSpawn;

    [Header("Spawn Unique Ice Cream Config")]
    [SerializeField] private int spawnUniqueIceCreamAfterMin;
    [SerializeField] private int spawnUniqueIceCreamAfterMax;
    [SerializeField] private float uniqueIceCreamSpawnTimer;
    public bool holdUniqueSpawn;

    [Header("Spawn Cool Down Evolution")]
    [SerializeField] private int reduceSpawnCooldownTarget;
    [SerializeField] private float spawnCooldownReducer;
    [SerializeField] private float minSpawnCooldown;
    public float foodSpawnCooldown;

    [Header("Spawn Timers")]

    // Waterfall effect variables
    [Header("Waterfall")]
    [SerializeField] private float waterfallCoolDown;
    private int currentPositionIndex = 0;
    private Vector3[] spawnPositions;
    private float spawnPosY;
    private float minScreenLimitX;
    private float maxScreenLimitX;
    private float step;
    public bool waterfallOn = false;
    public float waterfallTimer;
    private float coolDownBeforeWaterfall;
    private float waterfallFoodSpawnCoolDown;

    //Rain Bomb effect variables
    [Header("Bomb Rain")]
    [SerializeField] private int spawnChilliBombAt;
    [SerializeField] private int spawnBombLineAt;
    public int bombLineCount;
    public bool bombLineOn = false;

    //Boss Bomb effect variables
    public int bossBombLineCount;
    public bool bossBombLineOn = false;

    private GeneralConfigData configData;

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
        configData = FirebaseRemoteConfigManager.Instance.generalConfigData;

        holdRegularSpawn = false;

        FoodController.vector3SpeedMultiplier = configData.foodVector3SpeedMultiplier;

        SetInitialSpawnStructure();

        if (GamePlayManager.currentLevel == 2)
        {
            spawnChilliAt += 5;
        }

        SetScreenReferencesForSpawners();

        if (GamePlayManager.Instance.useWaterfallEffect)
        {
            waterfallTimer = waterfallCoolDown;
        }

        if (GamePlayManager.Instance.useBombRainEffect || GamePlayManager.Instance.bossLevel)
        {
            SetBombLineSpawnPositions();
        }
    }

    private void Update()
    {
        UpdateSpawnTimers();

        SpawnIceCreams();
    }

    private void UpdateSpawnTimers()
    {
        if (!holdRegularSpawn)
        {
            regularIceCreamSpawnTimer -= Time.deltaTime;

            if (GamePlayManager.Instance.useWaterfallEffect)
            {
                waterfallTimer -= Time.deltaTime;
            }
        }

        if (!GamePlayManager.levelModeOn)
        {
            if (!holdSpecialSpawn)
            {
                specialIceCreamSpawnTimer -= Time.deltaTime;
            }

            if (!holdUniqueSpawn)
            {
                uniqueIceCreamSpawnTimer -= Time.deltaTime;
            }
        }
    }

    private void SpawnIceCreams()
    {
        if (waterfallTimer <= 0)
        {
            ToggleWaterfallEffect();
        }

        if (regularIceCreamSpawnTimer <= 0 && !holdRegularSpawn)
        {
            HandleRegularSpawn();
        }

        if (!GamePlayManager.levelModeOn)
        {
            if (specialIceCreamSpawnTimer <= 0 && !holdSpecialSpawn)
            {
                HandleSpecialSpawn();
            }

            if (uniqueIceCreamSpawnTimer <= 0 && !holdUniqueSpawn)
            {
                HandleUniqueSpawn();
            }
        }
    }

    private void HandleRegularSpawn()
    {
        regularIceCreamSpawnTimer = foodSpawnCooldown;

        // Determine the correct prefab to spawn
        if (!GamePlayManager.levelModeOn)
        {
            BasicIceCreamAndChilliSpawns();
        }
        else
        {
            if (!GamePlayManager.Instance.useWaterfallEffect && !GamePlayManager.Instance.useBombRainEffect && !GamePlayManager.Instance.bossLevel)
            {
                RegularGoalSpawns();
            }

            if (GamePlayManager.Instance.useWaterfallEffect)
            {
                LeveModeWaterfallSpawns();
            }

            if (GamePlayManager.Instance.useBombRainEffect)
            {
                LevelModeBombRainSpawns();
            }

            if (GamePlayManager.Instance.bossLevel)
            {
                LevelModeBossSpawns();
            }
        }

        // Only spawn food if a foodPrefab is set
        if (foodPrefab != null)
        {
            SpawnFood(foodPrefab);
        }

        totalSpawns++;

        if (totalSpawns % reduceSpawnCooldownTarget == 0)
        {
            AdjustSpawnCooldown();
        }
    }

    private void AdjustSpawnCooldown()
    {
        totalSpawns = 0;
        foodSpawnCooldown -= spawnCooldownReducer;

        if (maxChilliGroupSize < 5)
        {
            maxChilliGroupSize++;
        }

        if (foodSpawnCooldown < minSpawnCooldown)
        {
            foodSpawnCooldown = minSpawnCooldown;
            FoodController.vector3SpeedMultiplier += configData.foodVector3SpeedMultiplierIncrease;
        }
    }

    private void HandleSpecialSpawn()
    {
        holdRegularSpawn = true;
        GoalSpawnerHandler.Instance.holdGoalSpawn = true;
        holdUniqueSpawn = true;
        specialIceCreamSpawnTimer = Random.Range(spawnSpecialIceCreamAfterMin, spawnSpecialIceCreamAfterMax);
        SpawnFood(foodIceCreamSpecialPrefab);
        StartCoroutine(SwitchSpawnsOn());
    }

    private void HandleUniqueSpawn()
    {
        holdRegularSpawn = true;
        GoalSpawnerHandler.Instance.holdGoalSpawn = true;
        holdSpecialSpawn = true;
        uniqueIceCreamSpawnTimer = Random.Range(spawnUniqueIceCreamAfterMin, spawnUniqueIceCreamAfterMax);
        SpawnFood(foodIceCreamUniquePrefab);
        StartCoroutine(SwitchSpawnsOn());
    }

    private void BasicIceCreamAndChilliSpawns()
    {
        // Select the appropriate food prefab based on the game mode
        foodPrefab = GamePlayManager.levelModeOn ? foodIceCreamLevelPrefab : foodIceCreamRelaxPrefab;

        if (chilliSpawnCount == spawnChilliAt)
        {
            chilliSpawnCount = 0;
            foodPrefab = foodChilliPrefab;
            spawnChilliAt = Random.Range(spawnChilliAfterMin, spawnChilliAfterMax);
        }
        else
        {
            chilliSpawnCount++;

            CheckForIceCubeOrCoinSpawn();
        }
    }

    private void RegularGoalSpawns()
    {
        if (goalSpawnCount == spawnGoalAt)
        {
            goalSpawnCount = 0;
            spawnGoalAt = Random.Range(spawnGoalAtMin, spawnGoalAtMax);
            GoalSpawnerHandler.Instance.GoalSpawnTrigger();
            foodPrefab = null; // No food spawn when Goal is triggered

            if (chilliSpawnCount == spawnChilliAt)
            {
                chilliSpawnCount--; // to avoid goalSpawnCount and chilliSpawnCount overlaps
            }
        }
        else
        {
            goalSpawnCount++;

            if (GamePlayManager.currentLevel > 1)
            {
                BasicIceCreamAndChilliSpawns();
            }
            else
            {
                chilliSpawnCount++;

                if (chilliSpawnCount > spawnChilliAt)
                {
                    chilliSpawnCount = spawnChilliAt; // Ensure it does not exceed spawnChilliAt
                }

                foodPrefab = foodIceCreamLevelPrefab;

                CheckForIceCubeOrCoinSpawn();
            }
        }
    }

    private void LeveModeWaterfallSpawns()
    {
        if (chilliSpawnCount == spawnChilliAt)
        {
            chilliSpawnCount = 0;

            if (waterfallOn)
            {
                SingleRedChilliSpawn(foodChilliPrefab); // Switch on single Chilli Spawn
                foodPrefab = null; // Switch off normal Chilli Spawn
            }
            else
            {
                foodPrefab = foodChilliPrefab;
            }

            spawnChilliAt = Random.Range(spawnChilliAfterMin, spawnChilliAfterMax);

            // Increment goalSpawnCount but check for overlap
            goalSpawnCount++;
            if (goalSpawnCount >= spawnGoalAt)
            {
                goalSpawnCount = spawnGoalAt; // Ensure it does not exceed spawnGoalAt
            }
        }
        else
        {
            chilliSpawnCount++;

            if (goalSpawnCount == spawnGoalAt)
            {
                goalSpawnCount = 0;
                spawnGoalAt = Random.Range(spawnGoalAtMin, spawnGoalAtMax);
                GoalSpawnerHandler.Instance.GoalSpawnTrigger();
                foodPrefab = null; // No food spawn when Goal is triggered
            }
            else
            {
                goalSpawnCount++;

                if (goalSpawnCount > spawnGoalAt)
                {
                    goalSpawnCount = spawnGoalAt; // Ensure it does not exceed spawnGoalAt
                }

                foodPrefab = foodIceCreamLevelPrefab;

                CheckForIceCubeOrCoinSpawn();
            }
        }
    }

    private void LevelModeBombRainSpawns()
    {
        if (chilliSpawnCount == spawnChilliBombAt)
        {
            chilliSpawnCount = 0;

            if (bombLineOn)
            {
                bombLineOn = false;
                BombRainLineSpawn(foodChilliBombPrefab); // Bomb Line Spawn
                foodPrefab = null; // Switch off normal Bomb Spawn
            }
            else
            {
                foodPrefab = foodChilliBombPrefab;
                bombLineCount++;

                if (bombLineCount == spawnBombLineAt)
                {
                    bombLineOn = true;
                    bombLineCount = 0;
                }
            }

            // Increment goalSpawnCount but check for overlap
            goalSpawnCount++;

        }
        else
        {
            chilliSpawnCount++;

            if (goalSpawnCount >= spawnGoalAt)
            {
                goalSpawnCount = 0;
                spawnGoalAt = Random.Range(spawnGoalAtMin - 1, spawnGoalAtMax - 1);
                GoalSpawnerHandler.Instance.GoalSpawnTrigger();
                foodPrefab = null; // No food spawn when Goal is triggered
            }
            else
            {
                goalSpawnCount++;

                foodPrefab = foodIceCreamLevelPrefab;

                CheckForIceCubeOrCoinSpawn();
            }
        }
    }

    private void LevelModeBossSpawns()
    {
        if (bossBombLineOn && bossBombLineCount == 0 || bossBombLineOn && bossBombLineCount == 4)
        {
            BombRainLineSpawn(foodChilliBombPrefab); // Bomb Line Spawn
            foodPrefab = null; // Switch off normal Bomb Spawn
            bossBombLineCount++;
            goalSpawnCount++;
        }
        else if (bossBombLineOn && bossBombLineCount == 1 || bossBombLineOn && bossBombLineCount == 3 || bossBombLineOn && bossBombLineCount == 5 || bossBombLineOn && bossBombLineCount == 7)
        {
            SingleBombSpawn(foodChilliBombPrefab); // Bomb Line Spawn
            foodPrefab = null; // Switch off normal Bomb Spawn
            bossBombLineCount++;
            goalSpawnCount++;
        }
        else if (bossBombLineOn && bossBombLineCount == 8)
        {
            bossBombLineOn = false;
            bossBombLineCount = 0;
            BombRainLineSpawn(foodChilliBombPrefab); // Bomb Line Spawn
            foodPrefab = null; // Switch off normal Bomb Spawn
            goalSpawnCount++;
        }
        else if (bossBombLineOn && bossBombLineCount == 2 || bossBombLineOn && bossBombLineCount == 6)
        {
            bossBombLineCount++;
            goalSpawnCount++;
            foodPrefab = foodChilliBombPrefab;
        }
        else
        {
            if (goalSpawnCount >= spawnGoalAt && !bombLineOn)
            {
                goalSpawnCount = 0;
                spawnGoalAt = Random.Range(spawnGoalAtMin, spawnGoalAtMax);
                GoalSpawnerHandler.Instance.GoalSpawnTrigger();
                foodPrefab = null; // No food spawn when Goal is triggered
            }
            else
            {
                goalSpawnCount++;

                foodPrefab = foodChilliBombPrefab;
            }
        }
    }

    private void CheckForIceCubeOrCoinSpawn()
    {
        if (CoinsSpawner.Instance.spawnCoin)
        {
            CoinsSpawner.Instance.spawnCoin = false;
            foodPrefab = foodCoinPrefab;
        }
        else if (IceCubeSpawner.Instance.spawnIceCube)
        {
            IceCubeSpawner.Instance.spawnIceCube = false;
            foodPrefab = foodIceCubePrefab;
        }
    }

    public void SpawnFood(FoodController foodPrefab)
    {
        FoodController _foodController = Instantiate(foodPrefab, foodHolder);
        Vector3 _spawnPosition;

        if (waterfallOn)
        {
            _spawnPosition = spawnPositions[currentPositionIndex];
            currentPositionIndex = (currentPositionIndex + 1) % spawnPositions.Length;
        }
        else
        {
            // Use random position
            _spawnPosition = new Vector3
            {
                x = Random.Range(minScreenLimitX, maxScreenLimitX),
                y = spawnPosY
            };
        }

        _foodController.transform.position = _spawnPosition;
        _foodController.Setup();
    }

    private void SingleRedChilliSpawn(FoodChillies foodChilliPrefab)
    {
        FoodChillies _newChilli = Instantiate(foodChilliPrefab, foodHolder);
        Vector3 _spawnPosition;

        if (waterfallOn)
        {
            _spawnPosition = spawnPositions[currentPositionIndex];
            currentPositionIndex = (currentPositionIndex + 1) % spawnPositions.Length;
        }
        else
        {
            // Use random position
            _spawnPosition = new Vector3
            {
                x = Random.Range(minScreenLimitX, maxScreenLimitX),
                y = spawnPosY
            };
        }

        _newChilli.transform.position = _spawnPosition;
        _newChilli.CanSplit = false;
        _newChilli.Setup();
    }

    private void BombRainLineSpawn(FoodChilliBomb foodChilliBombPrefab)
    {
        // Determine the position to leave empty
        int skipPositionIndex = Random.Range(0, spawnPositions.Length);

        // Loop through all positions
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            if (i != skipPositionIndex)
            {
                FoodChilliBomb _newChilliBomb = Instantiate(foodChilliBombPrefab, foodHolder);
                Vector3 _spawnPosition = spawnPositions[i];
                _newChilliBomb.transform.position = _spawnPosition;
                _newChilliBomb.CanSplit = false;
                _newChilliBomb.Setup();
            }
        }
    }

    private void SingleBombSpawn(FoodChilliBomb foodChilliBombPrefab)
    {
        FoodChilliBomb _newChilliBomb = Instantiate(foodChilliBombPrefab, foodHolder);
        Vector3 _spawnPosition;

        // Use random position
        _spawnPosition = new Vector3
        {
            x = Random.Range(minScreenLimitX, maxScreenLimitX),
            y = spawnPosY
        };

        _newChilliBomb.transform.position = _spawnPosition;
        _newChilliBomb.CanSplit = false;
        _newChilliBomb.Setup();
    }

    private IEnumerator SwitchSpawnsOn()
    {
        yield return new WaitForSeconds(0.5f);

        holdRegularSpawn = false;
        GoalSpawnerHandler.Instance.holdGoalSpawn = false;

        yield return new WaitForSeconds(0.8f);

        if (holdUniqueSpawn)
        {
            holdUniqueSpawn = false;
        }

        if (holdSpecialSpawn)
        {
            holdSpecialSpawn = false;
        }
    }

    public void HoldSpawnersCoroutine()
    {
        StartCoroutine(HoldSpawnersForGoal());
    }

    private IEnumerator HoldSpawnersForGoal()
    {
        holdRegularSpawn = true;

        yield return new WaitForSeconds(foodSpawnCooldown);

        holdRegularSpawn = false;
    }

    private void SetInitialSpawnStructure()
    {        
        spawnCooldownReducer = configData.spawnCooldownReducer;
        minSpawnCooldown = configData.minSpawnCooldown;

        if (!GamePlayManager.levelModeOn)
        {
            foodSpawnCooldown = configData.foodSpawnCooldownRelax;
            reduceSpawnCooldownTarget = configData.reduceSpawnCooldownTargetRelax;
        }
        else
        {
            switch (GamePlayManager.currentLevel)
            {
                //Intro Levels
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    foodSpawnCooldown = configData.foodSpawnCooldownIntro;
                    reduceSpawnCooldownTarget = configData.reduceSpawnCooldownTargetIntro;
                    spawnGoalAtMin = configData.spawnGoalAtMinIntro;
                    spawnGoalAtMax = configData.spawnGoalAtMaxIntro;
                    waterfallFoodSpawnCoolDown = configData.waterfallFoodSpawnCoolDownIntro;
                    break;

                // Easy Levels
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                    foodSpawnCooldown = configData.foodSpawnCooldownEasy;
                    reduceSpawnCooldownTarget = configData.reduceSpawnCooldownTargetEasy;
                    spawnGoalAtMin = configData.spawnGoalAtMinEasy;
                    spawnGoalAtMax = configData.spawnGoalAtMaxEasy;
                    waterfallFoodSpawnCoolDown = configData.waterfallFoodSpawnCoolDownEasy;
                    break;

                // Medium Levels
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                    foodSpawnCooldown = configData.foodSpawnCooldownMedium;
                    reduceSpawnCooldownTarget = configData.reduceSpawnCooldownTargetMedium;
                    spawnGoalAtMin = configData.spawnGoalAtMinMedium;
                    spawnGoalAtMax = configData.spawnGoalAtMaxMedium;
                    waterfallFoodSpawnCoolDown = configData.waterfallFoodSpawnCoolDownMedium;
                    break;

                // Hard Levels
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                    foodSpawnCooldown = configData.foodSpawnCooldownHard;
                    reduceSpawnCooldownTarget = configData.reduceSpawnCooldownTargetHard;
                    spawnGoalAtMin = configData.spawnGoalAtMinHard;
                    spawnGoalAtMax = configData.spawnGoalAtMaxHard;
                    waterfallFoodSpawnCoolDown = configData.waterfallFoodSpawnCoolDownHard;
                    break;

                case 46:
                    foodSpawnCooldown = configData.foodSpawnCooldownBoss;
                    reduceSpawnCooldownTarget = configData.reduceSpawnCooldownTargetBoss;
                    spawnGoalAtMin = configData.spawnGoalAtMinBoss;
                    spawnGoalAtMax = configData.spawnGoalAtMaxBoss;
                    waterfallFoodSpawnCoolDown = configData.waterfallFoodSpawnCoolDownBoss;
                    break;

                default:
                    foodSpawnCooldown = configData.foodSpawnCooldownIntro;
                    reduceSpawnCooldownTarget = configData.reduceSpawnCooldownTargetIntro;
                    spawnGoalAtMin = configData.spawnGoalAtMinIntro;
                    spawnGoalAtMax = configData.spawnGoalAtMaxIntro;
                    waterfallFoodSpawnCoolDown = configData.waterfallFoodSpawnCoolDownIntro;
                    break;
            }
        }
    }

    private void SetScreenReferencesForSpawners()
    {
        spawnPosY = Screen.height + (Screen.height * 0.1f);
        minScreenLimitX = Screen.width / 12f;
        maxScreenLimitX = Screen.width - Screen.width / 12f;
        step = (maxScreenLimitX - minScreenLimitX) / 5f; // Dividing the range into 5 equal steps
    }

    private void SetBombLineSpawnPositions()
    {
        spawnPositions = new Vector3[6];
        for (int i = 0; i < 6; i++)
        {
            spawnPositions[i] = new Vector3(minScreenLimitX + i * step, spawnPosY, 0);
        }
    }

    private void ActivateWaterfallEffect()
    {
        waterfallOn = true;
        waterfallTimer = waterfallCoolDown;
        AdjustSpawnCoolDownForWaterfall();

        spawnPositions = new Vector3[10];
        for (int i = 0; i < 6; i++)
        {
            spawnPositions[i] = new Vector3(minScreenLimitX + i * step, spawnPosY, 0);
        }

        // Manually set the positions for the reverse sequence
        spawnPositions[6] = spawnPositions[4];
        spawnPositions[7] = spawnPositions[3];
        spawnPositions[8] = spawnPositions[2];
        spawnPositions[9] = spawnPositions[1];
    }

    private void DeactivateWaterfallEffect()
    {
        waterfallOn = false;
        waterfallTimer = waterfallCoolDown;
        AdjustSpawnCoolDownAfterWaterfall();
    }

    private void ToggleWaterfallEffect()
    {
        if (waterfallOn)
        {
            DeactivateWaterfallEffect();
        }
        else
        {
            ActivateWaterfallEffect();
        }
    }

    private void AdjustSpawnCoolDownForWaterfall()
    {
        coolDownBeforeWaterfall = foodSpawnCooldown;
        foodSpawnCooldown = waterfallFoodSpawnCoolDown;

    }

    private void AdjustSpawnCoolDownAfterWaterfall()
    {
        foodSpawnCooldown = coolDownBeforeWaterfall;
    }
}
