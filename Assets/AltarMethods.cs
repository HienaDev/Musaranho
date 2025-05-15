using UnityEngine;

public class AltarMethods : MonoBehaviour
{

    [SerializeField] private AudioClip bellSound;
    [SerializeField] private Transform bellPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBell()
    {
        SoundManager.Instance.PlayClipAtTransform(bellSound, bellPosition);
        Debug.Log("Bell sound played");
    }
}
