using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public enum PlayerTxt
{
    Call,
    NotCall,
    NotPlay,
    NotRob,
    Rob,
}
public class PlayerItem : MonoBehaviour
{
    /// <summary>
    /// 玩家名称
    /// </summary>
    public Text playerName;
    /// <summary>
    /// 玩家金币
    /// </summary>
    public Text playerCoin;
    /// <summary>
    /// 玩家状态
    /// </summary>
    public Text playerState;
    /// <summary>
    /// 聊天内容
    /// </summary>
    public Text chatText;
    /// <summary>
    /// 出牌的位置
    /// </summary>
    public GameObject maoDian;
    /// <summary>
    /// 玩家角色图片
    /// </summary>
    public Image playerImage;
    /// <summary>
    /// 玩家牌预制体
    /// </summary>
    public GameObject cardItem;

    public GameObject ChatObj;
    public bool isReady;

    public void SetPlayerInfo(PlayerInfo playerInfo, bool isReady2)
    {
        playerName.text = playerInfo.name;
        playerCoin.text = playerInfo.coin.ToString();
        playerState.text = isReady2 ? "准备" : "未准备";
        isReady = isReady2;
    }
    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }
    public void SetPlayerState(bool isReady2)
    {
        playerState.text = isReady2 ? "准备" : "未准备";
        isReady = isReady2;
    }
    public void ChatPlayAudio(AudioClip audioClip, int audioId)
    {
        CancelInvoke("HideTxt");
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(audioClip);
        chatText.text = RoomAudioName.roomAudioName[audioId];
        ChatObj.SetActive(true);
        Invoke("HideTxt", 3f);
    }
    public void GamePlayAudio(AudioClip audioClip)
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(audioClip);
    }
    public void UpdatePlayerImage(Sprite sprite)
    {
        playerImage.sprite = sprite;
    }
    private void HideTxt()
    {
        ChatObj.SetActive(false);
    }
    public void ShowPlayTxt(PlayerTxt playerTxt)
    {
        GameObject prefab = Resources.Load<GameObject>("Word/" + playerTxt);
        GameObject p = Instantiate(prefab, maoDian.transform);
        StartCoroutine(DestroyTxt(p));
    }
    IEnumerator DestroyTxt(GameObject obj)
    {
        yield return new WaitForSeconds(2f);
        Destroy(obj);
    }
    public void PlayCp(List<Card> cards)
    {
        //清空
        for (int i = 0; i < maoDian.transform.childCount; i++)
        {
            Destroy(maoDian.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = Instantiate(cardItem, maoDian.transform);
            //移除按钮
            card.GetComponent<Button>().enabled = false;
            card.GetComponent<CardItem>().Init(cards[i]);
        }
    }
}
