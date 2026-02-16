using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Constantes globais do jogo usadas por múltiplos sistemas.
    /// </summary>
    public static class GameConstants
    {
        /// <summary>Número de faixas (lanes) disponíveis no gameplay.</summary>
        public const int LaneCount = 3;
        
        /// <summary>Posições X das faixas no mundo 3D.</summary>
        public static readonly float[] LanesX = { -2f, 0f, 2f };
        
        /// <summary>Número máximo de skins disponíveis para compra.</summary>
        public const int MaxSkins = 3;
        
        /// <summary>Número total de mundos no jogo.</summary>
        public const int MaxWorlds = 5;
    }
}
