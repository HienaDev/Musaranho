using UnityEngine;
using TMPro;
using DG.Tweening;

public class DeathUIAnimate : MonoBehaviour
{
    [Header("Material Animation")]
    [SerializeField] private Material distortionMaterial;
    [SerializeField] private float startDistortion = 3f;
    [SerializeField] private float endDistortion = 0.3f;
    [SerializeField] private float distortionDuration = 1f;

    [Header("Text Fade-In")]
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private float textFadeDuration = 1f;

    private void Start()
    {
        if (distortionMaterial == null || deathText == null)
        {
            Debug.LogWarning("DeathUIAnimate: Missing references.");
            return;
        }

        // Set starting states
        distortionMaterial.SetFloat("_DistortionStrength", startDistortion);
        Color startColor = deathText.color;
        startColor.a = 0f;
        deathText.color = startColor;

        // Animate distortion
        DOTween.To(() => distortionMaterial.GetFloat("_DistortionStrength"),
                   value => distortionMaterial.SetFloat("_DistortionStrength", value),
                   endDistortion,
                   distortionDuration)
               .SetEase(Ease.OutQuad)
               .OnComplete(() =>
               {
                   // Fade in text alpha
                   deathText.DOFade(1f, textFadeDuration).SetEase(Ease.InOutSine);
               });
    }
}
