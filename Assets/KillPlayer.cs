using UnityEngine;

public class KillPlayer : MonoBehaviour
{

    [SerializeField] private GameObject deathScreen;
    private bool deadScreen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !deadScreen)
        {
            deadScreen = true;
            deathScreen.SetActive(true);
        }
    }
}
