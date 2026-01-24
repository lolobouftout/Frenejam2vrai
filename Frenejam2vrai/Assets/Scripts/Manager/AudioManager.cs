using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField] private bool loopMusic = true;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip keyPickupSound;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.7f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        // Créer AudioSource pour la musique
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = loopMusic;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        // Créer AudioSource pour les effets sonores
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    private void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PlayDashSound()
    {
        PlaySFX(dashSound);
    }

    public void PlayJumpSound()
    {
        PlaySFX(jumpSound);
    }

    public void PlayDeathSound()
    {
        PlaySFX(deathSound);
    }

    public void PlayKeyPickupSound()
    {
        PlaySFX(keyPickupSound);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }
}