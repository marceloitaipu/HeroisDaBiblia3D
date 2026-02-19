using System;
using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Modo de jogo para o Mundo 5 - Jesus (Coleta de corações).
    /// Similar ao Runner, mas o objetivo é coletar corações para completar a fase.
    /// </summary>
    public sealed class JesusCollectMode : MonoBehaviour
    {
        /// <summary>Velocidade de movimento para frente.</summary>
        public float forwardSpeed = 6.5f;
        
        /// <summary>Velocidade de mudança de faixa (lane).</summary>
        public float laneChangeSpeed = 12f;
        
        /// <summary>Distância até spawnar novos coletáveis.</summary>
        public float spawnDistance = 90f;
        
        /// <summary>Meta de corações para completar a fase.</summary>
        public int heartsTarget = 18;
        
        /// <summary>Quantidade de corações coletados atualmente.</summary>
        public int hearts = 0;
        
        /// <summary>Callback executado quando o jogador atinge a meta de corações.</summary>
        public Action onWin;

        private Rigidbody _rb;
        private int _lane = 1; // Faixa atual (0=esquerda, 1=centro, 2=direita)
        private float _nextCollectibleZ;
        private float _distance = 0f;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Reinicia o modo de coleta, resetando progresso e posição.
        /// </summary>
        public void ResetMode()
        {
            hearts = 0;
            _distance = 0f;
            _lane = 1;
            
            if (_rb != null)
            {
                _rb.isKinematic = false;
                _rb.velocity = Vector3.zero;
            }
            
            transform.position = new Vector3(0, 1.1f, 2);
            _nextCollectibleZ = 15f;
            
            // Spawna batch inicial de coletáveis
            SpawnBatchAhead(spawnDistance);
        }

        /// <summary>
        /// Define a faixa (lane) com base no input analógico do joystick.
        /// </summary>
        /// <param name="x">Valor do eixo X do joystick (-1 a 1).</param>
        public void SetLaneByAxis(float x)
        {
            if (x < -0.35f)
                _lane = 0;
            else if (x > 0.35f)
                _lane = 2;
            else
                _lane = 1;
        }

        void Update()
        {
            float currentZ = transform.position.z;
            
            // Spawna novos coletáveis conforme necessário
            while (_nextCollectibleZ < currentZ + spawnDistance)
            {
                SpawnCollectible(_nextCollectibleZ);
                _nextCollectibleZ += 5.5f;
            }

            // Remove objetos que ficaram para trás do jogador
            CleanupBehindPlayer(currentZ);
        }

        void FixedUpdate()
        {
            if (_rb == null) return;

            // Movimento para frente constante
            var velocity = _rb.velocity;
            velocity.z = forwardSpeed;
            
            // Movimento lateral suave entre faixas
            float targetX = GameConstants.LanesX[Mathf.Clamp(_lane, 0, 2)];
            float newX = Mathf.Lerp(transform.position.x, targetX, Time.fixedDeltaTime * laneChangeSpeed);
            
            var pos = _rb.position;
            pos.x = newX;
            _rb.position = pos;
            _rb.velocity = velocity;
            
            _distance += forwardSpeed * Time.fixedDeltaTime;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Collectible"))
            {
                var collectible = other.GetComponent<SimpleCollectible>();
                if (collectible != null && collectible.type == CollectibleType.Coracao)
                {
                    hearts++;
                    AudioManager.I?.Beep();
                    
                    // Verifica se atingiu a meta
                    if (hearts >= heartsTarget)
                    {
                        CompleteLevel();
                    }
                }
                
                Destroy(other.gameObject);
            }
        }

        /// <summary>
        /// Completa o nível, parando o movimento e acionando o callback de vitória.
        /// </summary>
        private void CompleteLevel()
        {
            enabled = false;
            
            if (_rb != null)
            {
                _rb.velocity = Vector3.zero;
                _rb.isKinematic = true;
            }
            
            onWin?.Invoke();
        }

        /// <summary>
        /// Remove objetos que ficaram para trás do jogador para evitar acúmulo em memória.
        /// </summary>
        private void CleanupBehindPlayer(float playerZ)
        {
            float despawnZ = playerZ - GameConstants.DespawnBehindDistance;

            foreach (var c in GameObject.FindGameObjectsWithTag("Collectible"))
            {
                if (c != null && c.transform.position.z < despawnZ)
                    Destroy(c);
            }
        }

        /// <summary>
        /// Spawna um lote de coletáveis à frente do jogador.
        /// </summary>
        /// <param name="range">Distância à frente para spawnar.</param>
        private void SpawnBatchAhead(float range)
        {
            float startZ = transform.position.z;
            for (float z = startZ + 15f; z < startZ + range; z += 5.5f)
            {
                SpawnCollectible(z);
            }
        }

        /// <summary>
        /// Cria um coletável (coração) em uma posição específica.
        /// </summary>
        /// <param name="z">Posição Z onde spawnar o coletável.</param>
        private void SpawnCollectible(float z)
        {
            int lane = UnityEngine.Random.Range(0, 3);
            
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Heart";
            go.tag = "Collectible";
            go.transform.position = new Vector3(GameConstants.LanesX[lane], 1.4f, z);
            go.transform.localScale = Vector3.one * 0.7f;
            
            var collectible = go.AddComponent<SimpleCollectible>();
            collectible.type = CollectibleType.Coracao;
            
            var material = new Material(GameConstants.SafeStandardShader);
            material.color = new Color(1f, 0.3f, 0.5f, 1f); // Rosa/vermelho para corações
            go.GetComponent<Renderer>().material = material;
            
            var collider = go.GetComponent<SphereCollider>();
            if (collider != null)
                collider.isTrigger = true;
        }
    }
}
