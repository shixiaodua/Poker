using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T>where T : new()
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //Debug.Log("Singleton is null");
                instance = new T();
            }
            return instance;
        }
    }

    protected Singleton() { }
}
