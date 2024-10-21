using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatListPanel : MonoBehaviour
{
    public List<AudioClip> audioClips;
    public void OnClickAudioBtn()
    {
        //获取按钮
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (clickedButton != null)
        {
            // 获取按钮在父节点下的索引
            int siblingIndex = clickedButton.transform.GetSiblingIndex();
            Debug.Log("Button clicked with sibling index: " + siblingIndex);
            //隐藏面板
            gameObject.SetActive(false);
            //播放音频
            SendMsg.RoomAudioPlay(GameManager.Instance.roomId, siblingIndex, RoomAudioPlayCallBack);
            gameObject.SetActive(false);
        }
    }
    private void RoomAudioPlayCallBack(RoomAudioPlayResult roomAudioPlayResult)
    {
        if (roomAudioPlayResult == RoomAudioPlayResult.Success)
        {
            Debug.Log("音频播放成功");
        }
        else
        {
            Debug.Log("音频播放失败");
        }
    }
    public AudioClip GetAudioClip(int audioId)
    {
        return audioClips[audioId];
    }
}
