using System.Collections;
using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Gerenciador avançado de áudio com suporte a música e SFX.
    /// Singleton que persiste entre cenas.
    /// </summary>
    public sealed class AdvancedAudioManager : MonoBehaviour
    {
        /// <summary>Instância singleton.</summary>
        public static AdvancedAudioManager Instance { get; private set; }

        [Header("Configurações")]
        public AudioSettings settings;

        [Header("Fontes de Áudio")]
        private AudioSource _musicSource;
        private AudioSource[] _sfxSources;
        private const int SFX_SOURCE_COUNT = 5; // Pool de fontes para SFX simultâneos

        private Coroutine _musicFadeCoroutine;
        private float _masterVolume = 1f;
        private float _musicVolume = 0.7f;
        private float _sfxVolume = 0.9f;

        void Awake()
        {
            // Implementação Singleton
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        /// <summary>
        /// Inicializa as fontes de áudio.
        /// </summary>
        private void Initialize()
        {
            // Cria fonte para música
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
            _musicSource.priority = 0; // Alta prioridade

            // Cria pool de fontes para SFX
            _sfxSources = new AudioSource[SFX_SOURCE_COUNT];
            for (int i = 0; i < SFX_SOURCE_COUNT; i++)
            {
                _sfxSources[i] = gameObject.AddComponent<AudioSource>();
                _sfxSources[i].playOnAwake = false;
                _sfxSources[i].priority = 128; // Prioridade média
            }

            // Carrega configurações se disponíveis
            if (settings != null)
            {
                _masterVolume = settings.masterVolume;
                _musicVolume = settings.musicVolume;
                _sfxVolume = settings.sfxVolume;
                UpdateVolumes();
            }
        }

        #region Música

        /// <summary>
        /// Toca uma música com fade opcional.
        /// </summary>
        /// <param name="clip">Clip de áudio a tocar.</param>
        /// <param name="fadeDuration">Duração do fade in (0 = sem fade).</param>
        public void PlayMusic(AudioClip clip, float fadeDuration = 1.5f)
        {
            if (clip == null)
                return;

            if (!settings.enableMusic)
                return;

            // Para fade anterior se houver
            if (_musicFadeCoroutine != null)
            {
                StopCoroutine(_musicFadeCoroutine);
            }

            // Se já está tocando a mesma música, não faz nada
            if (_musicSource.clip == clip && _musicSource.isPlaying)
                return;

            if (fadeDuration > 0)
            {
                _musicFadeCoroutine = StartCoroutine(FadeToNewMusic(clip, fadeDuration));
            }
            else
            {
                _musicSource.clip = clip;
                _musicSource.volume = _musicVolume * _masterVolume;
                _musicSource.Play();
            }
        }

        /// <summary>
        /// Para a música atual com fade opcional.
        /// </summary>
        public void StopMusic(float fadeDuration = 1.5f)
        {
            if (_musicFadeCoroutine != null)
            {
                StopCoroutine(_musicFadeCoroutine);
            }

            if (fadeDuration > 0)
            {
                _musicFadeCoroutine = StartCoroutine(FadeOutMusic(fadeDuration));
            }
            else
            {
                _musicSource.Stop();
            }
        }

        /// <summary>
        /// Pausa a música.
        /// </summary>
        public void PauseMusic()
        {
            _musicSource.Pause();
        }

        /// <summary>
        /// Resume a música.
        /// </summary>
        public void ResumeMusic()
        {
            _musicSource.UnPause();
        }

        private IEnumerator FadeToNewMusic(AudioClip newClip, float duration)
        {
            // Fade out da música atual
            float startVolume = _musicSource.volume;
            float elapsed = 0f;

            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                _musicSource.volume = Mathf.Lerp(startVolume, 0, elapsed / (duration / 2));
                yield return null;
            }

            // Troca a música
            _musicSource.Stop();
            _musicSource.clip = newClip;
            _musicSource.Play();

            // Fade in da nova música
            elapsed = 0f;
            float targetVolume = _musicVolume * _masterVolume;

            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                _musicSource.volume = Mathf.Lerp(0, targetVolume, elapsed / (duration / 2));
                yield return null;
            }

            _musicSource.volume = targetVolume;
            _musicFadeCoroutine = null;
        }

        private IEnumerator FadeOutMusic(float duration)
        {
            float startVolume = _musicSource.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _musicSource.volume = Mathf.Lerp(startVolume, 0, elapsed / duration);
                yield return null;
            }

            _musicSource.Stop();
            _musicSource.volume = _musicVolume * _masterVolume;
            _musicFadeCoroutine = null;
        }

        #endregion

        #region Efeitos Sonoros

        /// <summary>
        /// Reproduz um efeito sonoro.
        /// </summary>
        /// <param name="clip">Clip de áudio.</param>
        /// <param name="volumeScale">Escala de volume (0-1).</param>
        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null)
                return;

            if (!settings.enableSFX)
                return;

            // Encontra uma fonte disponível
            AudioSource availableSource = null;
            foreach (var source in _sfxSources)
            {
                if (!source.isPlaying)
                {
                    availableSource = source;
                    break;
                }
            }

            // Se não encontrou fonte disponível, usa a primeira (interrompe o som atual)
            if (availableSource == null)
            {
                availableSource = _sfxSources[0];
            }

            float volume = _sfxVolume * _masterVolume * Mathf.Clamp01(volumeScale);
            availableSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Toca o som de clique da UI.
        /// </summary>
        public void PlayUIClick()
        {
            if (settings != null && settings.uiClick != null)
                PlaySFX(settings.uiClick);
        }

        /// <summary>
        /// Toca o som de coleta de item.
        /// </summary>
        public void PlayCollect()
        {
            if (settings != null && settings.collectSound != null)
                PlaySFX(settings.collectSound);
        }

        /// <summary>
        /// Toca o som de pulo.
        /// </summary>
        public void PlayJump()
        {
            if (settings != null && settings.jumpSound != null)
                PlaySFX(settings.jumpSound);
        }

        /// <summary>
        /// Toca o som de impacto/hit.
        /// </summary>
        public void PlayHit()
        {
            if (settings != null && settings.hitSound != null)
                PlaySFX(settings.hitSound);
        }

        /// <summary>
        /// Toca o som de vitória.
        /// </summary>
        public void PlayVictory()
        {
            if (settings != null && settings.victorySound != null)
                PlaySFX(settings.victorySound);
        }

        /// <summary>
        /// Toca o som de derrota.
        /// </summary>
        public void PlayDefeat()
        {
            if (settings != null && settings.defeatSound != null)
                PlaySFX(settings.defeatSound);
        }

        /// <summary>
        /// Toca o som de acerto no boss.
        /// </summary>
        public void PlayBossHit()
        {
            if (settings != null && settings.bossHitSound != null)
                PlaySFX(settings.bossHitSound);
        }

        #endregion

        #region Controle de Volume

        /// <summary>
        /// Define o volume master (afeta música e SFX).
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Define o volume da música.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Define o volume dos efeitos sonoros.
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
        }

        private void UpdateVolumes()
        {
            _musicSource.volume = _musicVolume * _masterVolume;
        }

        /// <summary>
        /// Obtém o volume master atual.
        /// </summary>
        public float GetMasterVolume() => _masterVolume;

        /// <summary>
        /// Obtém o volume da música atual.
        /// </summary>
        public float GetMusicVolume() => _musicVolume;

        /// <summary>
        /// Obtém o volume dos SFX atual.
        /// </summary>
        public float GetSFXVolume() => _sfxVolume;

        #endregion

        /// <summary>
        /// Para todos os sons (música e SFX).
        /// </summary>
        public void StopAll()
        {
            StopMusic(0);
            foreach (var source in _sfxSources)
            {
                source.Stop();
            }
        }
    }
}
