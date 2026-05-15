using UnityEngine;

public class Musicmanager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource musicSource;

    void Start()
    {
        // Auto plays if a clip is already assigned 
        musicSource?.Play();
    }

    public void SetMusic(AudioClip newClip)
    {
        if (musicSource == null || newClip == null)
            return;

        musicSource.clip = newClip;
        musicSource.Play();
    }
}