using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// This is a class containing generic color and sprite functions
/// <remarks></remarks>
/// </summary>
public class IO
{
    /// <summary> Share mp4 video to other users </summary>
    public static void Share(string VIDEO_NAME = "Record")
    {
        string path = Path.Combine(Application.persistentDataPath, VIDEO_NAME + ".mp4");
        //new NativeShare().AddFile(path, "video/mp4").SetSubject("www.heavycourt.com").SetText(Project.appUrl).Share();
    }
    /// <summary> Load bytes array from persistent path </summary>
    public static string GetPath(string filename)
    { // Returns the used path with filename and its ending
        return Path.Combine(Application.persistentDataPath, filename);
    }
    /// <summary> Save bytes array to persistent path </summary>
    public static void SaveFile(byte[] bytes, string name, bool overwrite = true)
    { // save file from persistent path
        string destination = Path.Combine(Application.persistentDataPath, name);
        if (overwrite || !File.Exists(destination))
            File.WriteAllBytes(destination, bytes);
    }
    /// <summary> Load image from persistent path </summary>
    public static Sprite LoadImage(string name)
    {
        string destination = Path.Combine(Application.persistentDataPath, name + ".png");
        if (File.Exists(destination))
            return TextureToSprite(LoadTexture(destination));
        return null;  //if not exist then create first
    }
    /// <summary> Save image to persistent path </summary>
    public static void SaveImage(Texture2D texture, string name, bool overwrite = true, bool freeMemory = true)
    {
        string destination = Path.Combine(Application.persistentDataPath, name + ".png");
        if (overwrite || !File.Exists(destination))
            File.WriteAllBytes(destination, texture.EncodeToPNG());
        if (freeMemory)
            GameObject.Destroy(texture); // for free the memory again
    }
    /// <summary> Delete image from persistent path </summary>
    public static void DeleteImage(string name)
    {
        string path = Path.Combine(Application.persistentDataPath, name + ".png");
        if (File.Exists(path))
            File.Delete(path); //Delete photo permenantly
    }

    public static Texture2D LoadTexture(string destination)
    {
        Texture2D texture = new Texture2D(0, 0, TextureFormat.RGBA32, true);
        texture.LoadImage(File.ReadAllBytes(destination));
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }
    public static byte[] TextureToBytes(Texture2D texture)
    {
        return texture.EncodeToPNG();
    }
    public static Texture2D BytesToTexture(byte[] bytes)
    {
        Texture2D texture = new Texture2D(0, 0, TextureFormat.RGBA32, true);
        texture.LoadImage(bytes);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    public static Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
    }
    /// <summary> Assign the loaded audio clip to the source or does nothing if the argument filename or path is inexistend. Call this method inside the StartCoroutine of C# </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">The filename with path of audio file</param>
    /// <param name="action">The event when audio file loaded</param>
    public static IEnumerator LoadAudio(string path, Action<AudioClip> action)
    {
        if (File.Exists(path))
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + path, AudioType.MPEG))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError )
                    Debug.Log(www.error);
                else
                    action(DownloadHandlerAudioClip.GetContent(www));
            }
        }
    }
    /// <summary> The function to save any data onto a file </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName">The filename, can be the project name for projects, username for userdata etc</param>
    /// <param name="classObj">The serializable class that needs to be saved</param>
    public static void SerializeFile<T>(string fileName, T classObj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(Path.Combine(Application.persistentDataPath, fileName), FileMode.OpenOrCreate);
        bf.Serialize(file, classObj);
        file.Close();
    }
    /// <summary> The function to load data from a file onto the memory </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName">The filename, can be the project name for projects, username for userdata etc</param>
    /// <param name="classObj">The class in which the object will be deserialized</param>
    public static bool DeserializeFile<T>(string fileName, ref T classObj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(Path.Combine(Application.persistentDataPath, fileName)))
        {
            FileStream file = new FileStream(Path.Combine(Application.persistentDataPath, fileName), FileMode.Open);
            classObj = (T)bf.Deserialize(file);
            file.Close();
            return true;
        }
        return false;
    }
    /// <summary> Checking that hte files exists or not </summary>
    /// <param name="fileName">File name to search for</param>
    /// <returns>True or false depending if the the file exists or not</returns>
    public static bool FileExists(string fileName)
    {
        return File.Exists(Path.Combine(Application.persistentDataPath, fileName));
    }
    /// <summary> Returns file size </summary>
    /// <param name="fileName">The name of the file</param>
    /// <returns>Returns the file size</returns>
    public static float FileSize(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        long info = new FileInfo(path).Length;
        return info;
    }
    /// <summary> The function to delete data from a file </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName">The filename, can be the project name for projects, username for userdata etc</param>
    public static void DeleteFile(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
