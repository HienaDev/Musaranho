using System;
using UnityEngine;

namespace AndreStuff
{
    public class BucketChecker : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.parent == null) return;
            if (other.transform.parent.TryGetComponent(out BucketInteractable bucketInteractable))
            {
                bucketInteractable.StoreFish(gameObject);
            }
        }
    }
}