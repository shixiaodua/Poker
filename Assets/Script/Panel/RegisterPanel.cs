using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel<LoginScene>
{
    /// <summary>
    /// 是否注册
    /// </summary>
    [HideInInspector]
    public bool isRegister = false;
    /// <summary>
    /// 是否正在注册
    /// </summary>
    [HideInInspector]
    public bool isRegistering = false;
    /// <summary>
    /// 用户名
    /// </summary>
    [SerializeField]
    public Text userName;
    /// <summary>
    /// 密码
    /// </summary>
    [SerializeField]
    public InputField passWord;
    /// <summary>
    /// 确认密码
    /// </summary>
    public InputField passWordAgain;
    /// <summary>
    /// 邮箱
    /// </summary>
    public Text email;
    /// <summary>
    /// 验证码
    /// </summary>
    public Text yzm;
    /// <summary>
    /// 注册按钮
    /// </summary>
    public void OnClickRegister()
    {
        //检查用户名和密码是否为空
        if (userName.text == "" || passWord.text == "" || passWordAgain.text == "" || email.text == "" || yzm.text == "")
        {
            Debug.Log("请完善信息后重试");
            GameManager.Instance.Tips("请完善信息后重试");
            return;
        }
        //检查是否正在注册
        if (isRegistering)
        {
            Debug.Log("正在注册中");
            GameManager.Instance.Tips("正在注册中");
            return;
        }
        //检查是否已经注册
        if (isRegister)
        {
            Debug.Log("已经注册");
            GameManager.Instance.Tips("已经注册");
            return;
        }
        //开始注册
        isRegistering = true;
        Debug.Log("开始注册");
        SendMsg.Register(userName.text, passWord.text, email.text, yzm.text, RegisterCallBack);
    }
    /// <summary>
    /// 注册回调
    /// </summary>
    /// <param name="RegisterResult"></param>
    private void RegisterCallBack(RegisterResult RegisterResult)
    {
        if (RegisterResult == RegisterResult.Success)
        {
            Debug.Log("注册成功");
            GameManager.Instance.Tips("注册成功");
            isRegister = true;
            isRegistering = false;
        }
        else
        {
            isRegistering = false;
            if (RegisterResult == RegisterResult.Fail)
            {
                Debug.Log("注册失败");
                GameManager.Instance.Tips("注册失败");
            }
            else if (RegisterResult == RegisterResult.UserNameExist)
            {
                Debug.Log("用户名已存在");
                GameManager.Instance.Tips("用户名已存在");
            }
        }
    }
    /// <summary>
    /// 返回登录界面
    /// </summary>
    public void OnClickReturn()
    {
        ExitPanel();
        GameObject loginPanel = scene.getPanel(LoginResEnum.Login);
        loginPanel.GetComponent<LoginPanel>().EnterPanel();
    }
    public void OnClickGetYZM()
    {
        if (userName.text == "" || passWord.text == "" || passWordAgain.text == "" || email.text == "")
        {
            Debug.Log("请完善信息后重试");
            GameManager.Instance.Tips("请完善信息后重试");
            return;
        }
        if (passWord.text != passWordAgain.text)
        {
            Debug.Log("两次密码不一致");
            GameManager.Instance.Tips("两次密码不一致");
            return;
        }
        SendMsg.GetYzm(userName.text, passWord.text, email.text, GetYZMCallBack);

    }
    public void GetYZMCallBack(YzmResult yzmResult)
    {
        if (yzmResult == YzmResult.Success)
        {
            Debug.Log("验证码发送成功");
            GameManager.Instance.Tips("验证码发送成功");
        }
        else
        {
            if (yzmResult == YzmResult.Fail)
            {
                Debug.Log("验证码发送失败");
                GameManager.Instance.Tips("验证码发送失败");
            }
            else if (yzmResult == YzmResult.YzmExist)
            {
                Debug.Log("发送过于频繁，请稍后再试");
                GameManager.Instance.Tips("发送过于频繁，请稍后再试");
            }
        }
    }

    public void EnterPanel()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
    public void ExitPanel()
    {
        gameObject.SetActive(false);
    }
}
