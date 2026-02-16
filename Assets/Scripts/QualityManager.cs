using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Gerenciador de qualidade gráfica com perfis otimizados para diferentes dispositivos.
    /// Ajusta automaticamente com base na performance do dispositivo.
    /// </summary>
    public sealed class QualityManager : MonoBehaviour
    {
        public enum QualityTier
        {
            Low,
            Medium,
            High
        }

        public static QualityManager Instance { get; private set; }

        [Header("Configuração")]
        [Tooltip("Tier de qualidade atual")]
        public QualityTier currentTier = QualityTier.Medium;

        [Tooltip("Detecta automaticamente baseado no hardware")]
        public bool autoDetect = true;

        [Header("Performance Monitor")]
        [Tooltip("Monitor FPS para ajuste dinâmico")]
        public bool adaptiveQuality = true;

        [Tooltip("FPS mínimo antes de reduzir qualidade")]
        public int minAcceptableFPS = 30;

        [Tooltip("Intervalo de verificação em segundos")]
        public float checkInterval = 5f;

        private float _lastCheckTime;
        private int _frameCount;
        private float _deltaTimeSum;
        private float _currentFPS;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (autoDetect)
            {
                DetectQualityTier();
            }

            ApplyQualitySettings(currentTier);
        }

        void Update()
        {
            if (!adaptiveQuality)
                return;

            // Monitora FPS
            _frameCount++;
            _deltaTimeSum += Time.deltaTime;

            if (Time.time - _lastCheckTime >= checkInterval)
            {
                _currentFPS = _frameCount / _deltaTimeSum;
                _frameCount = 0;
                _deltaTimeSum = 0f;
                _lastCheckTime = Time.time;

                // Ajusta qualidade se FPS estiver muito baixo
                if (_currentFPS < minAcceptableFPS && currentTier != QualityTier.Low)
                {
                    ReduceQuality();
                }
            }
        }

        /// <summary>
        /// Detecta automaticamente o tier de qualidade baseado no hardware.
        /// </summary>
        private void DetectQualityTier()
        {
            // Detecta baseado na memória do sistema
            int systemMemoryMB = SystemInfo.systemMemorySize;
            int graphicsMemoryMB = SystemInfo.graphicsMemorySize;
            int processorCount = SystemInfo.processorCount;

            // Lógica simples de detecção
            if (systemMemoryMB >= 4096 && processorCount >= 4 && graphicsMemoryMB >= 1024)
            {
                currentTier = QualityTier.High;
            }
            else if (systemMemoryMB >= 2048 && processorCount >= 2)
            {
                currentTier = QualityTier.Medium;
            }
            else
            {
                currentTier = QualityTier.Low;
            }

            Debug.Log($"Quality auto-detected: {currentTier} " +
                     $"(RAM: {systemMemoryMB}MB, CPU: {processorCount} cores, " +
                     $"VRAM: {graphicsMemoryMB}MB)");
        }

        /// <summary>
        /// Aplica configurações de qualidade baseado no tier.
        /// </summary>
        public void ApplyQualitySettings(QualityTier tier)
        {
            currentTier = tier;

            switch (tier)
            {
                case QualityTier.Low:
                    ApplyLowQuality();
                    break;
                case QualityTier.Medium:
                    ApplyMediumQuality();
                    break;
                case QualityTier.High:
                    ApplyHighQuality();
                    break;
            }

            Debug.Log($"Quality settings applied: {tier}");
        }

        /// <summary>
        /// Aplica configurações de qualidade baixa (máxima performance).
        /// </summary>
        private void ApplyLowQuality()
        {
            QualitySettings.SetQualityLevel(0, true);
            Application.targetFrameRate = 30;

            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.shadowResolution = ShadowResolution.Low;
            QualitySettings.antiAliasing = 0;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.softParticles = false;
            QualitySettings.realtimeReflectionProbes = false;
            QualitySettings.pixelLightCount = 1;
        }

        /// <summary>
        /// Aplica configurações de qualidade média (balanceada).
        /// </summary>
        private void ApplyMediumQuality()
        {
            QualitySettings.SetQualityLevel(1, true);
            Application.targetFrameRate = 60;

            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.shadowResolution = ShadowResolution.Medium;
            QualitySettings.antiAliasing = 2;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            QualitySettings.softParticles = true;
            QualitySettings.realtimeReflectionProbes = false;
            QualitySettings.pixelLightCount = 2;
        }

        /// <summary>
        /// Aplica configurações de qualidade alta (máxima qualidade).
        /// </summary>
        private void ApplyHighQuality()
        {
            QualitySettings.SetQualityLevel(2, true);
            Application.targetFrameRate = 60;

            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowResolution = ShadowResolution.High;
            QualitySettings.antiAliasing = 4;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
            QualitySettings.softParticles = true;
            QualitySettings.realtimeReflectionProbes = true;
            QualitySettings.pixelLightCount = 4;
        }

        /// <summary>
        /// Reduz a qualidade em um nível.
        /// </summary>
        public void ReduceQuality()
        {
            if (currentTier == QualityTier.Low)
                return;

            QualityTier newTier = currentTier - 1;
            ApplyQualitySettings(newTier);
            Debug.LogWarning($"Quality reduced to {newTier} due to low FPS ({_currentFPS:F1})");
        }

        /// <summary>
        /// Aumenta a qualidade em um nível.
        /// </summary>
        public void IncreaseQuality()
        {
            if (currentTier == QualityTier.High)
                return;

            QualityTier newTier = currentTier + 1;
            ApplyQualitySettings(newTier);
            Debug.Log($"Quality increased to {newTier}");
        }

        /// <summary>
        /// Obtém o FPS atual médio.
        /// </summary>
        public float GetCurrentFPS() => _currentFPS;

        /// <summary>
        /// Define se o ajuste adaptativo de qualidade está ativo.
        /// </summary>
        public void SetAdaptiveQuality(bool enabled)
        {
            adaptiveQuality = enabled;
        }
    }
}
