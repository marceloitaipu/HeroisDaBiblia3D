using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// ScriptableObject contendo configurações de áudio do jogo.
    /// </summary>
    [CreateAssetMenu(fileName = "AudioSettings", menuName = "Heróis da Bíblia/Audio Settings", order = 3)]
    public sealed class AudioSettings : ScriptableObject
    {
        [Header("Volume Master")]
        [Range(0f, 1f)]
        public float masterVolume = 1f;

        [Header("Música")]
        [Range(0f, 1f)]
        public float musicVolume = 0.7f;
        
        public AudioClip menuMusic;
        public AudioClip level1Music;
        public AudioClip level2Music;
        public AudioClip level3Music;
        public AudioClip level4Music;
        public AudioClip level5Music;

        [Header("Efeitos Sonoros")]
        [Range(0f, 1f)]
        public float sfxVolume = 0.9f;
        
        public AudioClip uiClick;
        public AudioClip collectSound;
        public AudioClip jumpSound;
        public AudioClip hitSound;
        public AudioClip victorySound;
        public AudioClip defeatSound;
        public AudioClip bossHitSound;

        [Header("Configurações")]
        public bool enableMusic = true;
        public bool enableSFX = true;
        
        [Tooltip("Fade in/out de música em segundos")]
        public float musicFadeDuration = 1.5f;
    }
}
