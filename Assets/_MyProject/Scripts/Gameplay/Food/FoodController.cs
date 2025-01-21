using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static FirebaseRemoteConfigManager;

public class FoodController : MonoBehaviour
{
    public static Action<FoodController> OnReachedBorder;
    public static Action OnMelted;

    [HideInInspector] public bool CanBePunched = true;
    [SerializeField] private int minSpeed;
    [SerializeField] private int maxSpeed;
    [SerializeField] private int score;
    [SerializeField] private FoodType type;
    [SerializeField] private Explosion explosionPrefab;

    public bool fall;
    public bool variableFoodSpeed;
    public int speedDifference;
    protected int speed;
    public static float vector3SpeedMultiplier;
    protected float torque;

    public bool isInMouth;
    public FoodType Type => type;
    public int Score => score;

    protected PawButtonController pawButtonController;
    protected GamePlayUI gamePlayUI;

    private GeneralConfigData config;

    public virtual void Awake()
    {
        config = Instance.generalConfigData;
    }

    public virtual void Setup(bool _randomRotation = true)
    {
        if (FrozenChilliController.Instance.freezeChillies &&
            (type == FoodType.Chilli || type == FoodType.ChilliGreen || type == FoodType.ChilliBomb))
        {
            Destroy(gameObject);
        }

        if (_randomRotation)
        {
            transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
        }

        LoadRemoteConfig();

        SetFoodSpeed();

        torque = UnityEngine.Random.Range(-100, 101);
        fall = true;

        HandleTutorial();
    }

    public virtual void Start()
    {
        pawButtonController = FindAnyObjectByType<PawButtonController>();
        gamePlayUI = FindAnyObjectByType<GamePlayUI>();
    }

    protected virtual void Update()
    {
        if (!fall || this == null || GamePlayManager.isPaused)
        {
            return;
        }

        if (gameObject != null)
        {
            MoveFood();
        }
    }

    protected virtual void OnEnable()
    {
        PawButtonController.OnScratchThemAll += ScratchAllItems;
    }

    protected virtual void OnDisable()
    {
        PawButtonController.OnScratchThemAll -= ScratchAllItems;

        DOTween.Kill(this, true); // Kills tweens associated with this GameObject
    }

    private void LoadRemoteConfig()
    {
        variableFoodSpeed = config.variableFoodSpeed;
        minSpeed = config.foodMinSpeed;
        maxSpeed = config.foodMaxSpeed;
    }
    protected virtual void SetFoodSpeed()
    {
        switch (Type)
        {
            case FoodType.IceCream:
            case FoodType.IceCreamSpecial:
            case FoodType.IceCreamUnique:
            case FoodType.IceCreamGoal:
                speed = CalculateSpeed(0); // No speedDifference for IceCream
                break;

            case FoodType.Chilli:
            case FoodType.ChilliGreen:
            case FoodType.ChilliBomb:
                speed = CalculateSpeed(config.chilliSpeedDifference);
                break;

            case FoodType.IceCube:
                speed = CalculateSpeed(config.iceCubeSpeedDifference);
                break;

            case FoodType.Coin:
                speed = CalculateSpeed(config.coinSpeedDifference);
                break;

            case FoodType.RewardingIceCream:
                speed = CalculateSpeed(config.rewardingIceCreamSpeedDifference);
                break;

            default:
                Debug.LogError($"Unhandled FoodType: {Type}. Defaulting speed to maxSpeed.");
                speed = maxSpeed;
                break;
        }
    }

    private int CalculateSpeed(int speedDifference)
    {
        return variableFoodSpeed
            ? UnityEngine.Random.Range(minSpeed + speedDifference, maxSpeed + speedDifference)
            : maxSpeed + speedDifference;
    }


    protected virtual void HandleTutorial()
    {
        if (TutorialGameplay.Instance.holdTutorials) { return; }

        if (type == FoodType.IceCream && TutorialManager.Instance.iceCreamTutorialActive && PlayerPrefs.GetInt(TutorialManager.ICECREAM_TUTORIAL, -1) == -1)
        {
            StartShowIntructionRoutine("Munch ice creams to progress, unlock power-ups, and avoid ice cream floods!", TutorialManager.ICECREAM_TUTORIAL);
        }
        else if (type == FoodType.Chilli && TutorialManager.Instance.chilliTutorialActive && PlayerPrefs.GetInt(TutorialManager.CHILLI_TUTORIAL, -1) == -1)
        {
            StartShowIntructionRoutine("Avoid red chillies! They are too spicy for our hero!", TutorialManager.CHILLI_TUTORIAL);
        }
        else if (type == FoodType.ChilliGreen && TutorialManager.Instance.chilliGreenTutorialActive && PlayerPrefs.GetInt(TutorialManager.GREEN_CHILLI_TUTORIAL, -1) == -1)
        {
            StartShowIntructionRoutine("Watch out for green chillies! They're spicier and cause more damage!", TutorialManager.GREEN_CHILLI_TUTORIAL);
        }
        else if (type == FoodType.ChilliBomb && TutorialManager.Instance.chilliBombTutorialActive && PlayerPrefs.GetInt(TutorialManager.BOMB_CHILLI_TUTORIAL, -1) == -1 && !TutorialGameplay.Instance.calledChilliBombTutorial)
        {
            TutorialGameplay.Instance.calledChilliBombTutorial = true;
            StartShowIntructionRoutine("One bite of a bomb chili and it's game over for your hero!", TutorialManager.BOMB_CHILLI_TUTORIAL);
            TutorialGameplay.Instance.toggleFoodTutorial = true;
        }
        else if (type == FoodType.Coin && TutorialManager.Instance.coinTutorialActive && PlayerPrefs.GetInt(TutorialManager.COIN_TUTORIAL, -1) == -1)
        {
            StartShowIntructionRoutine("Collect coins and use them to buy items or skip ads!", TutorialManager.COIN_TUTORIAL);
            TutorialGameplay.Instance.toggleFoodTutorial = true;
        }
        else if (type == FoodType.RewardingIceCream && TutorialManager.Instance.rewardingIceCreamTutorialActive && PlayerPrefs.GetInt(TutorialManager.REWARDING_ICE_CREAM_TUTORIAL, -1) == -1)
        {
            // Show Tutorial without changing the spawn position;
            StartCoroutine(TutorialGameplay.Instance.ShowInstruction(gameObject, "Eat the reward ice cream for special boosts to help you complete the level!", TutorialManager.REWARDING_ICE_CREAM_TUTORIAL));
            TutorialGameplay.Instance.toggleFoodTutorial = true;
        }
    }

    protected void StartShowIntructionRoutine(string message, string tutorialKey)
    {
        transform.position = new Vector3(Screen.width / 2f, transform.position.y, transform.position.z);
        StartCoroutine(TutorialGameplay.Instance.ShowInstruction(gameObject, message, tutorialKey));
    }

    protected virtual void OnTriggerEnter2D(Collider2D _collision)
    {
        if (gameObject != null)
        {
            if (_collision.CompareTag("LeftPunch") && !AnnoyingBoss.Instance.punchFromRightSide && AnnoyingBoss.Instance.bossCanPunch)
            {
                if (CanBePunched)
                {
                    CanBePunched = false;
                    AnnoyingBoss.Instance.bossCanPunch = false;
                }

                AnnoyingBoss.Instance.CallPunchRoutine();
                StartCoroutine(PunchMoveFoodDelay(0.1f));
            }

            if (_collision.CompareTag("RightPunch") && AnnoyingBoss.Instance.punchFromRightSide && AnnoyingBoss.Instance.bossCanPunch)
            {
                if (CanBePunched)
                {
                    CanBePunched = false;
                    AnnoyingBoss.Instance.bossCanPunch = false;
                }

                AnnoyingBoss.Instance.CallPunchRoutine();
                StartCoroutine(PunchMoveFoodDelay(0.1f));
            }

            if (_collision.CompareTag("Border"))
            {
                HandleCollisionWithBorder();
            }

            if (_collision.CompareTag("Destroyer"))
            {
                Destroy(gameObject);
            }
        }
    }

    protected virtual void MoveFood()
    {
        if (this == null || transform == null) return;

        var _position = transform.position;
        _position = Vector3.MoveTowards(
            _position, isInMouth ? CharacterVisual.Instance.mouthMask.position : _position + Vector3.down * vector3SpeedMultiplier,
            speed * Time.deltaTime * Screen.height / 2000f);
        transform.position = _position;
    }

    protected virtual IEnumerator PunchMoveFoodDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        PunchMoveFood();
    }

    protected virtual void PunchMoveFood()
    {
        if (this == null || transform == null) return;

        AudioManager.Instance.Play(AudioManager.PUNCH_SOUND);

        // Determine direction based on punch side
        Vector3 direction = AnnoyingBoss.Instance.punchFromRightSide ? Vector3.left : Vector3.right;

        float punchDistanceMin = 400f;
        float punchDistanceMax = 700f;
        float punchDistance = UnityEngine.Random.Range(punchDistanceMin, punchDistanceMax); // Adjust distance as necessary for effect strength
        float punchDuration = 0.3f; // Duration of the punch effect

        // Calculate target position based on direction
        Vector3 targetPosition = transform.position + (direction * punchDistance);

        // Use DOTween to animate the punch movement
        transform.DOMove(targetPosition, punchDuration).SetEase(Ease.OutQuad).SetTarget(this);
    }

    protected virtual void ScratchAllItems()
    {
        FoodController _foodController = gameObject.GetComponent<FoodController>();

        if (_foodController == null)
        {
            return;
        }

        switch (Type)
        {
            case FoodType.IceCream:
                FoodIceCream _foodIceCream = _foodController as FoodIceCream;
                if (_foodIceCream != null && !GamePlayManager.levelModeOn)
                {
                    var _amountToAdd = _foodIceCream.Score * GamePlayManager.Instance.Multiplier * 2;
                    GamePlayManager.Instance.Score += _amountToAdd;
                    _foodIceCream.IceCreamValueDisplay(_amountToAdd);
                }
                break;

            case FoodType.IceCreamSpecial:
                FoodIceCreamSpecial _foodIceCreamSpecial = _foodController as FoodIceCreamSpecial;
                if (_foodIceCreamSpecial != null && !GamePlayManager.levelModeOn)
                {
                    var _amountToAdd = _foodIceCreamSpecial.Score * GamePlayManager.Instance.Multiplier * 2;
                    GamePlayManager.Instance.Score += _amountToAdd;
                    _foodIceCreamSpecial.IceCreamValueDisplay(_amountToAdd);
                }
                break;

            case FoodType.IceCreamUnique:
                FoodIceCreamUnique _foodIceCreamUnique = _foodController as FoodIceCreamUnique;
                if (_foodIceCreamUnique != null && !GamePlayManager.levelModeOn)
                {
                    var _amountToAdd = _foodIceCreamUnique.Score * GamePlayManager.Instance.Multiplier * 2;
                    GamePlayManager.Instance.Score += _amountToAdd;
                    _foodIceCreamUnique.IceCreamValueDisplay(_amountToAdd);
                }
                break;

            case FoodType.IceCreamGoal:
                FoodIceCreamGoal _foodIceCreamGoal = _foodController as FoodIceCreamGoal;
                if (_foodIceCreamGoal != null)
                {
                    var _amountToAdd = _foodIceCreamGoal.Score * GamePlayManager.Instance.Multiplier;
                    _foodIceCreamGoal.IceCreamValueDisplay(_amountToAdd);
                    gamePlayUI.UpdateGoalDisplay(_foodIceCreamGoal.goalSpawner, _amountToAdd);
                    LevelCompletedHandler.Instance.CheckForLevelCompletion();
                    PowerWords.Instance.ActivatePowerWord();
                    AudioManager.Instance.Play(AudioManager.ICE_CREAM_GOAL_SOUND);
                }
                break;

            case FoodType.Coin:
                FoodCoin _foodCoin = _foodController as FoodCoin;
                int _amountOfCoins = _foodController.Score * GamePlayManager.Instance.Multiplier * 2;
                DataManager.Instance.PlayerData.Coins += _amountOfCoins;
                _foodCoin.CoinValueDisplay(_amountOfCoins);
                AudioManager.Instance.Play(AudioManager.COIN_COLLECT_SOUND);
                break;

            case FoodType.RewardingIceCream:
                break;

            case FoodType.Chilli:
                FoodChillies _foodChillies = _foodController as FoodChillies;
                _foodChillies.ExplodeThisFood();
                break;

            case FoodType.ChilliGreen:
                FoodChilliGreen _foodChilliGreen = _foodController as FoodChilliGreen;
                _foodChilliGreen.ExplodeThisFood();
                break;

            case FoodType.ChilliBomb:
                FoodChilliBomb _foodChilliBomb = _foodController as FoodChilliBomb;
                _foodChilliBomb.ExplodeThisFood();
                break;

            case FoodType.IceCube:
                FoodIceCube _foodIceCube = _foodController as FoodIceCube;
                MyCharacterController.OnEatenIceCube?.Invoke();
                _foodIceCube.ExplodeThisFood();
                break;

            default:
                throw new Exception("Don`t know how to scratch: " + _foodController.Type);
        }

        if (Type != FoodType.RewardingIceCream)
        {
            if (!_foodController.isInMouth)
                Destroy(gameObject);
        }
    }

    protected virtual void ExplodeThisFood()
    {
        if (explosionPrefab != null)
        {
            Explosion _explosion = Instantiate(explosionPrefab, GameObject.FindGameObjectWithTag("FoodHolder").transform);
            _explosion.transform.localPosition = transform.localPosition;
        }
    }

    protected virtual bool AllowAutomaticEating()
    {
        return true;
    }

    protected virtual void HandleCollisionWithBorder()
    {
        OnReachedBorder?.Invoke(this);

        if (Type == FoodType.Chilli || Type == FoodType.Coin || Type == FoodType.ChilliGreen || Type == FoodType.ChilliBomb || Type == FoodType.IceCube)
        {
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected IEnumerator Melt(IceCreamSO _iceCream)
    {
        if (_iceCream == null)
        {
            Debug.Log("Ice Cream became null. breaking Melt coroutine now...");
            yield break;
        }

        if (GamePlayManager.destroyMeltedIceCreams)
        {
            Destroy(gameObject);
            yield break;
        }

        if (Type == FoodType.IceCream || Type == FoodType.IceCreamSpecial || Type == FoodType.IceCreamUnique || Type == FoodType.IceCreamGoal)
        {
            OnMelted?.Invoke();
        }

        fall = false;
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<BoxCollider2D>());

        Image _image = GetComponent<Image>();
        if (_image == null) yield break; // Ensure _image exists

        _image.sprite = _iceCream.SemiMelted;

        // Wait 1 second (real time), pausing if the game is paused
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            while (GamePlayManager.isPaused) 
            {
                if (GamePlayManager.destroyMeltedIceCreams)
                {
                    Destroy(gameObject); // Destroy melted ice cream if necessary;
                    yield break;
                }

                yield return null; // Wait if the game is paused
            } 
            
            yield return null;
        }

        _image.sprite = _iceCream.Melted;

        // Wait another 1 second (real time), pausing if the game is paused
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            while (GamePlayManager.isPaused)
            {
                if (GamePlayManager.destroyMeltedIceCreams)
                {
                    Destroy(gameObject); // Destroy melted ice cream if necessary;
                    yield break;
                }

                yield return null; // Wait if the game is paused
            }

            yield return null;
        }

        if (_image == null) yield break; // Ensure _image still exists

        Color _color = _image.color;
        float _duration = 3; // Duration of fade-out

        // DOTween sequence to fade out the image
        Sequence _sequence = DOTween.Sequence();
        _sequence.Append(DOTween.To(() => _color.a, _x => _color.a = _x, 0, _duration).OnUpdate(() =>
        {
            if (_image != null)
            {
                _image.color = _color;
            }
            else
            {
                _sequence.Kill(); // Stop sequence if _image is missing
            }
        }));
        _sequence.OnComplete(() =>
        {
            if (_image != null)
            {
                Destroy(gameObject);
            }
        });
        _sequence.Play();
    }
}
