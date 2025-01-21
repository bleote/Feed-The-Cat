using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoalSpawnerHandler : MonoBehaviour
{
    public static GoalSpawnerHandler Instance;

    [SerializeField] private GoalSpawner[] goalSpawners;
    
    public bool holdGoalSpawn;
    private int goalSpawnerIndex;

    // Variables to update randomSpawneravailability
    public bool firstSpawnerAvailable = true;
    public bool secondSpawnerAvailable = false;
    public bool thirdSpawnerAvailable = false;
    public bool fourthSpawnerAvailable = false;


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
        if (!GamePlayManager.levelModeOn)
        {
            holdGoalSpawn = true;
        }
        else
        {
            holdGoalSpawn = false;
            SetAvailableGoalSpawners();
        }
    }

    public void GoalSpawnTrigger()
    {
        goalSpawnerIndex = ChooseRandomGoalSpawner();
        goalSpawners[goalSpawnerIndex].HandleGoalSpawn();
    }

    private void SetAvailableGoalSpawners()
    {
        if (GamePlayManager.Instance.bossLevel)
        {
            firstSpawnerAvailable = false;
            secondSpawnerAvailable = false;
            thirdSpawnerAvailable = true;
            fourthSpawnerAvailable = true;
        }
        else
        {
            if (GamePlayManager.currentLevel >= 1)
            {
                firstSpawnerAvailable = true;
            }
            if (GamePlayManager.currentLevel >= 6)
            {
                secondSpawnerAvailable = true;
            }
            if (GamePlayManager.currentLevel >= 13)
            {
                thirdSpawnerAvailable = true;
            }
            if (GamePlayManager.currentLevel >= 22)
            {
                fourthSpawnerAvailable = true;
            }
        }
    }

    private int ChooseRandomGoalSpawner()
    {
        List<int> availableSpawners = new List<int>();
        List<int> weights = new List<int>();

        if (firstSpawnerAvailable)
        {
            availableSpawners.Add(0);
            weights.Add(4); // 4 in 10
        }
        if (secondSpawnerAvailable)
        {
            availableSpawners.Add(1);
            weights.Add(3); // 3 in 10
        }
        if (thirdSpawnerAvailable)
        {
            availableSpawners.Add(2);
            weights.Add(2); // 2 in 10
        }
        if (fourthSpawnerAvailable)
        {
            availableSpawners.Add(3);
            weights.Add(1); // 1 in 10
        }

        if (availableSpawners.Count == 0)
        {
            // Debug.Log("No available spawners!");
            return 0; // Return a default value or handle the case where no spawners are available
        }

        // Adjust weights based on the number of available spawners
        int totalWeight = weights.Sum();
        List<int> cumulativeWeights = new List<int>();
        int cumulativeSum = 0;

        foreach (int weight in weights)
        {
            cumulativeSum += weight;
            cumulativeWeights.Add(cumulativeSum);
        }

        // Generate a random number in the range of the total weight
        int randomValue = Random.Range(0, totalWeight);

        // Find the spawner based on the random value and cumulative weights
        for (int i = 0; i < cumulativeWeights.Count; i++)
        {
            if (randomValue < cumulativeWeights[i])
            {
                return availableSpawners[i];
            }
        }

        // Fallback in case something goes wrong
        return availableSpawners[0];
    }
}
