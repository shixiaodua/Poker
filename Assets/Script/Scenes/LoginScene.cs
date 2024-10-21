using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 登录场景所需的资源枚举
/// </summary>
public enum LoginResEnum
{
    Login,
    Register
}
public class LoginScene : BaseScene<LoginResEnum>
{

    /// <summary>
    /// 登录界面预制体
    /// </summary>
    public GameObject loginPreafab;
    /// <summary>
    /// 注册界面预制体
    /// </summary>
    public GameObject registerPreafab;
    /// <summary>
    /// 登录界面
    /// </summary>
    /// 
    private void Start()
    {
        //加载所需资源
        LoadPanel();
        //初始化
        Init();
        //隐藏加载面板
        GameManager.Instance.LoadPanel.SetActive(false);
    }
    private void LoadPanel()
    {
        panelDic[LoginResEnum.Login] = Instantiate(loginPreafab, transform);
        panelDic[LoginResEnum.Register] = Instantiate(registerPreafab, transform);
        panelDic[LoginResEnum.Login].SetActive(false);
        panelDic[LoginResEnum.Register].SetActive(false);
    }
    private void Init()
    {
        //初始化
        panelDic[LoginResEnum.Login].GetComponent<LoginPanel>().EnterPanel();
    }

    public void ExitScene()
    {

    }
}
