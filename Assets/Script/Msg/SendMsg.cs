using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 提供发送消息接口
/// </summary>
public static class SendMsg
{
    /// <summary>
    /// 发送登录消息
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="passWord"></param>
    /// <param name="callBack"></param>
    public static void Login(string userName, string passWord, Action<LoginResult, PlayerInfo> callBack)
    {
        MsgLogin MsgLogin = new MsgLogin();
        MsgLogin.userName = userName;
        MsgLogin.passWord = passWord;
        NetManager.Instance.SendMessage(MsgLogin);
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgLogin, (msg) =>
        {
            MsgLogin result = (MsgLogin)msg;
            callBack(result.loginResult, result.playerInfo);
        });
        return;
    }

    /// <summary>
    /// 发送注册消息
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="passWord"></param>
    /// <param name="callBack"></param>
    public static void Register(string userName, string passWord, string email, string yzm, Action<RegisterResult> callBack)
    {
        MsgRegister msgRegister = new MsgRegister();
        msgRegister.userName = userName;
        msgRegister.passWord = passWord;
        msgRegister.email = email;
        msgRegister.yzm = yzm;
        NetManager.Instance.SendMessage(msgRegister);
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgRegister, (msg) =>
        {
            MsgRegister result = (MsgRegister)msg;
            callBack(result.registerResult);
        });
        return;
    }
    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="passWord"></param>
    /// <param name="callBack"></param>
    public static void GetYzm(string userName, string passWord, string email, Action<YzmResult> callBack)
    {
        MsgYzm MsgYzm = new MsgYzm();
        MsgYzm.userName = userName;
        MsgYzm.passWord = passWord;
        MsgYzm.email = email;
        Debug.Log("获取验证码");
        NetManager.Instance.SendMessage(MsgYzm);
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgYzm, (msg) =>
        {
            Debug.Log("收到验证码");
            MsgYzm result = (MsgYzm)msg;
            callBack(result.yzmResult);
        });
        return;
    }
    /// <summary>
    /// 获取房间列表
    /// </summary>
    /// <param name="callBack"></param>
    public static void GetRoomList(Action<UpdateRoomResult, List<RoomInfo>> callBack)
    {
        MsgUpdateRoom msgUpdateRoom = new MsgUpdateRoom();
        Debug.Log("获取房间列表");
        NetManager.Instance.SendMessage(msgUpdateRoom);
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgUpdateRoom, (msg) =>
        {
            MsgUpdateRoom result = (MsgUpdateRoom)msg;
            callBack(result.updateRoomResult, result.roomInfoList);
        });
    }
    public static void CreateRoom(Action<CreateRoomResult, RoomInfo> callBack)
    {
        MsgCreateRoom msgCreateRoom = new MsgCreateRoom();
        Debug.Log("发送创建房间消息");
        NetManager.Instance.SendMessage(msgCreateRoom);
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgCreateRoom, (msg) =>
        {
            MsgCreateRoom result = (MsgCreateRoom)msg;
            callBack(result.createRoomResult, result.roomInfo);
        });
    }
    public static void EnterRoom(int roomId, Action<EnterRoomResult> callBack)
    {
        MsgEnterRoom msgEnterRoom = new MsgEnterRoom();
        msgEnterRoom.roomId = roomId;
        Debug.Log("发送进入房间消息");
        NetManager.Instance.SendMessage(msgEnterRoom);
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgEnterRoom, (msg) =>
        {
            MsgEnterRoom result = (MsgEnterRoom)msg;
            callBack(result.enterRoomResult);
        });
    }
    public static void ExitRoom(int roomId, Action<ExitRoomResult> callBack)
    {
        MsgExitRoom msgExitRoom = new MsgExitRoom();
        msgExitRoom.roomId = roomId;
        Debug.Log("发送退出房间消息");
        NetManager.Instance.SendMessage(msgExitRoom);
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgExitRoom, (msg) =>
        {
            MsgExitRoom result = (MsgExitRoom)msg;
            callBack(result.exitRoomResult);
        });
    }
    public static void Ready(int roomId, bool isReady, Action<ReadyResult> callBack)
    {
        MsgReady msgReady = new MsgReady();
        msgReady.roomId = roomId;
        msgReady.isReady = isReady;
        Debug.Log("发送准备消息");
        NetManager.Instance.SendMessage(msgReady);
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgReady, (msg) =>
        {
            MsgReady result = (MsgReady)msg;
            callBack(result.readyResult);
        });
    }
    public static void RoomAudioPlay(int roomId, int audioId, Action<RoomAudioPlayResult> callBack)
    {
        MsgRoomAudioPlay msgRoomAudioPlay = new MsgRoomAudioPlay();
        msgRoomAudioPlay.roomId = roomId;
        msgRoomAudioPlay.audioId = audioId;
        Debug.Log("发送房间语音播放消息");
        NetManager.Instance.SendMessage(msgRoomAudioPlay);
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgRoomAudioPlay, (msg) =>
        {
            MsgRoomAudioPlay result = (MsgRoomAudioPlay)msg;
            callBack(result.roomAudioPlayResult);
        });
    }
    public static void Qdz(bool isQdz, Action<QdzResult> callBack)
    {
        MsgQdz msgQdz = new MsgQdz();
        msgQdz.isQdz = isQdz;
        Debug.Log("发送抢地主消息");
        NetManager.Instance.SendMessage(msgQdz);
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgQdz, (msg) =>
        {
            MsgQdz result = (MsgQdz)msg;
            callBack(result.qdzResult);
        });
    }
    public static void Cp(List<Card> cards, Action<CpResult> callBack)
    {
        MsgCp msgCp = new MsgCp();
        msgCp.cards = cards;
        Debug.Log("发送出牌消息");
        NetManager.Instance.SendMessage(msgCp);
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgCp, (msg) =>
        {
            MsgCp result = (MsgCp)msg;
            callBack(result.cpResult);
        });
    }
    public static void By(Action<ByResult> callBack)
    {
        MsgBy msgBy = new MsgBy();
        Debug.Log("发送不要消息");
        NetManager.Instance.SendMessage(msgBy);
        NetManager.Instance.AddProtoListener(ProtocolEnum.MsgBy, (msg) =>
        {
            MsgBy result = (MsgBy)msg;
            callBack(result.byResult);
        });
    }
}
