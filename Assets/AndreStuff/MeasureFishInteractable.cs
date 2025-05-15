using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

namespace AndreStuff
{
    public class MeasureFishInteractable : MonoBehaviour
    {
        [SerializeField] private float radiusMeasure = 10f;
        private TMP_Text _measureTmp;

        [SerializeField] private Transform weightPosition;

        private float currentWeight = 0f;

        [SerializeField] private Vector2 pointerAngles = new Vector2(90f, -75f);
        [SerializeField] private Transform pointer;

        private GameManager _gameManager;

        [SerializeField] private UnityEvent altarEventGood;
        [SerializeField] private UnityEvent altarEventBad;
        [SerializeField] private UnityEvent altarEventDefault;
        [SerializeField] private bool isAltar = false;

        [SerializeField] private float measureCooldown = 2f;
        private float lastMeasureTime = -Mathf.Infinity;

        private void Awake()
        {
            _measureTmp = transform.Find("MeasureCanvas").Find("Text").GetComponent<TMP_Text>();
        }

        private void Start()
        {
            _gameManager = FindAnyObjectByType<GameManager>();
        }

        public bool MeasureAround()
        {
            if (Time.time < lastMeasureTime + measureCooldown)
            {
                Debug.Log("MeasureAround is on cooldown.");
                return false;
            }

            lastMeasureTime = Time.time;

            Collider[] hitColliders;

            if (weightPosition == null)
                hitColliders = Physics.OverlapSphere(transform.position, radiusMeasure);
            else
                hitColliders = Physics.OverlapSphere(weightPosition.position, radiusMeasure);

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
            currentWeight = totalWeight;

            if (!isAltar)
            {
                float targetZ = Mathf.Lerp(pointerAngles.x, pointerAngles.y, currentWeight / _gameManager.GetDailyWeightNeeded());
                Vector3 targetRotation = new Vector3(0, 0, targetZ);
                pointer.transform.DOLocalRotate(targetRotation, 2f).SetEase(Ease.OutSine);
            }

            if (isAltar && currentWeight >= _gameManager.GetDailyWeightNeeded())
            {
                altarEventGood.Invoke();
                altarEventDefault.Invoke();
            }
            else if (isAltar && currentWeight < _gameManager.GetDailyWeightNeeded())
            {
                altarEventBad.Invoke();
                altarEventDefault.Invoke();
            }

            return false;
        }

        private void UpdateMeasureCanvas(float value)
        {
            _measureTmp.text = $"Fishes Weight: {value:F1}";
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (weightPosition == null)
                Gizmos.DrawWireSphere(transform.position, radiusMeasure);
            else
                Gizmos.DrawWireSphere(weightPosition.position, radiusMeasure);
        }
    }
}
