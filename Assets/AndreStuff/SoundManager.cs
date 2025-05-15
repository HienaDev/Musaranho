using System;
using System.Collections.Generic;
using AndreStuff;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    
    [SerializeField] private List<SoundMapping> soundMappings = new List<SoundMapping>();
    public static SoundManager Instance { get; private set; }
    
    private Dictionary<SoundType, AudioClip> _sounds = new Dictionary<SoundType, AudioClip>();

    private AudioSource _audioSource;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeSoundDictionary();

        _audioSource = GetComponent<AudioSource>();
    }
    
    private void InitializeSoundDictionary()
    {
        _sounds.Clear();
        foreach (var mapping in soundMappings)
        {
            if (mapping.audioClip != null) _sounds[mapping.soundType] = mapping.audioClip;
        }
    }
    
    public bool PlaySound(SoundType soundType, Vector3? position = null, float minPitch = 1f, float maxPitch = 1f)
    {
        Debug.Log("Trying to play sound: " + soundType);
        if (_sounds.TryGetValue(soundType, out AudioClip clip))
        {
            // change volume later for which kind of sound it is and the volume value in settings.
            float volume = 1f;
            _audioSource.pitch = Random.Range(minPitch, maxPitch);
            if (position == null)
            {
                Debug.Log("position null, playing on manager pos");
                _audioSource.PlayOneShot(clip, volume);
            }
            else
            {
                Debug.Log("position given, creating new instance");
                GameObject newObj = new GameObject("SoundInstance");
                newObj.transform.position = position.Value;
                AudioSource source = newObj.AddComponent<AudioSource>();
                source.PlayOneShot(clip, volume);
                Destroy(newObj, 1f +clip.length * 2); // *2 just to ensure audio ended before deleting
            }
            return true;
        }
        // couldn't find audio clip, so false for error.
        return false;
    }
    
    public bool PlaySound(SoundType soundType, GameObject onAnObject, bool playOneShot = false)
    {
        Debug.Log("Trying to play sound: " + soundType);
        if (_sounds.TryGetValue(soundType, out AudioClip clip))
        {
            // change volume later for which kind of sound it is and the volume value in settings.
            float volume = 1f;
            Debug.Log("position given, creating new instance");
            if (!onAnObject.TryGetComponent(out AudioSource audioSource))
                audioSource = onAnObject.AddComponent<AudioSource>();
            if (!playOneShot)
            {
                if (audioSource.isPlaying) audioSource.Stop();
                audioSource.clip = clip;
                audioSource.volume = 1f;
                audioSource.Play();
            }
            else
            {
                audioSource.PlayOneShot(clip, volume);
            }
            return true;
        }
        // couldn't find audio clip, so false for error.
        return false;
    }

    public void PlayClipAtTransform(AudioClip clip, Transform targetTransform, float volume = 1f)
    {
        if (clip == null || targetTransform == null) return;

        AudioSource source = targetTransform.GetComponent<AudioSource>();
        if (source == null)
            source = targetTransform.gameObject.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 1f; // Optional: makes it 3D spatial
        source.Play();
    }


    public void UpdateSound(SoundType soundType, AudioClip clip)
    {
        if (clip == null) return;
        _sounds[soundType] = clip;
    }

    public bool HasSound(SoundType soundType) => _sounds.ContainsKey(soundType);
    
    [Serializable]
    public class SoundMapping
    {
        public SoundType soundType;
        public AudioClip audioClip;
    }
}
