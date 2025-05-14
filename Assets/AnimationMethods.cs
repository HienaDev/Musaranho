using UnityEngine;

public class AnimationMethods : MonoBehaviour
{

    [SerializeField] private FishingController fishingController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UncastLure()
    {
        fishingController.UncastAnimatorLine();
    }

    public void ThrowLure()
    {
        fishingController.ThrowLure();
    }
}
