using System;
using AndreStuff;
using UnityEngine;

public class BucketInteractable : MonoBehaviour
{
    
    [SerializeField] private Transform fishesStorageParent;
    [SerializeField] private Transform lowestPoint;
    [SerializeField] private Transform highestPoint;
    
    public void StoreFish(GameObject fishObject)
    {
        fishObject.transform.parent = fishesStorageParent;
        if (fishObject.TryGetComponent(out EquipInteractable equipInteractable))
        {
            equipInteractable.Equipped();
        }

        int fishCount = Mathf.Min(10, fishesStorageParent.childCount);
        float v = (float)fishCount / 10;
        fishObject.transform.localPosition = Vector3.Lerp(lowestPoint.position, highestPoint.position, v);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered: " + other.gameObject.name);
    }
}
