using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 登录场景所需的资源枚举
/// </summary>
public enum MainResEnum
{
    RoomList,
    Room
}
public class MainScene : BaseScene<MainResEnum>
{

    /// <summary>
    /// 房间列表预制体
    /// </summary>
    public GameObject roomListPreafab;
    /// <summary>
    /// 房间预制体
    /// </summary>
    public GameObject roomPreafab;
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
        panelDic[MainResEnum.RoomList] = Instantiate(roomListPreafab, transform);
        panelDic[MainResEnum.Room] = Instantiate(roomPreafab, transform);
        panelDic[MainResEnum.RoomList].SetActive(false);
        panelDic[MainResEnum.Room].SetActive(false);
    }
    private void Init()
    {
        //初始化
        panelDic[MainResEnum.RoomList].GetComponent<RoomListPanel>().EnterPanel();
    }
    private void AddListener()
    {
        //其他玩家进入房间
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgEnterRoom2, (msg) =>
       {
           MsgEnterRoom2 result = (MsgEnterRoom2)msg;
           EnterRoom2CallBack(result.playerInfoDic, result.isReadyDic);
       });
        //其他玩家退出房间
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgExitRoom2, (msg) =>
       {
           MsgExitRoom2 result = (MsgExitRoom2)msg;
           ExitRoom2CallBack(result.userName);
       });
        //其他玩家点击准备按钮
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgReady2, (msg) =>
        {
            MsgReady2 result = (MsgReady2)msg;
            Ready2CallBack(result.userName, result.isReady);
        });
        //其他玩家播放房间音频
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgRoomAudioPlay2, (msg) =>
        {
            MsgRoomAudioPlay2 result = (MsgRoomAudioPlay2)msg;
            RoomAudioPlay2CallBack(result.userName, result.audioId);
        });
        //玩家全部准备，开始游戏
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgStartGame, (msg) =>
        {
            MsgStartGame result = (MsgStartGame)msg;
            StartGameCallBack(result.cards, result.userInfos, result.roomId);
        });
    }
    private void StartGameCallBack(List<Card> cards, List<PlayerInfo> userSx, int roomId)
    {
        Debug.Log("开始游戏");
        GameManager.Instance.StartGame(cards, userSx, roomId);
    }
    private void RoomAudioPlay2CallBack(string userName, int audioId)
    {
        if (GameManager.Instance.sceneName == "MainScene")
        {
            panelDic[MainResEnum.Room].GetComponent<RoomPanel>().AudipPlay(userName, audioId);
        }
        else
        {
            Debug.Log("场景错误,无法播放");
        }
    }
    private void Ready2CallBack(string userName, bool isReady)
    {
        if (GameManager.Instance.sceneName == "MainScene")
        {
            panelDic[MainResEnum.Room].GetComponent<RoomPanel>().UpdateReady(userName, isReady);
        }
        else
        {
            Debug.Log("场景错误,无法准备");
        }
    }
    private void ExitRoom2CallBack(string userName)
    {
        if (GameManager.Instance.sceneName == "MainScene")
        {
            panelDic[MainResEnum.Room].GetComponent<RoomPanel>().ExitPlayer(userName);
        }
        else
        {
            Debug.Log("场景错误,无法退出房间");
        }
    }
    public void EnterRoom2CallBack(Dictionary<int, PlayerInfo> players, Dictionary<int, bool> isReady)
    {
        if (GameManager.Instance.sceneName == "MainScene")
        {
            panelDic[MainResEnum.Room].GetComponent<RoomPanel>().EnterPanel(players, isReady);
        }
        else
        {
            Debug.Log("场景错误,无法进入房间");
        }
    }

    public void ExitScene()
    {

    }
}
