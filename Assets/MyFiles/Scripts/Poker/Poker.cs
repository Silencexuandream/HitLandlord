/* ==============================================================================
 * 功能描述：Poker  
 * 创 建 者：Administrator
 * 邮箱：136569631@qq.com
 * 版本：v 1.0
 * 创建日期：2019/6/6 9:51:46
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Poker
{
    //play_Cards = "05X06X06"
    //记录上家出的牌 前2位记录出牌类别 0单张 1对子 2三张 3三带一 4三带二 5单顺子 6双顺子 7三顺子 8飞机带翅膀 9四带二 10炸弹 11火箭（双王）05X(单顺子)
    //中间2位记录的是 主要牌面 06X（即从6开始的单顺子 6 7 8 9 10...）
    //后两位 出牌的个数 06(最后得出 6 7 8 9 10 J)
    public string CheckCardList(List<int> list)
    {
        string cards = null;
        int lent = list.Count;
        int num = 0;
        int cardNum = -1;
        bool isOk = false;
        Debug.Log(" lent -------------- " + lent);
        switch (lent)
        {
            case 1://单张
                cards = "00X" + AddZero(list[0]) + "X01";
                break;
            case 2://对子 火箭
                if (list[0] == list[1])
                {
                    cards = "01X" + AddZero(list[0]) + "X02";
                }
                else
                {
                    if ((list[0] == 98 && list[1] == 99) || (list[0] == 99 && list[1] == 98))
                    {
                        cards = "11X98X02";//火箭
                    }
                    else
                    {
                        return null;
                    }  
                }
                break;
            case 3://三张
                if (list[0] == list[1] && list[1] == list[2])
                {
                    cards = "02X" + AddZero(list[0]) + "X03";
                }
                else return null;
                break;
            case 4://三带一 炸弹
                num = GetNum(list);
                Debug.Log("num ============ " + num);
                //0炸弹 1三带一 -1其他
                if (num == 1)//三带一
                {
                    cardNum = -1;
                    if (list[0] == list[1] && list[1] == list[2]) cardNum = list[0];
                    else if (list[0] == list[1] && list[1] == list[3]) cardNum = list[0];
                    else if (list[0] == list[2] && list[2] == list[3]) cardNum = list[0];
                    else cardNum = list[1];
                    cards = "03X" + AddZero(cardNum) + "X04";
                }
                else if (num == 0)//炸弹
                {
                    cards = "10X" + AddZero(list[0]) + "X04";
                }
                else return null;

                break;
            case 5://三带二 单顺子 四带一（暂时不要）
                list = PX(list);
                num = num = GetNum(list);
                Debug.Log("num ============ " + num);
                //0 单顺子或者？ 1 三带二 2 四带一
                if (num == 1)//三带二
                {
                    cardNum = MaxMin(list, 3);
                    cards = "04X" + AddZero(cardNum) + "X05";
                }
                else if(num == 2)//四带一
                {
                    return null;
                }
                else if (num == 0)
                {
                    isOk = DSZ(list);
                    if (isOk)//单顺子
                    {
                        cards = "05X" + AddZero(list[0]) + "X05";
                    }
                    else
                    {
                        return null;
                    }
                }
                break;
            case 6://单顺子 双顺子 三顺子 四带二
                list = PX(list);
                isOk = DSZ(list);
                if (isOk)//单顺子
                {
                    cards = "05X" + AddZero(list[0]) + "X06";
                }
                else
                {
                    isOk = SSZ(list);
                    if (!isOk)//双顺子
                    {
                        cards = "06X" + AddZero(list[0]) + "X06";
                    }
                    else
                    {
                        isOk = TSZ(list);
                        if (!isOk)//三顺子
                        {
                            cards = "07X" + AddZero(list[0]) + "X06";
                        }
                        else
                        {
                            num = GetNum(list);
                            //1 四带二 0其他
                            if (num == 1)//四带二
                            {
                                cardNum = MaxMin(list, 4);
                                cards = "09X" + AddZero(cardNum) + "X06";
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }

                break;
            case 7://单顺子 
                list = PX(list);
                isOk = DSZ(list);
                if (isOk)//单顺子
                {
                    cards = "05X" + AddZero(list[0]) + "X07";
                }
                else
                {
                    return null;
                }
                break;
            case 8://单顺子 双顺子 二飞机带单翅膀
                list = PX(list);
                isOk = DSZ(list);
                if (isOk)//单顺子
                {
                    cards = "05X" + AddZero(list[0]) + "X08";
                }
                else
                {
                    isOk = SSZ(list);
                    if (!isOk)//双顺子
                    {
                        cards = "06X" + AddZero(list[0]) + "X08";
                    }
                    else
                    {
                        cardNum = EFJ_DCB(list);
                        if (cardNum != -1)//二飞机带单翅膀
                        {
                            cards = "08X" + AddZero(cardNum) + "X08";
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                break;
            case 9://单顺子 三顺子
                list = PX(list);
                isOk = DSZ(list);
                if (isOk)//单顺子
                {
                    cards = "05X" + AddZero(list[0]) + "X09";
                }
                else
                {
                    isOk = TSZ(list);
                    if (!isOk)//三顺子
                    {
                        cards = "07X" + AddZero(list[0]) + "X09";
                    }
                    else
                    {
                        return null;
                    }
                }
                break;
            case 10://单顺子  双顺子 二飞机带双翅膀
                list = PX(list);
                isOk = DSZ(list);
                if (isOk)//单顺子
                {
                    cards = "05X" + AddZero(list[0]) + "X10";
                }
                else
                {
                    isOk = SSZ(list);
                    if (!isOk)//双顺子
                    {
                        cards = "06X" + AddZero(list[0]) + "X10";
                    }
                    else
                    {
                        cardNum = EFJ_2DCB(list);
                        if(cardNum != -1)//二飞机带双翅膀
                        {
                            cards = "08X" + AddZero(cardNum) + "X10";
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                break;
            case 11://单顺子  
                list = PX(list);
                isOk = DSZ(list);
                if (isOk)//单顺子
                {
                    cards = "05X" + AddZero(list[0]) + "X11";
                }
                else
                {
                    return null;
                }
                break;
            case 12://单顺子 双顺子 三顺子 三飞机带单翅膀
                list = PX(list);
                isOk = DSZ(list);
                if (isOk)//单顺子
                {
                    cards = "05X" + AddZero(list[0]) + "X12";
                }
                else
                {
                    isOk = SSZ(list);
                    if (!isOk)//双顺子
                    {
                        cards = "06X" + AddZero(list[0]) + "X12";
                    }
                    else
                    {
                        isOk = TSZ(list);
                        if (!isOk)//三顺子
                        {
                            cards = "07X" + AddZero(list[0]) + "X12";
                        }
                        else
                        {
                            cardNum = SFJ_DCB(list);
                            if(cardNum != -1)//三飞机带单翅膀
                            {
                                cards = "08X" + AddZero(cardNum) + "X12";
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                break;
            case 14://双顺子 
                list = PX(list);
                isOk = SSZ(list);
                if (!isOk)//双顺子
                {
                    cards = "06X" + AddZero(list[0]) + "X14";
                }
                else
                {
                    return null;
                }
                break;
            case 15://三顺子 三飞机带双翅膀 
                list = PX(list);
                isOk = TSZ(list);
                if (!isOk)//三顺子
                {
                    cards = "07X" + AddZero(list[0]) + "X15";
                }
                else
                {
                    cardNum = SFJ_2DCB(list);
                    if(cardNum != -1)//三飞机带双翅膀 
                    {
                        cards = "08X" + AddZero(cardNum) + "X15";
                    }else
                    {
                        return null;
                    }
                }
                break;
            case 16://双顺子 四飞机带单翅膀
                list = PX(list);
                isOk = SSZ(list);
                if (!isOk)//双顺子
                {
                    cards = "06X" + AddZero(list[0]) + "X16";
                }
                else
                {
                    cardNum = S4FJ_DCB(list);
                    if (cardNum != -1)
                    {
                        cards = "08X" + AddZero(cardNum) + "X16";
                    }
                    else
                    {
                        return null;
                    }
                }
                break;
            case 18://双顺子 三顺子
                list = PX(list);
                isOk = SSZ(list);
                if (!isOk)//双顺子
                {
                    cards = "06X" + AddZero(list[0]) + "X18";
                }
                else
                {
                    isOk = TSZ(list);
                    if (!isOk)//三顺子
                    {
                        cards = "07X" + AddZero(list[0]) + "X18";
                    }
                    else
                    {
                        return null;
                    }
                }
                break;
            case 20://双顺子 四飞机带双翅膀
                list = PX(list);
                isOk = SSZ(list);
                if (!isOk)//双顺子
                {
                    cards = "06X" + AddZero(list[0]) + "X20";
                }
                else
                {
                    cardNum = S4FJ_SCB(list);
                    if (cardNum != -1)
                    {
                        cards = "08X" + AddZero(cardNum) + "X20";
                    }
                    else
                    {
                        return null;
                    }
                }
                break;
            default://无效
                break;
        }
        Debug.Log("此时的出牌面是：" + cards);
        if (Compare_Card(cards))
        {
            return cards;
        }
        else
        {
            return null;
        }
    }

    private int GetNum(List<int> list)
    {
        int num = 0;
        if(list.Count == 4)//未排序的
        {
            if (list[0] == list[1] && list[1] == list[2] && list[2] != list[3]) return 1;
            else if (list[0] == list[1] && list[1] == list[3] && list[2] != list[3]) return 1;
            else if (list[0] != list[1] && list[0] == list[2] && list[2] == list[3]) return 1;
            else if (list[0] != list[1] && list[1] == list[2] && list[2] == list[3]) return 1;
            else if (list[0] == list[1] && list[1] == list[2] && list[2] == list[3]) return 0;
            else return -1;
        }
        else if (list.Count == 5)//排序后的
        {
            if (list[0] == list[1] && list[1] == list[2] && list[2] != list[3] && list[3] == list[4]) return 1;
            else if (list[0] == list[1] && list[1] != list[2] && list[2] == list[3] && list[3] == list[4]) return 1;
            else if (list[0] == list[1] && list[1] == list[2] && list[2] == list[3] && list[3] != list[4]) return 2;
            else if (list[0] != list[1] && list[1] == list[2] && list[2] == list[3] && list[3] == list[4]) return 2;
            else return 0;
        }
        else if (list.Count == 6)//排序后的
        {
            if (list[0] == list[1] && list[1] == list[2] && list[2] == list[3] && list[3] != list[4] && list[3] != list[5]) return 1;
            else if (list[0] != list[1] && list[1] == list[2] && list[2] == list[3] && list[3] == list[4] && list[4] != list[5]) return 1;
            else if (list[0] != list[2] && list[1] != list[2] && list[2] == list[3] && list[3] == list[4] && list[4] == list[5]) return 1;
            else return 0;
        }
        else
            return num;
    }

    //排序
    private List<int> PX(List<int> list)
    {
        string str = null;
        for (int i = 0; i < list.Count; i++)
        {
            str += list[i] + "  ";
        }

        Debug.Log("排序前的列表： " + str);

        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                if (list[i] > list[j])
                {
                    int tp = list[i];
                    list[i] = list[j];
                    list[j] = tp;
                }
            }
        }
        string str1 = null;
        for (int i = 0; i < list.Count; i++)
        {
            str1 += list[i] + "  ";
        }

        Debug.Log("排序后的列表： " + str1);

        return list;
    }

    //个数前补0
    private string AddZero(int num)
    {
        string str = null;
        if (num < 10)
        {
            str = "0" + num;
        }
        else str = num.ToString();
        return str;
    }

    //单顺子判断
    private bool DSZ(List<int> list)
    {
        bool isOk = true;

        for (int i = 0; i < list.Count; i++)
        {
            if (i < list.Count - 1)
            {
                if (list[i + 1] - list[i] != 1)
                {
                    isOk = false;
                    break;
                }
            }
        }

        Debug.Log("是否为单顺子：" + isOk);
        return isOk;
    }

    //双顺子判断
    private bool SSZ(List<int> list)
    {
        bool isOk = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (i % 2 == 0)
            {
                if (list[i] != list[i + 1])
                {
                    isOk = true;
                    break;
                }
            }
        }
        Debug.Log("是否为双顺子：" + isOk);
        return isOk;
    }

    //三顺子判断
    private bool TSZ(List<int> list)
    {
        bool isOk = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (i % 3 == 0)
            {
                if (list[i] != list[i + 1] || list[i] != list[i + 2] || list[i+1] != list[i + 2])
                {
                    isOk = true;
                    break;
                }
            }
        }
        Debug.Log("是否为三顺子：" + isOk);
        return isOk;
    }

    //三带二 四带二 得出主牌
    private int MaxMin(List<int> list,int index)
    {
        int cardNum = -1;
        if(index == 3)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0)
                {
                    if (list[0] > list[i])
                    {
                        cardNum = list[0];
                        break;
                    }
                    else
                    {
                        cardNum = list[i];
                        if (list[0] < list[i])
                        {
                            break;
                        }
                    }
                }
            }
        }
        else if (index == 4)
        {
            if (list[0] == list[3])
            {
                cardNum = list[0];
            }
            else if (list[2] == list[5])
            {
                cardNum = list[2];
            }
            else if (list[1] == list[4])
            {
                cardNum = list[1];
            }
        }

        Debug.Log("是否为三带二 四带二 得出主牌：" + cardNum);
        return cardNum;
    }

    //二飞机带单翅膀
    private int EFJ_DCB(List<int> list)
    {
        int cardNum = -1;

        if(list[2] == list[4] && list[5] == list[7])
        {
            cardNum = list[2];
        }else if(list[1] == list[3] && list[4] == list[6])
        {
            cardNum = list[1];
        }
        else if(list[0] == list[2] && list[3] == list[5])
        {
            cardNum = list[0];
        }
        Debug.Log("是否为二飞机带单翅膀：" + cardNum);
        return cardNum;
    }

    //二飞机带双翅膀
    private int EFJ_2DCB(List<int> list)
    {
        int cardNum = -1;

        if (list[0] == list[2] && list[3] == list[5] && list[6] == list[7] && list[8] == list[9])
        {
            cardNum = list[0];
        }
        else if (list[4] == list[6] && list[7] == list[9] && list[0] == list[1] && list[2] == list[3])
        {
            cardNum = list[4];
        }
        else if (list[2] == list[4] && list[5] == list[7] && list[0] == list[1] && list[8] == list[9])
        {
            cardNum = list[2];
        }
        Debug.Log("是否为二飞机带双翅膀：" + cardNum);
        return cardNum;
    }

    //三飞机带单翅膀
    private int SFJ_DCB(List<int> list)
    {
        int cardNum = -1;

        if (list[0] == list[2] && list[3] == list[5] && list[6] == list[8])
        {
            cardNum = list[0];
        }
        else if (list[3] == list[5] && list[6] == list[8] && list[9] == list[11])
        {
            cardNum = list[3];
        }
        else if (list[1] == list[3] && list[4] == list[6] && list[7] == list[9])
        {
            cardNum = list[0];
        }
        else if (list[2] == list[4] && list[5] == list[7] && list[8] == list[10])
        {
            cardNum = list[0];
        }
        Debug.Log("是否为三飞机带单翅膀：" + cardNum);
        return cardNum;
    }

    //三飞机带双翅膀
    private int SFJ_2DCB(List<int> list)
    {
        int cardNum = -1;

        if (list[0] == list[2] && list[3] == list[5] && list[6] == list[8] && list[9] == list[10] && list[11] == list[12] && list[13] == list[14])
        {
            cardNum = list[0];
        }
        else if (list[6] == list[8] && list[9] == list[11] && list[12] == list[14] && list[0] == list[1] && list[2] == list[3] && list[4] == list[5])
        {
            cardNum = list[4];
        }
        else if (list[2] == list[4] && list[5] == list[7] && list[8] == list[10] && list[0] == list[1] && list[11] == list[12] && list[13] == list[14])
        {
            cardNum = list[2];
        }
        else if (list[4] == list[6] && list[7] == list[9] && list[10] == list[12] && list[0] == list[1] && list[2] == list[3] && list[13] == list[14])
        {
            cardNum = list[2];
        }
        Debug.Log("是否为三飞机带双翅膀：" + cardNum);
        return cardNum;
    }

    //四飞机带单翅膀
    private int S4FJ_DCB(List<int> list)
    {
        int cardNum = -1;

        if (list[0] == list[2] && list[3] == list[5] && list[6] == list[8] && list[9] == list[11])
        {
            cardNum = list[0];
        }
        else if (list[4] == list[6] && list[7] == list[9] && list[10] == list[12] && list[13] == list[15])
        {
            cardNum = list[3];
        }
        else if (list[1] == list[3] && list[4] == list[6] && list[7] == list[9] && list[10] == list[12])
        {
            cardNum = list[0];
        }
        else if (list[2] == list[4] && list[5] == list[7] && list[8] == list[10] && list[11] == list[13])
        {
            cardNum = list[0];
        }
        else if (list[3] == list[5] && list[6] == list[8] && list[9] == list[11] && list[12] == list[14])
        {
            cardNum = list[0];
        }
        Debug.Log("是否为四飞机带单翅膀：" + cardNum);
        return cardNum;
    }

    //四飞机带双翅膀
    private int S4FJ_SCB(List<int> list)
    {
        int cardNum = -1;

        if (list[8] == list[10] && list[11] == list[13] && list[14] == list[16] && list[17] == list[19] && list[0] == list[1] && list[2] == list[3] && list[4] == list[5] && list[6] == list[7])
        {
            cardNum = list[8];
        }
        else if (list[6] == list[8] && list[9] == list[11] && list[12] == list[14] && list[15] == list[17] && list[18] == list[19] && list[0] == list[1] && list[2] == list[3] && list[4] == list[5])
        {
            cardNum = list[3];
        }
        else if (list[4] == list[6] && list[7] == list[9] && list[10] == list[12] && list[13] == list[15] && list[0] == list[1] && list[2] == list[3] && list[16] == list[17] && list[18] == list[19])
        {
            cardNum = list[4];
        }
        else if (list[2] == list[4] && list[5] == list[7] && list[8] == list[10] && list[11] == list[13] && list[0] == list[1] && list[14] == list[15] && list[16] == list[17] && list[18] == list[19])
        {
            cardNum = list[2];
        }
        else if (list[0] == list[2] && list[3] == list[5] && list[6] == list[8] && list[9] == list[11] && list[12] == list[13] && list[14] == list[15] && list[16] == list[17] && list[18] == list[19])
        {
            cardNum = list[0];
        }
        Debug.Log("是否为四飞机带双翅膀：" + cardNum);
        return cardNum;
    }

    private bool Compare_Card(string card)
    {
        bool isMax = false;
        string old = GameMgr.instance.upTeamPlayCards;
        Debug.Log("上家出的牌： " + old + " 将要出的牌:  " + card);
        if (old != "0")
        {
            int old_C1 = int.Parse(old.Substring(0, 2));//确定牌的样式
            int old_C2 = int.Parse(old.Substring(3, 2));//用来比大小
            int old_C3 = int.Parse(old.Substring(6, 2));//确定牌的个数

            int new_C1 = int.Parse(card.Substring(0, 2));//确定牌的样式
            int new_C2 = int.Parse(card.Substring(3, 2));//用来比大小
            int new_C3 = int.Parse(card.Substring(6, 2));//确定牌的个数

            isMax = OldNewCom(new_C1, new_C2, new_C3, old_C1, old_C2, old_C3);
        }
        else isMax = true;
        Debug.Log("是否比上家牌面大：" + isMax);
        return isMax;
    }

    private bool OldNewCom(int new1,int new2,int new3,int old1,int old2,int old3)
    {
        bool isNew = false;
        Debug.Log("是否为同类：" + new1 + " " + old1);
        if (new1 == old1)
        {
            if (new2 - old2 > 0 && new3 == old3)
            {
                isNew = true;
            }
            else isNew = false;
        }
        else
        {
            if(new1 >= 10)
            {
                if (new1 == 10) isNew = true;//炸弹，以后要翻倍的
                else if (new1 == 11) isNew = true;//火箭，以后要翻倍的
                else isNew = false;
            }
            else
            {
                isNew = false;
            }
        }

        return isNew;
    }
}

