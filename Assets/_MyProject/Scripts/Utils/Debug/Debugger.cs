using UnityEngine;

public static class Debugger
{
    public static void DrawCube(Vector3 pos, Quaternion rot, Vector3 scale, Color color, float time = 0.02f)
    {
        // create matrix
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, scale);

        var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
        var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
        var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
        var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));

        var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
        var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
        var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
        var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));

        Debug.DrawLine(point1, point2, color, time);
        Debug.DrawLine(point2, point3, color, time);
        Debug.DrawLine(point3, point4, color, time);
        Debug.DrawLine(point4, point1, color, time);

        Debug.DrawLine(point5, point6, color, time);
        Debug.DrawLine(point6, point7, color, time);
        Debug.DrawLine(point7, point8, color, time);
        Debug.DrawLine(point8, point5, color, time);

        Debug.DrawLine(point1, point5, color, time);
        Debug.DrawLine(point2, point6, color, time);
        Debug.DrawLine(point3, point7, color, time);
        Debug.DrawLine(point4, point8, color, time);
    }

    public static void DrawCube(Vector3 min, Vector3 max, Color color, float time = 0.02f)
    {
        Debug.DrawLine(min, new Vector3 (max.x, min.y, min.z), color, time);
        Debug.DrawLine(min, new Vector3 (min.x, max.y, min.z), color, time);
        Debug.DrawLine(min, new Vector3 (min.x, min.y, max.z), color, time);
        Debug.DrawLine(new Vector3(min.x, max.y, max.z), max, color, time);
        Debug.DrawLine(new Vector3(max.x, min.y, max.z), max, color, time);
        Debug.DrawLine(new Vector3(max.x, max.y, min.z), max, color, time);
        Debug.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, max.y, max.z), color, time);
        Debug.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z), color, time);
        Debug.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z), color, time);
        Debug.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z), color, time);
        Debug.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(min.x, min.y, max.z), color, time);
        Debug.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(max.x, min.y, min.z), color, time);
    }
}
