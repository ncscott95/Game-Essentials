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

    public Coroutine FadeOutAndDo(Action onComplete = null) => StartCoroutine(FadeCoroutine(true, onComplete));
    public Coroutine FadeInAndDo(Action onComplete = null) => StartCoroutine(FadeCoroutine(false, onComplete));
    public Coroutine BlinkAndDo(Action duringBlink = null) => StartCoroutine(BlinkCoroutine(duringBlink));
    public Coroutine CloseEyesAndDo(Action onComplete = null) => StartCoroutine(SlowEyesCoroutine(true, onComplete));
    public Coroutine OpenEyesAndDo(Action onComplete = null) => StartCoroutine(SlowEyesCoroutine(false, onComplete));
    public void SetEyesOpen() => SetEyesOpen(true);
    public void SetEyesClosed() => SetEyesOpen(false);

    private IEnumerator FadeCoroutine(bool fadeOut, Action onComplete)
    {
        float from = fadeOut ? 0f : 1f;
        float to = fadeOut ? 1f : 0f;
        yield return Fade(from, to, _fadeDuration);
        onComplete?.Invoke();
    }

    private IEnumerator BlinkCoroutine(Action duringBlink)
    {
        yield return Blink(true, _blinkDuration);
        duringBlink?.Invoke();
        yield return new WaitForSeconds(0.1f);
        yield return Blink(false, _blinkDuration);
    }

    private IEnumerator SlowEyesCoroutine(bool closeEyes, Action onComplete)
    {
        yield return Blink(closeEyes, _slowEyesDuration);
        onComplete?.Invoke();
    }

    private IEnumerator Animate(float from, float to, float duration, Action<float> onUpdate, Func<float, float> easing = null)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            if (easing != null)
            {
                t = easing(t);
            }
            float value = Mathf.Lerp(from, to, t);
            onUpdate(value);
            yield return new WaitForEndOfFrame();
        }
        onUpdate(to);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        yield return Animate(from, to, duration, alpha =>
        {
            _fadeImage.color = new(0, 0, 0, alpha);
        });
    }

    private IEnumerator Blink(bool closing, float duration)
    {
        if (closing) StartCoroutine(Fade(0f, 0.75f, duration));
        else StartCoroutine(Fade(0.75f, 0f, duration));

        float from = closing ? 0f : BLINK_START_Y;
        float to = closing ? BLINK_START_Y : 0f;

        Func<float, float> easing;
        if (closing) easing = t => t * t * t * t; // Quartic In
        else easing = t => 1 - Mathf.Pow(1 - t, 4); // Quartic Out

        yield return Animate(from, to, duration, y =>
        {
            _blinkTop.anchoredPosition = new(0, -y);
            _blinkBottom.anchoredPosition = new(0, y);
        }, easing);
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
