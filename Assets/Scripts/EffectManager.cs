using System.Collections.Generic;
using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Gerenciador de efeitos visuais (partículas) do jogo.
    /// Usa object pooling para eficiência.
    /// </summary>
    public sealed class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }

        [Header("Particle Systems")]
        public GameObject collectEffectPrefab;
        public GameObject jumpEffectPrefab;
        public GameObject hitEffectPrefab;
        public GameObject victoryEffectPrefab;
        public GameObject levelUpEffectPrefab;

        private Dictionary<string, Queue<GameObject>> _effectPools;
        private const int INITIAL_POOL_SIZE = 10;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializePools();
        }

        /// <summary>
        /// Inicializa os pools de efeitos.
        /// </summary>
        private void InitializePools()
        {
            _effectPools = new Dictionary<string, Queue<GameObject>>();

            // Cria pools para cada tipo de efeito
            CreatePool("Collect", collectEffectPrefab);
            CreatePool("Jump", jumpEffectPrefab);
            CreatePool("Hit", hitEffectPrefab);
            CreatePool("Victory", victoryEffectPrefab);
            CreatePool("LevelUp", levelUpEffectPrefab);
        }

        /// <summary>
        /// Cria um pool de efeitos.
        /// </summary>
        private void CreatePool(string poolName, GameObject prefab)
        {
            if (prefab == null)
                return;

            var poolContainer = new GameObject($"Pool_{poolName}");
            poolContainer.transform.SetParent(transform);

            var queue = new Queue<GameObject>();

            for (int i = 0; i < INITIAL_POOL_SIZE; i++)
            {
                var effect = Instantiate(prefab, poolContainer.transform);
                effect.SetActive(false);
                queue.Enqueue(effect);
            }

            _effectPools.Add(poolName, queue);
        }

        /// <summary>
        /// Spawna um efeito visual em uma posição.
        /// </summary>
        /// <param name="effectType">Tipo do efeito.</param>
        /// <param name="position">Posição onde spawnar.</param>
        /// <param name="autoReturn">Se true, retorna ao pool automaticamente.</param>
        /// <returns>GameObject do efeito spawnado.</returns>
        public GameObject SpawnEffect(string effectType, Vector3 position, bool autoReturn = true)
        {
            if (!_effectPools.ContainsKey(effectType))
            {
                Debug.LogWarning($"Pool de efeito '{effectType}' não existe!");
                return null;
            }

            var queue = _effectPools[effectType];
            GameObject effect;

            if (queue.Count > 0)
            {
                effect = queue.Dequeue();
            }
            else
            {
                // Pool vazio, cria um novo
                Debug.LogWarning($"Pool '{effectType}' vazio, criando novo objeto");
                return null;
            }

            effect.transform.position = position;
            effect.SetActive(true);

            // Auto retorna ao pool após a duração do efeito
            if (autoReturn)
            {
                var particleSystem = effect.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    float duration = particleSystem.main.duration + particleSystem.main.startLifetime.constantMax;
                    StartCoroutine(ReturnToPoolAfter(effectType, effect, duration));
                }
                else
                {
                    // Fallback: retorna após 2 segundos
                    StartCoroutine(ReturnToPoolAfter(effectType, effect, 2f));
                }
            }

            return effect;
        }

        /// <summary>
        /// Retorna um efeito ao pool após um delay.
        /// </summary>
        private System.Collections.IEnumerator ReturnToPoolAfter(string effectType, GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnToPool(effectType, effect);
        }

        /// <summary>
        /// Retorna um efeito ao pool manualmente.
        /// </summary>
        public void ReturnToPool(string effectType, GameObject effect)
        {
            if (!_effectPools.ContainsKey(effectType))
                return;

            effect.SetActive(false);
            _effectPools[effectType].Enqueue(effect);
        }

        #region Métodos de Conveniência

        /// <summary>
        /// Spawna efeito de coleta.
        /// </summary>
        public void PlayCollectEffect(Vector3 position)
        {
            SpawnEffect("Collect", position);
        }

        /// <summary>
        /// Spawna efeito de pulo.
        /// </summary>
        public void PlayJumpEffect(Vector3 position)
        {
            SpawnEffect("Jump", position);
        }

        /// <summary>
        /// Spawna efeito de impacto.
        /// </summary>
        public void PlayHitEffect(Vector3 position)
        {
            SpawnEffect("Hit", position);
        }

        /// <summary>
        /// Spawna efeito de vitória.
        /// </summary>
        public void PlayVictoryEffect(Vector3 position)
        {
            SpawnEffect("Victory", position);
        }

        /// <summary>
        /// Spawna efeito de level up.
        /// </summary>
        public void PlayLevelUpEffect(Vector3 position)
        {
            SpawnEffect("LevelUp", position);
        }

        #endregion

        /// <summary>
        /// Limpa todos os efeitos ativos.
        /// </summary>
        public void ClearAllEffects()
        {
            foreach (var pool in _effectPools.Values)
            {
                foreach (var effect in pool)
                {
                    if (effect != null && effect.activeSelf)
                    {
                        effect.SetActive(false);
                    }
                }
            }
        }
    }
}
