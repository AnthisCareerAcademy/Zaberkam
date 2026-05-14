using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource sfxSource;

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) 
            return;

        sfxSource.clip = clip;
    }
}

// later on assign afx audios to public AudioClip fields in gameplay scripts (I'll help when i'm back from taking my test)