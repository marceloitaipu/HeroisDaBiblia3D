using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Componente responsável por aplicar a customização visual do herói
    /// baseado nas escolhas do jogador (personagem e skin).
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public sealed class HeroCustomizer : MonoBehaviour
    {
        private Renderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        /// <summary>
        /// Aplica a customização visual baseada nos dados salvos do jogador.
        /// </summary>
        /// <param name="saveData">Dados de save contendo herói e skin selecionados.</param>
        public void Apply(SaveData saveData)
        {
            if (_renderer == null || saveData == null)
                return;

            // Cor base do herói selecionado
            Color baseColor = saveData.selectedHero switch
            {
                0 => new Color(0.95f, 0.95f, 0.95f),  // Theo - Branco
                1 => new Color(0.95f, 0.90f, 0.60f),  // Lia - Amarelo claro
                _ => new Color(0.80f, 0.95f, 0.95f)   // Nina - Ciano
            };

            // Tint da skin selecionada
            Color skinTint = saveData.selectedSkin switch
            {
                0 => Color.white,                      // Skin Básica
                1 => new Color(0.55f, 0.85f, 1f),     // Skin Azul
                _ => new Color(0.75f, 0.65f, 1f)      // Skin Roxa
            };

            // Combina cor base com tint da skin
            Color finalColor = new Color(
                baseColor.r * skinTint.r,
                baseColor.g * skinTint.g,
                baseColor.b * skinTint.b
            );

            var material = new Material(GameConstants.SafeStandardShader);
            material.color = finalColor;
            _renderer.material = material;
        }
    }
}
