
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ip : MonoBehaviour
{
    public InputField ipText;
    public void OnClickBtn()
    {
        NetManager.Instance.m_Ip = ipText.text;
        SceneManager.LoadScene("StartScene");
    }
    public void OnClickBtn2()
    {
        SceneManager.LoadScene("StartScene");
    }
}
