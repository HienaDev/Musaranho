using UnityEngine;

public class Lure : MonoBehaviour
{
    [Header("Lure Properties")]
    [Tooltip("How attractive this lure is to fish (0-1)")]
    [Range(0, 1)]
    [SerializeField] private float attractiveness = 0.5f;

    [Tooltip("Maximum distance at which fish can see this lure")]
    [SerializeField] private float visibilityRange = 8f;

    [Tooltip("How quickly the lure moves through water")]
    [SerializeField] private float moveSpeed = 1f;

    // Optional particle effect for when fish nibbles
    [SerializeField] private ParticleSystem rippleEffect;

    // Optional audio source for lure sounds
    [SerializeField] private AudioSource lureSoundEffect;

    public float Attractiveness { get { return attractiveness; } }
    public float VisibilityRange { get { return visibilityRange; } }

    // Called when a fish bites the lure
    public void OnFishBite(IndependentFish fish)
    {
        // Play ripple effect
        if (rippleEffect != null && !rippleEffect.isPlaying)
        {
            rippleEffect.Play();
        }

        // Play sound effect
        if (lureSoundEffect != null && !lureSoundEffect.isPlaying)
        {
            lureSoundEffect.Play();
        }

        // You could send this event to a fishing rod or game manager
        // to handle actual fish catching mechanics
    }

    // Optional: Visualize the lure's visibility range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, visibilityRange);
    }
}