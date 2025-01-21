using UnityEngine;

/// <summary>
/// This is a class used to find meshes boundary, size and center
/// <remarks></remarks>
/// </summary>
[System.Serializable]
public class MeshEdges
{
    public Vector3 min;
    public Vector3 max;

    public MeshEdges()
    {
        min = new Vector3(+999, +999, +999);
        max = new Vector3(-999, -999, -999);
    }

    public MeshEdges(Vector3 centre)
    {
        min = max = centre;
    }

    public MeshEdges(Vector3 _min, Vector3 _max)
    {
        min = _min;
        max = _max;
    }

    public Vector3 Center()
    {
        return new Vector3((min.x + max.x) / 2f, (min.y + max.y) / 2f, (min.z + max.z) / 2f);
    }

    public bool Contains(Vector3 position)
    {
        return position.x >= min.x && position.x <= max.x && position.y >= min.y && position.y <= max.y && position.z >= min.z && position.z <= max.z;
    }

    public Vector3 Size()
    {
        return new Vector3(max.x - min.x, max.y - min.y, max.z - min.z);
    }

    public float MaxSize()
    {
        Vector3 size = Size();
        return size.x > size.y ? (size.x > size.z ? size.x : size.z) : (size.y > size.z ? size.y : size.z);
    }

    public MeshEdges CompareAndGetExtremeEdges(MeshEdges toCompare)
    {
        toCompare.min.x = min.x < toCompare.min.x ? min.x : toCompare.min.x;
        toCompare.min.y = min.y < toCompare.min.y ? min.y : toCompare.min.y;
        toCompare.min.z = min.z < toCompare.min.z ? min.z : toCompare.min.z;
        toCompare.max.x = max.x > toCompare.max.x ? max.x : toCompare.max.x;
        toCompare.max.y = max.y > toCompare.max.y ? max.y : toCompare.max.y;
        toCompare.max.z = max.z > toCompare.max.z ? max.z : toCompare.max.z;
        return toCompare;
    }

    public override string ToString()
    {
        return "min:" + min + ",max:" + max;
    }

    public static MeshEdges GetEdges(GameObject obj)
    {
        MeshEdges meshEdges = null;
        return GetEdges(obj.transform.GetComponent<MeshRenderer>(), ref meshEdges);
    }
    public static MeshEdges GetEdges(GameObject[] objs)
    {
        MeshEdges meshEdges = null;
        for (int i = 0; i < objs.Length && objs[i].GetComponent<MeshRenderer>(); i++)
            meshEdges = GetEdges(objs[i].GetComponent<MeshRenderer>(), ref meshEdges);
        return meshEdges;
    }

    public static MeshEdges GetEdges(MonoBehaviour[] objs)
    {
        MeshEdges meshEdges = null;
        for (int i = 0; i < objs.Length && objs[i].gameObject.GetComponent<MeshRenderer>(); i++)
            meshEdges = GetEdges(objs[i].gameObject.GetComponent<MeshRenderer>(), ref meshEdges);
        return meshEdges;
    }

    public static MeshEdges GetEdgesWithChildren(GameObject obj)
    {
        MeshEdges meshEdges = null;
        foreach (var renderer in obj.GetComponentsInChildren<MeshRenderer>())
            meshEdges = GetEdges(renderer, ref meshEdges);
        return meshEdges;
    }

    static MeshEdges GetEdges(MeshRenderer renderer, ref MeshEdges meshEdges)
    {
        if (renderer.gameObject.activeSelf)
        {
            Bounds bounds = renderer.bounds;  //Debug.DrawLine(meshEdges.min, meshEdges.max, Color.blue, 100);
            MeshEdges tempEdges = new MeshEdges(bounds.min, bounds.max); //Debug.Log(renderer.gameObject.name + " (" + bounds.min + "," + bounds.max + ") , " + meshEdges);
            meshEdges = meshEdges != null ? tempEdges.CompareAndGetExtremeEdges(meshEdges) : tempEdges;
        }
        return meshEdges;
    }
}
