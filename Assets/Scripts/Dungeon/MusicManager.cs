using UnityEngine;

public class Musicmanager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource musicSource;

    void Start()
    {
        // Auto plays if a clip is already assigned 
        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.Play();
        }
    }

    public void SetMusic(AudioClip newClip)
    {
        if (musicSource == null || newClip == null)
            return;

        musicSource.clip = newClip;
        musicSource.Play();
    }
}