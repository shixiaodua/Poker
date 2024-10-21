using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene<T> : MonoBehaviour
{
    public Dictionary<T, GameObject> panelDic = new Dictionary<T, GameObject>();
    public GameObject getPanel(T key)
    {
        if (panelDic.ContainsKey(key))
        {
            return panelDic[key];
        }
        else
        {
            Debug.Log("panelDic not contain key");
            return null;
        }
    }
}
