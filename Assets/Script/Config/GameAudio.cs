using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 抢地主音频类型
/// </summary>
public enum QdzAudioType
{
    Qdz,
    NoQdz,
    Jdz,
    NoJdz,
}
public enum GameCpType
{
    None,//不合法
    No,//不出
    Dn,//大你
    One,//一张牌
    Two,//一对
    Three,//三张
    ThreeOne,//三带一
    Fj,//飞机
    FJOne,//飞机带一张
    Zd,//炸弹
    Wz,//王炸
    Sz,//顺子
    Ld,//连对
    Sde,//四带二
}
public enum FixAudioType
{
    CardBtn,//点击牌
    ChatBtn,//点击聊天
    ReturnBtn,
    Fp,//发牌
    Zd,//炸弹
    Sz,//顺子
    Fj,//飞机
    ReTime,//倒计时
    Bj2,//报警两张牌
    Bj1,//报警一张牌
    Win,//胜利
    Lose,//失败
}
public class GameAudio
{
    static public Dictionary<QdzAudioType, string> qdzAudioDic = new Dictionary<QdzAudioType, string>(){
        {QdzAudioType.Qdz,"Rob1"},
        {QdzAudioType.NoQdz,"NoRob"},
        {QdzAudioType.Jdz,"Order"},
        {QdzAudioType.NoJdz,"NoOrder"},
    };
    static public Dictionary<GameCpType, string> gameAudioDic = new Dictionary<GameCpType, string>(){
        {GameCpType.No,"buyao"},
        {GameCpType.Dn,"dani"},
        {GameCpType.One,""},
        {GameCpType.Two,"dui"},
        {GameCpType.Three,"tuple"},
        {GameCpType.ThreeOne,"sandaiyi"},
        {GameCpType.Fj,"feiji"},
        {GameCpType.FJOne,"feiji"},
        {GameCpType.Zd,"zhadan"},
        {GameCpType.Wz,"wangzha"},
        {GameCpType.Sz,"shunzi"},
        {GameCpType.Ld,"liandui"},
        {GameCpType.Sde,"sidaier"},
    };
    static public Dictionary<FixAudioType, string> btnAudioDic = new Dictionary<FixAudioType, string>(){
        {FixAudioType.CardBtn,"SpecSelectCard"},
        {FixAudioType.ChatBtn,"SpecOk"},
        {FixAudioType.ReturnBtn,"ReturnBtn"},
        {FixAudioType.Fp,"Special_Dispatch"},
        {FixAudioType.Zd,"Special_Bomb"},
        {FixAudioType.Sz,"Special_flower"},
        {FixAudioType.Fj,"Special_plane"},
        {FixAudioType.ReTime,"Special_alert"},
        {FixAudioType.Bj2,"Man_baojing2"},
        {FixAudioType.Bj1,"Man_baojing1"},
        {FixAudioType.Win,"MusicEx_Win"},
        {FixAudioType.Lose,"MusicEx_Lose"},
    };

    static public AudioClip GetQdzAudioClip(QdzAudioType type)
    {
        if (qdzAudioDic.ContainsKey(type))
        {
            return Resources.Load<AudioClip>("Sounds/Man_" + qdzAudioDic[type]);
        }
        return null;
    }
    static public AudioClip GetGameAudioClip(GameCpType type, int? num)
    {
        if (gameAudioDic.ContainsKey(type))
        {
            string path = "Sounds/Man_" + gameAudioDic[type];
            if (num.HasValue)
            {
                path += num.Value.ToString();
            }
            return Resources.Load<AudioClip>(path);
        }
        return null;
    }
    static public AudioClip GetFixAudioClip(FixAudioType type)
    {
        if (btnAudioDic.ContainsKey(type))
        {
            return Resources.Load<AudioClip>("Sounds/" + btnAudioDic[type]);
        }
        return null;
    }
}