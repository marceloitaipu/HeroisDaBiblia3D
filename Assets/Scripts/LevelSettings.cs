using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// ScriptableObject contendo configurações específicas de um nível/mundo.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "Heróis da Bíblia/Level Settings", order = 2)]
    public sealed class LevelSettings : ScriptableObject
    {
        [Header("Informações do Nível")]
        public string levelName = "Mundo 1";
        public string heroName = "Noé";
        
        [TextArea(2, 4)]
        public string description = "Descrição do nível";

        [Header("Runner Settings")]
        public float runnerSpeed = 7.5f;
        public float runnerDistance = 320f;
        public float laneChangeSpeed = 12f;
        public float jumpForce = 7f;

        [Header("Dificuldade")]
        [Range(1, 10)]
        public int difficulty = 5;
        
        [Tooltip("Distância mínima entre obstáculos")]
        public float minObstacleSpacing = 8f;
        
        [Tooltip("Distância máxima entre obstáculos")]
        public float maxObstacleSpacing = 12f;

        [Header("Recompensas")]
        public int baseCoins = 50;
        public int baseVirtues = 1;
        
        [Tooltip("Próximo mundo que será desbloqueado")]
        public int unlocksWorld = 2;

        [Header("Visual")]
        public Color groundColor = new Color(0.70f, 0.86f, 0.72f);
        public Color skyColor = new Color(0.53f, 0.81f, 0.92f);
    }
}
