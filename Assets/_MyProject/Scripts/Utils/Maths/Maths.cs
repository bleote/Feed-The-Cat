using UnityEngine;

/// <summary>
/// This is a class containing general Math functions
/// <remarks></remarks>
/// </summary>
public static class Maths
{
    public static bool GetNearestPointOnLine3D(Vector3 lineStart, Vector3 lineEnd, Vector3 point, ref Vector3 result)
    {
        float angle = Vector3.Angle(lineEnd - lineStart, point - lineStart);
        float cosVal = Mathf.Cos(Mathf.Deg2Rad * angle);
        float distance = Vector3.Distance(lineStart, point) * cosVal;
        result = lineStart + ((lineEnd - lineStart).normalized * distance);
        point = (lineStart + lineEnd) / 2f; // getting mid point of line
        if ((result - point).sqrMagnitude < (lineStart - point).sqrMagnitude) // checking if result point is in between the line
            return true;
        return false;
    }

    public static bool GetNearestPointOnLine2D(Vector2 lineStart, Vector2 lineEnd, Vector2 point, ref Vector2 result)
    {
        Vector2 direction = lineEnd - lineStart;
        direction.Normalize();
        Vector2 lhs = point - lineStart;
        float dotP = Vector2.Dot(lhs, direction);
        result = lineStart + direction * dotP;
        point = (lineStart + lineEnd) / 2f; // getting mid point of line
        if ((result - point).sqrMagnitude < (lineStart - point).sqrMagnitude) // checking if result point is in between the line
            return true;
        return false;
    }
    public static Vector3 LineIntersection3D(Vector3 point1, Vector3 direction1, Vector3 point2, Vector3 direction2)
    {
        var n = Vector3.Cross(direction1, direction2);
        var u = Vector3.Cross(n, point1 - point2) / Vector3.Dot(n, n);
        return point1 - direction1 * Vector3.Dot(direction2, u);
    }

    public static bool LineIntersection2D(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection)
    {
        float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num, offset;
        Ax = p2.x - p1.x;
        Bx = p3.x - p4.x;
        Ay = p2.y - p1.y;
        By = p3.y - p4.y;
        Cx = p1.x - p3.x;
        Cy = p1.y - p3.y;
        d = By * Cx - Bx * Cy;  // alpha numerator//
        f = Ay * Bx - Ax * By;  // both denominator//
        e = Ax * Cy - Ay * Cx;  // beta numerator//

        // beta tests //
        if (f > 0)
        {
            if (e < 0 || e > f) return false;
        }
        else
        {
            if (e > 0 || e < f) return false;
        }

        // check if they are parallel
        if (f == 0) return false;

        // compute intersection coordinates //
        num = d * Ax; // numerator //
        offset = (num * f) >= 0f ? f * 0.5f : -f * 0.5f; //if same_sign  // round direction //
        intersection.x = p1.x + (num + offset) / f;

        num = d * Ay;
        offset = (num * f) >= 0f ? f * 0.5f : -f * 0.5f; //if same_sign
        intersection.y = p1.y + (num + offset) / f;

        return true;
    }
    public static Quaternion GetNearestRotation(Quaternion rotation)
    {
        if (Mathf.Abs(Mathf.DeltaAngle(rotation.eulerAngles.y, 0)) < 45)
            return Quaternion.Euler(0, 0, 0);
        else if (Mathf.Abs(Mathf.DeltaAngle(rotation.eulerAngles.y, 90)) < 45)
            return Quaternion.Euler(0, 90, 0);
        else if (Mathf.Abs(Mathf.DeltaAngle(rotation.eulerAngles.y, 180)) < 45)
            return Quaternion.Euler(0, 180, 0);
        else if (Mathf.Abs(Mathf.DeltaAngle(rotation.eulerAngles.y, 270)) < 45)
            return Quaternion.Euler(0, 270, 0);
        return Quaternion.identity;
    }

    public static void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        // pivot
        Vector3 pivotDelta = target.transform.localPosition - pivot; // diff from object pivot to desired pivot/origin
        Vector3 scaleFactor = new Vector3(
            newScale.x / target.transform.localScale.x,
            newScale.y / target.transform.localScale.y,
            newScale.z / target.transform.localScale.z);
        pivotDelta.Scale(scaleFactor);
        target.transform.localPosition = pivot + pivotDelta;
        //scale
        target.transform.localScale = newScale;
    }
}
