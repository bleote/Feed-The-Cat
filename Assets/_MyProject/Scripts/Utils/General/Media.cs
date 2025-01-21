using System.IO;
using UnityEngine;
using System.Threading;


/// <summary>
/// This is a class containing generic color and sprite functions
/// <remarks></remarks>
/// </summary>
public static class Media
{
    public static Color ToGray(Color c)
    {
        c.r = c.g = c.b = (c.r + c.g + c.b) / 15f + .6f;
        return c;
    }

    public static Color ToGray2(Color c)
    {
        c.r = c.g = c.b = (c.r + c.g + c.b) / 3f + .1f;
        return c;
    }

    public static float ColorDifference(Color c1, Color c2)
    {
        return Mathf.Abs(c1.r - c2.r) + Mathf.Abs(c1.g - c2.g) + Mathf.Abs(c1.b - c2.b);
    }

    public static Color Get32BitColor(Color c)
    {
        return new Color((int)(c.r * 8) / 8f, (int)(c.g * 8) / 8f, (int)(c.b * 8) / 8f, c.a);
    }

    public static Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }

    public static string ColorToHex(Color color)
    {
        string r = ((int)(color.r * 255)).ToString("X");
        string g = ((int)(color.g * 255)).ToString("X");
        string b = ((int)(color.b * 255)).ToString("X");
        return (r.Length == 1 ? "0" + r : r) + (g.Length == 1 ? "0" + g : g) + (b.Length == 1 ? "0" + b : b);
    }

    public static Vector2Int WorldToImagePixel(Vector2 mouseWorldPosition, Vector2 imagePosition, Vector2 imageWorldSize, Vector2Int imagePixelSize)
    {
        Vector2Int pixelIndex = Vector2Int.zero;
        pixelIndex.x = Mathf.FloorToInt((mouseWorldPosition.x + imageWorldSize.x / 2f - imagePosition.x) / imageWorldSize.x * imagePixelSize.x);
        pixelIndex.y = Mathf.FloorToInt((mouseWorldPosition.y + imageWorldSize.y / 2f - imagePosition.y) / imageWorldSize.y * imagePixelSize.y);
        return pixelIndex;
    }

    public static Vector2 ImageToWorldPosition(Vector2Int pixel, Vector2 imagePosition, Vector2 imageWorldSize, Vector2Int imagePixelSize)
    {
        Vector2 position = Vector2.zero;
        position.x = (pixel.x + .5f) / imagePixelSize.x * imageWorldSize.x - imageWorldSize.x / 2f + imagePosition.x;
        position.y = (pixel.y + .5f) / imagePixelSize.y * imageWorldSize.y - imageWorldSize.y / 2f + imagePosition.y;
        return position;
    }

    public static Texture2D RescaleTexture(Texture2D texture, int size)
    {
        int width, height;
        if (texture.width > texture.height)
        { // for auto resize height
            width = size > texture.width ? texture.width : size; // if bigger than maxDimension than make it maxDimension
            height = (int)(1f * width * texture.height / texture.width);
        }
        else
        { // for auto resize width
            height = size > texture.height ? texture.height : size; // if bigger than maxDimension than make it maxDimension
            width = (int)(1f * height * texture.width / texture.height);
        }
        TextureScale.Bilinear(texture, width, height);
        return texture;
    }

    public static Texture2D CropTexture(Texture2D texture, int width, int height, bool freeMemory = true)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false); // RGBA for high quality image
        int deltaX = width / 2 - texture.width / 2;
        int deltaY = height / 2 - texture.height / 2;
        for (int i = 0; i < width; i++) // Reading Image
            for (int j = 0; j < height; j++)
                tex.SetPixel(i, j, i >= deltaX && j >= deltaY && i < (width - deltaX) && j < (height - deltaY) ? texture.GetPixel(i - deltaX, j - deltaY) : Color.white);
        if (freeMemory)
            Object.Destroy(texture); // for free the memory again
        tex.Apply();
        return tex;
    }

    public static Texture2D CropTexture(Texture2D texture, int widthRatio, int heightRatio, Vector2 pivot, bool freeMemory = true)
    {
        RectInt rect;
        if (1f * texture.width / texture.height > 1f * widthRatio / heightRatio)
        { // to determine that match width or height (which is greater) with screen size 
            widthRatio = widthRatio * texture.height / heightRatio; // make your picture width acording to required size but do not disturb ratio
            heightRatio = texture.height; // match height
            int x = (int)((texture.width * pivot.x) - (widthRatio / 2f)); // orignal width * pivot - half required height
            x = x < 0 ? 0 : (x > texture.width - widthRatio ? texture.width - widthRatio : x); //for handling image pixels out of bounds
            rect = new RectInt(x, 0, widthRatio, heightRatio);
        }
        else
        {
            heightRatio = heightRatio * texture.width / widthRatio; // make your picture height acording to required size but do not disturb ratio
            widthRatio = texture.width; // this comes after height calculation // match width
            int y = (int)((texture.height * pivot.y) - (heightRatio / 2f)); // orignal height * pivot - half required height
            y = y < 0 ? 0 : (y > texture.height - heightRatio ? texture.height - heightRatio : y);  //for handling image pixels out of bounds
            rect = new RectInt(0, y, widthRatio, heightRatio);
        }
        Texture2D tex = new Texture2D(rect.width, rect.height, TextureFormat.RGB24, false); // RGBA for high quality image
        Color c;
        for (int i = 0; i < rect.width; i++)
            for (int j = 0; j < rect.height; j++)
                tex.SetPixel(i, j, ((c = texture.GetPixel(i + rect.x, j + rect.y)).a == 0) ? Color.white : c); //for png set background color white // Reading Image from min points of rect
        if (freeMemory)
            Object.Destroy(texture); // for free the memory again
        tex.Apply();
        return tex;
    }

    public static Texture2D RotateTexture(Texture2D source)
    {
        Texture2D texture = new Texture2D(source.height, source.width);
        for (int i = 0; i < texture.width; i++)
            for (int j = 0; j < texture.height; j++)
                texture.SetPixel(i, j, source.GetPixel(texture.height - (j + 1), i));
        Object.Destroy(source); // for free the memory again
        texture.Apply();
        return texture;
    }

    public static Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
    }

    public static Texture2D GrayScaleTexture(Texture2D texture)
    {
        for (int i = 0; i < texture.width; i++)
            for (int j = 0; j < texture.height; j++)
                texture.SetPixel(i, j, ToGray2(texture.GetPixel(i, j)));
        texture.Apply();
        return texture;
    }

    public static Texture2D LoadTexture(string destination)
    {
        Texture2D texture = new Texture2D(0, 0, TextureFormat.RGB24, false);
        texture.LoadImage(File.ReadAllBytes(destination));
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }
}

public class TextureScale
{
    public class ThreadData
    {
        public int start;
        public int end;
        public ThreadData(int s, int e)
        {
            start = s;
            end = e;
        }
    }

    private static Color[] texColors;
    private static Color[] newColors;
    private static int w;
    private static float ratioX;
    private static float ratioY;
    private static int w2;
    private static int finishCount;
    private static Mutex mutex;

    public static void Point(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, false);
    }

    public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, true);
    }

    private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
    {
        texColors = tex.GetPixels();
        newColors = new Color[newWidth * newHeight];
        if (useBilinear)
        {
            ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
            ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
        }
        else
        {
            ratioX = ((float)tex.width) / newWidth;
            ratioY = ((float)tex.height) / newHeight;
        }
        w = tex.width;
        w2 = newWidth;
        int cores = Mathf.Min(SystemInfo.processorCount, newHeight);
        int slice = newHeight / cores;

        finishCount = 0;
        if (mutex == null)
        {
            mutex = new Mutex(false);
        }
        if (cores > 1)
        {
            int i = 0;
            ThreadData threadData;
            for (i = 0; i < cores - 1; i++)
            {
                threadData = new ThreadData(slice * i, slice * (i + 1));
                ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
                Thread thread = new Thread(ts);
                thread.Start(threadData);
            }
            threadData = new ThreadData(slice * i, newHeight);
            if (useBilinear)
                BilinearScale(threadData);
            else
                PointScale(threadData);
            while (finishCount < cores)
                Thread.Sleep(1);
        }
        else
        {
            ThreadData threadData = new ThreadData(0, newHeight);
            if (useBilinear)
                BilinearScale(threadData);
            else
                PointScale(threadData);
        }
        tex.Reinitialize(newWidth, newHeight);
        tex.SetPixels(newColors);
        Object.Destroy(tex); // for free the memory again
        tex.Apply();

        texColors = null;
        newColors = null;
    }

    public static void BilinearScale(System.Object obj)
    {
        ThreadData threadData = (ThreadData)obj;
        for (int y = threadData.start; y < threadData.end; y++)
        {
            int yFloor = (int)Mathf.Floor(y * ratioY);
            int y1 = yFloor * w;
            int y2 = (yFloor + 1) * w;
            int yw = y * w2;

            for (int x = 0; x < w2; x++)
            {
                int xFloor = (int)Mathf.Floor(x * ratioX);
                float xLerp = x * ratioX - xFloor;
                newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
                    ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
                    y * ratioY - yFloor);
            }
        }

        mutex.WaitOne();
        finishCount++;
        mutex.ReleaseMutex();
    }

    public static void PointScale(System.Object obj)
    {
        ThreadData threadData = (ThreadData)obj;
        for (int y = threadData.start; y < threadData.end; y++)
        {
            int thisY = (int)(ratioY * y) * w;
            int yw = y * w2;
            for (int x = 0; x < w2; x++)
            {
                newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
            }
        }

        mutex.WaitOne();
        finishCount++;
        mutex.ReleaseMutex();
    }

    private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
    {
        return new Color(c1.r + (c2.r - c1.r) * value,
            c1.g + (c2.g - c1.g) * value,
            c1.b + (c2.b - c1.b) * value,
            c1.a + (c2.a - c1.a) * value);
    }
}
