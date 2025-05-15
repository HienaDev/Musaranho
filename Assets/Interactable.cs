using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    [SerializeField] private UnityEvent onInteract;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        // Implement interaction logic here
        Debug.Log("Interacted with " + gameObject.name);
        onInteract.Invoke();
    }
}
