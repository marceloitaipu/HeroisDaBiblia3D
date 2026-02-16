using UnityEngine;
using UnityEngine.EventSystems;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Joystick virtual para controle touch em dispositivos móveis.
    /// Implementa interfaces de Event System para capturar input de toque.
    /// </summary>
    public sealed class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        /// <summary>RectTransform do fundo do joystick.</summary>
        public RectTransform bg;
        
        /// <summary>RectTransform da alça (handle) do joystick.</summary>
        public RectTransform handle;
        
        /// <summary>Raio máximo de movimento do joystick.</summary>
        [Range(40, 240)]
        public float radius = 140f;
        
        /// <summary>Valor normalizado do joystick (-1 a 1 em cada eixo).</summary>
        public Vector2 Value { get; private set; }

        void Awake()
        {
            if (bg == null)
                bg = GetComponent<RectTransform>();
            
            ResetStick();
        }

        /// <summary>
        /// Chamado quando o jogador toca no joystick.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        /// <summary>
        /// Chamado enquanto o jogador arrasta o joystick.
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (bg == null)
                return;

            // Converte posição da tela para coordenadas locais do joystick
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                bg,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            // Limita o movimento ao raio máximo
            Vector2 clampedPosition = Vector2.ClampMagnitude(localPoint, radius);
            
            // Normaliza o valor (-1 a 1)
            Value = clampedPosition / radius;
            
            // Atualiza posição visual da alça
            if (handle != null)
                handle.anchoredPosition = clampedPosition;
        }

        /// <summary>
        /// Chamado quando o jogador solta o joystick.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            ResetStick();
        }

        /// <summary>
        /// Reseta o joystick para a posição central.
        /// </summary>
        private void ResetStick()
        {
            Value = Vector2.zero;
            
            if (handle != null)
                handle.anchoredPosition = Vector2.zero;
        }
    }
}
