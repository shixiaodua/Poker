using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsPanel : MonoBehaviour
{
    public Text text;
    public void Play(string tips)
    {
        //play tips
        text.text = tips;
        transform.GetComponent<Animation>().Play();
        StartCoroutine(DestoryTpis());
    }
    public IEnumerator DestoryTpis()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
