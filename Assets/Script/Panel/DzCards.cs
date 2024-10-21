using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DzCards : MonoBehaviour
{
    /// <summary>
    /// 地主的三张牌
    /// </summary>
    public List<Image> dzCardsImage;
    public void PlayDzCards(List<Card> cards)
    {
        int i = 0;
        foreach (Card card in cards)
        {
            if (card.cardNum >= 99)
            {
                name = CardName.cardNames[card.cardNum];
            }
            else
            {
                name = CardName.hsNames[card.hs] + CardName.cardNames[card.cardNum];
            }
            //加载图片资源
            dzCardsImage[i++].sprite = Resources.Load<Sprite>("Card/" + name);
        }
        //播放动画
        foreach (Image image in dzCardsImage)
        {
            image.transform.parent.GetComponent<Animation>().Play();
        }
    }
}
