using Firebase.Analytics;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterMover : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;

    private delegate Vector3 GetPositionHandler();
    private bool inputting;
    private float minLimit;
    private float maxLimit;

    [SerializeField] private Transform playerHolder;

    public static CharacterMover Instance;
    private void Awake() => Instance = this;
    private void Start()
    {
        minLimit = Screen.width / 12f;
        maxLimit = Screen.width - Screen.width / 12f;
    }

    private void Update()
    {
        if (!GamePlayManager.isPaused)
        {
            if (inputting)
            {
                playerHolder.position += new Vector3(GamePlayManager.input.Drag().x, 0, 0);
                if (playerHolder.position.x < minLimit)
                    playerHolder.position = new Vector3(minLimit, playerHolder.position.y, playerHolder.position.z);
                if (playerHolder.position.x > maxLimit)
                    playerHolder.position = new Vector3(maxLimit, playerHolder.position.y, playerHolder.position.z);
            }
        }
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        var tutorial = TutorialGameplay.Instance;

        if (tutorial.catMovementTutorialMask.gameObject.activeInHierarchy)
        {
            if (tutorial.toggleCatMovementTutorial && PlayerPrefs.GetInt(TutorialManager.CAT_MOVEMENT_TUTORIAL, -1) == -1)
            {
                PlayerPrefs.SetInt(TutorialManager.CAT_MOVEMENT_TUTORIAL, 1);

                FirebaseAnalytics.LogEvent("tutorial_complete_level_start");
            }

            tutorial.TutorialMaskAndMessageSwitch(false);
        }

        if (tutorial.tutorialGameplayMask.gameObject.activeInHierarchy)
        {
            if (tutorial.toggleRelaxGameTutorial && PlayerPrefs.GetInt(TutorialManager.RELAX_GAME_TUTORIAL, -1) == -1)
            {
                PlayerPrefs.SetInt(TutorialManager.RELAX_GAME_TUTORIAL, 1);
            }

            if (tutorial.toggleMeltedLevelsTutorial)
            {
                tutorial.SwitchMaskSpriteForMeltedTutorial(0);
            }

            tutorial.TutorialMaskAndMessageSwitch(false);
        }

        GamePlayManager.input.OnInputDown();
        inputting = true;
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        GamePlayManager.input.OnInputUp();
        inputting = false;
    }
}
