using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private UnityEvent onInteract;

    [SerializeField] private float interactCooldown = 5f;
    private float lastInteractTime = -Mathf.Infinity;

    public void Interact()
    {
        if (Time.time < lastInteractTime + interactCooldown)
        {
            Debug.Log("Interact is on cooldown.");
            return;
        }

        lastInteractTime = Time.time;

        Debug.Log("Interacted with " + gameObject.name);
        onInteract.Invoke();
    }
}
