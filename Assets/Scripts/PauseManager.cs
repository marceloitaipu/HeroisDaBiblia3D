using System;
using UnityEngine;
using UnityEngine.UI;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Gerenciador de pause com overlay visual e controles de volume.
    /// Bloqueia o gameplay quando ativo (Time.timeScale = 0).
    /// </summary>
    public sealed class PauseManager : MonoBehaviour
    {
        /// <summary>Se o jogo está pausado.</summary>
        public bool IsPaused { get; private set; }

        /// <summary>Callback ao clicar em Restart.</summary>
        public Action onRestart;

        /// <summary>Callback ao clicar em Quit to Menu.</summary>
        public Action onQuit;

        private Canvas _canvas;
        private GameObject _pausePanel;
        private UIAnimator _animator;
        private Slider _musicSlider;
        private Slider _sfxSlider;

        /// <summary>
        /// Inicializa o sistema de pause com referências necessárias.
        /// </summary>
        /// <param name="canvas">Canvas raiz da UI.</param>
        /// <param name="animator">Animator de UI para transições.</param>
        public void Initialize(Canvas canvas, UIAnimator animator)
        {
            _canvas = canvas;
            _animator = animator;
            BuildPauseUI();
        }

        /// <summary>
        /// Alterna o estado de pause.
        /// </summary>
        public void TogglePause()
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }

        /// <summary>
        /// Pausa o jogo.
        /// </summary>
        public void Pause()
        {
            if (IsPaused) return;
            IsPaused = true;
            Time.timeScale = 0f;

            if (_animator != null)
                _animator.FadeIn(_pausePanel, 0.2f);
            else
                _pausePanel.SetActive(true);
        }

        /// <summary>
        /// Resume o jogo.
        /// </summary>
        public void Resume()
        {
            if (!IsPaused) return;
            IsPaused = false;
            Time.timeScale = 1f;

            if (_animator != null)
                _animator.FadeOut(_pausePanel, 0.2f);
            else
                _pausePanel.SetActive(false);
        }

        private void BuildPauseUI()
        {
            // Painel de fundo escuro
            _pausePanel = new GameObject("PausePanel");
            _pausePanel.transform.SetParent(_canvas.transform, false);
            var img = _pausePanel.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0.75f);
            var rt = _pausePanel.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            // Card central
            var card = new GameObject("Card");
            card.transform.SetParent(_pausePanel.transform, false);
            var cardImg = card.AddComponent<Image>();
            cardImg.color = new Color(1, 1, 1, 0.97f);
            var cardRt = card.GetComponent<RectTransform>();
            cardRt.anchorMin = cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.sizeDelta = new Vector2(800, 1000);
            card.AddComponent<Shadow>().effectDistance = new Vector2(0, -6);

            // Título
            CreateText(card.transform, "Title", "⏸ PAUSADO", 40, FontStyle.Bold,
                new Vector2(0.1f, 0.85f), new Vector2(0.9f, 0.95f));

            // Label Música
            CreateText(card.transform, "MusicLabel", "🎵 Música", 22, FontStyle.Normal,
                new Vector2(0.1f, 0.72f), new Vector2(0.9f, 0.78f));

            // Slider Música
            _musicSlider = CreateSlider(card.transform, "MusicSlider",
                new Vector2(0.1f, 0.65f), new Vector2(0.9f, 0.72f), 0.7f);
            _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            // Label SFX
            CreateText(card.transform, "SfxLabel", "🔊 Efeitos Sonoros", 22, FontStyle.Normal,
                new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.61f));

            // Slider SFX
            _sfxSlider = CreateSlider(card.transform, "SfxSlider",
                new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.55f), 0.9f);
            _sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

            // Qualidade label
            CreateText(card.transform, "QualityLabel", "📊 Qualidade Gráfica", 22, FontStyle.Normal,
                new Vector2(0.1f, 0.38f), new Vector2(0.9f, 0.44f));

            // Botões de qualidade
            CreateSmallButton(card.transform, "LowQ", "Baixa", () => SetQuality(QualityManager.QualityTier.Low),
                new Vector2(0.12f, 0.32f), new Vector2(0.36f, 0.38f));
            CreateSmallButton(card.transform, "MedQ", "Média", () => SetQuality(QualityManager.QualityTier.Medium),
                new Vector2(0.38f, 0.32f), new Vector2(0.62f, 0.38f));
            CreateSmallButton(card.transform, "HighQ", "Alta", () => SetQuality(QualityManager.QualityTier.High),
                new Vector2(0.64f, 0.32f), new Vector2(0.88f, 0.38f));

            // Botão Continuar
            CreateButton(card.transform, "Resume", "▶ CONTINUAR", () => Resume(),
                new Vector2(0.15f, 0.19f), new Vector2(0.85f, 0.28f),
                new Color(0.20f, 0.75f, 0.30f));

            // Botão Recomeçar
            CreateButton(card.transform, "Restart", "🔄 RECOMEÇAR", () => { Resume(); onRestart?.Invoke(); },
                new Vector2(0.15f, 0.10f), new Vector2(0.85f, 0.19f),
                new Color(0.95f, 0.65f, 0.20f));

            // Botão Sair
            CreateButton(card.transform, "Quit", "🏠 SAIR PARA MENU", () => { Resume(); onQuit?.Invoke(); },
                new Vector2(0.15f, 0.02f), new Vector2(0.85f, 0.10f),
                new Color(0.85f, 0.25f, 0.25f));

            _pausePanel.SetActive(false);
        }

        private Text CreateText(Transform parent, string name, string value, int fontSize, FontStyle style,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var txt = go.AddComponent<Text>();
            txt.text = value;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.fontSize = fontSize;
            txt.fontStyle = style;
            txt.color = new Color(0.12f, 0.12f, 0.12f);
            txt.alignment = TextAnchor.MiddleCenter;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            return txt;
        }

        private Slider CreateSlider(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, float defaultValue)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            // Background
            var bgGo = new GameObject("Background");
            bgGo.transform.SetParent(go.transform, false);
            var bgImg = bgGo.AddComponent<Image>();
            bgImg.color = new Color(0.85f, 0.85f, 0.85f);
            var bgRt = bgGo.GetComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(0, 0.35f);
            bgRt.anchorMax = new Vector2(1, 0.65f);
            bgRt.offsetMin = bgRt.offsetMax = Vector2.zero;

            // Fill area
            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(go.transform, false);
            var fillRt = fillArea.AddComponent<RectTransform>();
            fillRt.anchorMin = new Vector2(0, 0.35f);
            fillRt.anchorMax = new Vector2(1, 0.65f);
            fillRt.offsetMin = fillRt.offsetMax = Vector2.zero;

            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fillImg = fill.AddComponent<Image>();
            fillImg.color = new Color(0.20f, 0.45f, 0.95f);
            var fillFillRt = fill.GetComponent<RectTransform>();
            fillFillRt.anchorMin = Vector2.zero;
            fillFillRt.anchorMax = Vector2.one;
            fillFillRt.offsetMin = fillFillRt.offsetMax = Vector2.zero;

            // Handle
            var handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(go.transform, false);
            var handleAreaRt = handleArea.AddComponent<RectTransform>();
            handleAreaRt.anchorMin = Vector2.zero;
            handleAreaRt.anchorMax = Vector2.one;
            handleAreaRt.offsetMin = handleAreaRt.offsetMax = Vector2.zero;

            var handle = new GameObject("Handle");
            handle.transform.SetParent(handleArea.transform, false);
            var handleImg = handle.AddComponent<Image>();
            handleImg.color = new Color(0.20f, 0.45f, 0.95f);
            var handleRt = handle.GetComponent<RectTransform>();
            handleRt.sizeDelta = new Vector2(30, 30);

            var slider = go.AddComponent<Slider>();
            slider.fillRect = fillFillRt;
            slider.handleRect = handleRt;
            slider.targetGraphic = handleImg;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = defaultValue;

            return slider;
        }

        private Button CreateButton(Transform parent, string name, string label, Action onClick,
            Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            var btn = go.AddComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                AudioManager.I?.Beep();
                onClick?.Invoke();
            });

            var txtGo = new GameObject("Label");
            txtGo.transform.SetParent(go.transform, false);
            var txt = txtGo.AddComponent<Text>();
            txt.text = label;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.fontSize = 24;
            txt.fontStyle = FontStyle.Bold;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            var trt = txtGo.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = trt.offsetMax = Vector2.zero;

            return btn;
        }

        private Button CreateSmallButton(Transform parent, string name, string label, Action onClick,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            return CreateButton(parent, name, label, onClick, anchorMin, anchorMax,
                new Color(0.45f, 0.45f, 0.55f));
        }

        private void OnMusicVolumeChanged(float value)
        {
            if (AudioManager.I != null)
            {
                var sfx = AudioManager.I.GetComponent<AudioSource>();
                if (sfx != null) sfx.volume = value;
            }

            if (AdvancedAudioManager.Instance != null)
                AdvancedAudioManager.Instance.SetMusicVolume(value);
        }

        private void OnSfxVolumeChanged(float value)
        {
            if (AdvancedAudioManager.Instance != null)
                AdvancedAudioManager.Instance.SetSFXVolume(value);
        }

        private void SetQuality(QualityManager.QualityTier tier)
        {
            if (QualityManager.Instance != null)
                QualityManager.Instance.ApplyQualitySettings(tier);
        }

        void Update()
        {
            // ESC ou botão de pause físico
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        void OnDestroy()
        {
            // Garante que o timeScale volta ao normal
            if (IsPaused)
                Time.timeScale = 1f;
        }
    }
}
