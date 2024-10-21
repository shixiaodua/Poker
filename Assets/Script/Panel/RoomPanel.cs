using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel<MainScene>
{
    public GameObject playerItem;
    public Transform content;
    public Text readyText;
    public ChatListPanel chatListPanel;
    /// <summary>
    /// 当前玩家是否准备
    /// </summary>
    [HideInInspector]
    public bool isReady = false;
    public void OnClickExit()
    {
        SendMsg.ExitRoom(GameManager.Instance.roomId, ExitRoomCallback);
    }
    public void OnClickReady()
    {
        SendMsg.Ready(GameManager.Instance.roomId, !isReady, ReadyCallback);
    }
    public void OnClickChat()
    {
        chatListPanel.gameObject.SetActive(!chatListPanel.gameObject.activeSelf);
    }
    private void ReadyCallback(ReadyResult readyResult)
    {
        if (readyResult == ReadyResult.Success)
        {
            Debug.Log("准备成功");
            isReady = !isReady;
            readyText.text = isReady ? "取消" : "准备";
        }
        else
        {
            Debug.Log("准备失败");
        }
    }
    private void ExitRoomCallback(ExitRoomResult exitRoomResult)
    {
        if (exitRoomResult == ExitRoomResult.Success)
        {
            Debug.Log("退出房间");
            ExitPanel();
            scene.getPanel(MainResEnum.RoomList).GetComponent<RoomListPanel>().EnterPanel();
        }
        else
        {
            Debug.Log("退出房间失败");
        }
    }
    public void AudipPlay(string userName, int audioId)
    {
        AudioClip audioClip = chatListPanel.GetAudioClip(audioId);
        if (audioClip != null)
        {
            GetPlayerItem(userName).ChatPlayAudio(audioClip, audioId);
        }

    }
    public void AddPlayer(PlayerInfo playerInfo, bool isReady)
    {
        GameObject item = Instantiate(playerItem, content);
        item.SetActive(true);
        item.GetComponent<PlayerItem>().SetPlayerInfo(playerInfo, isReady);
    }
    public void ExitPlayer(string userName)
    {
        Destroy(GetPlayerItem(userName).gameObject);
    }
    private PlayerItem GetPlayerItem(string userName)
    {
        foreach (Transform child in content)
        {
            if (child.GetComponent<PlayerItem>().playerName.text == userName)
            {
                return child.GetComponent<PlayerItem>();
            }
        }
        return null;
    }

    public void UpdateReady(string userName, bool isReady)
    {
        GetPlayerItem(userName).SetPlayerState(isReady);
    }
    public void ClearPlayerList()
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

    public void EnterPanel(Dictionary<int, PlayerInfo> players, Dictionary<int, bool> isReadys)
    {
        Init();
        UpdatePlayers(players, isReadys);
    }
    public void EnterPanel(PlayerInfo playerInfo, bool isReady, int roomId)
    {
        GameManager.Instance.roomId = roomId;
        Init();
        AddPlayer(playerInfo, isReady);
    }
    public void EnterPanel(int roomId)
    {
        GameManager.Instance.roomId = roomId;
        Init();
    }
    private void Init()
    {
        ClearPlayerList();
        gameObject.SetActive(true);
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        isReady = false;
        readyText.text = "准备";
    }
    public void UpdatePlayers(Dictionary<int, PlayerInfo> players, Dictionary<int, bool> isReadys)
    {
        //遍历players，创建玩家
        foreach (var key in players.Keys)
        {
            AddPlayer(players[key], isReadys[key]); //添加玩家
        }
        UpdateContent();
    }
    public void ExitPanel()
    {
        GameManager.Instance.roomId = -1;
        gameObject.SetActive(false);
    }
}
