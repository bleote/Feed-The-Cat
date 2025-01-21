using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FoodChillies : FoodController
{
    [HideInInspector] public bool CanSplit = true;
    [SerializeField] private float splitOffsetMin;
    [SerializeField] private float splitOffsetMax;
    [SerializeField] protected Sprite[] chilliSprites;
    [SerializeField] protected Image image;
    [SerializeField] protected GameObject iceExplosionPrefab;
    [SerializeField] protected Transform foodHolder;
    [SerializeField] private ValueDisplay valueDisplayPrefab;
    private float splitOffset;
    public int damageValue;

    public bool isFrozen;

    Coroutine shakeCoroutine;

    protected override void OnEnable()
    {
        base.OnEnable();

        FrozenChilliController.OnUnfrozen += InitiateUnfreezeRoutine;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        FrozenChilliController.OnUnfrozen -= InitiateUnfreezeRoutine;

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }

        DOTween.Kill(this, true); // Stop all DOTween animations specific to this object
    }

    protected override void OnTriggerEnter2D(Collider2D _collision)
    {
        if (gameObject != null)
        {
            if (_collision.CompareTag("Split"))
            {
                if (!CanSplit)
                {
                    return;
                }

                CanSplit = false;

                if (GamePlayManager.Instance.useBombRainEffect)
                {
                    SplitChilli(AdjustedSplitChilliForBombRain());
                }
                else if (GamePlayManager.Instance.bossLevel)
                {
                    SplitChilli(3);
                }
                else
                {
                    SplitChilli(FoodSpawner.Instance.maxChilliGroupSize + 1);
                }
            }

            if (_collision.CompareTag("Freeze"))
            {
                fall = false;
                isFrozen = true;

                FrozenChilliSpriteSwitch();
            }

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

    protected override IEnumerator PunchMoveFoodDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        PunchMoveFood();
    }

    protected override void PunchMoveFood()
    {
        if (this == null || transform == null) return;

        AudioManager.Instance.Play(AudioManager.PUNCH_SOUND);

        if (isFrozen)
        {
            UnfreezeThis();
        }

        // Determine direction based on punch side
        Vector3 direction = AnnoyingBoss.Instance.punchFromRightSide ? Vector3.left : Vector3.right;

        float punchDistanceMin = 400f;
        float punchDistanceMax = 800f;
        float punchDistance = UnityEngine.Random.Range(punchDistanceMin, punchDistanceMax); // Adjust distance as necessary for effect strength
        float punchDuration = 0.3f; // Duration of the punch effect

        // Calculate target position based on direction
        Vector3 targetPosition = transform.position + (direction * punchDistance);

        // Use DOTween to animate the punch movement
        transform.DOMove(targetPosition, punchDuration).SetEase(Ease.OutQuad).SetTarget(this);
    }

    protected virtual void SplitChilli(int maxChilliGroupSize)
    {
        int chilliGroupSize = Random.Range(1, maxChilliGroupSize);
        RectTransform rectTransform = GetComponent<RectTransform>();
        float direction = rectTransform.anchoredPosition.x;

        if (chilliGroupSize == 1)
        {
            return;
        }
        else if (chilliGroupSize == 2)
        {

            SpawnNewChilli(direction >= 0 ? Vector3.left : Vector3.right);
        }
        else if (chilliGroupSize == 3)
        {
            if (direction <= -300)
            {
                SpawnNewChilli(Vector3.down);
                SpawnNewChilli(Vector3.right);
            }
            else if (direction >= 300)
            {
                SpawnNewChilli(Vector3.down);
                SpawnNewChilli(Vector3.left);
            }
            else
            {
                SpawnNewChilli(Vector3.left);
                SpawnNewChilli(Vector3.right);
            }
        }
        else
        {
            if (direction <= -300)
            {
                SpawnNewChilli(Vector3.down);
                SpawnNewChilli(Vector3.right);
                SpawnNewChilli(Vector3.right);
            }
            else if (direction >= 300)
            {
                SpawnNewChilli(Vector3.down);
                SpawnNewChilli(Vector3.left);
                SpawnNewChilli(Vector3.left);
            }
            else
            {
                SpawnNewChilli(Vector3.down);
                SpawnNewChilli(Vector3.left);
                SpawnNewChilli(Vector3.right);
            }
        }
    }

    protected virtual int AdjustedSplitChilliForBombRain()
    {
        int adjustedSplitChilli;

        if (GamePlayManager.currentLevel <= 17)
        {
            adjustedSplitChilli = 2;
        }
        else if (GamePlayManager.currentLevel >= 18 && GamePlayManager.currentLevel <= 32)
        {
            adjustedSplitChilli = 3;
        }
        else
        {
            adjustedSplitChilli = 3;
        }

        return adjustedSplitChilli + 1;
    }

    protected virtual void SpawnNewChilli(Vector3 _direction)
    {
        FoodChillies _newChilli = Instantiate(this, FoodSpawner.Instance.FoodHolder);
        _newChilli.transform.position = transform.position;
        _newChilli.CanSplit = false;
        _newChilli.Setup();
        _newChilli.image.sprite = image.sprite;
        _newChilli.damageValue = damageValue;
        _newChilli.Move(_direction);
    }

    protected override bool AllowAutomaticEating()
    {
        return false;
    }

    protected virtual void Move(Vector3 _direction)
    {
        splitOffset = Random.Range(splitOffsetMin, splitOffsetMax);
        var _position = transform.position;
        float _endPositionX = _position.x + (_direction.x * splitOffset);
        float _currentX = _position.x;
        float _animationTime = 0.3f;
        DG.Tweening.Sequence _sequence = DOTween.Sequence();
        _sequence.Append(DOTween.To(() => _currentX, _x => _currentX = _x, _endPositionX, _animationTime).OnUpdate(() =>
        {
            if (this == null) // Check if object is destroyed
            {
                _sequence.Kill();
                return;
            }
            transform.position = new Vector3(_currentX, _position.y, _position.z);
            if (transform.position.x < Screen.width / 12f || transform.position.x > Screen.width - Screen.width / 12f)
            {
                _sequence.Kill();
            }
        }))

        .SetTarget(this); // Link the sequence to the game object

        _sequence.Play();
    }

    public void DamageValueDisplay(int amount)
    {
        ValueDisplay _textObj = Instantiate(valueDisplayPrefab, GameObject.FindGameObjectWithTag("FoodHolder").transform);
        _textObj.transform.localPosition = transform.localPosition;
        _textObj.DamageSetup(amount);
    }

    private void FrozenChilliSpriteSwitch()
    {
        if (image == null || transform == null)
        {
            Debug.LogWarning("Image or Transform is missing or has been destroyed.");
            return;
        }

        if (isFrozen)
        {
            image.sprite = chilliSprites[1];

            if (Type == FoodType.ChilliBomb)
            {
                transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
            else
            {
                transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            }
        }
        else
        {
            image.sprite = chilliSprites[0];
            transform.localScale = new Vector3(1, 1, 1);
        }

    }

    private void InitiateUnfreezeRoutine()
    {
        shakeCoroutine = StartCoroutine(ShakeFrozenChilli());
    }

    private IEnumerator ShakeFrozenChilli()
    {
        if (this == null || transform == null) yield break;

        Vector3 startPosition = transform.position;
        Vector3 rightXPosition = new(transform.position.x + 10, transform.position.y, transform.position.z);
        Vector3 leftXPosition = new(transform.position.x - 10, transform.position.y, transform.position.z);
        int shakeLoops = 0;

        while (shakeLoops < 5)
        {
            float elapsedTime1 = 0;
            float elapsedTime2 = 0;
            float elapsedTime3 = 0;

            while (elapsedTime1 < 0.05f)
            {
                // Wait while the game is paused
                while (GamePlayManager.isPaused)
                {
                    yield return null; // Wait until the next frame
                }

                transform.position = Vector3.Lerp(startPosition, rightXPosition, elapsedTime1 / 0.05f);
                elapsedTime1 += Time.deltaTime;
                yield return null;
            }

            transform.position = rightXPosition;

            while (elapsedTime2 < 0.1f)
            {
                // Wait while the game is paused
                while (GamePlayManager.isPaused)
                {
                    yield return null; // Wait until the next frame
                }

                transform.position = Vector3.Lerp(rightXPosition, leftXPosition, elapsedTime2 / 0.1f);
                elapsedTime2 += Time.deltaTime;
                yield return null;
            }

            transform.position = leftXPosition;

            while (elapsedTime3 < 0.05f)
            {
                // Wait while the game is paused
                while (GamePlayManager.isPaused)
                {
                    yield return null; // Wait until the next frame
                }

                transform.position = Vector3.Lerp(leftXPosition, startPosition, elapsedTime3 / 0.05f);
                elapsedTime3 += Time.deltaTime;
                yield return null;
            }

            transform.position = startPosition;

            shakeLoops++;
        }

        UnfreezeThis();
    }

    private void UnfreezeThis()
    {
        isFrozen = false;
        Instantiate(iceExplosionPrefab, transform.position, Quaternion.identity, transform);
        FrozenChilliController.Instance.iceBreakSoundCall++;
        FrozenChilliSpriteSwitch();
        fall = true;
    }
}
