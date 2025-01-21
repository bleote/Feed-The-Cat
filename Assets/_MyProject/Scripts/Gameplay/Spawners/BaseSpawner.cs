using UnityEngine;

namespace _MyProject.Scripts.GamePlay.Spawner
{
    public class BaseSpawner : MonoBehaviour
    {
        [SerializeField] private FoodController foodPrefab;
        [SerializeField] private Transform foodHolder;
        [SerializeField] private Transform leftBoundary;
        [SerializeField] private Transform rightBoundary;

        [SerializeField] private float initialCooldown;
        [SerializeField] private float minCooldown;
        [SerializeField] private float maxCooldown;
        [SerializeField] private bool randomRotation;

        private float counter;

        private void Start()
        {
            counter = initialCooldown;
        }

        private void Update()
        {
            counter -= Time.deltaTime;

            if (counter <= 0)
            {
                counter = Random.Range(minCooldown, maxCooldown);
                FoodController _foodObject = Instantiate(foodPrefab, foodHolder);
                _foodObject.transform.position = GetSpawnPosition();
                _foodObject.Setup(randomRotation);
            }
        }

        protected virtual Vector3 GetSpawnPosition()
        {
            var _leftBoundary = leftBoundary.position;
            var _rightBoundary = rightBoundary.position;
            return new Vector3(Random.Range(_leftBoundary.x, _rightBoundary.x),
                Random.Range(_leftBoundary.y, _rightBoundary.y),
                Random.Range(_leftBoundary.z, _rightBoundary.z));
        }

    }
}