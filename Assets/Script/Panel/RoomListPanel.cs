using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel<MainScene>
{
    /// <summary>
    /// 房间
    /// </summary>
    public GameObject Room;
    /// <summary>
    /// content
    /// </summary>
    public Transform content;
    public Text userName;
    public Text coin;
    public void OnClickUpdate()
    {
        SendMsg.GetRoomList(UpdateCallBack);
    }
    public void OnClickCreate()
    {
        SendMsg.CreateRoom(CreateCallBack);
    }
    private void CreateCallBack(CreateRoomResult result, RoomInfo roomInfo)
    {
        if (result == CreateRoomResult.Success)
        {
            Debug.Log("创建房间成功");
            ExitPanel();
            Debug.Log(scene);
            scene.getPanel(MainResEnum.Room).GetComponent<RoomPanel>().EnterPanel(GameManager.Instance.playerInfo, false, roomInfo.id);
        }
        else
        {
            Debug.Log("创建房间失败");
        }
    }
    private void UpdateCallBack(UpdateRoomResult result, List<RoomInfo> roomList)
    {
        if (result == UpdateRoomResult.Success)
        {
            ClearRoomList();
            if (roomList != null)
            {
                foreach (RoomInfo roomInfo in roomList)
                {
                    AddRoomToList(roomInfo);
                }
            }
            UpdateContent();
        }
        else
        {
            Debug.Log("更新房间列表失败");
        }
    }
    /// <summary>
    /// 添加房间到列表中
    /// </summary>
    public void AddRoomToList(RoomInfo roomInfo)
    {
        GameObject room = Instantiate(Room, content);
        room.SetActive(true);
        room.GetComponent<RoomItem>().SetRoomInfo(roomInfo);
    }
    public void ClearRoomList()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
    /// <summary>
    /// 刷新content的高度
    /// </summary>
    /// <param name="roomInfos"></param>
    public void UpdateContent()
    {
        RectTransform rect = content.GetComponent<RectTransform>();
        if (content.childCount == 0)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 0);
            return;
        }
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, ((content.childCount - 1) / 3 + 1) * content.GetComponent<GridLayoutGroup>().cellSize.y);
    }

    public void EnterPanel()
    {
        userName.text = GameManager.Instance.playerInfo.name;
        coin.text = GameManager.Instance.playerInfo.coin.ToString();
        gameObject.SetActive(true);
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        OnClickUpdate();//更新房间列表
    }
    public void ExitPanel()
    {
        gameObject.SetActive(false);
    }
}
