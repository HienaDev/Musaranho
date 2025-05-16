using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Fish Spawn Settings")]
    public Fish fishPrefab; // Prefab of the fish to spawn

    [SerializeField] private float fishSpawnCooldown = 10f;
    private float currentFishCooldown = 0f;
    private float justSpawnedFish;

    public bool canSpawnFish = false;

    [SerializeField] private Transform spawnUnderThis; // Object above which fish must spawn below
    [SerializeField] private Transform waterSurface;   // New: the water surface object (for Y level)

    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private int spawnCount = 13;
    [SerializeField] private float minHeight = 0.5f;
    [SerializeField] private float maxHeight = 2f;

    [SerializeField] private FishingController fishingController;
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private int chanceToSpawn = 20;

    void Start()
    {
        justSpawnedFish = Time.time;
        currentFishCooldown = Random.Range(fishSpawnCooldown - 2f, fishSpawnCooldown + 2f);
    }

    void Update()
    {
        if (!dayNightCycle.dayStarted && fishingController.isCast)
            justSpawnedFish = Time.time;

        if (canSpawnFish && Time.time - justSpawnedFish > currentFishCooldown)
        {
            if (Random.Range(0, 100) < chanceToSpawn)
            {
                SpawnFish();
            }

            justSpawnedFish = Time.time;
            currentFishCooldown = Random.Range(fishSpawnCooldown - 2f, fishSpawnCooldown + 2f);
        }
    }

    private void SpawnFish()
    {
        if (waterSurface == null)
        {
            Debug.LogWarning("Water surface not assigned! Spawning fish at lure height.");
        }

        float waterY = waterSurface != null ? waterSurface.position.y : spawnUnderThis.position.y;

        for (int i = 0; i < spawnCount; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            float angleInRadians = randomAngle * Mathf.Deg2Rad;

            // Spawn around the lure on X and Z axes
            float x = spawnUnderThis.position.x + Mathf.Cos(angleInRadians) * Random.Range(spawnRadius - 2.5f, spawnRadius + 2.5f);
            float z = spawnUnderThis.position.z + Mathf.Sin(angleInRadians) * Random.Range(spawnRadius - 2.5f, spawnRadius + 2.5f);

            // Y is below water surface
            float y = waterY - Random.Range(minHeight, maxHeight);

            Vector3 spawnPosition = new Vector3(x, y, z);

            Fish fish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
            fish.SetLureTarget(spawnUnderThis, fishingController);
        }
    }


    public void ToggleFishSpawn(bool canSpawn)
    {
        canSpawnFish = canSpawn;
    }
}
