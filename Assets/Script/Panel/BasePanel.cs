using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel<T> : MonoBehaviour
{
    /// <summary>
    /// 获取场景脚本
    /// </summary>
    [HideInInspector]
    public T scene;
    private void Awake()
    {
        scene = GameObject.Find("Canvas").GetComponent<T>();
    }
}
