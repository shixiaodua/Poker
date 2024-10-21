
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel<LoginScene>
{
    /// <summary>
    /// 是否登录
    /// </summary>
    public bool isLogin = false;
    /// <summary>
    /// 是否正在登录
    /// </summary>
    public bool isLoging = false;
    /// <summary>
    /// 用户名
    /// </summary>
    [SerializeField]
    public Text userName;
    /// <summary>
    /// 密码
    /// </summary>
    [SerializeField]
    public InputField password;
    /// <summary>
    /// 登录按钮
    /// </summary>
    public void OnClickLogin()
    {
        //检查用户名和密码是否为空
        if (userName.text == "" || password.text == "")
        {
            Debug.Log("用户名或密码不能为空");
            GameManager.Instance.Tips("用户名或密码不能为空");
            return;
        }
        //检查是否正在登录
        if (isLoging)
        {
            Debug.Log("正在登录中");
            GameManager.Instance.Tips("正在登录中");
            return;
        }
        //检查是否已经登录
        if (isLogin)
        {
            Debug.Log("已经登录");
            GameManager.Instance.Tips("已经登录");
            return;
        }
        //开始登录
        isLoging = true;
        Debug.Log("开始登录");
        SendMsg.Login(userName.text, password.text, LoginCallBack);
    }
    /// <summary>
    /// 登录回调
    /// </summary>
    /// <param name="loginResult"></param>
    private void LoginCallBack(LoginResult loginResult, PlayerInfo playerInfo)
    {
        if (loginResult == LoginResult.Success)
        {
            isLogin = true;
            isLoging = false;
            GameManager.Instance.playerInfo = playerInfo;//初始化玩家信息
            Debug.Log("登录成功");
            GameManager.Instance.Tips("登录成功");
            //跳转到主界面
            scene.ExitScene();
            GameManager.Instance.EnterScene(SceneEnum.MainScene.ToString());
        }
        else
        {
            isLoging = false;
            Debug.Log("登录失败");
            GameManager.Instance.Tips("登录失败");
        }
    }
    public void OnClickRegister()
    {
        //弹出注册面板
        ExitPanel();
        GameObject registerPanel = scene.getPanel(LoginResEnum.Register);
        registerPanel.GetComponent<RegisterPanel>().EnterPanel();
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
