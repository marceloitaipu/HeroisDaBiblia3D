using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Sistema de animações de UI com fade, scale e slide.
    /// Fornece transições suaves entre telas e feedback visual em botões.
    /// </summary>
    public sealed class UIAnimator : MonoBehaviour
    {
        #region Configurações

        /// <summary>Duração padrão do fade em segundos.</summary>
        public float fadeDuration = 0.3f;

        /// <summary>Duração padrão do slide em segundos.</summary>
        public float slideDuration = 0.4f;

        /// <summary>Escala de botão ao pressionar.</summary>
        public float buttonPressScale = 0.93f;

        /// <summary>Duração da animação de botão.</summary>
        public float buttonAnimDuration = 0.12f;

        #endregion

        #region Fade

        /// <summary>
        /// Aplica fade-in em um painel (alpha 0 → 1).
        /// Adiciona CanvasGroup automaticamente se não existir.
        /// </summary>
        /// <param name="panel">GameObject do painel.</param>
        /// <param name="duration">Duração do fade (0 = usa padrão).</param>
        /// <param name="onComplete">Callback ao finalizar.</param>
        public void FadeIn(GameObject panel, float duration = 0f, Action onComplete = null)
        {
            if (panel == null) return;
            if (duration <= 0f) duration = fadeDuration;

            panel.SetActive(true);
            var cg = EnsureCanvasGroup(panel);
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            StartCoroutine(FadeRoutine(cg, 0f, 1f, duration, () =>
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;
                onComplete?.Invoke();
            }));
        }

        /// <summary>
        /// Aplica fade-out em um painel (alpha 1 → 0) e desativa ao final.
        /// </summary>
        /// <param name="panel">GameObject do painel.</param>
        /// <param name="duration">Duração do fade (0 = usa padrão).</param>
        /// <param name="onComplete">Callback ao finalizar.</param>
        public void FadeOut(GameObject panel, float duration = 0f, Action onComplete = null)
        {
            if (panel == null || !panel.activeSelf) return;
            if (duration <= 0f) duration = fadeDuration;

            var cg = EnsureCanvasGroup(panel);
            cg.interactable = false;
            cg.blocksRaycasts = false;

            StartCoroutine(FadeRoutine(cg, cg.alpha, 0f, duration, () =>
            {
                panel.SetActive(false);
                cg.alpha = 1f; // Restaura para próximo uso
                onComplete?.Invoke();
            }));
        }

        /// <summary>
        /// Troca dois painéis com crossfade (fade-out do atual, fade-in do novo).
        /// </summary>
        /// <param name="from">Painel a esconder.</param>
        /// <param name="to">Painel a mostrar.</param>
        /// <param name="duration">Duração total da transição.</param>
        /// <param name="onComplete">Callback ao finalizar.</param>
        public void CrossFade(GameObject from, GameObject to, float duration = 0f, Action onComplete = null)
        {
            if (duration <= 0f) duration = fadeDuration;

            float halfDur = duration * 0.5f;

            FadeOut(from, halfDur, () =>
            {
                FadeIn(to, halfDur, onComplete);
            });
        }

        private IEnumerator FadeRoutine(CanvasGroup cg, float from, float to, float duration, Action onComplete)
        {
            float elapsed = 0f;
            cg.alpha = from;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime; // Funciona mesmo com timeScale=0 (pause)
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = EaseOutQuad(t);
                cg.alpha = Mathf.Lerp(from, to, eased);
                yield return null;
            }

            cg.alpha = to;
            onComplete?.Invoke();
        }

        #endregion

        #region Scale (Botões)

        /// <summary>
        /// Animação de pressionar botão (scale down e volta).
        /// </summary>
        /// <param name="buttonTransform">Transform do botão.</param>
        /// <param name="onComplete">Callback após a animação.</param>
        public void ButtonPress(Transform buttonTransform, Action onComplete = null)
        {
            if (buttonTransform == null) return;
            StartCoroutine(ButtonPressRoutine(buttonTransform, onComplete));
        }

        private IEnumerator ButtonPressRoutine(Transform t, Action onComplete)
        {
            Vector3 original = Vector3.one;
            Vector3 pressed = Vector3.one * buttonPressScale;
            float half = buttonAnimDuration * 0.5f;

            // Scale down
            float elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / half);
                t.localScale = Vector3.Lerp(original, pressed, EaseOutQuad(progress));
                yield return null;
            }

            // Scale up
            elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / half);
                t.localScale = Vector3.Lerp(pressed, original, EaseOutQuad(progress));
                yield return null;
            }

            t.localScale = original;
            onComplete?.Invoke();
        }

        #endregion

        #region Slide

        /// <summary>
        /// Slide-in de um painel (desliza de uma direção).
        /// </summary>
        /// <param name="panel">GameObject do painel.</param>
        /// <param name="fromOffset">Offset inicial (ex: new Vector2(0, -500) = slide de baixo).</param>
        /// <param name="duration">Duração da animação.</param>
        /// <param name="onComplete">Callback ao finalizar.</param>
        public void SlideIn(GameObject panel, Vector2 fromOffset, float duration = 0f, Action onComplete = null)
        {
            if (panel == null) return;
            if (duration <= 0f) duration = slideDuration;

            panel.SetActive(true);
            var rt = panel.GetComponent<RectTransform>();
            if (rt == null) return;

            Vector2 target = rt.anchoredPosition;
            rt.anchoredPosition = target + fromOffset;

            StartCoroutine(SlideRoutine(rt, rt.anchoredPosition, target, duration, onComplete));
        }

        private IEnumerator SlideRoutine(RectTransform rt, Vector2 from, Vector2 to, float duration, Action onComplete)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = EaseOutQuad(t);
                rt.anchoredPosition = Vector2.Lerp(from, to, eased);
                yield return null;
            }

            rt.anchoredPosition = to;
            onComplete?.Invoke();
        }

        #endregion

        #region Stagger (Entrada sequencial de elementos)

        /// <summary>
        /// Anima elementos filhos sequencialmente com delay entre cada um.
        /// </summary>
        /// <param name="parent">Transform pai cujos filhos serão animados.</param>
        /// <param name="staggerDelay">Delay entre elementos adjacentes.</param>
        /// <param name="duration">Duração de cada animação individual.</param>
        public void StaggerFadeIn(Transform parent, float staggerDelay = 0.05f, float duration = 0.2f)
        {
            if (parent == null) return;
            StartCoroutine(StaggerRoutine(parent, staggerDelay, duration));
        }

        private IEnumerator StaggerRoutine(Transform parent, float staggerDelay, float duration)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                var cg = EnsureCanvasGroup(child);
                cg.alpha = 0f;
                StartCoroutine(FadeRoutine(cg, 0f, 1f, duration, null));
                yield return new WaitForSecondsRealtime(staggerDelay);
            }
        }

        #endregion

        #region Notification (Toast)

        /// <summary>
        /// Mostra uma notificação temporária no topo da tela.
        /// </summary>
        /// <param name="canvas">Canvas onde criar a notificação.</param>
        /// <param name="message">Texto da notificação.</param>
        /// <param name="displayTime">Tempo de exibição em segundos.</param>
        /// <param name="bgColor">Cor de fundo.</param>
        public void ShowNotification(Canvas canvas, string message, float displayTime = 2.5f, Color? bgColor = null)
        {
            if (canvas == null) return;
            StartCoroutine(NotificationRoutine(canvas, message, displayTime, bgColor ?? new Color(0.20f, 0.75f, 0.30f, 0.95f)));
        }

        private IEnumerator NotificationRoutine(Canvas canvas, string message, float displayTime, Color bgColor)
        {
            // Cria o container da notificação
            var notif = new GameObject("Notification");
            notif.transform.SetParent(canvas.transform, false);
            var img = notif.AddComponent<Image>();
            img.color = bgColor;
            var rt = notif.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.05f, 0.88f);
            rt.anchorMax = new Vector2(0.95f, 0.95f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            // Texto
            var txtGo = new GameObject("Text");
            txtGo.transform.SetParent(notif.transform, false);
            var txt = txtGo.AddComponent<Text>();
            txt.text = message;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.fontSize = 22;
            txt.fontStyle = FontStyle.Bold;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            var trt = txtGo.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = trt.offsetMax = Vector2.zero;

            // Sombra
            notif.AddComponent<Shadow>().effectDistance = new Vector2(0, -3);

            // Fade in
            var cg = notif.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            yield return FadeRoutine(cg, 0f, 1f, 0.25f, null);

            // Espera
            yield return new WaitForSecondsRealtime(displayTime);

            // Fade out e destroi
            yield return FadeRoutine(cg, 1f, 0f, 0.25f, null);
            Destroy(notif);
        }

        #endregion

        #region Utilidades

        /// <summary>
        /// Garante que o GameObject tem um CanvasGroup.
        /// </summary>
        private CanvasGroup EnsureCanvasGroup(GameObject go)
        {
            var cg = go.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = go.AddComponent<CanvasGroup>();
            return cg;
        }

        /// <summary>
        /// Easing function: suaviza final da animação.
        /// </summary>
        private static float EaseOutQuad(float t)
        {
            return 1f - (1f - t) * (1f - t);
        }

        #endregion
    }
}
