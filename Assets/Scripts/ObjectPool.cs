using System.Collections.Generic;
using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Sistema genérico de Object Pooling para reutilização eficiente de GameObjects.
    /// Evita custosas operações de Instantiate e Destroy.
    /// </summary>
    public sealed class ObjectPool : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int initialSize = 10;
            public int maxSize = 50;
            public bool expandable = true;
        }

        [Header("Configuração")]
        [Tooltip("Lista de pools a serem criados")]
        public List<Pool> pools;

        [Header("Debug")]
        [Tooltip("Mostra estatísticas no console")]
        public bool showStats = false;

        private Dictionary<string, Queue<GameObject>> _poolDictionary;
        private Dictionary<string, Pool> _poolConfigs;
        private Dictionary<string, int> _poolCounts;
        private Transform _poolContainer;

        /// <summary>Instância singleton do ObjectPool.</summary>
        public static ObjectPool Instance { get; private set; }

        void Awake()
        {
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
        /// Inicializa todos os pools configurados.
        /// </summary>
        private void Initialize()
        {
            _poolContainer = new GameObject("PoolContainer").transform;
            _poolContainer.SetParent(transform);

            _poolDictionary = new Dictionary<string, Queue<GameObject>>();
            _poolConfigs = new Dictionary<string, Pool>();
            _poolCounts = new Dictionary<string, int>();

            if (pools == null)
            {
                pools = new List<Pool>();
                Debug.LogWarning("ObjectPool: lista de pools não configurada. Inicializando vazia.");
            }

            foreach (var pool in pools)
            {
                if (pool.prefab == null)
                {
                    Debug.LogWarning($"Pool '{pool.tag}' tem prefab nulo!");
                    continue;
                }

                var poolTransform = new GameObject($"Pool_{pool.tag}").transform;
                poolTransform.SetParent(_poolContainer);

                var queue = new Queue<GameObject>();

                // Pré-instancia objetos iniciais
                for (int i = 0; i < pool.initialSize; i++)
                {
                    var obj = CreateNewObject(pool.prefab, poolTransform);
                    queue.Enqueue(obj);
                }

                _poolDictionary.Add(pool.tag, queue);
                _poolConfigs.Add(pool.tag, pool);
                _poolCounts.Add(pool.tag, pool.initialSize);

                if (showStats)
                {
                    Debug.Log($"Pool '{pool.tag}' inicializado com {pool.initialSize} objetos");
                }
            }
        }

        /// <summary>
        /// Cria um novo objeto e configura para o pool.
        /// </summary>
        private GameObject CreateNewObject(GameObject prefab, Transform parent)
        {
            var obj = Instantiate(prefab, parent);
            obj.SetActive(false);
            return obj;
        }

        /// <summary>
        /// Obtém um objeto do pool.
        /// </summary>
        /// <param name="tag">Tag do pool.</param>
        /// <param name="position">Posição onde spawnar o objeto.</param>
        /// <param name="rotation">Rotação do objeto.</param>
        /// <returns>GameObject do pool, ou null se não encontrado.</returns>
        public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool com tag '{tag}' não existe!");
                return null;
            }

            GameObject objectToSpawn;
            var queue = _poolDictionary[tag];

            // Se o pool está vazio, tenta expandir
            if (queue.Count == 0)
            {
                var config = _poolConfigs[tag];

                if (!config.expandable || _poolCounts[tag] >= config.maxSize)
                {
                    Debug.LogWarning($"Pool '{tag}' esgotado e não pode expandir!");
                    return null;
                }

                // Cria novo objeto
                var parent = _poolContainer.Find($"Pool_{tag}");
                objectToSpawn = CreateNewObject(config.prefab, parent);
                _poolCounts[tag]++;

                if (showStats)
                {
                    Debug.Log($"Pool '{tag}' expandido para {_poolCounts[tag]} objetos");
                }
            }
            else
            {
                objectToSpawn = queue.Dequeue();
            }

            // Configura e ativa o objeto
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);

            return objectToSpawn;
        }

        /// <summary>
        /// Retorna um objeto para o pool.
        /// </summary>
        /// <param name="tag">Tag do pool.</param>
        /// <param name="obj">Objeto a ser retornado.</param>
        public void Despawn(string tag, GameObject obj)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Tentando retornar objeto para pool inexistente '{tag}'!");
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            _poolDictionary[tag].Enqueue(obj);
        }

        /// <summary>
        /// Retorna um objeto para o pool após um delay.
        /// </summary>
        /// <param name="tag">Tag do pool.</param>
        /// <param name="obj">Objeto a ser retornado.</param>
        /// <param name="delay">Delay em segundos.</param>
        public void DespawnAfter(string tag, GameObject obj, float delay)
        {
            StartCoroutine(DespawnCoroutine(tag, obj, delay));
        }

        private System.Collections.IEnumerator DespawnCoroutine(string tag, GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Despawn(tag, obj);
        }

        /// <summary>
        /// Limpa completamente um pool específico.
        /// </summary>
        public void ClearPool(string tag)
        {
            if (!_poolDictionary.ContainsKey(tag))
                return;

            var queue = _poolDictionary[tag];
            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                if (obj != null)
                    Destroy(obj);
            }

            _poolCounts[tag] = 0;
        }

        /// <summary>
        /// Limpa todos os pools.
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var tag in _poolDictionary.Keys)
            {
                ClearPool(tag);
            }
        }

        /// <summary>
        /// Retorna estatísticas de um pool específico.
        /// </summary>
        public (int active, int available, int total) GetPoolStats(string tag)
        {
            if (!_poolDictionary.ContainsKey(tag))
                return (0, 0, 0);

            int total = _poolCounts[tag];
            int available = _poolDictionary[tag].Count;
            int active = total - available;

            return (active, available, total);
        }

        void OnDestroy()
        {
            ClearAllPools();
        }

#if UNITY_EDITOR
        void OnGUI()
        {
            if (!showStats)
                return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
            GUILayout.Label("=== Object Pool Stats ===");

            foreach (var tag in _poolDictionary.Keys)
            {
                var stats = GetPoolStats(tag);
                GUILayout.Label($"{tag}: {stats.active} active | {stats.available} available | {stats.total} total");
            }

            GUILayout.EndArea();
        }
#endif
    }
}
