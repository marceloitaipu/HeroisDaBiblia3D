using System;
using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Modo de jogo Runner para o Mundo 1 - Noé e a Arca.
    /// O jogador corre para frente automaticamente, desviando de obstáculos e coletando itens.
    /// </summary>
    public sealed class RunnerMode : MonoBehaviour
    {
        #region Configurações Públicas
        
        /// <summary>Velocidade de movimento para frente.</summary>
        public float forwardSpeed = 7.5f;
        
        /// <summary>Velocidade de mudança entre faixas (lanes).</summary>
        public float laneChangeSpeed = 12f;
        
        /// <summary>Força do pulo.</summary>
        public float jumpForce = 7f;
        
        /// <summary>Multiplicador de gravidade para queda mais rápida.</summary>
        public float gravityMultiplier = 2.2f;
        
        /// <summary>Distância total da fase em metros.</summary>
        public float phaseLengthMeters = 320f;
        
        #endregion

        #region Estatísticas da Corrida
        
        /// <summary>Número de pergaminhos coletados nesta corrida.</summary>
        public int pergaminhos = 0;
        
        /// <summary>Número de corações coletados nesta corrida.</summary>
        public int coracoes = 0;
        
        /// <summary>Distância percorrida em metros.</summary>
        public float distance = 0f;
        
        #endregion

        #region Callbacks
        
        /// <summary>Callback executado quando o jogador completa a corrida.</summary>
        public Action onRunFinished;
        
        /// <summary>Callback executado quando o jogador colide com um obstáculo.</summary>
        public Action onPlayerHit;
        
        #endregion

        #region Campos Privados
        
        private Rigidbody _rb;
        private CapsuleCollider _col;
        private int _lane = 1; // Faixa atual (0=esquerda, 1=centro, 2=direita)
        private bool _grounded; // Se o jogador está no chão
        private bool _sliding; // Se está executando slide
        private float _slideUntil; // Tempo até terminar o slide
        private float _baseH; // Altura original do collider
        private Vector3 _baseC; // Centro original do collider
        private float _nextObstacleZ; // Posição Z do próximo obstáculo a spawnar
        private float _nextCollectibleZ; // Posição Z do próximo coletável a spawnar
        private float _nextDecoZ; // Posição Z da próxima decoração
        
        #endregion

        #region Unity Callbacks
        
        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _col = GetComponent<CapsuleCollider>();
            _baseH = _col.height;
            _baseC = _col.center;
        }

        void Update()
        {
            // Verifica se terminou o slide
            if (_sliding && Time.time > _slideUntil)
                SetSlide(false);

            float currentZ = transform.position.z;

            // Spawna obstáculos conforme necessário
            while (_nextObstacleZ < currentZ + 90f)
            {
                SpawnObstacle(_nextObstacleZ);
                _nextObstacleZ += 9f;
            }

            // Spawna coletáveis conforme necessário
            while (_nextCollectibleZ < currentZ + 90f)
            {
                SpawnCollectible(_nextCollectibleZ);
                _nextCollectibleZ += 5.8f;
            }

            // Spawna decorações laterais
            var env = EnvironmentBuilder.Instance;
            if (env != null)
            {
                while (_nextDecoZ < currentZ + 100f)
                {
                    env.SpawnSideDecorations(_nextDecoZ);
                    _nextDecoZ += 10f;
                }
                env.CleanupBehindZ(currentZ);
            }

            // Remove objetos que ficaram para trás do jogador
            CleanupBehindPlayer(currentZ);
        }

        void FixedUpdate()
        {
            // Movimento para frente constante
            var velocity = _rb.velocity;
            velocity.z = forwardSpeed;

            // Movimento lateral suave entre faixas
            float targetX = GameConstants.LanesX[Mathf.Clamp(_lane, 0, 2)];
            float newX = Mathf.Lerp(
                transform.position.x,
                targetX,
                Time.fixedDeltaTime * laneChangeSpeed
            );

            var position = _rb.position;
            position.x = newX;
            _rb.position = position;
            _rb.velocity = velocity;

            // Aplica gravidade extra quando no ar
            if (!_grounded)
            {
                _rb.AddForce(
                    Physics.gravity * (gravityMultiplier - 1f),
                    ForceMode.Acceleration
                );
            }

            // Atualiza distância percorrida
            distance += forwardSpeed * Time.fixedDeltaTime;

            // Verifica se completou a fase
            if (distance >= phaseLengthMeters)
            {
                CompleteRun();
            }
        }

        void OnCollisionStay(Collision collision)
        {
            if (collision.collider.gameObject.name.Contains("Ground"))
                _grounded = true;
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.collider.gameObject.name.Contains("Ground"))
                _grounded = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Collectible"))
            {
                var collectible = other.GetComponent<SimpleCollectible>();
                if (collectible != null)
                {
                    if (collectible.type == CollectibleType.Pergaminho)
                        pergaminhos++;
                    else
                        coracoes++;
                }

                // Efeito visual de coleta
                Color fx = (collectible != null && collectible.type == CollectibleType.Pergaminho)
                    ? new Color(1f, 0.9f, 0.3f)
                    : new Color(1f, 0.4f, 0.55f);
                EnvironmentBuilder.PlayBurstEffect(other.transform.position, fx, 4);

                Destroy(other.gameObject);
                AudioManager.I?.Beep();
                return;
            }

            if (other.CompareTag("Obstacle"))
            {
                EnvironmentBuilder.PlayBurstEffect(transform.position, new Color(1f, 0.3f, 0.2f), 5);
                HitObstacle();
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Reinicia a corrida, resetando todas as estatísticas e posição.
        /// </summary>
        public void ResetRun()
        {
            pergaminhos = 0;
            coracoes = 0;
            distance = 0;
            _lane = 1;

            if (_rb != null)
            {
                _rb.isKinematic = false;
                _rb.velocity = Vector3.zero;
            }

            transform.position = new Vector3(0, 1.1f, 2);

            _nextObstacleZ = 25f;
            _nextCollectibleZ = 27f;
            _nextDecoZ = 5f;

            // Spawna batch inicial de objetos
            SpawnBatchAhead(90f);
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

        /// <summary>
        /// Executa um pulo se o jogador estiver no chão.
        /// </summary>
        public void Jump()
        {
            if (!_grounded)
                return;

            _rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            _grounded = false;
        }

        /// <summary>
        /// Executa um slide por uma duração específica.
        /// </summary>
        /// <param name="duration">Duração do slide em segundos.</param>
        public void Slide(float duration = 0.8f)
        {
            if (!_grounded)
                return;

            SetSlide(true);
            _slideUntil = Time.time + duration;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Ativa ou desativa o estado de slide, ajustando o collider.
        /// </summary>
        private void SetSlide(bool on)
        {
            _sliding = on;

            if (on)
            {
                // Reduz o collider para permitir passar por baixo de obstáculos
                _col.height = _baseH * 0.55f;
                _col.center = _baseC + new Vector3(0, -0.35f, 0);
            }
            else
            {
                // Restaura o collider ao tamanho normal
                _col.height = _baseH;
                _col.center = _baseC;
            }
        }

        /// <summary>
        /// Completa a corrida com sucesso.
        /// </summary>
        private void CompleteRun()
        {
            enabled = false;

            if (_rb != null)
            {
                _rb.velocity = Vector3.zero;
                _rb.isKinematic = true;
            }

            onRunFinished?.Invoke();
        }

        /// <summary>
        /// Chamado quando o jogador colide com um obstáculo.
        /// </summary>
        private void HitObstacle()
        {
            enabled = false;

            if (_rb != null)
            {
                _rb.velocity = Vector3.zero;
                _rb.isKinematic = true;
            }

            onPlayerHit?.Invoke();
        }

        /// <summary>
        /// Remove objetos (obstáculos e coletáveis) que ficaram para trás do jogador.
        /// Evita acúmulo de centenas de objetos em memória durante corridas longas.
        /// </summary>
        private void CleanupBehindPlayer(float playerZ)
        {
            float despawnZ = playerZ - GameConstants.DespawnBehindDistance;

            foreach (var o in GameObject.FindGameObjectsWithTag("Obstacle"))
            {
                if (o != null && o.transform.position.z < despawnZ)
                    Destroy(o);
            }

            foreach (var c in GameObject.FindGameObjectsWithTag("Collectible"))
            {
                if (c != null && c.transform.position.z < despawnZ)
                    Destroy(c);
            }
        }

        /// <summary>
        /// Spawna um lote de obstáculos e coletáveis à frente.
        /// </summary>
        private void SpawnBatchAhead(float range)
        {
            float currentZ = transform.position.z;

            for (float z = currentZ + 25f; z < currentZ + range; z += 9f)
                SpawnObstacle(z);

            for (float z = currentZ + 27f; z < currentZ + range; z += 5.8f)
                SpawnCollectible(z);
        }

        /// <summary>
        /// Cria um obstáculo em uma posição aleatória.
        /// </summary>
        private void SpawnObstacle(float z)
        {
            int lane = UnityEngine.Random.Range(0, 3);

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Obstacle";
            go.tag = "Obstacle";
            go.transform.position = new Vector3(GameConstants.LanesX[lane], 0.6f, z);

            // Variação de escala para interesse visual
            float sx = UnityEngine.Random.Range(1.0f, 1.35f);
            float sy = UnityEngine.Random.Range(0.9f, 1.4f);
            float sz = UnityEngine.Random.Range(0.8f, 1.2f);
            go.transform.localScale = new Vector3(sx, sy, sz);
            go.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(-8f, 8f), 0);

            // Material melhorado com variação de cor
            float brown = UnityEngine.Random.Range(0.35f, 0.52f);
            Color obsColor = new Color(brown + 0.08f, brown, brown - 0.08f);
            go.GetComponent<Renderer>().material =
                EnvironmentBuilder.CreateMat(obsColor, 0.05f, 0.25f);

            go.GetComponent<BoxCollider>().isTrigger = true;
        }

        /// <summary>
        /// Cria um coletável (pergaminho ou coração) em uma posição aleatória.
        /// </summary>
        private void SpawnCollectible(float z)
        {
            int lane = UnityEngine.Random.Range(0, 3);

            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Collectible";
            go.tag = "Collectible";
            go.transform.position = new Vector3(GameConstants.LanesX[lane], 1.4f, z);
            go.transform.localScale = Vector3.one * 0.65f;

            var collectible = go.AddComponent<SimpleCollectible>();
            collectible.type = (UnityEngine.Random.value < 0.72f)
                ? CollectibleType.Pergaminho
                : CollectibleType.Coracao;

            Color baseColor, emitColor;
            if (collectible.type == CollectibleType.Pergaminho)
            {
                baseColor = new Color(1f, 0.88f, 0.35f);   // Dourado
                emitColor = new Color(0.6f, 0.5f, 0.1f);   // Brilho dourado
            }
            else
            {
                baseColor = new Color(1f, 0.38f, 0.52f);   // Rosa
                emitColor = new Color(0.6f, 0.15f, 0.25f); // Brilho rosa
            }

            go.GetComponent<Renderer>().material =
                EnvironmentBuilder.CreateMat(baseColor, 0.15f, 0.55f, emitColor);
        }

        #endregion
    }
}