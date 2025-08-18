using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : Singleton<FadeManager>
{
    private const float BLINK_START_Y = 1000f;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private float _blinkDuration;
    [SerializeField] private float _slowEyesDuration;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private RectTransform _blinkTop;
    [SerializeField] private RectTransform _blinkBottom;

    public Coroutine FadeOutAndDo(Action onComplete = null) => StartCoroutine(Fade(0f, 1f, _fadeDuration, onComplete));
    public Coroutine FadeInAndDo(Action onComplete = null) => StartCoroutine(Fade(1f, 0f, _fadeDuration, onComplete));
    public Coroutine BlinkAndDo(Action duringBlink = null) => StartCoroutine(BlinkSequence(_blinkDuration, duringBlink));
    public Coroutine CloseEyesAndDo(Action onComplete = null) => StartCoroutine(Blink(true, _slowEyesDuration, onComplete));
    public Coroutine OpenEyesAndDo(Action onComplete = null) => StartCoroutine(Blink(false, _slowEyesDuration, onComplete));
    public void SetEyesOpen() => SetEyesOpen(true);
    public void SetEyesClosed() => SetEyesOpen(false);

    private IEnumerator Fade(float from, float to, float duration, Action onComplete = null)
    {
        yield return Interpolation.Interpolate(
            onStart: null,
            tween: t =>
            {
                float alpha = Mathf.Lerp(from, to, t);
                _fadeImage.color = new Color(0, 0, 0, alpha);
            },
            onComplete: onComplete,
            duration: duration
        );
    }

    private IEnumerator BlinkSequence(float duration, Action duringBlink)
    {
        // Close eyes
        yield return Blink(true, duration, () =>
        {
            duringBlink?.Invoke();
            // Reopen eyes
            StartCoroutine(Blink(false, duration));
        });
    }

    private IEnumerator Blink(bool closing, float duration, Action onComplete = null)
    {
        // Fade while blinking
        StartCoroutine(Fade(closing ? 0f : 0.75f, closing ? 0.75f : 0f, duration));

        float from = closing ? 0f : BLINK_START_Y;
        float to = closing ? BLINK_START_Y : 0f;
        Func<float, float, float, float> easing = closing ? Interpolation.EaseInQuart : Interpolation.EaseOutQuart;

        yield return Interpolation.Interpolate(
            onStart: null,
            tween: t =>
            {
                float y = easing(from, to, t);
                _blinkTop.anchoredPosition = new Vector2(0, -y);
                _blinkBottom.anchoredPosition = new Vector2(0, y);
            },
            onComplete: onComplete,
            duration: duration
        );
    }

    private void SetEyesOpen(bool isOpen)
    {
        float a = isOpen ? 0f : 0.75f;
        float y = isOpen ? 0f : BLINK_START_Y;
        _fadeImage.color = new(0, 0, 0, a);
        _blinkTop.anchoredPosition = new(0, -y);
        _blinkBottom.anchoredPosition = new(0, y);
    }
}
