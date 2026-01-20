using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour, IAudioService
{
    [Header("Audio Sources")]
    private AudioSource musicSource;
    private AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;


    [Header("Sound Effects")]
    public AudioClip fragmentSound;
    public AudioClip heartSound;
    public AudioClip powerUpSound;
    public AudioClip attackSound;
    public AudioClip damageSound;
    public AudioClip victorySound;
    public AudioClip gameOverSound;
    public AudioClip teleportSound;

    private Dictionary<string, AudioClip> soundLibrary; // accesos rapidos por nombre a los clips


    private void Awake()
    {
        // evitamos duplicados
        AudioManager[] managers = FindObjectsOfType<AudioManager>();
        if (managers.Length > 1)
        {
            Debug.Log("Ya existe un AudioManager, destruyendo duplicado");
            Destroy(gameObject);
            return;
        }

        // creamos los sources dinamicamente
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = 0.5f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = 0.7f;

        InitializeSoundLibrary();

        ServiceLocator.Register<IAudioService>(this);

        DontDestroyOnLoad(gameObject);

        Debug.Log("muscia todo correcto");

    }

    private void InitializeSoundLibrary()
    {
        soundLibrary = new Dictionary<string, AudioClip>
        {
            // musica
            { "Menu", menuMusic },
            { "Game", gameMusic },

            // efectos
            { "Fragment", fragmentSound },
            { "Heart", heartSound },
            { "PowerUp", powerUpSound },
            { "Attack", attackSound },
            { "Damage", damageSound },
            { "Victory", victorySound },
            { "GameOver", gameOverSound },
            { "Teleport", teleportSound }
        };
    }

    public void PlaySound(string soundId)
    {
        if (soundLibrary.ContainsKey(soundId) && soundLibrary[soundId] != null)
        {
            
            if (soundId == "Menu" || soundId == "Game") // si es musica
            {
                // no repetir si ya está sonando
                if (musicSource.clip == soundLibrary[soundId] && musicSource.isPlaying)
                    return;

                musicSource.clip = soundLibrary[soundId];
                musicSource.Play();
                Debug.Log($"Playing music: {soundId}");
            }
            else
            {
                sfxSource.PlayOneShot(soundLibrary[soundId]); // efectos
                Debug.Log($"Playing sound: {soundId}");
            }
        }
        else
        {
            Debug.LogWarning($"Sound '{soundId}' not found or clip is null!");
        }
    }

    public void StopSound(string soundId)
    {
        if (soundId == "Menu" || soundId == "Game") // si es musica
        {
            musicSource.Stop();
            Debug.Log($"Stopping music: {soundId}");
        }
        else
        {
            Debug.Log($"Stopping music: {soundId}");
        }
    }
}
