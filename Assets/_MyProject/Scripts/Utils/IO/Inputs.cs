using UnityEngine;

/// <summary>
/// This is a class containing generic input functions (i.e drag, zoom, click, double click, long click)
/// <remarks>
/// You can derive your own input class from InputSystem base class for other platforms.
/// </remarks>
/// </summary>

#region Controller Class
public class Inputs
{
    public static InputSystem system = new InputSystem();
    public static void Initialize(InputSystem inputSystem)
    {
        system = inputSystem;
    }
}
#endregion Controller Class

#region InputSystem Base Class
public class InputSystem
{
    const float DOUBLE_TAP_WAIT = 0.25f;
    const float LONG_TAP_WAIT = 1f;
    float MIN_SWIPE_DISTANCE = 50;

    // General
    bool rePosition; // flag to reposition image
    Vector2 firstPosition;
    public Vector2 lastPosition;

    //Click
    Coroutine doubleClickWait;

    //Zoom
    float minZoom = 1;
    float maxZoom = 20;
    float lastTapTime; // for zoom on double tap
    float startTapTime; // for zoom on double tap
    public float zoomSpeed = 60; // The rate of change of zoom.
    public InputSystem()
    {
        MIN_SWIPE_DISTANCE = Screen.dpi * .05f;
    }
    public virtual void OnInputDown()
    {
        startTapTime = Time.time;
        firstPosition = Input.mousePosition;
    }

    public virtual void OnInputUp()
    {
    }

    public virtual Vector3 InputPosition()
    {
        return Vector3.zero;
    }

    public virtual Ray ScreenToRay(Vector3 position)
    {
        return new Ray();
    }

    public virtual bool Inputting()
    {
        return false;
    }

    public virtual Vector2 Drag()
    {
        return Vector2.zero;
    }

    public virtual float Zoom(Transform target = null, Transform content = null)
    {
        return 0;
    }

    public Click InputType()
    {
        int upsCount = 0;
        foreach (Touch t in Input.touches)
            if (t.phase == TouchPhase.Ended)
                upsCount++;
        if (Input.touches.Length == upsCount && Vector2.Distance(Input.mousePosition, firstPosition) < MIN_SWIPE_DISTANCE)
        {
            if (Time.time - lastTapTime <= DOUBLE_TAP_WAIT && Vector2.Distance(Input.mousePosition, lastPosition) < MIN_SWIPE_DISTANCE)
            {// checking double tap
                return Click.DoubleTap;
            }
            else
            {
                lastPosition = Input.mousePosition;
                lastTapTime = Time.time;
                return Time.time - startTapTime > LONG_TAP_WAIT ? Click.LongTap : Click.Tap;
            }
        }
        return Click.Drag;
    }

    public void WaitInputType(System.Action<Click> action)
    {
        Click click = InputType();
        if (click == Click.DoubleTap)
        {
            Routine.Stop(doubleClickWait);
            action(click);
        }
        else if (click == Click.Tap)
            doubleClickWait = Routine.WaitAndCall(DOUBLE_TAP_WAIT, () => action(click));
    }

    public int InputDirection(Touch touch)
    {
        if (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y))
            return touch.deltaPosition.x >= 0 ? 1 : 2;
        else
            return touch.deltaPosition.y >= 0 ? 3 : 4;
    }

    public void SetBoundaries(Transform target, Vector2 screenRect, Vector2 viewRect)
    { // when input not zooming and lerping will change zoom // keep the target with in boundaries
        //Vector2 picSize = new Vector2 (targetExtent < SCREEN_EXTENT.x ? targetExtent : SCREEN_EXTENT.x, targetExtent < SCREEN_EXTENT.y ? targetExtent : SCREEN_EXTENT.y); // detect whether pic within screen or not. if in screen then limit min and max Drag wrt to target cordinates other wise screen corinates
        Vector2 targetExtent = screenRect / 2 * target.localScale; // as target.position is center point so adding extent to make comparison of target edges with screen edges
        Vector2 minDrag = new Vector2(-viewRect.x - targetExtent.x, -viewRect.y - targetExtent.y);
        Vector2 maxDrag = new Vector2(+viewRect.x + targetExtent.x, +viewRect.y + targetExtent.y);
        if (target.position.x < minDrag.x)
            target.position = new Vector3(minDrag.x, target.position.y, target.position.z);
        else if (target.position.x > maxDrag.x)
            target.position = new Vector3(maxDrag.x, target.position.y, target.position.z);
        if (target.position.y < minDrag.y)
            target.position = new Vector3(target.position.x, minDrag.y, target.position.z);
        else if (target.position.y > maxDrag.y)
            target.position = new Vector3(target.position.x, maxDrag.y, target.position.z);
    }

    public void ZoomingBackToOrigin(Transform target, Transform content)
    {
        if (target.localScale.x < minZoom)
            SetTargetOnZoom(target, content, Mathf.Lerp(target.localScale.x, minZoom, Time.deltaTime * 10), Camera.main.ScreenToWorldPoint(firstPosition));
        else if (target.localScale.x > maxZoom)
            SetTargetOnZoom(target, content, Mathf.Lerp(target.localScale.x, maxZoom, Time.deltaTime * 10), Camera.main.ScreenToWorldPoint(firstPosition));
        if (rePosition)
        { // keep the Picture in middle using lerp
            if (target.position.x < 0 || target.position.x > 0)
                target.position = new Vector3(Mathf.Lerp(target.position.x, 0, Time.deltaTime * 20), target.position.y, target.position.z);
            if (target.position.y < 0 || target.position.y > 0)
                target.position = new Vector3(target.position.x, Mathf.Lerp(target.position.y, 0, Time.deltaTime * 20), target.position.z);
            if (rePosition && Mathf.Abs(target.position.x) < .01f) // set reposition flag false 
                rePosition = false;
        }
        if (target.localScale.x < minZoom - .01f && !rePosition) // if Zoom Less Than MinZoom set reposition flag true 
            rePosition = true;
    }

    void SetTargetOnZoom(Transform target, Transform content, float value, Vector2 position)
    {
        Vector2 delta = (Vector2)target.localPosition - position;
        target.localPosition -= (Vector3)delta;
        content.localPosition += (Vector3)delta / target.localScale.x;
        target.localScale = Vector3.one * value;
        if (target.localScale.x < .03f)
            target.localScale = new Vector3(.03f, .03f, 1);
        else if (target.localScale.x > 100f)
            target.localScale = new Vector3(100f, 100f, 1);
        delta = content.localPosition;
        content.localPosition -= (Vector3)delta;
        target.localPosition += (Vector3)delta * target.localScale.x;
    }
}
#endregion InputSystem Base Class

#region InputStandalone Derived Class
public class InputStandalone : InputSystem
{
    public override void OnInputDown()
    {
        base.OnInputDown(); 
        lastPosition = Input.mousePosition;     //set lastPosition is to avoid jerk
    }

    public override void OnInputUp()
    {
        base.OnInputDown();
    }

    public override Vector3 InputPosition()
    {
        return Input.mousePosition;
    }

    public override bool Inputting()
    {
        return Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || Input.GetAxis("Mouse ScrollWheel") != 0;
    }

    public override Ray ScreenToRay(Vector3 position)
    {
        return Camera.main.ScreenPointToRay(position);
    }

    public override Vector2 Drag()
    {
        Vector2 deltaPos = (Vector2)Input.mousePosition - lastPosition;
        lastPosition = Input.mousePosition;
        return deltaPos;
    }

    public override float Zoom(Transform target = null, Transform content = null)
    {
        return Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * .1f;
    }
}
#endregion InputStandalone Derived Class

#region InputMobile Derived Class
public class InputMobile : InputSystem
{
    float lastZoom;
    public override void OnInputDown()
    {
        base.OnInputDown();
        lastZoom = GetZoomValue();    //set lastZoom is to avoid jerk
    }

    public override void OnInputUp()
    {
        base.OnInputDown();
    }
    public override Vector3 InputPosition()
    {
        return Input.touches[0].position;
    }

    public override bool Inputting()
    {
        return Input.touchCount > 0;
    }

    public override Ray ScreenToRay(Vector3 position)
    {
        return Camera.main.ScreenPointToRay(position);
    }

    public override Vector2 Drag()
    {
        Vector2 deltaPos = Vector2.zero;
        if (Input.touchCount > 0)
            deltaPos = Input.touches[0].deltaPosition;
        return deltaPos;
    }

    public override float Zoom(Transform target = null, Transform content = null)
    {
        float currZoom = GetZoomValue();
        float zoom = currZoom - lastZoom;
        lastZoom = currZoom;
        return zoom * zoomSpeed;
    }

    float GetZoomValue()
    {
        if (Input.touchCount < 2)
            return 0;
        return (new Vector2(Input.touches[0].position.x / Screen.width, Input.touches[0].position.y / Screen.height) - new Vector2(Input.touches[1].position.x / Screen.width, Input.touches[1].position.y / Screen.height)).magnitude;
    }
}
#endregion InputMobile Derived Class

#region Enums
public enum Click
{
    None = -1,
    Tap = 0,
    Drag = 1,
    Zoom = 2,
    DoubleTap = 3,
    LongTap = 4
};

#endregion Enums

