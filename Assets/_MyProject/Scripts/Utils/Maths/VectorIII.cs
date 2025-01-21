using UnityEngine;

public enum Axis { x, y, z }

[System.Serializable]
public class VectorIII
{
    public float x, y, z;
    public VectorIII(float a, float b, float c)
    {
        x = a;
        y = b;
        z = c;
    }

    public VectorIII(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
    public Vector3Int ToVector3Int()
    {
        return new Vector3Int((int)x, (int)y, (int)z);
    }
    public Quaternion ToQuaternion()
    {
        return Quaternion.Euler(x, y, z);
    }

    public float Axis(Axis axis)
    {
        switch (axis)
        {
            case (Axis)0: return x;
            case (Axis)1: return y;
            default: return z;
        }
    }

    public float MaxAxis()
    {
        return x > y ? (x > z ? x : z) : (y > z ? y : z);
    }

    public float MinAxis()
    {
        return x < y ? (x < z ? x : z) : (y < z ? y : z);
    }

    public Vector3Int RoundTo(int unit)
    {
        Vector3Int key = Vector3Int.zero;
        key.x = Mathf.RoundToInt(x) / unit;
        key.y = Mathf.RoundToInt(y) / unit;
        key.z = Mathf.RoundToInt(z) / unit;
        return key;
    }
    public Vector3 ToVectorXZ()
    {
        return new Vector3(x, 0, z);
    }

    public override string ToString()
    {
        return "(" + x + "," + y + "," + z + ")";
    }
}