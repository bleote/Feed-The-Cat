using System.Collections.Generic;
using UnityEngine;

//>>>This is a class create icon of 3d models and create camera on runtime to do it.
public static class IconMaker
{
    // Camera Properties
    static Camera iconCamera;
    static GameObject camerObj;

    //3D Model Properities (save model state to reset it after making icon)
    static Vector3 modelOldPosition;
    static MeshEdges extremeEdges; //to find boundary points of models
    static Quaternion modelOldRotation;
    static List<int> modelOldLayer = new List<int>();

    public static void Initialize()
    {
        camerObj = new GameObject();
        camerObj.name = "IconMaker";
        iconCamera = camerObj.AddComponent<Camera>();
        iconCamera.nearClipPlane = 0.01f;
        iconCamera.clearFlags = CameraClearFlags.Color;
        iconCamera.cullingMask = LayerMask.GetMask("Water");
    }

    public static Texture2D CreateIcon(GameObject model, int width, int height, Color backgroundColor, bool isOrthographic = false)
    {
        if (iconCamera == null)
            Initialize(); // as this code only needs to run once at start
        width = width < Screen.width ? width : Screen.width; // checking our size with screen size width
        height = height < Screen.height ? height : Screen.height; // checking our size with screen size height
        BeforeCreation(model);
        MeshEdges edges = GetExtremeEdges(model); //Debug.DrawLine(edges.min, edges.max, Color.green, 5000);
        Vector3 modelCentre = edges.Center();
        camerObj.transform.position = modelCentre + new Vector3(-0.75f, 0.75f, -0.75f) * edges.MaxSize();
        camerObj.transform.LookAt(modelCentre);

        RenderTexture renderTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);
        iconCamera.backgroundColor = backgroundColor;
        iconCamera.targetTexture = renderTexture;
        iconCamera.orthographic = isOrthographic;
        iconCamera.orthographicSize = 1f;
        iconCamera.Render();
        RenderTexture.active = renderTexture;

        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGBA32, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();
        RenderTexture.active = null;
        iconCamera.targetTexture = null;

        AfterCreation(model);
        return screenShot;
    }

    static void BeforeCreation(GameObject model)
    {
        iconCamera.enabled = true;
        //Debug.Log("model" + model.layer);
        modelOldPosition = model.transform.position;
        modelOldRotation = model.transform.rotation;
        model.transform.position = Vector3.zero;
        model.transform.rotation = Quaternion.identity;
        SetLayerRecursively (model, LayerMask.NameToLayer("Water"));
    }

    static void AfterCreation(GameObject model)
    {
        iconCamera.enabled = false;
        model.transform.position = modelOldPosition;
        model.transform.rotation = modelOldRotation;
        SetOldLayerRecursively(model);
    }

    static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        modelOldLayer.Add(obj.layer);
        obj.layer = newLayer;
        for (int i = 0; i < obj.transform.childCount; i++)
            SetLayerRecursively(obj.transform.GetChild(i).gameObject, newLayer);
    }

    static void SetOldLayerRecursively(GameObject obj)
    {
        obj.layer = modelOldLayer [0];
        modelOldLayer.RemoveAt (0);
        for (int i = 0; i < obj.transform.childCount; i++)
            SetOldLayerRecursively(obj.transform.GetChild(i).gameObject);
    }

    public static MeshEdges GetExtremeEdges(GameObject obj)
    {
        extremeEdges = new MeshEdges();
        return GetExtremeEdgesRecursively(obj);
    }

    public static MeshEdges GetExtremeEdgesRecursively(GameObject obj)
    {
        if (obj.transform.GetComponent<MeshRenderer>() != null)
        {
            Bounds bounds = obj.transform.GetComponent<MeshRenderer>().bounds;  //Debug.DrawLine(bounds.min, bounds.max, Color.magenta, 50);
            extremeEdges = new MeshEdges(bounds.min, bounds.max).CompareAndGetExtremeEdges(extremeEdges); //print(obj.name + " ("+bounds.min + "," + bounds.max + ") , " + extremeEdges);
        }
        for (int i = 0; i < obj.transform.childCount; i++)
            GetExtremeEdgesRecursively(obj.transform.GetChild(i).gameObject);
        return extremeEdges;
    }
}
