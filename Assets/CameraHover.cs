using UnityEngine;
using DG.Tweening;

public class CameraHover : MonoBehaviour
{
    [Header("Hover Settings")]
    [SerializeField] private float hoverHeight = 0.2f;
    [SerializeField] private float hoverDuration = 2f;
    [SerializeField] private Ease hoverEase = Ease.InOutSine;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;

        // Start hover loop
        Hover();
    }

    private void Hover()
    {
        transform.DOLocalMoveY(originalPosition.y + hoverHeight, hoverDuration)
            .SetEase(hoverEase)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
