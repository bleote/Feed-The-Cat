using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterVisual : MonoBehaviour
{
    [SerializeField] private Image imageDisplay;
    [SerializeField] private float idleWaitTimeBetweenFrames;
    [SerializeField] private float eatingWaitTimeBetweenFrames;
    [SerializeField] private Transform mouthPosition;
    //[SerializeField] private Transform icecreamStick;
    //[SerializeField] private Transform floorPosition;
    public Transform mouthMask;

    private CatSO cat;
    private float frameTimer;
    private int currentFrame;

    private enum State { EatingChilli, EatingIceCream, OpeningMouth, ReadyToEat };

    private State currentState;

    public static CharacterVisual Instance;
    
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
        //if (DataManager.Instance.PlayerData == null) { 
        //    DataManager.Instance.CreateNewPlayer();
        //    DataManager.Instance.CreateNewGameData();
        //    CatSO.Init();
        //    CatSO.SelectedCat = CatSO.Get(DataManager.Instance.PlayerData.SelectedCat);
        //}
        cat = CatSO.SelectedCat;
        imageDisplay.sprite = cat.NormalSprites[0];
        SetState(State.OpeningMouth);
    }

    public void EatChilly()
    {
        SetState(State.EatingChilli);
    }

    public void EatIceCream()
    {
        SetState(State.EatingIceCream);
    }

    public void GetReadyToEat()
    {
        SetState(State.ReadyToEat);
    }

    private void SetState(State _newState)
    {
        currentState = _newState;
        currentFrame = 0;
        frameTimer = 0;
    }

    //public void ThrowStick()
    //{
    //    icecreamStick.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
    //    Routine.MoveConstant(icecreamStick, mouthPosition.position, new Vector2(mouthPosition.position.x, floorPosition.position.y), 0.03f);
    //}

    private void FixedUpdate()
    {
        if (!GamePlayManager.isPaused)
        {
            AnimateTheCat();
        }
        else
        {
            if (TutorialGameplay.Instance.toggleCatMovementTutorial || TutorialGameplay.Instance.toggleRelaxGameTutorial)
            {
                AnimateTheCat();
            }
        }
    }

    private void AnimateTheCat()
    {
        frameTimer += Time.deltaTime;

        float _frameTime = currentState == State.ReadyToEat ? idleWaitTimeBetweenFrames : eatingWaitTimeBetweenFrames;

        if (frameTimer > _frameTime)
        {
            frameTimer -= _frameTime;

            List<Sprite> _sprites;

            switch (currentState)
            {
                case State.EatingChilli:
                    _sprites = cat.EatChilliSprites;
                    break;
                case State.EatingIceCream:
                    _sprites = cat.EatIceCreamSprites;
                    break;
                case State.OpeningMouth:
                    _sprites = cat.OpenMouthSprites;
                    break;
                case State.ReadyToEat:
                    _sprites = cat.ReadyToEatSprites;
                    break;
                default:
                    return;
            }

            if (currentFrame >= _sprites.Count)
            {
                if (currentState == State.ReadyToEat)
                {
                    currentFrame = 0;
                }
                else
                {
                    SetState(State.ReadyToEat);
                    return;
                }
            }

            imageDisplay.sprite = _sprites[currentFrame];
            currentFrame++;
        }
    }
}
