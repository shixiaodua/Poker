using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
[ProtoContract]
public class RoomInfo
{
    /// <summary>
    /// 房间id
    /// </summary>
    [ProtoMember(1)]
    public int id;
    /// <summary>
    /// 房间内玩家数
    /// </summary>
    [ProtoMember(2)]
    public int playerNum;
    /// <summary>
    /// 房间状态
    /// </summary>
    [ProtoMember(3)]
    public RoomState state;
}
/// <summary>
/// 玩家信息
/// </summary>
[ProtoContract]
public class PlayerInfo
{
    public PlayerInfo()
    {
        name = "";
        coin = 0;
        roomId = -1;
    }
    /// <summary>
    /// 玩家名称
    /// </summary>
    [ProtoMember(1)]
    public string name;
    /// <summary>
    /// 玩家剩余金钱
    /// </summary>
    [ProtoMember(2)]
    public int coin;
    /// <summary>
    /// 玩家当前所在房间
    /// </summary>
    [ProtoMember(3)]
    public int roomId;
}
[ProtoContract]
public class Card
{
    /// <summary>
    /// 牌型
    /// </summary>
    [ProtoMember(1)]
    public int cardNum;
    /// <summary>
    /// 花色
    /// </summary>
    [ProtoMember(2)]
    public int hs;
}
