using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PlayRadioMusic : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private float normalMusicVolume = 1f;
    [SerializeField] private float staticMusicVolume = 0.4f;

    [Header("Static Effect")]
    [SerializeField] private AudioSource staticSource;
    [SerializeField] private AudioClip staticClip;
    [SerializeField] private Vector2 staticVolumeRange = new Vector2(0.3f, 0.6f);
    [SerializeField] private Vector2 blendDurationRange = new Vector2(0.3f, 1.2f);
    [SerializeField] private Vector2 staticIntervalRange = new Vector2(5f, 15f);
    [SerializeField] private Vector2 staticGlitchDuration = new Vector2(1f, 3f);

    private Queue<AudioClip> shuffledQueue = new Queue<AudioClip>();
    private Tween staticFlickerTween;

    void Start()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        musicSource.volume = normalMusicVolume;

        if (musicClips == null || musicClips.Length == 0)
        {
            Debug.LogWarning("No music clips assigned to PlayRadioMusic.");
            return;
        }

        if (staticSource != null && staticClip != null)
        {
            staticSource.clip = staticClip;
            staticSource.loop = true;
            staticSource.volume = 0f;
            staticSource.Play();
            InvokeRandomStaticGlitch();
        }

        ShuffleClips();
        PlayNextClip();
    }

    void Update()
    {
        if (!musicSource.isPlaying && shuffledQueue.Count > 0)
        {
            PlayNextClip();
        }
    }

    private void ShuffleClips()
    {
        List<AudioClip> tempList = new List<AudioClip>(musicClips);

        for (int i = 0; i < tempList.Count; i++)
        {
            int randIndex = Random.Range(i, tempList.Count);
            (tempList[i], tempList[randIndex]) = (tempList[randIndex], tempList[i]);
        }

        foreach (var clip in tempList)
        {
            shuffledQueue.Enqueue(clip);
        }
    }

    public void PlayNextClip()
    {
        if (shuffledQueue.Count == 0)
        {
            ShuffleClips();
        }

        var nextClip = shuffledQueue.Dequeue();
        musicSource.clip = nextClip;
        musicSource.Play();
        shuffledQueue.Enqueue(nextClip);
    }

    private void InvokeRandomStaticGlitch()
    {
        float delay = Random.Range(staticIntervalRange.x, staticIntervalRange.y);
        Invoke(nameof(PlayStaticGlitch), delay);
    }

    private void PlayStaticGlitch()
    {
        if (staticSource == null || staticClip == null) return;

        float targetStaticVolume = Random.Range(staticVolumeRange.x, staticVolumeRange.y);
        float blendInDuration = Random.Range(blendDurationRange.x, blendDurationRange.y);
        float glitchDuration = Random.Range(staticGlitchDuration.x, staticGlitchDuration.y);
        float blendOutDuration = Random.Range(blendDurationRange.x, blendDurationRange.y);

        // Fade static in & music down
        staticSource.DOFade(targetStaticVolume, blendInDuration);
        musicSource.DOFade(staticMusicVolume, blendInDuration);

        // Start flickering
        staticFlickerTween = DOVirtual.Float(0, 1, glitchDuration, _ =>
        {
            float flicker = Random.Range(-0.1f, 0.1f);
            staticSource.volume = Mathf.Clamp(targetStaticVolume + flicker, 0f, 1f);
        }).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        // Schedule end of glitch
        DOVirtual.DelayedCall(glitchDuration, () =>
        {
            staticFlickerTween?.Kill();

            // Fade static out & music back up
            staticSource.DOFade(0f, blendOutDuration);
            musicSource.DOFade(normalMusicVolume, blendOutDuration);
        });

        InvokeRandomStaticGlitch();
    }
}
