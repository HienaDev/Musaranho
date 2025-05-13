using System;
using TMPro;
using UnityEngine;

namespace AndreStuff
{
    public class MeasureFishInteractable : MonoBehaviour
    {

        [SerializeField] private float radiusMeasure = 10f;
        private TMP_Text _measureTmp;

        private void Awake()
        {
            _measureTmp = transform.Find("MeasureCanvas").Find("Text").GetComponent<TMP_Text>();
        }

        public bool MeasureAround()
        {
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radiusMeasure);

            float totalWeight = 0f;
            foreach (Collider collider in hitColliders)
            {
                if (collider.gameObject != gameObject && collider.gameObject.TryGetComponent(out FishManager fishManager))
                {
                    FishData fishData = fishManager.GetFishData();
                    if (fishData == null) continue;
                    Debug.Log($"{fishData.FishType} {fishData.WeighType} {fishData.Weight}");
                    totalWeight += fishData.Weight;
                }
            }
            UpdateMeasureCanvas(totalWeight);
            return false;
        }

        private void UpdateMeasureCanvas(float value)
        {
            _measureTmp.text = $"Fishes Weight: {value:F1}";
        }

    }
}