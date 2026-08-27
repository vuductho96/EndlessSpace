using UnityEngine;

namespace SpaceShooter.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                if (_musicSource == null) _musicSource = gameObject.AddComponent<AudioSource>();
                if (_sfxSource == null) _sfxSource = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlaySfx(AudioClip clip, float volume = 1f)
        {
            if (clip != null && _sfxSource != null)
            {
                _sfxSource.PlayOneShot(clip, volume);
            }
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip != null && _musicSource != null)
            {
                _musicSource.clip = clip;
                _musicSource.loop = loop;
                _musicSource.Play();
            }
        }
    }
}
