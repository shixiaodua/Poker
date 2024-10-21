using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadPanel : MonoBehaviour
{
    public GameObject JdtObject;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        //重置
        SetJdtValue(0);
    }
    public void SetJdtValue(float value)
    {
        JdtObject.GetComponent<Slider>().value = value;
        JdtObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = (value * 100).ToString("0") + "%";
    }
}
