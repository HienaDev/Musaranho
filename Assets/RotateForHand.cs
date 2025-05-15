using UnityEngine;

public class RotateForHand : MonoBehaviour
{

    [SerializeField] private Vector3 rotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rotate()
    {
        transform.localEulerAngles = rotation;
    }
}
