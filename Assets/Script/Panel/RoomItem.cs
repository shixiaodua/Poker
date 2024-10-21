using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : BasePanel<MainScene>
{
    /// <summary>
    /// 房间人数
    /// </summary>
    public Text playerNum;
    /// <summary>
    /// 房间id
    /// </summary>
    public Text roomId;
    /// <summary>
    /// 房间状态
    /// </summary>
    public Text roomState;
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        playerNum.text = roomInfo.playerNum.ToString();
        roomId.text = roomInfo.id.ToString();
        roomState.text = roomInfo.state.ToString();
    }
    public void OnClickEnterRoom()
    {
        SendMsg.EnterRoom(int.Parse(roomId.text), EnterRoomCallBack);
    }
    private void EnterRoomCallBack(EnterRoomResult enterRoomResult)
    {
        if (enterRoomResult == EnterRoomResult.Success)
        {
            Debug.Log("进入房间成功");
            scene.getPanel(MainResEnum.Room).GetComponent<RoomPanel>().EnterPanel(int.Parse(roomId.text));
        }
        else
        {
            Debug.Log("进入房间失败");
        }
    }
}
