using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class AnimateBell : MonoBehaviour
{
    [Header("Bell Setup")]
    [SerializeField] private Transform cordTransform;
    [SerializeField] private Transform bellTransform;
    [SerializeField] private float cordPullDistance = 0.3f;
    [SerializeField] private float cordPullDuration = 0.2f;
    [SerializeField] private float bellSwingAngle = 30f;
    [SerializeField] private float bellSwingDuration = 0.3f;

    private bool isAnimating = false;
    private Vector3 initialCordPosition;
    private Vector3 initialBellRotation;

    [SerializeField] private UnityEvent onBellRung;

    private void Start()
    {
        if (cordTransform == null || bellTransform == null)
        {
            Debug.LogError("AnimateBell: Assign cordTransform and bellTransform.");
            enabled = false;
            return;
        }

        initialCordPosition = cordTransform.localPosition;
        initialBellRotation = bellTransform.localEulerAngles;
    }

    public void RingBell()
    {
        if (isAnimating) return;
        isAnimating = true;

        // Animate cord down and up
        cordTransform.DOLocalMoveY(initialCordPosition.y - cordPullDistance, cordPullDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                cordTransform.DOLocalMoveY(initialCordPosition.y, cordPullDuration)
                    .SetEase(Ease.InOutQuad);

                AnimateBellSwing();
            });
    }

    private void AnimateBellSwing()
    {
        Sequence bellSequence = DOTween.Sequence();
        bellSequence.Append(bellTransform.DOLocalRotate(new Vector3(0, 0, bellSwingAngle), bellSwingDuration)
                            .SetEase(Ease.InOutSine)).OnComplete(() => onBellRung.Invoke()) ;
        bellSequence.Append(bellTransform.DOLocalRotate(new Vector3(0, 0, -bellSwingAngle), bellSwingDuration)
                            .SetEase(Ease.InOutSine)).OnComplete(() => onBellRung.Invoke()); 
        bellSequence.Append(bellTransform.DOLocalRotate(initialBellRotation, bellSwingDuration)
                            .SetEase(Ease.InOutSine));

        bellSequence.OnComplete(() => isAnimating = false);
    }
}
