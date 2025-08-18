using System;
using System.Collections;
using UnityEngine;

public class InterpolationDemo : MonoBehaviour
{
    private enum InterpolationType
    {
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce
    }

    [SerializeField] private InterpolationType interpolationType;
    [SerializeField] private float startPosition;
    [SerializeField] private float endPosition;
    [SerializeField] private float duration = 1f;
    private bool isAnimating = false;
    private delegate void InterpolationCallback(float t);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isAnimating) StartCoroutine(AnimateInterpolation());
        else if (Input.GetKeyDown(KeyCode.Alpha1)) FadeManager.Instance.FadeInAndDo(() => Debug.Log("Faded in"));
        else if (Input.GetKeyDown(KeyCode.Alpha2)) FadeManager.Instance.FadeOutAndDo(() => Debug.Log("Faded out"));
        else if (Input.GetKeyDown(KeyCode.Alpha3)) FadeManager.Instance.BlinkAndDo(() => Debug.Log("Blinking"));
        else if (Input.GetKeyDown(KeyCode.Alpha4)) FadeManager.Instance.CloseEyesAndDo(() => Debug.Log("Eyes closed"));
        else if (Input.GetKeyDown(KeyCode.Alpha5)) FadeManager.Instance.OpenEyesAndDo(() => Debug.Log("Eyes opened"));
    }

    private IEnumerator AnimateInterpolation()
    {
        var interpolationFunc = GetInterpolationFunction(interpolationType);
        yield return Interpolation.Interpolate
        (
            onStart: () => isAnimating = true,
            tween: t =>
            {
                float interpolatedValue = interpolationFunc(startPosition, endPosition, t);
                transform.position = new Vector3(interpolatedValue, transform.position.y, transform.position.z);
            },
            onComplete: () => isAnimating = false,
            duration: duration
        );
    }

    private Func<float, float, float, float> GetInterpolationFunction(InterpolationType interpolationType)
    {
        return interpolationType switch
        {
            InterpolationType.EaseInSine => Interpolation.EaseInSine,
            InterpolationType.EaseOutSine => Interpolation.EaseOutSine,
            InterpolationType.EaseInOutSine => Interpolation.EaseInOutSine,
            InterpolationType.EaseInQuad => Interpolation.EaseInQuad,
            InterpolationType.EaseOutQuad => Interpolation.EaseOutQuad,
            InterpolationType.EaseInOutQuad => Interpolation.EaseInOutQuad,
            InterpolationType.EaseInCubic => Interpolation.EaseInCubic,
            InterpolationType.EaseOutCubic => Interpolation.EaseOutCubic,
            InterpolationType.EaseInOutCubic => Interpolation.EaseInOutCubic,
            InterpolationType.EaseInQuart => Interpolation.EaseInQuart,
            InterpolationType.EaseOutQuart => Interpolation.EaseOutQuart,
            InterpolationType.EaseInOutQuart => Interpolation.EaseInOutQuart,
            InterpolationType.EaseInQuint => Interpolation.EaseInQuint,
            InterpolationType.EaseOutQuint => Interpolation.EaseOutQuint,
            InterpolationType.EaseInOutQuint => Interpolation.EaseInOutQuint,
            InterpolationType.EaseInExpo => Interpolation.EaseInExpo,
            InterpolationType.EaseOutExpo => Interpolation.EaseOutExpo,
            InterpolationType.EaseInOutExpo => Interpolation.EaseInOutExpo,
            InterpolationType.EaseInCirc => Interpolation.EaseInCirc,
            InterpolationType.EaseOutCirc => Interpolation.EaseOutCirc,
            InterpolationType.EaseInOutCirc => Interpolation.EaseInOutCirc,
            InterpolationType.EaseInBack => Interpolation.EaseInBack,
            InterpolationType.EaseOutBack => Interpolation.EaseOutBack,
            InterpolationType.EaseInOutBack => Interpolation.EaseInOutBack,
            InterpolationType.EaseInElastic => Interpolation.EaseInElastic,
            InterpolationType.EaseOutElastic => Interpolation.EaseOutElastic,
            InterpolationType.EaseInOutElastic => Interpolation.EaseInOutElastic,
            InterpolationType.EaseInBounce => Interpolation.EaseInBounce,
            InterpolationType.EaseOutBounce => Interpolation.EaseOutBounce,
            InterpolationType.EaseInOutBounce => Interpolation.EaseInOutBounce,
            _ => Interpolation.EaseInSine
        };
    }
}
