using System;
using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Modo de jogo Boss para o Mundo 2 - Davi x Golias.
    /// O jogador deve coletar pedras e acertar o boss com timing preciso.
    /// </summary>
    public sealed class BossMode : MonoBehaviour
    {
        #region Callbacks
        
        /// <summary>Callback para atualizar o HUD (pedras, acertos, acertos necessários).</summary>
        public Action<int, int, int> onHud;
        
        /// <summary>Callback executado quando o jogador derrota o boss.</summary>
        public Action onBossWin;
        
        /// <summary>Callback executado quando o jogador falha (demora muito).</summary>
        public Action onPlayerFail;
        
        #endregion

        #region Propriedades Públicas
        
        /// <summary>Indica se o jogador está mirando para atirar.</summary>
        public bool IsAiming => _aiming;
        
        /// <summary>Valor atual da mira (oscilante de 0 a 1).</summary>
        public float AimValue => _aimValue;
        
        #endregion

        #region Constantes
        
        private const int NeededHits = 3; // Acertos necessários para derrotar o boss
        private const float IntimidationLimit = 22f; // Tempo máximo antes de falhar
        private const float AimThreshold = 0.18f; // Margem de erro para acerto
        
        #endregion

        #region Campos Privados
        
        private Rigidbody _rb;
        private int _lane = 1; // Faixa atual do jogador
        private int _stones; // Pedras coletadas
        private int _hits; // Acertos no boss
        private float _timer; // Timer geral
        private bool _aiming; // Se está mirando
        private float _aimValue; // Valor oscilante da mira
        private GameObject _boss; // Referência ao boss (Golias)
        private float _bossIntimidation; // Tempo de intimidação do boss
        
        #endregion

        #region Unity Callbacks
        
        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            _timer += Time.deltaTime;

            // Sistema de intimidação - se não atacar, perde
            if (!_aiming)
            {
                _bossIntimidation += Time.deltaTime;
                
                if (_bossIntimidation > IntimidationLimit)
                {
                    FailBattle();
                }
            }

            // Atualiza o valor oscilante da mira
            if (_aiming)
            {
                _aimValue = 0.5f + 0.5f * Mathf.Sin(_timer * 4.2f);
            }

            // Atualiza HUD
            onHud?.Invoke(_stones, _hits, NeededHits);
        }

        void FixedUpdate()
        {
            // Movimento lento para frente
            var velocity = _rb.velocity;
            velocity.z = 0.6f;

            // Movimento lateral entre faixas
            float targetX = GameConstants.LanesX[Mathf.Clamp(_lane, 0, 2)];
            float newX = Mathf.Lerp(
                transform.position.x,
                targetX,
                Time.fixedDeltaTime * 14f
            );

            var position = _rb.position;
            position.x = newX;
            _rb.position = position;
            _rb.velocity = velocity;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Collectible"))
            {
                var collectible = other.GetComponent<SimpleCollectible>();
                if (collectible != null && collectible.type == CollectibleType.Pedra)
                {
                    _stones++;
                    _bossIntimidation = 0f; // Reseta intimidação ao coletar pedra
                    AudioManager.I?.Beep();
                    EnvironmentBuilder.PlayBurstEffect(other.transform.position, new Color(0.7f, 0.7f, 0.7f), 3);
                }

                Destroy(other.gameObject);
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Reinicia a batalha contra o boss, resetando todas as variáveis.
        /// </summary>
        public void ResetBoss()
        {
            _stones = 0;
            _hits = 0;
            _aiming = false;
            _aimValue = 0;
            _timer = 0;
            _bossIntimidation = 0;

            if (_rb != null)
            {
                _rb.isKinematic = false;
                _rb.velocity = Vector3.zero;
            }

            transform.position = new Vector3(0, 1.1f, 2);

            SpawnArena();
            onHud?.Invoke(_stones, _hits, NeededHits);
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
        /// Ação principal - inicia mira ou atira se já estiver mirando.
        /// </summary>
        public void Action()
        {
            TryAimOrThrow();
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Tenta mirar ou atirar, dependendo do estado atual.
        /// </summary>
        private void TryAimOrThrow()
        {
            if (_aiming)
            {
                // Está mirando - executa o arremesso
                _aiming = false;

                // Verifica se o timing foi bom
                bool goodTiming = Mathf.Abs(_aimValue - 0.5f) < AimThreshold;
                _bossIntimidation = 0f; // Reseta intimidação após tentar

                if (goodTiming)
                {
                    // Acertou!
                    _hits++;
                    AudioManager.I?.Beep();
                    PlayBossHitFeedback();

                    // Verifica se derrotou o boss
                    if (_hits >= NeededHits)
                    {
                        WinBattle();
                    }
                }
                else
                {
                    // Errou - feedback visual no boss
                    if (_boss != null)
                    {
                        var bodyRenderer = _boss.GetComponentInChildren<Renderer>();
                        if (bodyRenderer != null)
                            bodyRenderer.material.color = new Color(1f, 0.75f, 0.45f);
                        Invoke(nameof(ResetBossColor), 0.35f);
                    }
                }

                return;
            }

            // Não está mirando - tenta iniciar mira
            if (_stones <= 0)
                return; // Precisa de pedras

            _stones--;
            _aiming = true;
        }

        /// <summary>
        /// Vence a batalha contra o boss.
        /// </summary>
        private void WinBattle()
        {
            enabled = false;

            if (_rb != null)
            {
                _rb.velocity = Vector3.zero;
                _rb.isKinematic = true;
            }

            onBossWin?.Invoke();
        }

        /// <summary>
        /// Falha na batalha (foi intimidado).
        /// </summary>
        private void FailBattle()
        {
            enabled = false;

            if (_rb != null)
            {
                _rb.velocity = Vector3.zero;
                _rb.isKinematic = true;
            }

            onPlayerFail?.Invoke();
        }

        /// <summary>
        /// Cria a arena do boss com todos os elementos necessários.
        /// </summary>
        private void SpawnArena()
        {
            // Limpa arena antiga
            foreach (var boss in GameObject.FindGameObjectsWithTag("Boss"))
                Destroy(boss);

            foreach (var collectible in GameObject.FindGameObjectsWithTag("Collectible"))
                Destroy(collectible);

            // Cria o boss (Golias) — corpo composto para aparência mais robusta
            _boss = new GameObject("Golias");
            _boss.tag = "Boss";
            _boss.transform.position = new Vector3(0, 0, 18f);

            // Corpo principal
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(_boss.transform);
            body.transform.localPosition = new Vector3(0, 2.2f, 0);
            body.transform.localScale = new Vector3(2.35f, 2.35f, 2.35f);
            body.GetComponent<Renderer>().material = EnvironmentBuilder.CreateMat(
                new Color(0.30f, 0.40f, 0.72f), 0.2f, 0.4f,
                new Color(0.08f, 0.12f, 0.25f));
            Destroy(body.GetComponent<Collider>());

            // Cabeça
            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.transform.SetParent(_boss.transform);
            head.transform.localPosition = new Vector3(0, 4.8f, 0);
            head.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            head.GetComponent<Renderer>().material = EnvironmentBuilder.CreateMat(
                new Color(0.62f, 0.50f, 0.38f), 0.05f, 0.3f);
            Destroy(head.GetComponent<Collider>());

            // Ombros
            for (int i = -1; i <= 1; i += 2)
            {
                var shoulder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                shoulder.transform.SetParent(_boss.transform);
                shoulder.transform.localPosition = new Vector3(i * 1.6f, 3.6f, 0);
                shoulder.transform.localScale = new Vector3(1.0f, 0.8f, 0.8f);
                shoulder.GetComponent<Renderer>().material = EnvironmentBuilder.CreateMat(
                    new Color(0.32f, 0.42f, 0.74f), 0.15f, 0.35f);
                Destroy(shoulder.GetComponent<Collider>());
            }

            // Cria o escudo — mais imponente
            var shield = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shield.name = "Escudo";
            shield.tag = "Obstacle";
            shield.transform.position = new Vector3(2.6f, 1.5f, 14f);
            shield.transform.localScale = new Vector3(1.6f, 2.4f, 0.5f);
            shield.GetComponent<Renderer>().material = EnvironmentBuilder.CreateMat(
                new Color(0.42f, 0.38f, 0.32f), 0.45f, 0.55f);
            shield.GetComponent<BoxCollider>().isTrigger = true;

            // Decoração da arena — rochas nos lados
            for (int i = 0; i < 6; i++)
            {
                float side = (i % 2 == 0) ? -1f : 1f;
                float z = 4f + i * 3.5f;
                var rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rock.name = "Deco_ArenaRock";
                rock.tag = "Boss"; // Para cleanup
                rock.transform.position = new Vector3(side * UnityEngine.Random.Range(5f, 7f), 0.3f, z);
                float rs = UnityEngine.Random.Range(0.5f, 1.2f);
                rock.transform.localScale = new Vector3(rs * 1.1f, rs * 0.5f, rs * 0.8f);
                rock.transform.rotation = Quaternion.Euler(
                    UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(0, 360), 0);
                rock.GetComponent<Renderer>().material = EnvironmentBuilder.CreateMat(
                    new Color(0.55f, 0.48f, 0.38f), 0.08f, 0.25f);
                Destroy(rock.GetComponent<Collider>());
            }

            // Spawna pedras para coletar
            for (int i = 0; i < 12; i++)
            {
                int lane = UnityEngine.Random.Range(0, 3);
                float z = 6f + i * 1.8f;

                var stone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                stone.name = "Pedra";
                stone.tag = "Collectible";
                stone.transform.position = new Vector3(GameConstants.LanesX[lane], 1.2f, z);
                stone.transform.localScale = Vector3.one * 0.55f;

                var collectible = stone.AddComponent<SimpleCollectible>();
                collectible.type = CollectibleType.Pedra;

                stone.GetComponent<Renderer>().material = EnvironmentBuilder.CreateMat(
                    new Color(0.60f, 0.58f, 0.55f), 0.12f, 0.35f,
                    new Color(0.15f, 0.15f, 0.15f));
            }
        }

        /// <summary>
        /// Mostra feedback visual de que o boss foi atingido.
        /// </summary>
        private void PlayBossHitFeedback()
        {
            if (_boss == null)
                return;

            var bodyRenderer = _boss.GetComponentInChildren<Renderer>();
            if (bodyRenderer != null)
                bodyRenderer.material.color = new Color(0.45f, 0.95f, 0.65f);
            
            EnvironmentBuilder.PlayBurstEffect(
                _boss.transform.position + Vector3.up * 2f,
                new Color(0.4f, 0.95f, 0.6f), 6);

            Invoke(nameof(ResetBossColor), 0.35f);
        }

        /// <summary>
        /// Restaura a cor original do boss.
        /// </summary>
        private void ResetBossColor()
        {
            if (_boss == null)
                return;

            var bodyRenderer = _boss.GetComponentInChildren<Renderer>();
            if (bodyRenderer != null)
                bodyRenderer.material.color = new Color(0.30f, 0.40f, 0.72f);
        }

        #endregion
    }
}