using UnityEngine;

public class FishSpawner : MonoBehaviour
{

    [Header("Fish Spawn Settings")]
    public Fish fishPrefab;         // Prefab of the fish to spawn

    [SerializeField] private float fishSpawnCooldown = 10f;
    private float justSpawnedFish;

    private bool canSpawnFish = false;

    [SerializeField] private Transform spawnUnderThis; // The object to spawn under
    [SerializeField] private float spawnRadius = 5f; // Maximum distance from center
    [SerializeField] private int spawnCount = 13; // How many objects to spawn
    [SerializeField] private float minHeight = 0.5f; // Minimum height below the object
    [SerializeField] private float maxHeight = 2f; // Maximum height below the object

    [SerializeField] private FishingController fishingController;

    [SerializeField] private int chanceToSpawn = 20; // Chance to spawn fish (0-100)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        justSpawnedFish = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(canSpawnFish && Time.time - justSpawnedFish > fishSpawnCooldown)
        {
            if(Random.Range(0, 100) < chanceToSpawn)
            {
                SpawnFish();
            }

            justSpawnedFish = Time.time;
        }
    }

    private void SpawnFish()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // Get random angle between 180 degrees (left) and 360 degrees (right)
            float randomAngle = Random.Range(0f, 360f);

            // Convert angle to radians for calculations
            float angleInRadians = randomAngle * Mathf.Deg2Rad;

            // Calculate X and Z positions using the angle
            float x = Mathf.Cos(angleInRadians) * Random.Range(spawnRadius - 2.5f, spawnRadius + 2.5f);
            float z = Mathf.Sin(angleInRadians) * Random.Range(spawnRadius - 2.5f, spawnRadius + 2.5f);

            // Get random Y position (below the object)
            float y = -Random.Range(minHeight, maxHeight);

            // Create spawn position relative to the target object
            Vector3 spawnPosition = spawnUnderThis.position + new Vector3(x, y, z);

            // Instantiate the object
            Fish fish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);

            fish.SetLureTarget(spawnUnderThis, fishingController); // Set the target for the fish to swim towards
        }
    }

    public void ToggleFishSpawn(bool canSpawn)
    {
        canSpawnFish = canSpawn;
    }
}
