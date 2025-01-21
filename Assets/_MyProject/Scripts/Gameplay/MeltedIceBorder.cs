using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeltedIceBorder : MonoBehaviour
{
    [SerializeField] private RectTransform meltedIce;
    private readonly Vector3 offset = new Vector3(0, -80, 0);
    private readonly float movingSpeed = -18;
    private readonly List<Transform> iceCreams = new List<Transform>();
    private Transform myTransform;

    private bool tutorialTriggered;

    private void Awake()
    {
        myTransform = transform;
    }

    private void OnEnable()
    {
        FoodController.OnReachedBorder += AddIceCream;
    }

    private void OnDisable()
    {
        FoodController.OnReachedBorder -= AddIceCream;

        iceCreams.Clear();
    }

    private readonly HashSet<FoodType> validIceCreamTypes = new ()
    {
        FoodType.IceCream,
        FoodType.RewardingIceCream,
        FoodType.IceCreamSpecial,
        FoodType.IceCreamUnique,
        FoodType.IceCreamGoal
    };

    private void AddIceCream(FoodController _food)
    {
        if (validIceCreamTypes.Contains(_food.Type))
        {
            iceCreams.Add(_food.transform);

            if (TutorialManager.Instance.meltedLevelsTutorialActive && PlayerPrefs.GetInt(TutorialManager.MELTED_LEVELS_TUTORIAL, -1) == -1)
            {
                var _tutorial = TutorialGameplay.Instance;
                _tutorial.meltedLevelsTutorialCounter++;

                if (_tutorial.meltedLevelsTutorialCounter >= 3 &&
                    !tutorialTriggered &&
                    !_tutorial.toggleGoalTutorial &&
                    !_tutorial.toggleFoodTutorial &&
                    !_tutorial.togglePauseTutorial &&
                    !_tutorial.toggleIceButtonTutorial &&
                    !_tutorial.togglePawTutorial)
                {
                    tutorialTriggered = true;
                    _tutorial.SwitchMaskSpriteForMeltedTutorial(1);
                    _tutorial.MeltedLevelsTutorialSetup();
                }
            }
        }
    }

    public void Update()
    {
        if (MeltedIceCreamHandler.Instance == null || meltedIce == null || GamePlayManager.isPaused) return;

        var _iceCreamRect = meltedIce.rect;
        Vector3 _topLeftCorner = meltedIce.TransformPoint(new Vector3(_iceCreamRect.xMin, _iceCreamRect.yMax, 0f));
        myTransform.position = _topLeftCorner + (offset * MeltedIceCreamHandler.Instance.Size);
        myTransform.localRotation = Quaternion.identity;

        if (MeltedIceCreamHandler.Instance.Size == 0)
        {
            return;
        }

        foreach (var _iceCream in iceCreams.ToList())
        {
            if (_iceCream == null)
            {
                iceCreams.Remove(_iceCream);
                continue;
            }

            var _iceCreamTransform = _iceCream.transform;
            var _iceCreamPosition = _iceCreamTransform.position;
            Vector3 _newPos = new Vector3(
                _iceCreamPosition.x,
                myTransform.position.y,
                _iceCreamPosition.z);
            _newPos.x += movingSpeed * Time.deltaTime;
            _iceCreamPosition = _newPos;
            _iceCreamTransform.position = _iceCreamPosition;

            // Ensure the melted ice cream is correctly oriented
            _iceCreamTransform.rotation = Quaternion.identity;
        }
    }
}