
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏场景所需的资源枚举
/// </summary>
public enum GameResEnum
{
    BattlePanel,
}
public class GameScene : BaseScene<GameResEnum>
{
    public GameObject battlePanelPreafab;
    /// <summary>
    /// 是否是抢地主流程
    /// </summary>
    public bool isQdzState = true;
    /// <summary>
    /// 是否等待其他玩家
    /// </summary>
    public bool isWait = false;
    /// <summary>
    /// 当前是否已经有人叫地主
    /// </summary>
    public bool isJdz = false;
    /// <summary>
    /// 玩家当前牌面
    /// </summary>
    public List<Card> playerCards;
    /// <summary>
    /// 自己是否是地主
    /// </summary>
    public bool isDz = false;
    /// <summary>
    /// 自己是否第一个出牌
    /// </summary>
    public bool isFrist = false;
    private void Start()
    {
        //加载所需资源
        LoadPanel();
        //初始化
        Init();
        GameManager.Instance.LoadPanel.SetActive(false);
        AddListener();
    }
    private void LoadPanel()
    {
        panelDic[GameResEnum.BattlePanel] = Instantiate(battlePanelPreafab, transform);
        panelDic[GameResEnum.BattlePanel].SetActive(false);
    }
    private void Init()
    {
        isWait = true;
        isQdzState = true;
        isJdz = false;
        isDz = false;
        playerCards = GameManager.Instance.playerCards;
        //初始化
        panelDic[GameResEnum.BattlePanel].GetComponent<BattlePanel>().EnterPanel();
    }
    private void AddListener()
    {
        //添加监听
        //其他玩家播放房间音频
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgRoomAudioPlay2, (msg) =>
        {
            MsgRoomAudioPlay2 result = (MsgRoomAudioPlay2)msg;
            RoomAudioPlay2CallBack(result.userName, result.audioId);
        });
        //监听其他玩家抢地主
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgQdz2, (msg) =>
       {
           MsgQdz2 result = (MsgQdz2)msg;
           Qdz2CallBack(result.userNames, result.isQdz);
       });
        //监听抢地主结束
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgQdzEnd, (msg) =>
        {
            MsgQdzEnd result = (MsgQdzEnd)msg;
            QdzEndCallBack(result.userName, result.dzCards, result.currentCards, result.lastUserName, result.isQdz);
        });
        //监听出牌
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgCp2, (msg) =>
        {
            MsgCp2 result = (MsgCp2)msg;
            CpCallBack(result.userName, result.nextUserName, result.cards, result.currentCards, result.cpType, result.dx);
        });
        //监听不要
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgBy2, (msg) =>
        {
            MsgBy2 result = (MsgBy2)msg;
            ByCallBack(result.userName, result.nextUserName, result.isFrist);
        });
        //监听游戏结束
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgGameEnd, (msg) =>
        {
            MsgGameEnd result = (MsgGameEnd)msg;
            GameEndCallBack(result.isDzWin);
        });
    }
    private void GameEndCallBack(bool isDzWin)
    {
        if (GameManager.Instance.sceneName == "GameScene")
        {
            panelDic[GameResEnum.BattlePanel].GetComponent<BattlePanel>().GameEnd(isDzWin);
        }
        else
        {
            Debug.Log("场景错误,无法播放");
        }
    }
    private void ByCallBack(string userName, string nextUserName, bool isFrist)
    {
        if (GameManager.Instance.sceneName == "GameScene")
        {
            panelDic[GameResEnum.BattlePanel].GetComponent<BattlePanel>().By(userName, nextUserName, isFrist);
        }
        else
        {
            Debug.Log("场景错误,无法播放");
        }
    }
    private void CpCallBack(string userName, string nextUserName, List<Card> cards, List<Card> currentCards, GameCpType gameCpType, int dx)
    {
        if (GameManager.Instance.sceneName == "GameScene")
        {
            panelDic[GameResEnum.BattlePanel].GetComponent<BattlePanel>().Cp(userName, nextUserName, cards, currentCards, gameCpType, dx);
        }
        else
        {
            Debug.Log("场景错误,无法播放");
        }
    }
    private void QdzEndCallBack(string userName, List<Card> dzCards, List<Card> cards, string lastUserName, bool isQdz)
    {
        if (GameManager.Instance.sceneName == "GameScene")
        {
            panelDic[GameResEnum.BattlePanel].GetComponent<BattlePanel>().QdzEnd(userName, dzCards, cards, lastUserName, isQdz);
        }
        else
        {
            Debug.Log("场景错误,无法播放");
        }
    }
    private void Qdz2CallBack(List<string> names, bool isQdz)
    {
        if (GameManager.Instance.sceneName == "GameScene")
        {
            panelDic[GameResEnum.BattlePanel].GetComponent<BattlePanel>().UpdateQdzNextPlayer(names, isQdz);
        }
        else
        {
            Debug.Log("场景错误,无法播放");
        }
    }
    private void RoomAudioPlay2CallBack(string userName, int audioId)
    {
        if (GameManager.Instance.sceneName == "GameScene")
        {
            panelDic[GameResEnum.BattlePanel].GetComponent<BattlePanel>().ChatAudipPlay(userName, audioId);
        }
        else
        {
            Debug.Log("场景错误,无法播放");
        }
    }

}
