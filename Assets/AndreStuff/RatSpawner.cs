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
        [Space(10)] [Header("Rat")]
        [SerializeField] private GameObject ratPrefab;
        [Space(10)] [Header("SpawnPositions")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private Transform[] spawnPointsNight;

        private DayNightCycle _dayNightCycle;

        private float _timeForNextSpawn = -1f;
        private GameObject _spawnedRat;

        private void Start()
        {
            _timeForNextSpawn = Random.Range(minTime, (minTime + extraRandomTime));
            _dayNightCycle = FindAnyObjectByType<DayNightCycle>();
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
            Transform randomTrans;
            if(_dayNightCycle.dayStarted)
            {
                randomTrans = spawnPointsNight[Random.Range(0, spawnPointsNight.Length)];
            }
            else
            {
                randomTrans = spawnPoints[Random.Range(0, spawnPoints.Length)];
            }

            _spawnedRat = Instantiate(ratPrefab, randomTrans.position, randomTrans.rotation);
            _spawnedRat.transform.SetParent(randomTrans);
            _elapsed = 0f;
            _timeForNextSpawn = Random.Range(minTime, (minTime + extraRandomTime));
        }
        
    }
}