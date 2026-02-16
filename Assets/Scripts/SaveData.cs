using System;
using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Estrutura de dados para salvar o progresso do jogador.
    /// Serializável para JSON usando JsonUtility do Unity.
    /// </summary>
    [Serializable]
    public sealed class SaveData
    {
        /// <summary>Moedas coletadas pelo jogador.</summary>
        public int coins = 0;
        
        /// <summary>Virtudes conquistadas ao completar níveis.</summary>
        public int virtues = 0;
        
        /// <summary>Número de mundos desbloqueados (1-5).</summary>
        public int worldsUnlocked = 1;
        
        /// <summary>Índice do herói selecionado (0=Theo, 1=Lia, 2=Nina).</summary>
        public int selectedHero = 0;
        
        /// <summary>Array indicando quais skins o jogador possui.</summary>
        public bool[] ownedSkins = new bool[GameConstants.MaxSkins] { true, false, false };
        
        /// <summary>Índice da skin atualmente selecionada.</summary>
        public int selectedSkin = 0;
    }

    /// <summary>
    /// Sistema de salvamento usando PlayerPrefs e serialização JSON.
    /// </summary>
    public static class SaveSystem
    {
        private const string Key = "HBB3D_SAVE_V2";

        /// <summary>
        /// Carrega os dados salvos do jogador. Se não houver save, retorna dados padrão.
        /// </summary>
        /// <returns>Instância de SaveData com dados carregados ou padrão.</returns>
        public static SaveData Load()
        {
            try
            {
                if (!PlayerPrefs.HasKey(Key))
                    return new SaveData();

                string json = PlayerPrefs.GetString(Key, "");
                if (string.IsNullOrWhiteSpace(json))
                    return new SaveData();

                SaveData data = JsonUtility.FromJson<SaveData>(json);
                if (data == null)
                    return new SaveData();

                // Valida e corrige dados se necessário
                if (data.ownedSkins == null || data.ownedSkins.Length != GameConstants.MaxSkins)
                {
                    data.ownedSkins = new bool[GameConstants.MaxSkins] { true, false, false };
                }

                data.worldsUnlocked = Mathf.Clamp(data.worldsUnlocked, 1, GameConstants.MaxWorlds);
                data.selectedHero = Mathf.Clamp(data.selectedHero, 0, 2);
                data.selectedSkin = Mathf.Clamp(data.selectedSkin, 0, GameConstants.MaxSkins - 1);

                return data;
            }
            catch
            {
                // Em caso de erro (corrupção, etc), retorna dados padrão
                return new SaveData();
            }
        }

        /// <summary>
        /// Salva os dados do jogador em PlayerPrefs.
        /// </summary>
        /// <param name="data">Dados a serem salvos.</param>
        public static void Save(SaveData data)
        {
            if (data == null)
                return;

            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(Key, json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Reseta todo o progresso do jogador, deletando o save.
        /// </summary>
        public static void Reset()
        {
            PlayerPrefs.DeleteKey(Key);
            PlayerPrefs.Save();
        }
    }
}
