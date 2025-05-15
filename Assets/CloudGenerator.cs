using UnityEngine;

public class CloudGenerator : MonoBehaviour
{

    [Header("Prefab Settings")]
    public GameObject prefab;           // The prefab to spawn

    [Header("Spawn Settings")]
    public int count = 10;              // Number of objects to spawn
    public Vector3 basePosition = Vector3.zero; // Base (x, y, z) position
    public float heightStep = 2.0f;     // Distance between each object in Y

    void Start()
    {
        SpawnStack();
    }

    void SpawnStack()
    {
        if (prefab == null)
        {
            Debug.LogError("No prefab assigned!");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = basePosition + new Vector3(0, i * heightStep, 0);
            GameObject cloud = Instantiate(prefab, spawnPosition, Quaternion.identity);
            cloud.transform.SetParent(transform); // Set the parent to this object
            cloud.transform.eulerAngles = new Vector3(90, 0, 0);
        }
    }

}
