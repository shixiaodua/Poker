using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject LoadPanel;
    /// <summary>
    /// 玩家信息
    /// </summary>
    public PlayerInfo playerInfo;
    /// <summary>
    /// 玩家卡牌
    /// </summary>
    public List<Card> playerCards;
    /// <summary>
    /// 玩家出牌顺序
    /// </summary>
    public List<PlayerInfo> userSx;
    public int roomId;//房间号
    public string sceneName = "startScene";
    /// <summary>
    /// 主线程队列
    /// </summary>
    public static readonly Queue<Action> executionQueue = new Queue<Action>();
    public GameObject tipsPrefab;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);//设置为不销毁
        Instance = this;
    }
    private void Start()
    {
        NetManager.Instance.Connect();
        LoadPanel = GameObject.Find("LoadPanel");
    }
    private void Update()
    {
        NetManager.Instance.Update();
        // 执行主线程队列中的操作
        lock (executionQueue)
        {
            while (executionQueue.Count > 0)
            {
                var action = executionQueue.Dequeue();
                action?.Invoke();
            }
        }
    }
    /// <summary>
    /// 第一次连接成功调用
    /// </summary>
    public void ConnectSuccess()
    {
        executionQueue.Enqueue(() =>
        {
            EnterScene(SceneEnum.LoginScene.ToString());
        });
    }
    public void StartGame(List<Card> cards, List<PlayerInfo> userSx, int roomId)
    {
        this.playerCards = cards;
        this.userSx = userSx;
        this.roomId = roomId;
        EnterScene(SceneEnum.GameScene.ToString());
    }
    public void EnterScene(string sceneName)
    {
        try
        {
            LoadPanel.SetActive(true);//只能在主线程中执行
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
        StartCoroutine(LoadScene(sceneName));
    }
    private IEnumerator LoadScene(string sceneName)
    {
        Debug.Log("开始加载场景:" + sceneName);
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone)
        {
            LoadPanel.GetComponent<LoadPanel>().SetJdtValue(load.progress);
            yield return null;
        }
        this.sceneName = sceneName;
    }
    public void Tips(string tips)
    {
        GameObject tipsObj = Instantiate(tipsPrefab, GameObject.Find("Canvas").transform);
        tipsObj.GetComponent<TipsPanel>().Play(tips);
    }
}
