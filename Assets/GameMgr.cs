using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    public string id = "Tank";
    public int team = -1;
    public int nowTeamMove = 1;
    public bool isrefBool = false;
    /*********************************/
    //轮到谁叫地主或者加倍用
    public bool isLandlord = false;
    //最终地主是？
    public int isLanded = -1;
    //倒计时
    public int countDownNum = 15;
    //轮到谁出牌
    public int nowPlayCardTeam = -1;
    //上家出的牌
    public string upTeamPlayCards = "0";
    //最大牌出自谁手 即上家出牌人
    public int maxCardTeam = -1;
    //加倍是否结束
    public bool isMuled = false;
    //此轮倍数
    public int mulNum = 0;
    //队伍对应名称
    public Dictionary<int, string> teamName = new Dictionary<int, string>();


    // Use this for initialization
    void Awake()
    {
        instance = this;
    }
}
