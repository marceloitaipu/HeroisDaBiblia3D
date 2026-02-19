using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Tipos de conquistas disponíveis no jogo.
    /// </summary>
    public enum AchievementType
    {
        // Progressão
        CompleteWorld1,
        CompleteWorld2,
        CompleteWorld3,
        CompleteWorld4,
        CompleteWorld5,
        CompleteAllWorlds,

        // Coleta
        Collect100Coins,
        Collect500Coins,
        Collect1000Coins,
        Collect50Scrolls,
        Collect100Hearts,

        // Virtudes
        Earn10Virtues,
        Earn25Virtues,
        EarnAllVirtues,

        // Boss
        DefeatGoliathPerfect, // Sem errar nenhum arremesso
        DefeatGoliathFast, // Em menos de 30 segundos

        // Runner
        RunnerNoHits, // Completar sem colidir
        RunnerPerfect, // Coletar tudo

        // Customização
        UnlockAllSkins,
        BuyFirstSkin,

        // Especiais
        PlayFor1Hour,
        FirstVictory
    }

    /// <summary>
    /// Dados de uma conquista individual.
    /// </summary>
    [Serializable]
    public class Achievement
    {
        public AchievementType type;
        public string title;
        public string description;
        public int coinReward;
        public bool isUnlocked;
        public string unlockedDate;

        public Achievement(AchievementType type, string title, string description, int coinReward)
        {
            this.type = type;
            this.title = title;
            this.description = description;
            this.coinReward = coinReward;
            this.isUnlocked = false;
            this.unlockedDate = "";
        }
    }

    /// <summary>
    /// Gerenciador do sistema de conquistas.
    /// </summary>
    public sealed class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        /// <summary>Callback executado quando uma conquista é desbloqueada.</summary>
        public event Action<Achievement> OnAchievementUnlocked;

        private Dictionary<AchievementType, Achievement> _achievements;
        private const string SAVE_KEY = "HBB3D_ACHIEVEMENTS";

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAchievements();
            LoadProgress();
        }

        /// <summary>
        /// Inicializa todas as conquistas do jogo.
        /// </summary>
        private void InitializeAchievements()
        {
            _achievements = new Dictionary<AchievementType, Achievement>
            {
                // Progressão
                { AchievementType.CompleteWorld1, new Achievement(
                    AchievementType.CompleteWorld1,
                    "Sobrevivente da Arca",
                    "Complete o Mundo 1 - Noé",
                    50) },
                
                { AchievementType.CompleteWorld2, new Achievement(
                    AchievementType.CompleteWorld2,
                    "Derrotador de Gigantes",
                    "Complete o Mundo 2 - Davi x Golias",
                    75) },
                
                { AchievementType.CompleteWorld3, new Achievement(
                    AchievementType.CompleteWorld3,
                    "O Grande Peixe",
                    "Complete o Mundo 3 - Jonas",
                    75) },
                
                { AchievementType.CompleteWorld4, new Achievement(
                    AchievementType.CompleteWorld4,
                    "Milagre do Mar",
                    "Complete o Mundo 4 - Moisés",
                    100) },
                
                { AchievementType.CompleteWorld5, new Achievement(
                    AchievementType.CompleteWorld5,
                    "Amor e Bondade",
                    "Complete o Mundo 5 - Jesus",
                    100) },
                
                { AchievementType.CompleteAllWorlds, new Achievement(
                    AchievementType.CompleteAllWorlds,
                    "Herói da Bíblia",
                    "Complete todos os 5 mundos",
                    250) },

                // Coleta
                { AchievementType.Collect100Coins, new Achievement(
                    AchievementType.Collect100Coins,
                    "Colecionador Iniciante",
                    "Colete 100 moedas",
                    20) },
                
                { AchievementType.Collect500Coins, new Achievement(
                    AchievementType.Collect500Coins,
                    "Colecionador Experiente",
                    "Colete 500 moedas",
                    50) },
                
                { AchievementType.Collect1000Coins, new Achievement(
                    AchievementType.Collect1000Coins,
                    "Mestre Colecionador",
                    "Colete 1000 moedas",
                    100) },

                // Virtudes
                { AchievementType.Earn10Virtues, new Achievement(
                    AchievementType.Earn10Virtues,
                    "Virtuoso",
                    "Ganhe 10 virtudes",
                    50) },
                
                { AchievementType.Earn25Virtues, new Achievement(
                    AchievementType.Earn25Virtues,
                    "Super Virtuoso",
                    "Ganhe 25 virtudes",
                    100) },

                // Boss
                { AchievementType.DefeatGoliathPerfect, new Achievement(
                    AchievementType.DefeatGoliathPerfect,
                    "Atirador Perfeito",
                    "Derrote Golias sem errar nenhum arremesso",
                    150) },

                // Runner
                { AchievementType.RunnerNoHits, new Achievement(
                    AchievementType.RunnerNoHits,
                    "Intocável",
                    "Complete uma corrida sem colidir",
                    75) },

                // Customização
                { AchievementType.UnlockAllSkins, new Achievement(
                    AchievementType.UnlockAllSkins,
                    "Fashionista",
                    "Desbloqueie todas as skins",
                    100) },
                
                { AchievementType.BuyFirstSkin, new Achievement(
                    AchievementType.BuyFirstSkin,
                    "Primeira Compra",
                    "Compre sua primeira skin",
                    30) },

                // Especiais
                { AchievementType.FirstVictory, new Achievement(
                    AchievementType.FirstVictory,
                    "Primeira Vitória",
                    "Complete seu primeiro nível",
                    50) },

                // Coleta (faltantes)
                { AchievementType.Collect50Scrolls, new Achievement(
                    AchievementType.Collect50Scrolls,
                    "Estudioso",
                    "Colete 50 pergaminhos",
                    40) },

                { AchievementType.Collect100Hearts, new Achievement(
                    AchievementType.Collect100Hearts,
                    "Coração Cheio",
                    "Colete 100 corações",
                    60) },

                // Virtudes (faltante)
                { AchievementType.EarnAllVirtues, new Achievement(
                    AchievementType.EarnAllVirtues,
                    "Santo dos Santos",
                    "Ganhe todas as virtudes",
                    200) },

                // Boss (faltante)
                { AchievementType.DefeatGoliathFast, new Achievement(
                    AchievementType.DefeatGoliathFast,
                    "Velocista da Fé",
                    "Derrote Golias em menos de 30 segundos",
                    120) },

                // Runner (faltante)
                { AchievementType.RunnerPerfect, new Achievement(
                    AchievementType.RunnerPerfect,
                    "Corrida Perfeita",
                    "Colete todos os itens em uma corrida",
                    100) },

                // Especiais (faltante)
                { AchievementType.PlayFor1Hour, new Achievement(
                    AchievementType.PlayFor1Hour,
                    "Dedicado",
                    "Jogue por 1 hora no total",
                    80) }
            };
        }

        /// <summary>
        /// Desbloqueia uma conquista.
        /// </summary>
        /// <param name="type">Tipo da conquista a desbloquear.</param>
        /// <returns>True se foi desbloqueada agora, false se já estava desbloqueada.</returns>
        public bool UnlockAchievement(AchievementType type)
        {
            if (!_achievements.ContainsKey(type))
            {
                Debug.LogWarning($"Conquista {type} não existe!");
                return false;
            }

            var achievement = _achievements[type];

            if (achievement.isUnlocked)
                return false; // Já estava desbloqueada

            // Desbloqueia
            achievement.isUnlocked = true;
            achievement.unlockedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Salva progresso
            SaveProgress();

            // Notifica listeners
            OnAchievementUnlocked?.Invoke(achievement);

            Debug.Log($"🏆 Conquista desbloqueada: {achievement.title}");

            return true;
        }

        /// <summary>
        /// Verifica se uma conquista está desbloqueada.
        /// </summary>
        public bool IsUnlocked(AchievementType type)
        {
            return _achievements.ContainsKey(type) && _achievements[type].isUnlocked;
        }

        /// <summary>
        /// Retorna todas as conquistas.
        /// </summary>
        public List<Achievement> GetAllAchievements()
        {
            return _achievements.Values.ToList();
        }

        /// <summary>
        /// Retorna conquistas desbloqueadas.
        /// </summary>
        public List<Achievement> GetUnlockedAchievements()
        {
            return _achievements.Values.Where(a => a.isUnlocked).ToList();
        }

        /// <summary>
        /// Retorna conquistas bloqueadas.
        /// </summary>
        public List<Achievement> GetLockedAchievements()
        {
            return _achievements.Values.Where(a => !a.isUnlocked).ToList();
        }

        /// <summary>
        /// Retorna o progresso total (porcentagem de conquistas desbloqueadas).
        /// </summary>
        public float GetProgress()
        {
            int total = _achievements.Count;
            int unlocked = GetUnlockedAchievements().Count;
            return total > 0 ? (float)unlocked / total : 0f;
        }

        /// <summary>
        /// Calcula total de moedas ganhas por conquistas.
        /// </summary>
        public int GetTotalCoinsFromAchievements()
        {
            return GetUnlockedAchievements().Sum(a => a.coinReward);
        }

        /// <summary>
        /// Salva o progresso das conquistas.
        /// </summary>
        private void SaveProgress()
        {
            var saveData = new AchievementSaveData
            {
                unlockedTypes = GetUnlockedAchievements().Select(a => (int)a.type).ToList(),
                unlockedDates = GetUnlockedAchievements().Select(a => a.unlockedDate).ToList()
            };

            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Carrega o progresso das conquistas.
        /// </summary>
        private void LoadProgress()
        {
            if (!PlayerPrefs.HasKey(SAVE_KEY))
                return;

            try
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                var  saveData = JsonUtility.FromJson<AchievementSaveData>(json);

                for (int i = 0; i < saveData.unlockedTypes.Count; i++)
                {
                    var type = (AchievementType)saveData.unlockedTypes[i];
                    if (_achievements.ContainsKey(type))
                    {
                        _achievements[type].isUnlocked = true;
                        _achievements[type].unlockedDate = 
                            i < saveData.unlockedDates.Count ? saveData.unlockedDates[i] : "";
                    }
                }

                Debug.Log($"Conquistas carregadas: {saveData.unlockedTypes.Count} desbloqueadas");
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao carregar conquistas: {e.Message}");
            }
        }

        /// <summary>
        /// Reseta todas as conquistas (debug/teste).
        /// </summary>
        public void ResetAll()
        {
            foreach (var achievement in _achievements.Values)
            {
                achievement.isUnlocked = false;
                achievement.unlockedDate = "";
            }

            SaveProgress();
            Debug.Log("Todas as conquistas foram resetadas");
        }
    }

    /// <summary>
    /// Estrutura de dados para salvar conquistas.
    /// </summary>
    [Serializable]
    public class AchievementSaveData
    {
        public List<int> unlockedTypes = new List<int>();
        public List<string> unlockedDates = new List<string>();
    }
}
