using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Câmera que segue suavemente um alvo (jogador) com posição e rotação interpoladas.
    /// Otimizada para modo retrato (portrait).
    /// </summary>
    public sealed class PortraitFollowCamera : MonoBehaviour
    {
        /// <summary>Alvo que a câmera deve seguir (geralmente o jogador).</summary>
        public Transform target;
        
        /// <summary>Offset da câmera em relação ao alvo.</summary>
        public Vector3 offset = new Vector3(0, 5.2f, -9.2f);
        
        /// <summary>Velocidade de suavização do movimento da câmera.</summary>
        public float smooth = 7.5f;

        void LateUpdate()
        {
            if (target == null)
                return;

            // Posição desejada baseada no alvo + offset
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                Time.deltaTime * smooth
            );

            // Ponto para onde a câmera deve olhar (um pouco à frente do alvo)
            Vector3 lookAtPoint = target.position + new Vector3(0, 1.2f, 10f);
            Quaternion desiredRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
            
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRotation,
                Time.deltaTime * smooth
            );
        }
    }
}
