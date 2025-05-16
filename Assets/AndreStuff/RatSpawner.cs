using UnityEngine;
using Random = UnityEngine.Random;

namespace AndreStuff
{
    public class RatSpawner : MonoBehaviour
    {

        /*
         * ADD LATER:
         * - Make sure it spawns close to player and not in a random far away position.
         */

        [Header("Time between spawns")]
        [SerializeField] private float minTime = 10f;
        [SerializeField] private float extraRandomTime = 10f;
        [Space(10)]
        [Header("Rat")]
        [SerializeField] private GameObject ratPrefab;
        [SerializeField] private float minDistanceToPlayer = 8f;
        [SerializeField] private int maxTriesToFindSpot = 30;
        [Space(10)]
        [Header("SpawnPositions")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private Transform[] spawnPointsNight;

        private DayNightCycle _dayNightCycle;
        private GameManager _gameManager;

        private float _timeForNextSpawn = -1f;
        private GameObject _spawnedRat;

        private void Start()
        {
            _dayNightCycle = FindAnyObjectByType<DayNightCycle>();
            _gameManager = GetComponent<GameManager>();

            StartCountdownForNewRat();
        }

        private void StartCountdownForNewRat()
        {
            _elapsed = 0f;
            _timeForNextSpawn = Random.Range(minTime, (minTime + extraRandomTime));
            Debug.Log("Time to spawn new rat: " + _timeForNextSpawn);
        }

        private float _elapsed = 0f;
        private void Update()
        {
            //Debug.Log($"{_elapsed} / {_timeForNextSpawn}");
            if (Mathf.Abs(_timeForNextSpawn) <= -1f || _spawnedRat != null) return;
            _elapsed += Time.deltaTime;
            if (_elapsed >= _timeForNextSpawn) SpawnRat();
        }

        private void SpawnRat()
        {
            Transform randomTrans = _dayNightCycle.dayStarted ? spawnPointsNight[0] : spawnPoints[0];
            float distanceFound = float.PositiveInfinity;
            int tries = 0;
            while (distanceFound > minDistanceToPlayer && tries <= maxTriesToFindSpot)
            {
                randomTrans = _dayNightCycle.dayStarted
                    ? spawnPointsNight[Random.Range(0, spawnPointsNight.Length)]
                    : spawnPoints[Random.Range(0, spawnPoints.Length)];

                distanceFound = Vector3.Distance(_gameManager.GetPlayerLocation(), randomTrans.position);
                tries++;
            }
            Debug.Log("Distance to player: " + distanceFound);

            _spawnedRat = Instantiate(ratPrefab, randomTrans.position, Quaternion.identity);

            // Apply random Y rotation
            Vector3 randomYRotation = new Vector3(0, Random.Range(0f, 360f), 0);
            _spawnedRat.transform.eulerAngles = randomYRotation;

            _spawnedRat.transform.SetParent(randomTrans);
            StartCountdownForNewRat();
        }


    }
}