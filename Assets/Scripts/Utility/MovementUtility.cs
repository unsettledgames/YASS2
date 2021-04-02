using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class MovementUtility : MonoBehaviour
{
    public static MovementUtility Instance;
    public AnimationCurve defaultCurve;

    private void Awake()
    {
        Instance = this;
    }
    public static IEnumerator MoveToTransform(Transform source, Transform destination, float speed, AnimationCurve curve = null, 
                                              GenericDelegate toCall = null, ArrayList parameters = null)
    {
        float t = 0;
        float val;
        Vector3 start = source.position;

        if (curve == null)
        {
            curve = MovementUtility.Instance.defaultCurve;
        }

        while (t <= 1)
        {
            if (curve == null)
            {
                val = t;
            }
            else
            {
                val = curve.Evaluate(t);
            }
            source.position = Vector3.Lerp(start, destination.position, val);

            t += Time.deltaTime * speed;

            yield return null;
        }

        if (toCall != null)
        {
            toCall(parameters);
        }
    }
    public static IEnumerator rotate(Transform affected, float start, float end, float speed, AnimationCurve curve = null,
                                     GenericDelegate toCallAtEnd = null, ArrayList toCallParams = null)
    {
        float rate = speed;
        float t = 0.0f;
        float pos = 0;
        float y = 0;
        float value;
        float difference = end - start;

        if (curve == null)
        {
            curve = MovementUtility.Instance.defaultCurve;
        }

        while (t < 1.0)
        {
            t += Time.deltaTime * rate;

            if (curve != null)
            {
                pos = curve.Evaluate(t);
                value = start + difference * pos;
            }
            else
            {
                value = Mathf.Lerp(start, end, t);
            }

            affected.transform.rotation = Quaternion.Euler(new Vector3(0, 0, value));

            yield return null;
        }

        if (toCallAtEnd != null)
        {
            toCallAtEnd(toCallParams);
        }
    }
    public void SlideFloatNotRoutine(float start, float end, float speed, AnimationCurve curve = null, 
                                            GenericDelegateFloat update = null, GenericDelegate toCall = null, 
                                            ArrayList parameters = null)
    {
        StartCoroutine(SlideFloat(start, end, speed, curve, update, toCall, parameters));
    }
    public static IEnumerator SlideFloat(float start, float end, float speed,
                                         AnimationCurve curve = null, GenericDelegateFloat update = null,
                                         GenericDelegate toCall = null, ArrayList parameters = null)
    {
        float rate = speed;
        float t = 0.0f;
        float pos = 0;
        float y = 0;
        float value;
        float difference = end - start;

        if (curve == null)
        {
            curve = MovementUtility.Instance.defaultCurve;
        }

        while (t < 1.0)
        {
            t += Time.deltaTime * rate;

            if (curve != null)
            {
                pos = curve.Evaluate(t);
                value = start + difference * pos;
            }
            else
            {
                value = Mathf.Lerp(start, end, t);
            }

            if (update != null)
            {
                update(value);
            }

            yield return null;
        }

        if (toCall != null)
        {
            toCall(parameters);
        }
    }

    public static IEnumerator UnscaledSlideFloat(float start, float end, float speed,
                                         AnimationCurve curve = null, GenericDelegateFloat update = null,
                                         GenericDelegate toCall = null, ArrayList parameters = null)
    {
        float rate = speed;
        float t = 0.0f;
        float pos = 0;
        float y = 0;
        float value;
        float difference = end - start;

        if (curve == null)
        {
            curve = MovementUtility.Instance.defaultCurve;
        }

        while (t < 1.0)
        {
            t += Time.unscaledDeltaTime * rate;

            if (curve != null)
            {
                pos = curve.Evaluate(t);
                value = start + difference * pos;
            }
            else
            {
                value = Mathf.Lerp(start, end, t);
            }

            if (update != null)
            {
                update(value);
            }

            yield return null;
        }

        if (toCall != null)
        {
            toCall(parameters);
        }
    }

    public static IEnumerator SlideVector3(Vector3 start, Vector3 end, float speed,
                                         AnimationCurve curve = null, GenericDelegateVector3 update = null, GenericDelegate toCall = null, 
                                         ArrayList parameters = null, bool camera = false)
    {
        float rate = speed;
        float t = 0.0f;
        float x = 0;
        float y = 0;
        float z = 0;

        float xDiff = end.x - start.x;
        float yDiff = end.y - start.y;
        float zDiff = end.z - start.z;

        Vector3 value = ObjectPooler.Instance.GetVector3();

        if (curve == null)
        {
            curve = Instance.defaultCurve;
        }

        while (t < 1.0)
        {
            t += Time.deltaTime * rate;

            if (curve != null)
            {
                x = curve.Evaluate(t);
                y = x;
                z = x;

                value.x = start.x + xDiff * x;
                value.y = start.y + yDiff * y;
                value.z = start.z + zDiff * z;
            }
            else
            {
                value.x = Mathf.Lerp(start.x, end.x, t);
                value.y = Mathf.Lerp(start.y, end.y, t);
                value.z = Mathf.Lerp(start.z, end.z, t);
            }

            if (update != null)
            {
                update(value);
            }

            ObjectPooler.Instance.EnqueueVector3(value);
            yield return null;
        }

        if (toCall != null)
        {
            toCall(parameters);
        }
    }

    public static IEnumerator UnscaledSlideVector3(Vector3 start, Vector3 end, float speed,
                                         AnimationCurve curve = null, GenericDelegateVector3 update = null, GenericDelegate toCall = null, ArrayList parameters = null)
    {
        float rate = speed;
        float t = 0.0f;
        float x = 0;
        float y = 0;
        float z = 0;

        float xDiff = end.x - start.x;
        float yDiff = end.y - start.y;
        float zDiff = end.z - start.z;

        Vector3 value = ObjectPooler.Instance.GetVector3();

        if (curve == null)
        {
            curve = MovementUtility.Instance.defaultCurve;
        }

        while (t < 1.0)
        {
            t += Time.unscaledDeltaTime * rate;

            if (curve != null)
            {
                x = curve.Evaluate(t);
                y = x;
                z = x;

                value.x = start.x + xDiff * x;
                value.y = start.y + yDiff * y;
                value.z = start.z + zDiff * z;
            }
            else
            {
                value.x = Mathf.Lerp(start.x, end.x, t);
                value.y = Mathf.Lerp(start.y, end.y, t);
                value.z = Mathf.Lerp(start.z, end.z, t);
            }

            if (update != null)
            {
                update(value);
            }

            ObjectPooler.Instance.EnqueueVector3(value);
            yield return null;
        }

        if (toCall != null)
        {
            toCall(parameters);
        }
    }

    public static IEnumerator SlideColor(Color start, Color end, float speed,
                                         AnimationCurve curve = null, GenericDelegateColor update = null, GenericDelegate toCall = null, ArrayList parameters = null)
    {
        float rate = speed;
        float t = 0.0f;

        float rDiff = end.r - start.r;
        float gDiff = end.g - start.g;
        float bDiff = end.b - start.b;
        float aDiff = end.a - start.a;

        if (curve == null)
        {
            curve = MovementUtility.Instance.defaultCurve;
        }

        Color value = new Color();

        while (t < 1.0)
        {
            t += Time.deltaTime * rate;

            if (curve != null)
            {
                float curveEval = curve.Evaluate(t);

                value.r = start.r + rDiff * curveEval;
                value.g = start.g + gDiff * curveEval;
                value.b = start.b + bDiff * curveEval;
                value.a = start.a + aDiff * curveEval;
            }
            else
            {
                value.r = Mathf.Lerp(start.r, end.r, t);
                value.g = Mathf.Lerp(start.g, end.g, t);
                value.b = Mathf.Lerp(start.b, end.b, t);
                value.a = Mathf.Lerp(start.a, end.a, t);
            }

            if (update != null)
            {
                update(value);
            }

            yield return null;
        }

        if (toCall != null)
        {
            toCall(parameters);
        }
    }

    public static IEnumerator UnscaledSlideColor(Color start, Color end, float speed,
                                         AnimationCurve curve = null, GenericDelegateColor update = null, GenericDelegate toCall = null, ArrayList parameters = null)
    {
        float rate = speed;
        float t = 0.0f;

        float rDiff = end.r - start.r;
        float gDiff = end.g - start.g;
        float bDiff = end.b - start.b;
        float aDiff = end.a - start.a;

        if (curve == null)
        {
            curve = MovementUtility.Instance.defaultCurve;
        }

        Color value = new Color();

        while (t < 1.0)
        {
            t += Time.unscaledDeltaTime * rate;

            if (curve != null)
            {
                float curveEval = curve.Evaluate(t);

                value.r = start.r + rDiff * curveEval;
                value.g = start.g + gDiff * curveEval;
                value.b = start.b + bDiff * curveEval;
                value.a = start.a + aDiff * curveEval;
            }
            else
            {
                value.r = Mathf.Lerp(start.r, end.r, t);
                value.g = Mathf.Lerp(start.g, end.g, t);
                value.b = Mathf.Lerp(start.b, end.b, t);
                value.a = Mathf.Lerp(start.a, end.a, t);
            }

            if (update != null)
            {
                update(value);
            }

            yield return null;
        }

        if (toCall != null)
        {
            toCall(parameters);
        }
    }

    // TODO: (Unscaled)SlideVector2
}

