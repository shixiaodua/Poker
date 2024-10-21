using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{
    public BattlePanel battlePanel;
    public Image cardImage;
    [HideInInspector]
    public Card card;
    /// <summary>
    /// 是否已经被选中
    /// </summary>
    private bool isCp;
    public void Init(Card card)
    {
        this.card = card;
        isCp = false;
        string name = "";
        //初始化UI
        if (card.cardNum >= 99)
        {
            name = CardName.cardNames[card.cardNum];
        }
        else
        {
            name = CardName.hsNames[card.hs] + CardName.cardNames[card.cardNum];
        }
        //加载图片资源
        cardImage.sprite = Resources.Load<Sprite>("Card/" + name);
    }
    public void OnClickCard()
    {
        battlePanel.PlayAudio(FixAudioType.CardBtn);//播放音频
        isCp = !isCp;
        //y坐标增高20
        if (isCp)
        {
            transform.localPosition += new Vector3(0, 20, 0);
            battlePanel.cpList.Add(card);
        }
        else
        {
            transform.localPosition -= new Vector3(0, 20, 0);
            battlePanel.cpList.Remove(card);
        }
    }
}
