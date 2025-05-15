using Unity.Collections;
using UnityEngine;

public class ControlFishSpawn : MonoBehaviour
{

    [SerializeField] private FishSpawner fishSpawner;
    [SerializeField] private float checkCooldown = 2f;
    private float justCheckedTime = 0f;

    private bool fishSpawning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - justCheckedTime > checkCooldown)
        {
            fishSpawning = fishSpawner.canSpawnFish;
            if (transform.position.y > 0.5f && fishSpawning)
            {
                fishSpawning = false;
                fishSpawner.ToggleFishSpawn(false);
            }

            if (transform.position.y <= 0.5f && !fishSpawning)
            {
                fishSpawning = true;
                fishSpawner.ToggleFishSpawn(true);
            }

            justCheckedTime = Time.time;
        }
    }
}
