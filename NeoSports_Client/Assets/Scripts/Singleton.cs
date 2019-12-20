using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
	static bool errorNoticed = false;

    public static T Instance {
        get {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;

                if (instance == null && !errorNoticed)
                {
					errorNoticed = true;
					Debug.LogError("There's no active " + typeof(T) + " in this scene");
                }
            }

            return instance;
        }
    }
}
