using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// ScriptableObject contendo configurações gerais do jogo.
    /// Permite ajustar parâmetros sem modificar código.
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Heróis da Bíblia/Game Settings", order = 1)]
    public sealed class GameSettings : ScriptableObject
    {
        [Header("Performance")]
        [Tooltip("Taxa de quadros alvo do jogo")]
        public int targetFrameRate = 60;
        
        [Tooltip("Qualidade gráfica (0=Low, 1=Medium, 2=High)")]
        [Range(0, 2)]
        public int qualityLevel = 1;

        [Header("Comportamento do Jogo")]
        [Tooltip("Velocidade global do jogo (multiplier)")]
        [Range(0.5f, 2f)]
        public float gameSpeedMultiplier = 1f;

        [Header("Economia")]
        [Tooltip("Multiplicador de moedas ganhas")]
        [Range(0.5f, 3f)]
        public float coinMultiplier = 1f;
        
        [Tooltip("Preço da skin azul")]
        public int blueSkinPrice = 80;
        
        [Tooltip("Preço da skin roxa")]
        public int purpleSkinPrice = 120;

        [Header("Progressão")]
        [Tooltip("Moedas mínimas por completar um nível")]
        public int minCoinsPerLevel = 10;
        
        [Tooltip("Moedas máximas por completar um nível")]
        public int maxCoinsPerLevel = 120;

        [Header("Debug")]
        [Tooltip("Habilita mode de debug com cheats")]
        public bool debugMode = false;
        
        [Tooltip("Desbloqueia todos os níveis automaticamente")]
        public bool unlockAllLevels = false;
    }
}
