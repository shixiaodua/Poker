using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public enum BtnEnum
{
    Cp,
    Qdz,
    Jdz,
    Close,
}
public class BattlePanel : BasePanel<GameScene>
{
    /// <summary>
    /// 卡牌对象
    /// </summary>
    public GameObject cardItem;
    public List<PlayerItem> playerItems;//我是第一个，右边是第二个，左边是第三个
    public Text timeText;
    public ChatListPanel chatListPanel;
    public GameObject CpBtn;//出牌按钮
    public GameObject NoCpBtn;//不出牌按钮
    public GameObject QdzBtn;//抢地主按钮
    public GameObject NoQdzBtn;//不抢地主按钮
    public GameObject JdzBtn;//叫地主按钮
    public GameObject NoJdzBtn;//不叫地主按钮
    public Sprite dzSprite;//地主图片
    public List<Card> cpList;//出牌列表
    public GameObject winPanel;//胜利面板
    /// <summary>
    /// 当前面板的音效播放器
    /// </summary>
    public AudioSource audioSource;
    /// <summary>
    /// 地主三张牌脚本
    /// </summary>
    public DzCards dzCardsCs;
    /// <summary>
    /// 卡牌父物体
    /// </summary>
    public GameObject content;
    /// <summary>
    /// 倒计时协程迭代器
    /// </summary>
    private IEnumerator timeIe;

    public void EnterPanel()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        Init();
    }
    private void Init()
    {
        cpList = new List<Card>();
        //先初始化三个玩家的名字
        int idx = -1;
        for (int i = 0; i < GameManager.Instance.userSx.Count; i++)
        {
            if (GameManager.Instance.userSx[i].name == GameManager.Instance.playerInfo.name)
            {
                idx = i; break;//找到自己的出牌顺序
            }
        }
        for (int i = 0; i < GameManager.Instance.userSx.Count; i++)
        {
            int index = (i + idx) % 3;
            playerItems[i].SetPlayerName(GameManager.Instance.userSx[index].name);
        }
        //判断我是否是第一个出牌
        if (idx == 0)
        {
            scene.isWait = false;
            ShowBtn(BtnEnum.Jdz);
        }
        //开始倒计时
        ResetTime();
        //开始发牌
        StartCoroutine(Fp());
    }
    IEnumerator Fp()
    {
        PlayAudio(FixAudioType.Fp);//播放发牌音效
        Debug.Log("牌面：" + GameManager.Instance.playerCards);
        for (int i = 0; i < 17; i++)
        {
            GameObject p = Instantiate(cardItem, content.transform);
            p.GetComponent<CardItem>().Init(GameManager.Instance.playerCards[i]);
            yield return new WaitForSeconds(0.22f);
        }

    }
    IEnumerator Timer()
    {
        timeText.text = "20";
        while (true)
        {
            yield return new WaitForSeconds(1);
            int time = int.Parse(timeText.text);
            timeText.text = (time - 1).ToString();
            if (time == 5)
            {
                PlayAudio(FixAudioType.ReTime);
            }
            if (time == 0)
            {
                StopCoroutine(timeIe);
                //判断当前状态
                if (!scene.isWait)
                {
                    if (scene.isQdzState)
                    {
                        //不抢地主
                        scene.isWait = true;
                        OnClickNoQdz();
                    }
                    else
                    {
                        if (scene.isFrist)
                        {
                            cpList.Clear();
                            cpList.Add(content.transform.GetChild(0).transform.GetComponent<CardItem>().card);
                            OnClickCp();
                        }
                        else
                        {
                            //不要
                            scene.isWait = true;
                            OnClickNoCp();
                        }
                    }
                }
                break;
            }
        }
    }
    public void UpdateQdzNextPlayer(List<string> userSx, bool isQdz)
    {
        //找到上一个人
        foreach (var item in playerItems)
        {
            if (item.playerName.text == userSx[userSx.Count - 1])
            {
                if (scene.isJdz)
                {
                    //播放音频
                    item.GamePlayAudio(GameAudio.GetQdzAudioClip(isQdz ? QdzAudioType.Qdz : QdzAudioType.NoQdz));
                    item.ShowPlayTxt(isQdz ? PlayerTxt.Rob : PlayerTxt.NotRob);
                }
                else
                {
                    //播放音频
                    item.GamePlayAudio(GameAudio.GetQdzAudioClip(isQdz ? QdzAudioType.Jdz : QdzAudioType.NoJdz));
                    item.ShowPlayTxt(isQdz ? PlayerTxt.Call : PlayerTxt.NotCall);
                }
                break;
            }
        }
        if (isQdz)
        {
            scene.isJdz = true;
        }
        //判断是否是我的回合
        if (userSx[0] == GameManager.Instance.playerInfo.name)
        {
            scene.isWait = false;
            ShowBtn(scene.isJdz ? BtnEnum.Qdz : BtnEnum.Jdz);

        }
        else
        {

        }
        ResetTime();
    }
    public void ShowBtn(BtnEnum btnEnum)
    {
        if (btnEnum == BtnEnum.Cp)
        {
            CpBtn.SetActive(true);
            NoCpBtn.SetActive(true);
        }
        if (btnEnum == BtnEnum.Qdz)
        {
            QdzBtn.SetActive(true);
            NoQdzBtn.SetActive(true);
        }
        if (btnEnum == BtnEnum.Jdz)
        {
            JdzBtn.SetActive(true);
            NoJdzBtn.SetActive(true);
        }
        if (btnEnum == BtnEnum.Close)
        {
            CpBtn.SetActive(false);
            NoCpBtn.SetActive(false);
            QdzBtn.SetActive(false);
            NoQdzBtn.SetActive(false);
            JdzBtn.SetActive(false);
            NoJdzBtn.SetActive(false);
        }
    }
    public void OnClickQdz()
    {
        playerItems[0].ShowPlayTxt(scene.isJdz ? PlayerTxt.Rob : PlayerTxt.Call);
        //播放音频
        playerItems[0].GamePlayAudio(GameAudio.GetQdzAudioClip(scene.isJdz ? QdzAudioType.Qdz : QdzAudioType.Jdz));
        SendMsg.Qdz(true, QdzCallBack);
        scene.isJdz = true;
    }
    public void OnClickNoQdz()
    {
        playerItems[0].ShowPlayTxt(scene.isJdz ? PlayerTxt.NotRob : PlayerTxt.NotCall);
        //播放音频
        playerItems[0].GamePlayAudio(GameAudio.GetQdzAudioClip(scene.isJdz ? QdzAudioType.NoQdz : QdzAudioType.NoJdz));
        SendMsg.Qdz(false, QdzCallBack);
    }
    public void OnClickCp()
    {
        if (cpList.Count != 0 && !scene.isWait)
            SendMsg.Cp(cpList, CpCallBack);
    }
    public void OnClickNoCp()
    {
        SendMsg.By(ByCallBack);
    }
    public void OnClickChat()
    {
        PlayAudio(FixAudioType.ChatBtn);
        chatListPanel.gameObject.SetActive(!chatListPanel.gameObject.activeSelf);
    }
    public void OnClickAudioBtn()
    {
        chatListPanel.gameObject.SetActive(true);
    }
    public void ByCallBack(ByResult byResult)
    {
        if (byResult == ByResult.Success)
        {
            Debug.Log("不出牌消息发送成功");
        }
        else
        {
            Debug.Log("不出牌消息发送失败");
        }
    }
    public void CpCallBack(CpResult cpResult)
    {
        if (cpResult == CpResult.Success)
        {
            Debug.Log("出牌合法");
        }
        else
        {
            Debug.Log("出牌不合法");
        }
    }
    private void QdzCallBack(QdzResult qdzResult)
    {
        if (qdzResult == QdzResult.Success)
        {
            Debug.Log("抢地主消息发送成功");
            //重新开始计时，进入等待状态
            scene.isWait = true;
            ShowBtn(BtnEnum.Close);
            ResetTime();
        }
        else
        {
            Debug.Log("抢地主消息发送失败");
        }
    }
    public void ResetTime()
    {
        if (timeIe != null) StopCoroutine(timeIe);
        timeIe = Timer();
        StartCoroutine(timeIe);
    }
    /// <summary>
    /// 抢地主结束
    /// </summary>
    public void QdzEnd(string userName, List<Card> dzCards, List<Card> currentCards, string lastUserName, bool isQdz)
    {
        if (GameManager.Instance.playerInfo.name != lastUserName)
        {
            //找到上一个人
            foreach (var item in playerItems)
            {
                if (item.playerName.text == lastUserName)
                {
                    if (scene.isJdz)
                    {
                        //播放音频
                        item.GamePlayAudio(GameAudio.GetQdzAudioClip(isQdz ? QdzAudioType.Qdz : QdzAudioType.NoQdz));
                        item.ShowPlayTxt(isQdz ? PlayerTxt.Rob : PlayerTxt.NotRob);
                    }
                    else
                    {
                        //播放音频
                        item.GamePlayAudio(GameAudio.GetQdzAudioClip(isQdz ? QdzAudioType.Jdz : QdzAudioType.NoJdz));
                        item.ShowPlayTxt(isQdz ? PlayerTxt.Call : PlayerTxt.NotCall);
                    }
                    break;
                }
            }
        }
        //将三张牌翻出来
        for (int i = 0; i < dzCards.Count; i++)
        {
            dzCardsCs.PlayDzCards(dzCards);
        }
        //判断自己是不是地主
        if (userName == GameManager.Instance.playerInfo.name)
        {
            scene.isFrist = true;
            //更换角色图片
            playerItems[0].UpdatePlayerImage(dzSprite);
            //更换手牌
            //清空手牌
            content.transform.DetachChildren();
            //重新生成手牌
            for (int i = 0; i < currentCards.Count; i++)
            {
                GameObject cardItem2 = Instantiate(cardItem, content.transform);
                cardItem2.GetComponent<CardItem>().Init(currentCards[i]);
            }
            scene.isDz = true;
            scene.isWait = false;
            ShowBtn(BtnEnum.Cp);
            ResetTime();
        }
        else
        {
            //更换角色图片
            GetPlayerItem(userName).UpdatePlayerImage(dzSprite);
            scene.isWait = true;
            ShowBtn(BtnEnum.Close);
            ResetTime();
        }
        scene.isQdzState = false;//进入出牌状态
    }
    /// <summary>
    /// 玩家出牌通知回调
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="cards"></param>
    public void Cp(string userName, string nextUserName, List<Card> cards, List<Card> currentCards, GameCpType gameCpType, int dx)
    {
        scene.isFrist = false;
        //找到对应的玩家，播放出牌动画和音频
        PlayerItem playerItem = GetPlayerItem(userName);
        playerItem.PlayCp(cards);
        ResetTime();
        if (gameCpType == GameCpType.Sz || gameCpType == GameCpType.Ld || gameCpType == GameCpType.Fj || gameCpType == GameCpType.Sde || gameCpType == GameCpType.ThreeOne || gameCpType == GameCpType.Wz)
            GameAudioPlay(userName, GameAudio.GetGameAudioClip(gameCpType, null));
        else
        {
            GameAudioPlay(userName, GameAudio.GetGameAudioClip(gameCpType, dx));
        }
        if (gameCpType == GameCpType.Zd || gameCpType == GameCpType.Wz)
        {
            PlayAudio(FixAudioType.Zd);
        }
        if (gameCpType == GameCpType.Sz)
        {
            PlayAudio(FixAudioType.Sz);
        }
        if (gameCpType == GameCpType.Fj)
        {
            PlayAudio(FixAudioType.Fj);
        }
        if (currentCards != null)
        {
            if (currentCards.Count == 2)
            {
                PlayAudio(FixAudioType.Bj2);
            }
            if (currentCards.Count == 1)
            {
                PlayAudio(FixAudioType.Bj1);
            }
        }
        if (playerItem != null)
        {
            //判断是否是自己
            if (userName == GameManager.Instance.playerInfo.name)
            {
                //清空手牌
                content.transform.DetachChildren();
                //重新生成手牌
                for (int i = 0; i < currentCards.Count; i++)
                {
                    GameObject cardItem2 = Instantiate(cardItem, content.transform);
                    cardItem2.GetComponent<CardItem>().Init(currentCards[i]);
                }
                //重置出牌list
                cpList.Clear();
                //隐藏按钮
                ShowBtn(BtnEnum.Close);
                scene.isWait = true;//进入等待状态
            }
            else
            {
                //判断是否是下一个出牌人
                if (nextUserName == GameManager.Instance.playerInfo.name)
                {
                    ShowBtn(BtnEnum.Cp);
                    scene.isWait = false;//进入出牌状态
                }
                else
                {
                    ShowBtn(BtnEnum.Close);
                    scene.isWait = true;//进入等待状态
                }
            }
        }
    }
    public void By(string userName, string nextUserName, bool isFrist)
    {
        //找到对应的玩家，播放出牌动画和音频
        PlayerItem playerItem = GetPlayerItem(userName);
        playerItem.PlayCp(new List<Card>());
        if (playerItem != null)
        {
            playerItem.ShowPlayTxt(PlayerTxt.NotPlay);//显示不要
            ResetTime();
            GameAudioPlay(userName, GameAudio.GetGameAudioClip(GameCpType.No, Random.Range(1, 5)));
            //判断是否是自己
            if (userName == GameManager.Instance.playerInfo.name)
            {
                ShowBtn(BtnEnum.Close);
                scene.isWait = true;//进入等待状态
            }
            if (nextUserName == GameManager.Instance.playerInfo.name)
            {
                scene.isFrist = isFrist;
                ShowBtn(BtnEnum.Cp);
                scene.isWait = false;//进入出牌状态
            }
        }
        else
        {
            Debug.Log("找不到玩家");
        }
    }
    public void GameEnd(bool isDzWin)
    {
        ShowBtn(BtnEnum.Close);
        if (isDzWin)
        {
            //弹出胜利面板
            winPanel.SetActive(true);
            winPanel.transform.Find("WinText").GetComponent<Text>().text = "地主胜利";
            if (scene.isDz)
            {
                //地主胜利
                PlayAudio(FixAudioType.Win);
            }
            else
            {
                //地主失败
                PlayAudio(FixAudioType.Lose);
            }
        }
        else
        {
            //弹出胜利面板
            winPanel.SetActive(true);
            winPanel.transform.Find("WinText").GetComponent<Text>().text = "平民胜利";
            if (scene.isDz)
            {
                //地主失败
                PlayAudio(FixAudioType.Lose);
            }
            else
            {
                //地主胜利
                PlayAudio(FixAudioType.Win);
            }
        }
        StartCoroutine(ExitGame());
    }
    public IEnumerator ExitGame()
    {
        yield return new WaitForSeconds(5);
        GameManager.Instance.EnterScene("MainScene");
    }
    public void ChatAudipPlay(string userName, int audioId)
    {
        AudioClip audioClip = chatListPanel.GetAudioClip(audioId);
        if (audioClip != null)
        {
            GetPlayerItem(userName).ChatPlayAudio(audioClip, audioId);
        }
    }
    public void GameAudioPlay(string userName, AudioClip audioClip)
    {
        GetPlayerItem(userName).GamePlayAudio(audioClip);
    }
    private PlayerItem GetPlayerItem(string userName)
    {
        foreach (PlayerItem child in playerItems)
        {
            if (child.playerName.text == userName)
            {
                return child;
            }
        }
        return null;
    }
    /// <summary>
    /// 播放特效音乐
    /// </summary>
    /// <param name="audioClip"></param>
    public void PlayAudio(FixAudioType audioType)
    {
        AudioClip audioClip = GameAudio.GetFixAudioClip(audioType);
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
