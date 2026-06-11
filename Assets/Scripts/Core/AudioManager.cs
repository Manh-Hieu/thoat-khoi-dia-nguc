using UnityEngine;

namespace EscapeFromHell.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Settings")]
        [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.5f;
        [Range(0f, 1f)] [SerializeField] private float sfxVolume = 0.7f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Dynamically add AudioSources if not set in editor
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }

            UpdateVolumes();
        }

        public void PlayBGM(AudioClip musicClip)
        {
            if (musicSource.clip == musicClip && musicSource.isPlaying) return;

            musicSource.clip = musicClip;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

        public void StopBGM()
        {
            musicSource.Stop();
        }

        public void PlaySFX(AudioClip sfxClip)
        {
            if (sfxClip == null) return;
            sfxSource.PlayOneShot(sfxClip, sfxVolume);
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            musicSource.volume = musicVolume;
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }

        private void UpdateVolumes()
        {
            musicSource.volume = musicVolume;
        }
    }
}
