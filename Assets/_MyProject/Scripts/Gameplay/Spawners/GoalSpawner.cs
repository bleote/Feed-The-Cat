using UnityEngine;

public class GoalSpawner : MonoBehaviour
{
    public static GoalSpawner Instance;

    [SerializeField] private Transform foodHolder;
    [SerializeField] private FoodIceCreamGoal iceCreamGoalPrefab;
    [SerializeField] private GamePlayManager gamePlayManager;
    [SerializeField] private int goalSpawner;

    private int iceCreamGoalIndex;

    public Transform FoodHolder => foodHolder;

    private void Awake() => Instance = this;

    private void Start()
    {
        SetLocalIceCreamGoalIndex();
    }

    public void HandleGoalSpawn()
    {
        FoodIceCreamGoal _foodPrefab;
        _foodPrefab = iceCreamGoalPrefab;
        _foodPrefab.SetGoalSpawnerAndIceCreamGoalIndex(goalSpawner, iceCreamGoalIndex);
        FoodSpawner.Instance.SpawnFood(_foodPrefab);
    }

    private void SetLocalIceCreamGoalIndex()
    {
        switch (goalSpawner)
        {
            case 1:
                iceCreamGoalIndex = GamePlayManager.firstIceCreamGoalIndex;
                break;

            case 2:
                iceCreamGoalIndex = GamePlayManager.secondIceCreamGoalIndex;
                break;
            
            case 3:
                iceCreamGoalIndex = GamePlayManager.thirdIceCreamGoalIndex;
                break;

            case 4:
                iceCreamGoalIndex = GamePlayManager.fourthIceCreamGoalIndex;
                break;
        }
    }
}
