using UnityEngine;

namespace AndreStuff
{
    public class MeasureFishInteractable : MonoBehaviour
    {

        [SerializeField] private float radiusMeasure = 10f;
        
        public bool MeasureAround()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radiusMeasure);
        
            foreach (Collider collider in hitColliders)
            {
                if (collider.gameObject != gameObject)
                {
                    Debug.Log(collider.gameObject.name);
                }
            }
            return false;
        }

    }
}