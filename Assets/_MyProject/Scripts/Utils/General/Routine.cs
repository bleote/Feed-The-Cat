using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This is a class containing generic coroutine functions
/// <remarks></remarks>
/// </summary>
public static class Routine
{
    public static MonoBehaviour instance;
    public static void Initialize(MonoBehaviour mono)
    {
        instance = mono;
    }

    public static Coroutine WaitAndCall(float seconds, Action action)
    {
        return instance.StartCoroutine(WaitAndCallRoutine(seconds, action));
    }

    public static void Stop(Coroutine routine)
    {
        if (routine != null && instance)
            instance.StopCoroutine(routine);
    }

    public static Coroutine Lerp(float startValue, float endValue, float speed = .2f, Action<float> action = null, Action actionEnd = null)
    {
        return instance.StartCoroutine(LerpRoutine(startValue, endValue, speed, action, actionEnd));
    }
    public static Coroutine Lerp(Vector3 startPos, Vector3 targetPos, float speed = .2f, Action<Vector3> action = null, Action actionEnd = null)
    {
        return instance.StartCoroutine(LerpRoutine(0, 1, speed, (val) => action (startPos + (targetPos - startPos) * val), actionEnd));
    }

    public static Coroutine LerpConstant(float startValue, float endValue, float speed = .2f, Action<float> action = null, Action actionEnd = null)
    {
        return instance.StartCoroutine(LerpConstantRoutine(startValue, endValue, speed, action, actionEnd));
    }

    public static Coroutine Move(Transform trans, Vector3 startPos, Vector3 targetPos, float speed = .2f, Action action = null)
    {
        return instance.StartCoroutine(LerpRoutine(0, 1, speed, (val) => trans.position = startPos + (targetPos - startPos) * val, action));
    }

    public static Coroutine MoveAnchorPos(RectTransform trans, Vector2 startPos, Vector2 targetPos, float speed = .2f, Action action = null)
    {
        return instance.StartCoroutine(LerpRoutine(0, 1, speed, (val) => trans.anchoredPosition = startPos + (targetPos - startPos) * val, action));
    }

    public static Coroutine MoveAnchors(RectTransform trans, Vector2 startVal, Vector2 targetVal, float speed = .2f, Action action = null)
    {
        return instance.StartCoroutine(LerpRoutine(0, 1, speed, (val) => trans.anchorMin = trans.anchorMax = startVal + (targetVal - startVal) * val, action));
    }

    public static Coroutine MovePivot(RectTransform trans, Vector2 startVal, Vector2 targetVal, float speed = .2f, Action action = null)
    {
        return instance.StartCoroutine(LerpRoutine(0, 1, speed, (val) => trans.pivot = startVal + (targetVal - startVal) * val, action));
    }

    public static Coroutine MoveConstant(Transform trans, Vector3 startPos, Vector3 targetPos, float speed = 5f, Action action = null)
    {
        return instance.StartCoroutine(LerpConstantRoutine(0, 1, speed, (val) => trans.position = startPos + (targetPos - startPos) * val, action));
    }

    public static Coroutine Scale(Transform trans, Vector3 startScale, Vector3 targetScale, float speed = .2f, Action action = null)
    {
        return instance.StartCoroutine(LerpRoutine(0, 1, speed, (val) => trans.localScale = startScale + (targetScale - startScale) * val, action));
    }
    public static Coroutine ScaleConstant(Transform trans, Vector3 startScale, Vector3 targetScale, float speed = .2f, Action action = null)
    {
        return instance.StartCoroutine(LerpConstantRoutine(0, 1, speed, (val) => trans.localScale = startScale + (targetScale - startScale) * val, action));
    }

    public static Coroutine Rotate(Transform trans, Vector3 startAngles, Vector3 targetAngles, float speed = .2f, Action action = null)
    {
        return instance.StartCoroutine(LerpRoutine(0, 1, speed, (val) => trans.eulerAngles = startAngles + (targetAngles - startAngles) * val, action));
    }

    public static Coroutine Rotate(Transform trans, Quaternion startRot, Quaternion targetRot, float speed = .2f, Action action = null)
    {
        return instance.StartCoroutine(LerpRotation(trans, startRot, targetRot, speed, action));
    }

    public static Coroutine RotateAround(Transform trans, Vector3 point, Quaternion startRot, float angle, int speed = 5, Action actionEnd = null)
    {
        return instance.StartCoroutine(LerpRotateAround(trans, point, startRot, angle, speed, actionEnd));
    }

    public static void Open(Transform trans, float speed = .3f, float openValue = .1f)
    {
        instance.StartCoroutine(OpenRoutine(trans, speed, openValue));
    }

    public static IEnumerator FadeAndScaleText(Text text, string msg)
    {
        RectTransform textObj = text.gameObject.GetComponent<RectTransform>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        textObj.localScale = Vector3.one;
        while (text.color.a > 0)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - 0.1f);
            textObj.localScale = new Vector3(textObj.localScale.x - 0.01f, textObj.localScale.y - 0.01f, 1);
            yield return new WaitForSeconds(.01f);
        }

        text.text = msg;
        while (text.color.a < 1)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + 0.1f);
            textObj.localScale = new Vector3(textObj.localScale.x + 0.01f, textObj.localScale.y + 0.01f, 1);
            yield return new WaitForSeconds(.01f);
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        textObj.localScale = Vector3.one;
    }

    public static IEnumerator FadeImage(Image i, float start, float end, float speed = 1)
    {
        Color c = i.color;
        c.a = start;
        speed = (end - start) / 10f * speed;
        int sign = (int)Mathf.Sign(speed);
        while (c.a * sign < end * sign)
        {
            c.a += speed;
            i.color = c;
            yield return new WaitForFixedUpdate();
        }
        c.a = end;
    }

    public static IEnumerator FadeMusic(AudioSource audio, float volume = 1)
    {
        audio.UnPause();
        float delta = Mathf.Abs(audio.volume - volume) / 50f;
        if (audio.volume < volume)
        {
            while (audio.volume < volume)
            {
                yield return new WaitForFixedUpdate();
                audio.volume += delta;
            }
        }
        else
        {
            while (audio.volume > volume)
            {
                yield return new WaitForFixedUpdate();
                audio.volume -= delta;
            }
        }
        if (volume == 0)
            audio.Pause();
    }

    public static IEnumerator LoadingTextAnim(Text loading)
    { // // set message seen
        while (loading.text.Contains("Loading"))
        {
            loading.text = loading.text == "Loading..." ? "Loading." : loading.text + ".";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public static void SlightlyMove(RectTransform trans, Vector2 dir, float speed = .3f)
    {
        MoveAnchorPos(trans, trans.anchoredPosition + dir, trans.anchoredPosition, speed); // moving animation
    }

    public static IEnumerator ContentsScaleAnim(Transform content, float seconds = 0)
    {
        yield return new WaitForSeconds(seconds);
        for (int i = 0; i < content.childCount; i++)
            Scale(content.GetChild(i), Vector3.one / 1.3f, Vector3.one); //openeing scaling animation
    }

    public static IEnumerator ContentsOpenAnim(Transform content, float seconds = 0)
    {
        yield return new WaitForSeconds(seconds);
        for (int i = 0; i < content.childCount; i++)
        {
            Open(content.GetChild(i)); //openeing scaling animation
            yield return new WaitForFixedUpdate();
        }
    }

    public static IEnumerator ContentsFadeInAnim(MonoBehaviour reference, RectTransform content, float seconds = 0)
    {
        yield return new WaitForSeconds(0);
        for (int i = 0; i < content.childCount; i++)
        {
            reference.StartCoroutine(FadeImage(content.GetChild(i).GetComponent<Image>(), 0, 1));
            yield return new WaitForFixedUpdate();
        }
    }

    public static IEnumerator ContentsSlideInAnim(Transform content, float seconds = 0)
    {
        yield return new WaitForSeconds(0);
        for (int i = 0; i < content.childCount; i++) // set position first
            content.GetChild(i).GetComponent<RectTransform>().anchoredPosition = content.GetChild(i).GetComponent<RectTransform>().anchoredPosition - new Vector2(20, 0);
        for (int i = 0; i < content.childCount; i++)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            MoveAnchorPos(content.GetChild(i).GetComponent<RectTransform>(), content.GetChild(i).GetComponent<RectTransform>().anchoredPosition, content.GetChild(i).GetComponent<RectTransform>().anchoredPosition + new Vector2(20, 0), .3f);
        }
    }

    static IEnumerator WaitAndCallRoutine(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }

    static IEnumerator LerpRoutine(float startValue, float endValue, float speed = .2f, Action<float> action = null, Action actionEnd = null)
    {
        float threshold = Mathf.Abs(startValue - endValue) / 500f;
        while (Mathf.Abs(startValue - endValue) > threshold)
        {  //fill In
            startValue += (endValue - startValue) * speed;
            if (action != null)
                action(startValue);
            yield return new WaitForFixedUpdate();
        }
        if (action != null)
            action(endValue);
        if (actionEnd != null)
            actionEnd();
    }

    static IEnumerator LerpConstantRoutine(float startValue, float endValue, float speed = .2f, Action<float> action = null, Action actionEnd = null)
    {
        float _speed = Mathf.Sign(endValue - startValue) * speed;
        float sign = Mathf.Sign(_speed);
        for (float i = startValue; i * sign < endValue * sign; i += _speed)
        {
            action(i);
            yield return new WaitForFixedUpdate();
        }
        if (actionEnd != null)
            actionEnd();
    }

    static IEnumerator LerpRotation(Transform trans, Quaternion startValue, Quaternion endValue, float speed = .2f, Action actionEnd = null)
    {
        trans.rotation = startValue;
        while (trans.eulerAngles != endValue.eulerAngles)
        {  //fill In
            trans.rotation = Quaternion.Slerp(trans.rotation, endValue, speed);
            yield return new WaitForFixedUpdate();
        }
        if (actionEnd != null)
            actionEnd();
    }

    static IEnumerator LerpRotateAround(Transform trans, Vector3 point, Quaternion startValue, float angle, int speed = 5, Action actionEnd = null)
    {
        trans.rotation = startValue;
        float value = speed < 1 ? angle : 1f * angle / speed;
        for (int i = 0; i < speed; i++)
        {  //fill In
            trans.RotateAround(point, Vector3.up, value);
            yield return new WaitForFixedUpdate();
        }
        if (actionEnd != null)
            actionEnd();
    }

    static IEnumerator OpenRoutine(Transform transform, float speed = .3f, float openValue = .1f)
    {
        Vector3 targetScale = new Vector3(1 + openValue, 1 + openValue, 1);
        Vector3 origScale = transform.localScale;
        while (Vector2.Distance(transform.localScale, targetScale) > .01f)
        {  //Zoom In
            transform.localScale += (targetScale - transform.localScale) * speed;
            yield return new WaitForFixedUpdate();
        }
        while (Vector2.Distance(transform.localScale, origScale) > .01f)
        {  //Zoom Out
            transform.localScale += (Vector3.one - transform.localScale) * speed;
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = Vector3.one;
    }
}
