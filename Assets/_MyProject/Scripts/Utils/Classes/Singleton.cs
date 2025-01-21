using System;
using UnityEngine;

/// <summary>
/// Base class for every manager in the program. Every manager <b>MUST</b> inherit this class to reduce redundant code and increase help encapsulate it's features. 
/// <remarks>
/// No matter if it is used locally or networked, it will have to be derived from <b>NetworkBehaviour</b>.
/// When the user isn't in a session, they are hosting their own local server on their computer/device.
/// </remarks>. 
/// </summary>
/// <typeparam name="T">Input the class that is actively inheriting this class</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Ins;
    /// <summary>
    /// Look through the scene and check if this manager already exist.
    /// </summary>
    #region Singleton Life-time Management

    /// <summary>
    ///     Unity3D Awake method.
    /// </summary>
    /// <remarks>
    ///     This method will only be called once even if multiple instances of the
    ///     singleton MonoBehaviour exist in the scene.
    ///     You can override this method in derived classes to customize the initialization of your MonoBehaviour
    /// </remarks>
    
    public virtual void Awake()
    {
        var instance = GetComponent<T>();
        // Initialize the singleton if the script is already in the scene in a GameObject
        if (Ins == null)
        {
            Ins = instance;
        }
        else if (instance != Ins)
        {
            print($"Found a duplicated instance of a Singleton with type {GetType()} in the GameObject {gameObject.name}");
            return;
        }
    }

    /// <remarks>
    /// <b>DO NOT</b> override/implement the <b>Start</b> method in your the child class.
    /// <para>Instead use <b>OnManagerStart</b></para>
    /// </remarks>


    /// <remarks>
    /// <b>DO NOT</b> override/implement the <b>OnDestroy</b> method in your the child class.
    /// <para>Instead use <b>OnManagerDestroy</b></para>
    /// </remarks>
    public virtual void OnDestroy()
    {
        // Here we are dealing with a duplicate so we don't need to shut the singleton down
        if (this != Ins)
            return;

        /*
         * Flag set when Unity sends the message OnDestroy to this Component.
         * This is needed because there is a chance that the GO holding this singleton
         * is destroyed before some other object that also access this singleton when is being destroyed.
         * As the singleton instance is null, that would create both a new instance of this
         * MonoBehaviourSingleton and a brand new GO to which the singleton instance is attached to..

         * However as this is happening during the Unity app shutdown for some reason the newly created GO
         * is kept in the scene instead of being discarded after the game exists play mode.
         * (Unity bug?)
         */
    }

    // TODO: Figure out if it is worth having the updates here to and creating a OnManager[Fixed/Late]Update method,
    // so we can do some general-purpose analytics and other stuff on all the managers.

    #endregion

}
