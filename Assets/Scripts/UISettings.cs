using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// ScriptableObject contendo configurações visuais da UI.
    /// Permite ajustar cores, tamanhos e espaçamentos sem tocar no código.
    /// </summary>
    [CreateAssetMenu(fileName = "UISettings", menuName = "Heróis da Bíblia/UI Settings", order = 4)]
    public sealed class UISettings : ScriptableObject
    {
        [Header("Cores Principais")]
        public Color primaryColor = new Color(0.20f, 0.45f, 0.95f);
        public Color secondaryColor = new Color(0.35f, 0.35f, 0.35f);
        public Color backgroundColor = new Color(0, 0, 0, 0.35f);
        public Color textColor = new Color(0.12f, 0.12f, 0.12f);

        [Header("Cores de Estado")]
        public Color successColor = new Color(0.20f, 0.75f, 0.30f);
        public Color warningColor = new Color(1f, 0.75f, 0.20f);
        public Color errorColor = new Color(0.95f, 0.25f, 0.25f);
        public Color disabledColor = new Color(0.60f, 0.60f, 0.60f);

        [Header("Tamanhos de Fonte")]
        public int titleFontSize = 48;
        public int subtitleFontSize = 34;
        public int bodyFontSize = 24;
        public int smallFontSize = 20;
        public int buttonFontSize = 24;

        [Header("Tamanhos de Botão")]
        public Vector2 largeButtonSize = new Vector2(560, 120);
        public Vector2 mediumButtonSize = new Vector2(520, 110);
        public Vector2 smallButtonSize = new Vector2(420, 95);

        [Header("Animação")]
        [Tooltip("Duração de transições de fade em segundos")]
        [Range(0.1f, 2f)]
        public float fadeDuration = 0.3f;
        
        [Tooltip("Duração de animação de escala de botão")]
        [Range(0.05f, 0.5f)]
        public float buttonScaleDuration = 0.15f;
        
        [Tooltip("Escala do botão ao ser pressionado")]
        [Range(0.8f, 1f)]
        public float buttonPressScale = 0.95f;

        [Header("Joystick")]
        public float joystickRadius = 140f;
        public Color joystickBgColor = new Color(1, 1, 1, 0.18f);
        public Color joystickHandleColor = new Color(0.20f, 0.45f, 0.95f, 0.75f);

        [Header("HUD")]
        public Color hudBackgroundColor = new Color(1, 1, 1, 0.92f);
        public Color aimBarColor = new Color(0.20f, 0.75f, 0.30f);
        public Color aimBarBgColor = new Color(0, 0, 0, 0.25f);
    }
}
