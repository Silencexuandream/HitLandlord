/* ==============================================================================
 * 功能描述：Video  
 * 创 建 者：Administrator
 * 邮箱：136569631@qq.com
 * 版本：v 1.0
 * 创建日期：2019/6/13 14:03:42
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Video : MonoBehaviour
{
    public static Video instance;
    AudioSource _SoundSource = null;

    AudioSource _backMusicSource = null;

    void Start()
    {
        instance = this;
        _SoundSource = gameObject.AddComponent<AudioSource>();

        _backMusicSource = gameObject.AddComponent<AudioSource>();
        _backMusicSource.loop = true;

        PlayMusic("Sounds/bc");
    }

    //play_Cards = "05X06X06"
    //记录上家出的牌 前2位记录出牌类别 0单张 1对子 2三张 3三带一 4三带二 5单顺子 6双顺子 7三顺子 8飞机带翅膀 9四带二 10炸弹 11火箭（双王）05X(单顺子)
    //中间2位记录的是 主要牌面 06X（即从6开始的单顺子 6 7 8 9 10...）
    //后两位 出牌的个数 06(最后得出 6 7 8 9 10 J)
    //vIndex 0 叫地主 1 不叫 2 我枪 3 不抢 4 加倍 5 不加倍 6 要不起 7 大你 8 满上 9 压死 10 出牌 11 sort
    public void PlaySound(string soundPath,int vIndex)
    {
        string pt = null;
        if(soundPath != "")
        {
            switch (soundPath.Substring(0, 2))
            {
                case "00":
                    pt = "Woman_";
                    if (soundPath.Substring(3, 2) == "98")
                        pt += "14";
                    if (soundPath.Substring(3, 2) == "99")
                        pt += "15";
                    else pt += int.Parse(soundPath.Substring(3, 2));
                    break;
                case "01":
                    pt = "Woman_dui";
                    pt += int.Parse(soundPath.Substring(3, 2));
                    break;
                case "02":
                    pt = "Woman_tuple";
                    pt += int.Parse(soundPath.Substring(3, 2));
                    break;
                case "03":
                    pt = "Woman_sandaiyi";
                    break;
                case "04":
                    pt = "Woman_sandaiyidui";
                    break;
                case "05":
                    pt = "Woman_shunzi";
                    break;
                case "06":
                    pt = "Woman_liandui";
                    break;
                case "07":
                    pt = "Woman_feiji";
                    break;
                case "08":
                    pt = "Woman_feiji";
                    break;
                case "09":
                    pt = "Woman_sidailiangdui";
                    break;
                case "10":
                    pt = "Woman_zhadan";
                    break;
                case "11":
                    pt = "Woman_wangzha";
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (vIndex)
            {
                case 0:
                    pt = "Woman_Order";
                    break;
                case 1:
                    pt = "Woman_NoOrder";
                    break;
                case 2:
                    pt = "Woman_Rob3";
                    break;
                case 3:
                    pt = "Woman_NoRob";
                    break;
                case 4:
                    pt = "Woman_jiabei";
                    break;
                case 5:
                    pt = "Woman_bujiabei";
                    break;
                case 6:
                    pt = "Woman_buyao4";
                    break;
                case 7:
                    pt = "Woman_dani1";
                    break;
                case 8:
                    pt = "Woman_dani2";
                    break;
                case 9:
                    pt = "Woman_dani3";
                    break;
                case 10:
                    pt = "poker";
                    break;
                case 11:
                    pt = "sort";
                    break;
            }
        }

        print("---------pt----------- " + pt);

        if(pt != null)
        {
            AudioClip clip = Resources.Load("Sounds/" + pt) as AudioClip;
            _SoundSource.PlayOneShot(clip);
        }
    }

    void PlayMusic(string musicPath)
    {
        AudioClip clip = Resources.Load(musicPath) as AudioClip;
        _backMusicSource.clip = clip;
        _backMusicSource.Play();
    }

    public void CloseBcMusic()
    {
        _backMusicSource.Stop();
    }
}

