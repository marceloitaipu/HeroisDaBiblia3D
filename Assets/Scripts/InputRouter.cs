using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Roteador de input que centraliza os comandos do jogador.
    /// Gerencia estados de botões pressionados e fornece método para consumi-los.
    /// </summary>
    public sealed class InputRouter : MonoBehaviour
    {
        /// <summary>Referência ao joystick virtual (pode ser nulo se não usado).</summary>
        public VirtualJoystick joystick;
        
        /// <summary>Indica se o botão de pulo foi pressionado neste frame.</summary>
        public bool jumpPressed;
        
        /// <summary>Indica se o botão de deslizar foi pressionado neste frame.</summary>
        public bool slidePressed;
        
        /// <summary>Indica se o botão de ação foi pressionado neste frame.</summary>
        public bool actionPressed;

        /// <summary>
        /// Registra que o botão de pulo foi pressionado.
        /// </summary>
        public void PressJump()
        {
            jumpPressed = true;
        }

        /// <summary>
        /// Registra que o botão de deslizar foi pressionado.
        /// </summary>
        public void PressSlide()
        {
            slidePressed = true;
        }

        /// <summary>
        /// Registra que o botão de ação foi pressionado.
        /// </summary>
        public void PressAction()
        {
            actionPressed = true;
        }

        /// <summary>
        /// Consome (limpa) todos os inputs pressionados.
        /// Deve ser chamado após processar os inputs em cada frame.
        /// </summary>
        public void Consume()
        {
            jumpPressed = false;
            slidePressed = false;
            actionPressed = false;
        }
    }
}
