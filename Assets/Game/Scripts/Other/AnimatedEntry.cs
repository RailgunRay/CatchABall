using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedEntry : MonoBehaviour
{

    RectTransform rectTransform;
    public float animationLength, delayBeforeAnimation;

    [Space(10)]
    [Header("Scale")]
    public Vector3 startScale;
    public AnimationCurve scaleCurve;

    [Space(10)]
    [Header("Position")]
    public Vector3 deltaStartPosition;
    public AnimationCurve positionCurve;

    Vector3 endScale;
    Vector3 endPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        SetupVariables();
        StartCoroutine(AnimationRoutine());
    }

    void SetupVariables()
    {
        endScale = rectTransform.localScale;
        endPosition = rectTransform.localPosition;
    }


    IEnumerator AnimationRoutine()
    {
        rectTransform.localPosition = new Vector3(endPosition.x + deltaStartPosition.x, endPosition.y + deltaStartPosition.y
            , endPosition.z + deltaStartPosition.z);
        Vector3 deltaPos = rectTransform.localPosition;
        transform.localScale = startScale;
        yield return new WaitForSecondsRealtime(delayBeforeAnimation);
        float time = 0, percent = 0, lastTime = Time.realtimeSinceStartup;

        do
        {
            time += Time.realtimeSinceStartup - lastTime;
            lastTime = Time.realtimeSinceStartup;
            percent = Mathf.Clamp01(time / animationLength);

            Vector3 tempScale = Vector3.LerpUnclamped(startScale, endScale, scaleCurve.Evaluate(percent));
            Vector3 tempPosition = Vector3.LerpUnclamped(deltaPos, endPosition, positionCurve.Evaluate(percent));
            transform.localScale = tempScale;
            rectTransform.localPosition = tempPosition;
            yield return null;
        }
        while (percent < 1f);

        transform.localScale = endScale;
        rectTransform.localPosition = endPosition;

        yield return null;
    }
}
