using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public AudioSource baseAudioSource;

    [Range(0.8f, 1.2f)]
    public float minPitch = 0.95f;

    [Range(0.8f, 1.2f)]
    public float maxPitch = 1.05f;

    [Range(0f, 1f)]
    public float masterVolume = 1f;

    public int initialPoolSize = 5;

    private List<AudioSource> audioSourcePool;

    void OnEnable()
    {
        if (baseAudioSource == null)
        {
            baseAudioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSourcePool = new List<AudioSource>(initialPoolSize);
        audioSourcePool.Add(baseAudioSource);

        for (int i = 1; i < initialPoolSize; i++)
        {
            audioSourcePool.Add(CreateAudioSourceFromBase());
        }
    }


    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        AudioSource newSource = CreateAudioSourceFromBase();
        audioSourcePool.Add(newSource);

        return newSource;
    }

    private AudioSource CreateAudioSourceFromBase()
    {
        AudioSource src = gameObject.AddComponent<AudioSource>();

        // Basic playback
        src.outputAudioMixerGroup = baseAudioSource.outputAudioMixerGroup;
        src.mute = baseAudioSource.mute;
        src.bypassEffects = baseAudioSource.bypassEffects;
        src.bypassListenerEffects = baseAudioSource.bypassListenerEffects;
        src.bypassReverbZones = baseAudioSource.bypassReverbZones;
        src.playOnAwake = false;
        src.loop = baseAudioSource.loop;
        src.priority = baseAudioSource.priority;

        // Volume & pitch (you override these later anyway)
        src.volume = baseAudioSource.volume;
        src.pitch = baseAudioSource.pitch;

        // 3D sound settings
        src.panStereo = baseAudioSource.panStereo;
        src.spatialBlend = baseAudioSource.spatialBlend;
        src.reverbZoneMix = baseAudioSource.reverbZoneMix;
        src.dopplerLevel = baseAudioSource.dopplerLevel;
        src.spread = baseAudioSource.spread;
        src.rolloffMode = baseAudioSource.rolloffMode;
        src.minDistance = baseAudioSource.minDistance;
        src.maxDistance = baseAudioSource.maxDistance;

        // Curves
        src.SetCustomCurve(AudioSourceCurveType.CustomRolloff,
            baseAudioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
        src.SetCustomCurve(AudioSourceCurveType.SpatialBlend,
            baseAudioSource.GetCustomCurve(AudioSourceCurveType.SpatialBlend));
        src.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix,
            baseAudioSource.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix));
        src.SetCustomCurve(AudioSourceCurveType.Spread,
            baseAudioSource.GetCustomCurve(AudioSourceCurveType.Spread));

        return src;
    }



    public void PlaySoundAtRandomPitch(AudioClip clip, float volume = 1f, float? overrideMinPitch = null, float? overrideMaxPitch = null)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: PlaySoundAtRandomPitch called with null clip.");
            return;
        }

        float finalMinPitch = overrideMinPitch ?? minPitch;
        float finalMaxPitch = overrideMaxPitch ?? maxPitch;

        AudioSource source = GetAvailableAudioSource();
        source.pitch = Random.Range(finalMinPitch, finalMaxPitch);
        source.volume = Mathf.Clamp01(volume) * masterVolume; 
        source.clip = clip;
        source.Play();
    }

    public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: PlaySound called with null clip.");
            return;
        }

        AudioSource source = GetAvailableAudioSource();
        source.pitch = pitch;
        source.volume = Mathf.Clamp01(volume) * masterVolume;
        source.clip = clip;
        source.Play();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }
}
