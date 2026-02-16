using System.Collections.Generic;
using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Idiomas suportados pelo jogo.
    /// </summary>
    public enum Language
    {
        Portuguese,
        English,
        Spanish
    }

    /// <summary>
    /// Gerenciador de localização/internacionalização do jogo.
    /// Permite tradução de textos para múltiplos idiomas.
    /// </summary>
    public sealed class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        [Header("Configuração")]
        [Tooltip("Idioma atual")]
        public Language currentLanguage = Language.Portuguese;

        [Tooltip("Detecta idioma do sistema automaticamente")]
        public bool autoDetectLanguage = true;

        private Dictionary<string, Dictionary<Language, string>> _translations;
        private const string LANGUAGE_SAVE_KEY = "HBB3D_LANGUAGE";

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeTranslations();
            LoadLanguagePreference();
        }

        /// <summary>
        /// Inicializa todas as traduções do jogo.
        /// </summary>
        private void InitializeTranslations()
        {
            _translations = new Dictionary<string, Dictionary<Language, string>>();

            // UI Principal
            AddTranslation("game_title", "Heróis da Bíblia 3D", "Bible Heroes 3D", "Héroes de la Biblia 3D");
            AddTranslation("play", "JOGAR", "PLAY", "JUGAR");
            AddTranslation("hero_skin", "HERÓI / SKIN", "HERO / SKIN", "HÉROE / SKIN");
            AddTranslation("shop", "LOJA", "SHOP", "TIENDA");
            AddTranslation("reset_progress", "ZERAR PROGRESSO", "RESET PROGRESS", "REINICIAR PROGRESO");
            AddTranslation("back", "Voltar", "Back", "Volver");
            AddTranslation("continue", "CONTINUAR", "CONTINUE", "CONTINUAR");
            AddTranslation("ok", "OK", "OK", "OK");
            AddTranslation("yes", "Sim", "Yes", "Sí");
            AddTranslation("no", "Não", "No", "No");

            // Mundos
            AddTranslation("world_map", "Mapa de Mundos", "World Map", "Mapa de Mundos");
            AddTranslation("world1", "1 — Noé (Runner)", "1 — Noah (Runner)", "1 — Noé (Corredor)");
            AddTranslation("world2", "2 — Davi x Golias (Boss)", "2 — David vs Goliath (Boss)", "2 — David vs Goliat (Jefe)");
            AddTranslation("world3", "3 — Jonas (Puzzle)", "3 — Jonah (Puzzle)", "3 — Jonás (Rompecabezas)");
            AddTranslation("world4", "4 — Moisés (Puzzle)", "4 — Moses (Puzzle)", "4 — Moisés (Rompecabezas)");
            AddTranslation("world5", "5 — Jesus (Coleta)", "5 — Jesus (Collect)", "5 — Jesús (Colección)");

            // Moedas e Progresso
            AddTranslation("coins", "Moedas", "Coins", "Monedas");
            AddTranslation("virtues", "Virtudes", "Virtues", "Virtudes");
            AddTranslation("locked", "🔒 Bloqueado", "🔒 Locked", "🔒 Bloqueado");
            AddTranslation("complete_prev", "Complete o Mundo {0} primeiro.", "Complete World {0} first.", "Completa el Mundo {0} primero.");

            // Mensagens de Vitória/Derrota
            AddTranslation("victory", "Vitória!", "Victory!", "¡Victoria!");
            AddTranslation("congratulations", "Parabéns!", "Congratulations!", "¡Felicitaciones!");
            AddTranslation("try_again", "Quase!", "Almost!", "¡Casi!");
            AddTranslation("oops", "Ops!", "Oops!", "¡Ups!");

            // Ações
            AddTranslation("jump", "Pular", "Jump", "Saltar");
            AddTranslation("slide", "Deslizar", "Slide", "Deslizar");
            AddTranslation("action", "Ação", "Action", "Acción");
            AddTranslation("start", "COMEÇAR", "START", "COMENZAR");
            AddTranslation("retry", "Tentar de novo", "Try again", "Intentar de nuevo");
            AddTranslation("restart", "Recomeçar", "Restart", "Reiniciar");
            AddTranslation("map", "Mapa", "Map", "Mapa");

            // Loja
            AddTranslation("buy_blue_skin", "Comprar Skin Azul", "Buy Blue Skin", "Comprar Skin Azul");
            AddTranslation("buy_purple_skin", "Comprar Skin Roxa", "Buy Purple Skin", "Comprar Skin Púrpura");
            AddTranslation("skin_locked", "Skin bloqueada", "Skin locked", "Skin bloqueada");
            AddTranslation("buy_first", "Compre a skin na loja primeiro.", "Buy the skin in the shop first.", "Compra la skin en la tienda primero.");
            AddTranslation("already_own", "Já possui", "Already own", "Ya posees");
            AddTranslation("you_have_skin", "Você já tem essa skin ✅", "You already have this skin ✅", "Ya tienes esta skin ✅");
            AddTranslation("not_enough_coins", "Moedas insuficientes", "Not enough coins", "Monedas insuficientes");
            AddTranslation("need_coins", "Você precisa de {0} moedas.", "You need {0} coins.", "Necesitas {0} monedas.");
            AddTranslation("purchased", "Comprado!", "Purchased!", "¡Comprado!");
            AddTranslation("skin_unlocked", "Skin liberada ✅", "Skin unlocked ✅", "Skin desbloqueada ✅");

            // Herói
            AddTranslation("hero", "Herói", "Hero", "Héroe");
            AddTranslation("hero_theo", "Theo", "Theo", "Theo");
            AddTranslation("hero_lia", "Lia", "Lia", "Lia");
            AddTranslation("hero_nina", "Nina", "Nina", "Nina");
            AddTranslation("skin_basic", "Skin Básica", "Basic Skin", "Skin Básica");
            AddTranslation("skin_blue", "Skin Azul", "Blue Skin", "Skin Azul");
            AddTranslation("skin_purple", "Skin Roxa", "Purple Skin", "Skin Púrpura");

            // Conquistas
            AddTranslation("achievements", "Conquistas", "Achievements", "Logros");
            AddTranslation("achievement_unlocked", "🏆 Conquista Desbloqueada!", "🏆 Achievement Unlocked!", "¡🏆 Logro Desbloqueado!");
            AddTranslation("progress", "Progresso", "Progress", "Progreso");
        }

        /// <summary>
        /// Adiciona uma tradução para todas as línguas.
        /// </summary>
        private void AddTranslation(string key, string pt, string en, string es)
        {
            var translations = new Dictionary<Language, string>
            {
                { Language.Portuguese, pt },
                { Language.English, en },
                { Language.Spanish, es }
            };

            _translations[key] = translations;
        }

        /// <summary>
        /// Obtém uma string traduzida para o idioma atual.
        /// </summary>
        /// <param name="key">Chave da tradução.</param>
        /// <param name="args">Argumentos para formatação (opcional).</param>
        /// <returns>String traduzida ou a chave se não encontrada.</returns>
        public string GetString(string key, params object[] args)
        {
            if (!_translations.ContainsKey(key))
            {
                Debug.LogWarning($"Tradução não encontrada para chave: {key}");
                return key;
            }

            var languageTranslations = _translations[key];

            if (!languageTranslations.ContainsKey(currentLanguage))
            {
                Debug.LogWarning($"Idioma {currentLanguage} não tem tradução para: {key}");
                return key;
            }

            string translation = languageTranslations[currentLanguage];

            // Aplica formatação se houver argumentos
            if (args != null && args.Length > 0)
            {
                try
                {
                    translation = string.Format(translation, args);
                }
                catch
                {
                    Debug.LogError($"Erro ao formatar tradução: {key}");
                }
            }

            return translation;
        }

        /// <summary>
        /// Define o idioma atual.
        /// </summary>
        public void SetLanguage(Language language)
        {
            currentLanguage = language;
            SaveLanguagePreference();
            Debug.Log($"Idioma alterado para: {language}");
        }

        /// <summary>
        /// Carrega a preferência de idioma salva.
        /// </summary>
        private void LoadLanguagePreference()
        {
            if (autoDetectLanguage && !PlayerPrefs.HasKey(LANGUAGE_SAVE_KEY))
            {
                // Detecta idioma do sistema
                SystemLanguage systemLang = Application.systemLanguage;
                
                currentLanguage = systemLang switch
                {
                    SystemLanguage.Portuguese => Language.Portuguese,
                    SystemLanguage.Spanish => Language.Spanish,
                    _ => Language.English
                };

                Debug.Log($"Idioma detectado automaticamente: {currentLanguage}");
            }
            else if (PlayerPrefs.HasKey(LANGUAGE_SAVE_KEY))
            {
                int savedLang = PlayerPrefs.GetInt(LANGUAGE_SAVE_KEY);
                currentLanguage = (Language)savedLang;
            }
        }

        /// <summary>
        /// Salva a preferência de idioma.
        /// </summary>
        private void SaveLanguagePreference()
        {
            PlayerPrefs.SetInt(LANGUAGE_SAVE_KEY, (int)currentLanguage);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Obtém o idioma atual.
        /// </summary>
        public Language GetCurrentLanguage() => currentLanguage;
    }
}
