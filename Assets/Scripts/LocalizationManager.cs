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

            // Títulos dos mundos (HUD)
            AddTranslation("world1_title", "Mundo 1 — Noé e a Arca", "World 1 — Noah and the Ark", "Mundo 1 — Noé y el Arca");
            AddTranslation("world2_title", "Mundo 2 — Davi e Golias", "World 2 — David and Goliath", "Mundo 2 — David y Goliat");
            AddTranslation("world3_title", "Mundo 3 — Jonas (Puzzle)", "World 3 — Jonah (Puzzle)", "Mundo 3 — Jonás (Rompecabezas)");
            AddTranslation("world4_title", "Mundo 4 — Moisés (Puzzle)", "World 4 — Moses (Puzzle)", "Mundo 4 — Moisés (Rompecabezas)");
            AddTranslation("world5_title", "Mundo 5 — Jesus (Coleta)", "World 5 — Jesus (Collect)", "Mundo 5 — Jesús (Colección)");

            // Quiz
            AddTranslation("quiz_title", "Mini-Desafio", "Mini-Challenge", "Mini-Desafío");
            AddTranslation("quiz_noe", "Deus chamou Noé para fazer o quê?", "What did God call Noah to do?", "¿Qué le pidió Dios a Noé?");
            AddTranslation("quiz_noe_a1", "Construir a arca", "Build the ark", "Construir el arca");
            AddTranslation("quiz_noe_a2", "Plantar uma horta", "Plant a garden", "Plantar un huerto");
            AddTranslation("quiz_noe_a3", "Virar rei", "Become king", "Ser rey");
            AddTranslation("virtue_obedience", "Obediência", "Obedience", "Obediencia");

            // Mensagens de gameplay
            AddTranslation("hit_obstacle", "Você esbarrou num obstáculo. Tente de novo 😊", "You hit an obstacle. Try again 😊", "Chocaste con un obstáculo. ¡Inténtalo de nuevo 😊!");
            AddTranslation("boss_win_msg", "Você venceu com coragem e estratégia! 🌟\nVirtudes: Coragem + Fé", "You won with courage and strategy! 🌟\nVirtues: Courage + Faith", "¡Ganaste con coraje y estrategia! 🌟\nVirtudes: Coraje + Fe");
            AddTranslation("boss_fail_msg", "Golias te assustou, mas você pode tentar de novo 💪", "Goliath scared you, but you can try again 💪", "Goliat te asustó, pero puedes intentarlo de nuevo 💪");
            AddTranslation("great_job", "Muito bem!", "Great job!", "¡Muy bien!");
            AddTranslation("jonas_win_msg", "Você colocou a história na ordem certa!\nVirtude: Obediência", "You put the story in the right order!\nVirtue: Obedience", "¡Pusiste la historia en el orden correcto!\nVirtud: Obediencia");
            AddTranslation("puzzle_fail", "Não foi essa ordem. Tente de novo 😊", "That wasn't the right order. Try again 😊", "No fue ese orden. ¡Inténtalo de nuevo 😊!");
            AddTranslation("amazing", "Incrível!", "Amazing!", "¡Increíble!");
            AddTranslation("moises_win_msg", "Você lembrou a sequência do milagre!\nVirtude: Fé", "You remembered the miracle sequence!\nVirtue: Faith", "¡Recordaste la secuencia del milagro!\nVirtud: Fe");
            AddTranslation("jesus_win_msg", "Você coletou amor e bondade no caminho 💗\nVirtudes: Amor + Bondade", "You collected love and kindness along the way 💗\nVirtues: Love + Kindness", "Recolectaste amor y bondad en el camino 💗\nVirtudes: Amor + Bondad");

            // Mundos bloqueados
            AddTranslation("complete_prev_1", "Complete o Mundo 1 primeiro.", "Complete World 1 first.", "Completa el Mundo 1 primero.");
            AddTranslation("complete_prev_2", "Complete o Mundo 2 primeiro.", "Complete World 2 first.", "Completa el Mundo 2 primero.");
            AddTranslation("complete_prev_3", "Complete o Mundo 3 primeiro.", "Complete World 3 first.", "Completa el Mundo 3 primero.");
            AddTranslation("complete_prev_4", "Complete o Mundo 4 primeiro.", "Complete World 4 first.", "Completa el Mundo 4 primero.");

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
