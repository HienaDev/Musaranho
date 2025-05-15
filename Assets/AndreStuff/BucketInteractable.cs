using UnityEngine;

public class BucketInteractable : MonoBehaviour
{
    
    [SerializeField] private Transform fishesStorageParent;
    [SerializeField] private Transform lowestPoint;
    [SerializeField] private Transform highestPoint;
    
    public void StoreFish(GameObject fishObject)
    {
        fishObject.transform.parent = fishesStorageParent;
        if (fishObject.TryGetComponent(out Rigidbody fishRB))
        {
            fishRB.isKinematic = true;
        }
        if (fishObject.TryGetComponent(out Collider coll))
        {
            coll.isTrigger = true;
        }

        int fishCount = Mathf.Min(10, fishesStorageParent.childCount);
        float v = (float)fishCount / 10;
        fishObject.transform.rotation = Quaternion.identity;
        fishObject.transform.localPosition = Vector3.Lerp(lowestPoint.localPosition, highestPoint.localPosition, v);
    }
}
