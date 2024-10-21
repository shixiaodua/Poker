using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

public enum ProtocolEnum
{
    None = 0,
    MsgSecret = 1,
    MsgPing = 2,
    MsgRegister = 3,
    MsgLogin = 4,
    MsgYzm = 5,
    MsgCreateRoom = 6,
    MsgUpdateRoom = 7,
    MsgEnterRoom = 8,
    MsgEnterRoom2 = 9,
    MsgExitRoom = 10,
    MsgExitRoom2 = 11,
    MsgReady = 12,
    MsgReady2 = 13,
    MsgRoomAudioPlay = 14,
    MsgRoomAudioPlay2 = 15,
    MsgStartGame = 16,
    MsgQdz = 17,
    MsgQdz2 = 18,
    MsgQdzEnd = 19,
    MsgCp,
    MsgCp2,
    MsgBy,
    MsgBy2,
    MsgGameEnd,
    MsgTest = 9999,
}
/// <summary>
/// 心跳协议
/// </summary>
[ProtoContract]
public class MsgPing : MsgBase
{
    public MsgPing()
    {
        ProtoType = ProtocolEnum.MsgPing;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
}
/// <summary>
/// 登录协议
/// </summary>
[ProtoContract]
public class MsgLogin : MsgBase
{
    public MsgLogin()
    {
        ProtoType = ProtocolEnum.MsgLogin;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    [ProtoMember(2)]
    public string userName;
    [ProtoMember(3)]
    public string passWord;
    //接收字段
    [ProtoMember(4)]
    public LoginResult loginResult; //登录结果
    [ProtoMember(5)]
    public string token; //登录成功后返回的token
    [ProtoMember(6)]
    public PlayerInfo playerInfo; //登录成功后返回的用户信息
}
/// <summary>
/// 注册协议
/// </summary>
[ProtoContract]
public class MsgRegister : MsgBase
{
    public MsgRegister()
    {
        ProtoType = ProtocolEnum.MsgRegister;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    [ProtoMember(2)]
    public string userName;
    [ProtoMember(3)]
    public string passWord;
    [ProtoMember(4)]
    public string email;
    [ProtoMember(5)]
    public string yzm;
    //接收字段
    [ProtoMember(6)]
    public RegisterResult registerResult;
}
[ProtoContract]
public class MsgYzm : MsgBase
{
    public MsgYzm()
    {
        ProtoType = ProtocolEnum.MsgYzm;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    [ProtoMember(2)]
    public string userName;
    [ProtoMember(3)]
    public string passWord;
    [ProtoMember(4)]
    public string email;
    //接收字段
    [ProtoMember(5)]
    public YzmResult yzmResult;
}
[ProtoContract]
public class MsgCreateRoom : MsgBase
{
    public MsgCreateRoom()
    {
        ProtoType = ProtocolEnum.MsgCreateRoom;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public CreateRoomResult createRoomResult;
    [ProtoMember(3)]
    public RoomInfo roomInfo;

}
[ProtoContract]
public class MsgUpdateRoom : MsgBase
{
    public MsgUpdateRoom()
    {
        ProtoType = ProtocolEnum.MsgUpdateRoom;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public UpdateRoomResult updateRoomResult;
    [ProtoMember(3)]
    public List<RoomInfo> roomInfoList;
}
/// <summary>
/// 进入房间协议
/// </summary>
[ProtoContract]
public class MsgEnterRoom : MsgBase
{
    public MsgEnterRoom()
    {
        ProtoType = ProtocolEnum.MsgEnterRoom;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    [ProtoMember(2)]
    public int roomId;
    //接收字段
    [ProtoMember(3)]
    public EnterRoomResult enterRoomResult;
}
/// <summary>
/// 其他人进入房间协议
/// </summary>
[ProtoContract]
public class MsgEnterRoom2 : MsgBase
{
    public MsgEnterRoom2()
    {
        ProtoType = ProtocolEnum.MsgEnterRoom2;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public Dictionary<int, PlayerInfo> playerInfoDic;
    [ProtoMember(3)]
    public Dictionary<int, bool> isReadyDic;
}
[ProtoContract]
public class MsgExitRoom : MsgBase
{
    public MsgExitRoom()
    {
        ProtoType = ProtocolEnum.MsgExitRoom;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    [ProtoMember(2)]
    public int roomId;
    //接收字段
    [ProtoMember(3)]
    public ExitRoomResult exitRoomResult;
}
[ProtoContract]
public class MsgExitRoom2 : MsgBase
{
    public MsgExitRoom2()
    {
        ProtoType = ProtocolEnum.MsgExitRoom2;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public string userName;
}
[ProtoContract]
public class MsgReady : MsgBase
{
    public MsgReady()
    {
        ProtoType = ProtocolEnum.MsgReady;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    [ProtoMember(2)]
    public int roomId;
    [ProtoMember(3)]
    public bool isReady;
    //接收字段
    [ProtoMember(4)]
    public ReadyResult readyResult;
    [ProtoMember(5)]
    public Dictionary<int, bool> isReadyDic;
}
[ProtoContract]
public class MsgReady2 : MsgBase
{
    public MsgReady2()
    {
        ProtoType = ProtocolEnum.MsgReady2;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public string userName;
    [ProtoMember(3)]
    public bool isReady;
}
[ProtoContract]
public class MsgRoomAudioPlay : MsgBase
{
    public MsgRoomAudioPlay()
    {
        ProtoType = ProtocolEnum.MsgRoomAudioPlay;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    [ProtoMember(2)]
    public int roomId;
    [ProtoMember(3)]
    public int audioId;
    //接收字段
    [ProtoMember(4)]
    public RoomAudioPlayResult roomAudioPlayResult;
}
[ProtoContract]
public class MsgRoomAudioPlay2 : MsgBase
{
    public MsgRoomAudioPlay2()
    {
        ProtoType = ProtocolEnum.MsgRoomAudioPlay2;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public int audioId;
    [ProtoMember(3)]
    public string userName;
}
[ProtoContract]
public class MsgStartGame : MsgBase
{
    public MsgStartGame()
    {
        ProtoType = ProtocolEnum.MsgStartGame;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public List<Card> cards;
    [ProtoMember(3)]
    public List<PlayerInfo> userInfos;//玩家队列，第一个为当前回合的玩家，从左往右
    [ProtoMember(4)]
    public int roomId;//房间号
    [ProtoMember(5)]
    public StartGameResult startGameResult;
}
[ProtoContract]
public class MsgQdz : MsgBase
{
    public MsgQdz()
    {
        ProtoType = ProtocolEnum.MsgQdz;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    [ProtoMember(2)]
    public bool isQdz;//是否抢地主
    //接收字段
    [ProtoMember(3)]
    public QdzResult qdzResult;
}
[ProtoContract]
public class MsgQdz2 : MsgBase
{
    public MsgQdz2()
    {
        ProtoType = ProtocolEnum.MsgQdz2;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public List<string> userNames;//玩家队列，第一个为当前回合的玩家，从左往右
    [ProtoMember(3)]
    public bool isQdz;//上一个玩家是否抢地主
}
[ProtoContract]
public class MsgQdzEnd : MsgBase
{
    public MsgQdzEnd()
    {
        ProtoType = ProtocolEnum.MsgQdzEnd;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public string userName;//地主
    [ProtoMember(3)]
    public List<Card> dzCards;//地主三张牌
    [ProtoMember(4)]
    public List<Card> currentCards;//地主当前牌
    [ProtoMember(5)]
    public string lastUserName;//上一个玩家
    [ProtoMember(6)]
    public bool isQdz;//上一个玩家是否抢地主
}
[ProtoContract]
public class MsgCp : MsgBase
{
    public MsgCp()
    {
        ProtoType = ProtocolEnum.MsgCp;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    [ProtoMember(2)]
    public List<Card> cards;//出牌
    //接收字段
    [ProtoMember(4)]
    public CpResult cpResult;
}
[ProtoContract]
public class MsgCp2 : MsgBase
{
    public MsgCp2()
    {
        ProtoType = ProtocolEnum.MsgCp2;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public List<Card> cards;//出牌
    [ProtoMember(3)]
    public string userName;//出牌人
    [ProtoMember(4)]
    public string nextUserName;//上一个出牌人
    [ProtoMember(5)]
    public List<Card> currentCards;//出牌人当前的牌
    [ProtoMember(6)]
    public GameCpType cpType;//出牌类型
    [ProtoMember(7)]
    public int dx;//出牌大小
}
[ProtoContract]
public class MsgBy : MsgBase
{
    public MsgBy()
    {
        ProtoType = ProtocolEnum.MsgBy;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public ByResult byResult;
}
[ProtoContract]
public class MsgBy2 : MsgBase
{
    public MsgBy2()
    {
        ProtoType = ProtocolEnum.MsgBy2;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public string userName;//出牌人
    [ProtoMember(3)]
    public string nextUserName;//下一个出牌人
    [ProtoMember(4)]
    public bool isFrist;//下一个人是否必须出牌
}
[ProtoContract]
public class MsgGameEnd : MsgBase
{
    public MsgGameEnd()
    {
        ProtoType = ProtocolEnum.MsgGameEnd;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtoType { get; set; }
    //发送字段
    //接收字段
    [ProtoMember(2)]
    public bool isDzWin;//是否地主赢
}