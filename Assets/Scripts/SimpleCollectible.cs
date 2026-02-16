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
    /// Componente para itens coletáveis. Rotaciona automaticamente e
    /// configura o collider como trigger.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class SimpleCollectible : MonoBehaviour
    {
        /// <summary>Tipo do coletável.</summary>
        public CollectibleType type;
        
        private const float RotationSpeed = 120f;

        void Awake()
        {
            var collider = GetComponent<Collider>();
            if (collider != null)
                collider.isTrigger = true;
            
            gameObject.tag = "Collectible";
        }

        void Update()
        {
            // Rotação constante no eixo Y para efeito visual
            transform.Rotate(0, RotationSpeed * Time.deltaTime, 0, Space.World);
        }
    }
}
