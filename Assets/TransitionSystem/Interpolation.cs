using System;
using System.Collections;
using UnityEngine;


// All functions from https://easings.net
public static class Interpolation
{
    public static IEnumerator Interpolate(Action onStart, Action<float> tween, Action onComplete, float duration)
    {
        onStart();

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            tween(t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        tween(1f);
        onComplete();
    }

    // --- Sine ---
    public static float EaseInSine(float from, float to, float t)
    {
        t = 1 - Mathf.Cos(t * Mathf.PI / 2);
        return from + (t * (to - from));
    }

    public static float EaseOutSine(float from, float to, float t)
    {
        t = Mathf.Sin(t * Mathf.PI / 2);
        return from + (t * (to - from));
    }

    public static float EaseInOutSine(float from, float to, float t)
    {
        t = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
        return from + (t * (to - from));
    }

    // --- Quad ---
    public static float EaseInQuad(float from, float to, float t)
    {
        t = t * t;
        return from + (t * (to - from));
    }

    public static float EaseOutQuad(float from, float to, float t)
    {
        t = 1 - Mathf.Pow(1 - t, 2);
        return from + (t * (to - from));
    }

    public static float EaseInOutQuad(float from, float to, float t)
    {
        t = t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
        return from + (t * (to - from));
    }

    // --- Cubic ---
    public static float EaseInCubic(float from, float to, float t)
    {
        t = t * t * t;
        return from + (t * (to - from));
    }

    public static float EaseOutCubic(float from, float to, float t)
    {
        t = 1 - Mathf.Pow(1 - t, 3);
        return from + (t * (to - from));
    }

    public static float EaseInOutCubic(float from, float to, float t)
    {
        t = t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
        return from + (t * (to - from));
    }

    // --- Quart ---
    public static float EaseInQuart(float from, float to, float t)
    {
        t = t * t * t * t;
        return from + (t * (to - from));
    }

    public static float EaseOutQuart(float from, float to, float t)
    {
        t = 1 - Mathf.Pow(1 - t, 4);
        return from + (t * (to - from));
    }

    public static float EaseInOutQuart(float from, float to, float t)
    {
        t = t < 0.5f ? 8 * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 4) / 2;
        return from + (t * (to - from));
    }

    // --- Quint ---
    public static float EaseInQuint(float from, float to, float t)
    {
        t = t * t * t * t * t;
        return from + (t * (to - from));
    }

    public static float EaseOutQuint(float from, float to, float t)
    {
        t = 1 - Mathf.Pow(1 - t, 5);
        return from + (t * (to - from));
    }

    public static float EaseInOutQuint(float from, float to, float t)
    {
        t = t < 0.5f ? 16 * t * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 5) / 2;
        return from + (t * (to - from));
    }

    // --- Expo ---
    public static float EaseInExpo(float from, float to, float t)
    {
        t = t == 0 ? 0f : Mathf.Pow(2, 10 * t - 10);
        return from + (t * (to - from));
    }

    public static float EaseOutExpo(float from, float to, float t)
    {
        t = t == 1 ? 1f : 1 - Mathf.Pow(2, -10 * t);
        return from + (t * (to - from));
    }

    public static float EaseInOutExpo(float from, float to, float t)
    {
        t = t == 0 ? 0 : t == 1 ? 1 : t < 0.5f ? Mathf.Pow(2, 20 * t - 10) / 2 : (2 - Mathf.Pow(2, -20 * t + 10)) / 2;
        return from + (t * (to - from));
    }

    // --- Circ ---
    public static float EaseInCirc(float from, float to, float t)
    {
        t = 1 - Mathf.Sqrt(1 - Mathf.Pow(t, 2));
        return from + (t * (to - from));
    }

    public static float EaseOutCirc(float from, float to, float t)
    {
        t = Mathf.Sqrt(1 - Mathf.Pow(t - 1, 2));
        return from + (t * (to - from));
    }

    public static float EaseInOutCirc(float from, float to, float t)
    {
        t = t < 0.5f
            ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * t, 2))) / 2
            : (Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) / 2;
        return from + (t * (to - from));
    }

    // --- Back ---
    public static float EaseInBack(float from, float to, float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        t = c3 * t * t * t - c1 * t * t;
        return from + (t * (to - from));
    }

    public static float EaseOutBack(float from, float to, float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        t = 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
        return from + (t * (to - from));
    }

    public static float EaseInOutBack(float from, float to, float t)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;
        t = t < 0.5f
            ? Mathf.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2) / 2
            : (Mathf.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
        return from + (t * (to - from));
    }

    // --- Elastic ---
    public static float EaseInElastic(float from, float to, float t)
    {
        const float c4 = 2 * Mathf.PI / 3;

        t = t == 0
            ? 0
            : t == 1
            ? 1
            : -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * c4);

        return from + (t * (to - from));
    }

    public static float EaseOutElastic(float from, float to, float t)
    {
        const float c4 = 2 * Mathf.PI / 3;

        t = t == 0
            ? 0
            : t == 1
            ? 1
            : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;

        return from + (t * (to - from));
    }

    public static float EaseInOutElastic(float from, float to, float t)
    {
        const float c5 = 2 * Mathf.PI / 4.5f;
        if (t == 0)
            return from;
        if (t == 1)
            return to;
        t = t < 0.5f
            ? -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2
            : Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * c5) / 2 + 1;
        return from + (t * (to - from));
    }

    // --- Bounce ---
    public static float EaseInBounce(float from, float to, float t)
    {
        t = 1 - EaseOutBounce(0, 1, 1 - t);
        return from + (t * (to - from));
    }

    public static float EaseOutBounce(float from, float to, float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (t < 1 / d1)
        {
            t = n1 * t * t;
        }
        else if (t < 2 / d1)
        {
            t -= 1.5f / d1;
            t = n1 * t * t + 0.75f;
        }
        else if (t < 2.5 / d1)
        {
            t -= 2.25f / d1;
            t = n1 * t * t + 0.9375f;
        }
        else
        {
            t -= 2.625f / d1;
            t = n1 * t * t + 0.984375f;
        }

        return from + (t * (to - from));
    }

    public static float EaseInOutBounce(float from, float to, float t)
    {
        t = t < 0.5f
            ? (1 - EaseOutBounce(0, 1, 1 - 2 * t)) / 2
            : (1 + EaseOutBounce(0, 1, 2 * t - 1)) / 2;
        return from + (t * (to - from));
    }
}
