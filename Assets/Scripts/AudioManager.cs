using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Gerenciador global de áudio. Singleton que persiste entre cenas.
    /// </summary>
    public sealed class AudioManager : MonoBehaviour
    {
        /// <summary>Instância singleton do AudioManager.</summary>
        public static AudioManager I { get; private set; }
        
        /// <summary>Clip de áudio para feedback de UI (beep).</summary>
        public AudioClip uiBeep;
        
        private AudioSource _sfx;

        void Awake()
        {
            // Implementação do padrão Singleton
            if (I != null)
            {
                Destroy(gameObject);
                return;
            }
            
            I = this;
            DontDestroyOnLoad(gameObject);
            
            // Configura AudioSource para efeitos sonoros
            _sfx = gameObject.AddComponent<AudioSource>();
            _sfx.playOnAwake = false;
            _sfx.volume = 0.9f;
        }

        /// <summary>
        /// Reproduz o som de beep da UI.
        /// </summary>
        public void Beep()
        {
            if (uiBeep != null)
                _sfx.PlayOneShot(uiBeep, 0.9f);
        }
    }
}
