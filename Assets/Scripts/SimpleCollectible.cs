using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Tipos de coletáveis disponíveis no jogo.
    /// </summary>
    public enum CollectibleType
    {
        /// <summary>Pergaminho - dá moedas no Runner (Noé).</summary>
        Pergaminho,
        
        /// <summary>Coração - usado no Runner e no modo Jesus.</summary>
        Coracao,
        
        /// <summary>Pedra - usada no Boss (Davi x Golias).</summary>
        Pedra
    }

    /// <summary>
    /// Componente para itens coletáveis. Rotaciona automaticamente,
    /// balança no eixo Y e pulsa em escala.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class SimpleCollectible : MonoBehaviour
    {
        /// <summary>Tipo do coletável.</summary>
        public CollectibleType type;
        
        private const float RotationSpeed = 120f;
        private const float BobAmplitude = 0.18f;
        private const float BobFrequency = 2.5f;
        private const float PulseAmount = 0.06f;
        private const float PulseSpeed = 3f;

        private float _baseY;
        private float _baseScale;
        private float _phase;

        void Awake()
        {
            var collider = GetComponent<Collider>();
            if (collider != null)
                collider.isTrigger = true;
            
            gameObject.tag = "Collectible";

            _baseY = transform.position.y;
            _baseScale = transform.localScale.x;
            _phase = Random.Range(0f, Mathf.PI * 2f);
        }

        void Update()
        {
            // Rotação constante no eixo Y para efeito visual
            transform.Rotate(0, RotationSpeed * Time.deltaTime, 0, Space.World);

            // Balanço vertical (bobbing)
            var pos = transform.position;
            pos.y = _baseY + Mathf.Sin(Time.time * BobFrequency + _phase) * BobAmplitude;
            transform.position = pos;

            // Pulso de escala
            float pulse = 1f + Mathf.Sin(Time.time * PulseSpeed + _phase) * PulseAmount;
            transform.localScale = Vector3.one * (_baseScale * pulse);
        }
    }
}
