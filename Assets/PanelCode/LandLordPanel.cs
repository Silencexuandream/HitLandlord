using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LandLordPanel : PanelBase
{
    private ManagerVars vars;
    private int tId;

    //poker资源组件
    private GameObject initPoker;
    //父组件
    private Transform selfParent;
    //已出牌父物体
    private Transform cardsParent;
    //倒计时
    private Text countDownText;
    //倒计时按钮
    private Button countDownBtn;
    //准备按钮
    private Button readyPokerBtn;
    //叫地主按钮
    private Button landBtn_Yes;
    //不叫按钮
    private Button landBtn_No;
    //0倍按钮
    private Button mul_0Btn;
    //1倍按钮
    private Button mul_1Btn;
    //2倍按钮
    private Button mul_2Btn;
    //3倍按钮
    private Button mul_3Btn;
    //返回大厅按钮
    private Button backBtn;
    //退出按钮
    private Button quitBtn;



    //poker sprite列表
    private List<Sprite> pokerSpriteList = new List<Sprite>();
    //初始牌顺序
    private List<int> startList = new List<int>();
    //洗牌后顺序
    private List<int> endList = new List<int>();
    //发牌后玩家手中牌列表
    private int[,] playerPokerList = new int[3, 17];
    //底牌列表
    private int[] bottomPokerList = new int[3];
    //发牌速度
    private float speedPoker = 0.3f;
    //排序牌列表
    private List<GameObject> sortPokerList = new List<GameObject>();
    //临时存储父物体下的子物体列表
    private List<GameObject> tempPokerList = new List<GameObject>();

    //牌Sprite字典
    private Dictionary<string, Sprite> pokerSpriteDic = new Dictionary<string, Sprite>();

    //出牌字典
    private Dictionary<string, GameObject> cardsDict = new Dictionary<string, GameObject>();
    //临时存储出牌列表
    private List<int> tempCards = new List<int>();
    //出牌按钮
    private Button playCardBtn;
    //要不起按钮
    private Button cannotAffordBtn;

    //player1组件
    private GameObject team_1;
    //player2组件
    private GameObject team_2;
    //self组件
    private GameObject self;

    /// <summary> 初始化 </summary>
    public override void Init(params object[] args)
    {
        base.Init(args);
        vars = ManagerVars.GetManagerVars();
        skinPath = "ladnLordPanel";
        layer = PanelLayer.Panel;

    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;

        //监听
        NetMgr.srvConn.msgDist.AddListener("CountDown", RecvCountDown);
        NetMgr.srvConn.msgDist.AddListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.srvConn.msgDist.AddListener("Ready_Poker", RecvReady_Poker);
        NetMgr.srvConn.msgDist.AddListener("Send_Poker", RecvSend_Poker);
        NetMgr.srvConn.msgDist.AddListener("Land_Lord", RecvLand_Lord);
        NetMgr.srvConn.msgDist.AddListener("LandOK", RecvLandOK);
        NetMgr.srvConn.msgDist.AddListener("Play_Card", RecvPlay_Card);
        NetMgr.srvConn.msgDist.AddListener("Cd", RecvCd);
        NetMgr.srvConn.msgDist.AddListener("Win", RecvWin);
        NetMgr.srvConn.msgDist.AddListener("ClearUI", RecvClearUI);

        initPoker = vars.poker;
        //获取到父组件
        selfParent = skinTrans.Find("Player").transform;

        team_1 = skinTrans.Find("Player_1").gameObject;
        team_2 = skinTrans.Find("Player_2").gameObject;
        team_1.transform.GetChild(0).GetComponent<Text>().text = "剩余：0";
        team_2.transform.GetChild(0).GetComponent<Text>().text = "剩余：0";

        self = skinTrans.Find("self").gameObject;
        self.GetComponent<Image>().sprite = vars.selfSpriteHead[Random.Range(0, 8)];

        //获取已出牌父组件
        cardsParent = GameObject.Find("Canvas").transform.Find("cards");
        //countDownText = skinTrans.Find("countDown").GetComponent<Text>();
        countDownText = GameObject.Find("Canvas").transform.Find("CountDown").GetChild(0).GetComponent<Text>();
        countDownBtn = skinTrans.Find("countDownBtn").GetComponent<Button>();
        countDownBtn.onClick.AddListener(CountDownNum);
        countDownBtn.transform.GetChild(0).GetComponent<Text>().text = "倍数：25";
        readyPokerBtn = skinTrans.Find("readyPokerBtn").GetComponent<Button>();
        readyPokerBtn.onClick.AddListener(ReadyBtn);
        readyPokerBtn.GetComponent<Image>().color = Color.grey;
        readyPokerBtn.transform.GetChild(0).GetComponent<Text>().text = "请准备";

        landBtn_Yes = skinTrans.Find("landBtn_Yes").GetComponent<Button>();
        landBtn_Yes.onClick.AddListener(()=> { DelTimerSysTask(); LandBtn_Yes(); });
        landBtn_No = skinTrans.Find("landBtn_No").GetComponent<Button>();
        landBtn_No.onClick.AddListener(()=> { DelTimerSysTask(); LandBtn_No(); });

        mul_0Btn = skinTrans.Find("mul_0Btn").GetComponent<Button>();
        mul_0Btn.onClick.AddListener(() => { DelTimerSysTask(); MulBtn(5); });
        mul_1Btn = skinTrans.Find("mul_1Btn").GetComponent<Button>();
        mul_1Btn.onClick.AddListener(() => { DelTimerSysTask(); MulBtn(2); });
        mul_2Btn = skinTrans.Find("mul_2Btn").GetComponent<Button>();
        mul_2Btn.onClick.AddListener(() => { DelTimerSysTask(); MulBtn(3); });
        mul_3Btn = skinTrans.Find("mul_3Btn").GetComponent<Button>();
        mul_3Btn.onClick.AddListener(() => { DelTimerSysTask(); MulBtn(4); });

        playCardBtn = skinTrans.Find("playCardBtn").GetComponent<Button>();
        playCardBtn.onClick.AddListener(() => { DelTimerSysTask(); PlayCard(0); });
        cannotAffordBtn = skinTrans.Find("cannotAffordBtn").GetComponent<Button>();
        cannotAffordBtn.onClick.AddListener(() => {DelTimerSysTask(); PlayCard(1); });

        quitBtn = skinTrans.Find("quit").GetComponent<Button>();
        quitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        backBtn = skinTrans.Find("back").GetComponent<Button>();
        backBtn.onClick.AddListener(() =>
        {
            //TODO返回大厅函数
            ClearClientData();
        });


        TimerSys timerSys = GameObject.Find("Root").GetComponent<TimerSys>();
        timerSys.InitSys();

        //获取poker组件列表
        pokerSpriteList = vars.landlordSpriteList;
        for (int i = 0; i < 54; i++)
        {
            startList.Add(i);
            pokerSpriteDic.Add(pokerSpriteList[i].name, pokerSpriteList[i]);
        }

        landBtn_Yes.gameObject.SetActive(false);
        landBtn_No.gameObject.SetActive(false);
        mul_0Btn.gameObject.SetActive(false);
        mul_1Btn.gameObject.SetActive(false);
        mul_2Btn.gameObject.SetActive(false);
        mul_3Btn.gameObject.SetActive(false);
        playCardBtn.gameObject.SetActive(false);
        cannotAffordBtn.gameObject.SetActive(false);
        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomInfo");
        NetMgr.srvConn.Send(protocol);
    }

    public override void OnClosing()
    {
        NetMgr.srvConn.msgDist.DelListener("CountDown", RecvCountDown);
        NetMgr.srvConn.msgDist.DelListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.srvConn.msgDist.DelListener("Ready_Poker", RecvReady_Poker);
        NetMgr.srvConn.msgDist.DelListener("Send_Poker", RecvSend_Poker);
        NetMgr.srvConn.msgDist.DelListener("Land_Lord", RecvLand_Lord);
        NetMgr.srvConn.msgDist.DelListener("LandOK", RecvLandOK);
        NetMgr.srvConn.msgDist.DelListener("Play_Card", RecvPlay_Card);
        NetMgr.srvConn.msgDist.DelListener("Cd", RecvCd);
        NetMgr.srvConn.msgDist.DelListener("Win", RecvWin);
        NetMgr.srvConn.msgDist.DelListener("ClearUI", RecvClearUI);
    }

    //叫地主按钮点击事件
    private void LandBtn_Yes()
    {
        if(GameMgr.instance.mulNum == 0)
        {
            Video.instance.PlaySound("", 0);
        }else Video.instance.PlaySound("", 2);

        MulBtn(0);

        landBtn_Yes.gameObject.SetActive(false);
        landBtn_No.gameObject.SetActive(false);
    }

    //不叫按钮点击事件
    private void LandBtn_No()
    {
        if (GameMgr.instance.mulNum == 0)
        {
            Video.instance.PlaySound("", 1);
        }
        else Video.instance.PlaySound("", 3);

        MulBtn(1);

        landBtn_Yes.gameObject.SetActive(false);
        landBtn_No.gameObject.SetActive(false);
    }

    private void MulBtn(int mul)
    {
        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Land_Lord");
        protocol.AddInt(mul);
        NetMgr.srvConn.Send(protocol);

        if (mul >= 2)
        {
            mul_0Btn.gameObject.SetActive(false);
            mul_1Btn.gameObject.SetActive(false);
            mul_2Btn.gameObject.SetActive(false);
            mul_3Btn.gameObject.SetActive(false);
            if(mul != 5)
            {
                Video.instance.PlaySound("", 4);
            }else Video.instance.PlaySound("", 5);
        }
    }

    //倒计时按钮
    private void CountDownNum()
    {
        SetCountNum(15);
    }

    //准备按钮点击事件
    private void ReadyBtn()
    {
        readyPokerBtn.GetComponent<Image>().color = Color.green;
        readyPokerBtn.transform.GetChild(0).GetComponent<Text>().text = "已准备";

        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Ready_Poker");
        NetMgr.srvConn.Send(protocol);
    }
    
    //通知服务的发牌 暂时舍弃
    /*
    public void SendPk()
    {
        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Send_Poker");
        NetMgr.srvConn.Send(protocol);
    }*/


    //发牌动画
    private void PokerMove(int sindex)
    {
        Video.instance.PlaySound("", 10);
        GameObject ob = Instantiate(initPoker);
        ob.transform.SetParent(selfParent);
        //print("--------------- " + GameMgr.instance.team + "   " + sindex);
        Sprite sp = vars.landlordSpriteList[playerPokerList[GameMgr.instance.team-1, sindex]];
        ob.GetComponent<Image>().sprite = sp;
        ob.name = sp.name;
        //ob.GetComponent<RectTransform>().localPosition = new Vector3(-320 + i * 40, -155, 0);
        ob.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        ob.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        ob.transform.DOLocalMove(new Vector3(-320 + sindex * 40, -155, 0), speedPoker);
        ob.transform.DOScale(new Vector3(1, 1, 1), speedPoker).OnComplete(()=> {
            AddButtonOb(ob);
            sortPokerList.Add(ob);
            if (sindex < 16)
            {
                PokerMove(sindex + 1);
            }
            else
            {
                Sort();
            }
        });
    }

    //排序手中之牌
    private void Sort()
    {
        Video.instance.PlaySound("", 11);
        for (int i = 0; i < sortPokerList.Count; i++)
        {
            Transform poker1 = sortPokerList[i].transform;
            string str1Name = poker1.name;
            int pokerInt1 = GetPokerNum(str1Name);
            for (int j = i+1; j < sortPokerList.Count; j++)
            {
                Transform poker2 = sortPokerList[j].transform;
                string str2Name = poker2.name;
                int pokerInt2 = GetPokerNum(str2Name);
                if(pokerInt2 < pokerInt1)
                {
                    //print("pokerInt1  pokerInt2 --- " + pokerInt1 + " --- " + pokerInt2);
                    GameObject tepOb = poker1.gameObject;
                    sortPokerList[i] = poker2.gameObject;
                    sortPokerList[j] = tepOb;
                    poker1 = poker2;
                    pokerInt1 = pokerInt2;
                }
            }
        }

        SortAnim(sortPokerList.Count, 0);
    }

    //手中牌排序动画
    private void SortAnim(int pokerNum,int index)
    {
        int py = pokerNum / 2;
        sortPokerList[index].transform.SetSiblingIndex(index);
        sortPokerList[index].transform.DOLocalMove(new Vector3(-40*py + index * 40, -155, 0), 0.05f).OnComplete(()=>{
            if (index < pokerNum-1)
            {
                SortAnim(pokerNum, index + 1);
            }
            else
            {
                if (GameMgr.instance.isLandlord)
                {

                    countDownText.text = "15";
                    AddTimerSysTask(15, countDownText);
                    if (!GameMgr.instance.isMuled)
                    {
                        landBtn_Yes.gameObject.SetActive(true);
                        landBtn_No.gameObject.SetActive(true);
                    }
                    else
                    {
                        playCardBtn.gameObject.SetActive(true);
                    }

                    //发送查询
                    ProtocolBytes protocol = new ProtocolBytes();
                    protocol.AddString("Cd");
                    protocol.AddInt(GameMgr.instance.team);
                    NetMgr.srvConn.Send(protocol);
                }
            }
        });
    }

    //获取单张牌对应大小-数字
    private int GetPokerNum(string pokerName)
    {
        int _int1 = -1;
        string temName = null;
        //print("pokerName ==== " + pokerName);
        switch (pokerName.Substring(0, 3))
        {
            case "Red":
                temName = pokerName.Substring(pokerName.Length - 1, 1);
                break;
            case "Bla":
                temName = pokerName.Substring(pokerName.Length - 1, 1);
                break;
            case "flo":
                temName = pokerName.Substring(pokerName.Length - 1, 1);
                break;
            case "Squ":
                temName = pokerName.Substring(pokerName.Length - 1, 1);
                break;
            case "Sma":
                temName = "Sma";
                break;
            case "Big":
                temName = "Big";
                break;
            default:
                print("排序中截取名称字符错误！");
                break;
        }

        switch (temName)
        {
            case "0":
                _int1 = 10;
                break;
            case "J":
                _int1 = 11;
                break;
            case "Q":
                _int1 = 12;
                break;
            case "K":
                _int1 = 13;
                break;
            case "Sma":
                _int1 = 98;
                break;
            case "Big":
                _int1 = 99;
                break;
            default:
                _int1 = int.Parse(temName);
                if (_int1 == 1) _int1 = 14;
                if (_int1 == 2) _int1 = 20;
                break;
        }

        return _int1;
    }


    //获取房间信息列表
    public void RecvGetRoomInfo(ProtocolBase protocol)
    {
        //获取总数
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int count = proto.GetInt(start, ref start);
        //每个处理
        int i = 0;
        if (!GameMgr.instance.isrefBool && count >= 3)
        {
            GameMgr.instance.isrefBool = true;
            for (i = 0; i < count; i++)
            {
                string id = proto.GetString(start, ref start);
                int team = proto.GetInt(start, ref start);
                int win = proto.GetInt(start, ref start);
                int fail = proto.GetInt(start, ref start);
                int isOwner = proto.GetInt(start, ref start);
                if (GameMgr.instance.id == id)
                {
                    GameMgr.instance.team = team;
                    print("GameMgr.instance.team ==========id===== " + GameMgr.instance.team + "   " + id);
                    self.transform.GetChild(0).GetComponent<Text>().text = "<color=#00DFCD>自己：</color>" + id;
                }
                GameMgr.instance.teamName.Add(team, id);
            }
            GetTeamName();
        } 
    }

    //取出对应队信息
    private void GetTeamName()
    {
        string nm1 = null;
        string nm2 = null;
        if (GameMgr.instance.team == 1)
        {
            if (GameMgr.instance.teamName.ContainsKey(2))
            {
                nm2 = GameMgr.instance.teamName[2];
            }
            if (GameMgr.instance.teamName.ContainsKey(3))
            {
                nm1 = GameMgr.instance.teamName[3];
            }
        }
        else if (GameMgr.instance.team == 2)
        {
            if (GameMgr.instance.teamName.ContainsKey(3))
            {
                nm2 = GameMgr.instance.teamName[3];
            }
            if (GameMgr.instance.teamName.ContainsKey(1))
            {
                nm1 = GameMgr.instance.teamName[1];
            }
        }
        else
        {
            if (GameMgr.instance.teamName.ContainsKey(1))
            {
                nm2 = GameMgr.instance.teamName[1];
            }
            if (GameMgr.instance.teamName.ContainsKey(2))
            {
                nm1 = GameMgr.instance.teamName[2];
            }
        }

        team_1.transform.GetChild(2).GetComponent<Text>().text = "<color=#00DFCD>玩家：</color>" + nm1;
        team_2.transform.GetChild(2).GetComponent<Text>().text = "<color=#00DFCD>玩家：</color>" + nm2;
    }

    //设置服务的倒计时
    private void SetCountNum(int count)
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("CountDown");
        proto.AddInt(count);
        NetMgr.srvConn.Send(proto);
    }

    //服务端倒计时返回函数
    public void RecvCountDown(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        int count = proto.GetInt(start, ref start);
        if (GameMgr.instance.isLandlord)
        {
            countDownText.text = count.ToString();
        }
    }

    //准备事件返回函数
    public void RecvReady_Poker(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
    }

    //发牌返回函数
    public void RecvSend_Poker(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        string team1 = proto.GetString(start, ref start);
        string team2 = proto.GetString(start, ref start);
        string team3 = proto.GetString(start, ref start);
        string bList = proto.GetString(start, ref start);
        int land = proto.GetInt(start, ref start);

        if (GameMgr.instance.team == land + 1)
        {
            GameMgr.instance.isLandlord = true;
        }
        else GameMgr.instance.isLandlord = false;

        bottomPokerList = new int[3];
        playerPokerList = new int[3, 17];

        string[] arrPoker1 = team1.Split(';');
        string[] arrPoker2 = team2.Split(';');
        string[] arrPoker3 = team3.Split(';');
        string[] arrbPoker = bList.Split(';');

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 17; j++)
            {
                if (i == 0) playerPokerList[i, j] = int.Parse(arrPoker1[j]);
                else if (i == 1) playerPokerList[i, j] = int.Parse(arrPoker2[j]);
                else playerPokerList[i, j] = int.Parse(arrPoker3[j]);
            }
            bottomPokerList[i] = int.Parse(arrbPoker[i]);
        }
        readyPokerBtn.gameObject.SetActive(false);
        sortPokerList.Clear();
        PokerMove(0);
    }

    //叫地主 加倍返回函数
    public void RecvLand_Lord(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        int landIndex = proto.GetInt(start, ref start);
        int landNum = proto.GetInt(start, ref start);
        int noLandNum = proto.GetInt(start, ref start);
        int mul = proto.GetInt(start, ref start);
        print("GameMgr.instance.team ===== " + GameMgr.instance.team);
        if (landIndex+1 == GameMgr.instance.team)
        {
            GameMgr.instance.isLandlord = true;
            countDownText.text = "15";
            AddTimerSysTask(15, countDownText);
            if (landNum + noLandNum >= 3)
            {
                landBtn_Yes.gameObject.SetActive(false);
                landBtn_No.gameObject.SetActive(false);

                mul_0Btn.gameObject.SetActive(true);
                mul_1Btn.gameObject.SetActive(true);
                mul_2Btn.gameObject.SetActive(true);
                mul_3Btn.gameObject.SetActive(true);
            }
            else
            {
                landBtn_Yes.gameObject.SetActive(true);
                landBtn_Yes.transform.GetChild(0).GetComponent<Text>().text = "我抢";
                landBtn_No.gameObject.SetActive(true);
                landBtn_No.transform.GetChild(0).GetComponent<Text>().text = "不抢";
            }

            //发送查询
            ProtocolBytes protocol1 = new ProtocolBytes();
            protocol1.AddString("Cd");
            protocol1.AddInt(landIndex + 1);
            NetMgr.srvConn.Send(protocol1);
        }
        else
        {
            GameMgr.instance.isLandlord = false;
            
        }
        GameMgr.instance.mulNum = mul;
        countDownBtn.transform.GetChild(0).GetComponent<Text>().text = "倍数：" + mul.ToString();
    }

    //轮到谁谁其他客户端倒计时显示
    private void GetCountDown(int landTeam,int tm)
    {
        if (landTeam == GameMgr.instance.team) return;
        Text tex;
        print("landTeam =========== " + landTeam);
        if (landTeam == 1)
        {
            if (GameMgr.instance.team == 2)
            {
                tex = team_1.transform.GetChild(1).GetComponent<Text>();

            }
            else tex = team_2.transform.GetChild(1).GetComponent<Text>();

        }
        else if (landTeam == 2)
        {
            if (GameMgr.instance.team == 1)
            {
                tex = team_2.transform.GetChild(1).GetComponent<Text>();

            }
            else tex = team_1.transform.GetChild(1).GetComponent<Text>();

        }
        else
        {
            if (GameMgr.instance.team == 1)
            {
                tex = team_1.transform.GetChild(1).GetComponent<Text>();

            }
            else tex = team_2.transform.GetChild(1).GetComponent<Text>();
        }
        tex.text = tm.ToString();
        AddTimerSysTask(tm, tex);
    }

    //叫地主结束返回函数
    public void RecvLandOK(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        int nowLand = proto.GetInt(start, ref start);
        string upCards = proto.GetString(start, ref start);
        int mul = proto.GetInt(start, ref start);
        GameMgr.instance.isLanded = nowLand;
        GameMgr.instance.nowPlayCardTeam = nowLand;
        GameMgr.instance.upTeamPlayCards = upCards;
        GameMgr.instance.isMuled = true;
        GameMgr.instance.mulNum = mul;
        countDownBtn.transform.GetChild(0).GetComponent<Text>().text = "倍数：" + mul.ToString();
        team_1.transform.GetChild(0).GetComponent<Text>().text = "剩余：未知";
        team_2.transform.GetChild(0).GetComponent<Text>().text = "剩余：未知";
        print("---------------------landOk--------------- ");
        if (GameMgr.instance.isLanded == GameMgr.instance.team)
        {
            GameMgr.instance.isLandlord = true;
            AniBtmPoker(0);
        }
        else
        {
            GameMgr.instance.isLandlord = false;
        }

        ChangeHead(nowLand);
    }

    //动态改变其他队伍head
    private void ChangeHead(int land)
    {
        if(land == 1)
        {
            if(GameMgr.instance.team == 1)
            {
                team_1.GetComponent<Image>().sprite = vars.leftSpriteFarmer;
                team_2.GetComponent<Image>().sprite = vars.rightSpriteFarme;
                self.transform.GetChild(1).gameObject.SetActive(true);
            }
            else if(GameMgr.instance.team == 2)
            {
                team_1.GetComponent<Image>().sprite = vars.leftSpriteLand;
                team_2.GetComponent<Image>().sprite = vars.rightSpriteFarme;
            }
            else
            {
                team_1.GetComponent<Image>().sprite = vars.leftSpriteFarmer;
                team_2.GetComponent<Image>().sprite = vars.rightSpriteLand;
            }
        }else if (land == 2)
        {
            if (GameMgr.instance.team == 1)
            {
                team_1.GetComponent<Image>().sprite = vars.leftSpriteFarmer;
                team_2.GetComponent<Image>().sprite = vars.rightSpriteLand;
                self.transform.GetChild(1).gameObject.SetActive(true);
            }
            else if (GameMgr.instance.team == 2)
            {
                team_1.GetComponent<Image>().sprite = vars.leftSpriteFarmer;
                team_2.GetComponent<Image>().sprite = vars.rightSpriteFarme;
            }
            else
            {
                team_1.GetComponent<Image>().sprite = vars.leftSpriteLand;
                team_2.GetComponent<Image>().sprite = vars.rightSpriteFarme;
            }
        }
        else
        {
            if (GameMgr.instance.team == 1)
            {
                team_1.GetComponent<Image>().sprite = vars.leftSpriteLand;
                team_2.GetComponent<Image>().sprite = vars.rightSpriteFarme;
            }
            else if (GameMgr.instance.team == 2)
            {
                team_1.GetComponent<Image>().sprite = vars.leftSpriteFarmer;
                team_2.GetComponent<Image>().sprite = vars.rightSpriteLand;
            }
            else
            {
                team_1.GetComponent<Image>().sprite = vars.leftSpriteFarmer;
                team_2.GetComponent<Image>().sprite = vars.rightSpriteFarme;
                self.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    //三张底牌动画函数
    private void AniBtmPoker(int sindex)
    {
        Video.instance.PlaySound("", 10);
        GameObject ob = Instantiate(initPoker);
        ob.transform.SetParent(selfParent);
        //print("--------------- " + GameMgr.instance.team + "   " + sindex);
        Sprite sp = vars.landlordSpriteList[bottomPokerList[sindex]];
        ob.GetComponent<Image>().sprite = sp;
        ob.name = sp.name;
        //ob.GetComponent<RectTransform>().localPosition = new Vector3(-320 + i * 40, -155, 0);
        ob.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        ob.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        ob.transform.DOLocalMove(new Vector3(460 + sindex * 40, -155, 0), speedPoker);
        ob.transform.DOScale(new Vector3(1, 1, 1), speedPoker).OnComplete(() => {
            AddButtonOb(ob);
            sortPokerList.Add(ob);
            
            if (sindex < 2)
            {
                AniBtmPoker(sindex + 1);
            }
            else
            {
                Sort();
            }
        });
    }

    //出牌按钮点击事件
    private void PlayCard(int index)
    {
        if(GameMgr.instance.team != GameMgr.instance.nowPlayCardTeam)
        {
            print("别着急，还未轮到你！");
            PanelMgr.instance.OpenPanel<TipPanel>("", "别着急，还未轮到你！");
            return;
        }

        if(index == 1)//要不起
        {
            playCardBtn.gameObject.SetActive(false);
            cannotAffordBtn.gameObject.SetActive(false);

            //发送查询
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("Play_Card");
            protocol.AddString("0");
            NetMgr.srvConn.Send(protocol);
        }
        else
        {
            if(cardsDict.Count > 0)
            {
                DicCard();
            }
        }
    }


    private void DicCard()
    {
        if(tempCards.Count > 0)
        {
            tempCards.Clear();
        }

        foreach(string key in cardsDict.Keys)
        {
            tempCards.Add(GetPokerNum(key));
        }
        Poker poker = new Poker();

        if(GameMgr.instance.team == GameMgr.instance.maxCardTeam)
        {
            GameMgr.instance.upTeamPlayCards = "0";
        }

        //判断能否出牌
        string isCards = poker.CheckCardList(tempCards);
        print("上家出的牌 + 当前出的牌是  + tempCards ==== " + GameMgr.instance.upTeamPlayCards + "  " + isCards + " " + tempCards.Count);
        if (isCards != null)
        {
            string pName = null;
            
            foreach (KeyValuePair<string,GameObject>item in cardsDict)
            {
                pName += item.Key + ";";
                Destroy(item.Value);
            }

            cardsDict.Clear();

            DelTimerSysTask();
            if (playCardBtn.gameObject.activeSelf)
            {
                playCardBtn.gameObject.SetActive(false);
            }
            if (cannotAffordBtn.gameObject.activeSelf)
            {
                cannotAffordBtn.gameObject.SetActive(false);
            }

            StartCoroutine(DestoryPoker());

            //发送查询
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("Play_Card");
            protocol.AddString(isCards);
            protocol.AddString(pName);
            protocol.AddInt(tempCards.Count);
            NetMgr.srvConn.Send(protocol);

            if(GameMgr.instance.upTeamPlayCards == "0")
            {
                Video.instance.PlaySound(isCards, -1);
            }
            else
            {
                int rd = Random.Range(7, 10);
                Video.instance.PlaySound("", rd);
            }
        }
        else
        {
            print("您不能这样做！");
            PanelMgr.instance.OpenPanel<TipPanel>("", "您不能这样做！");
            Video.instance.PlaySound("", 6);
        }
    }


    IEnumerator DestoryPoker()
    {
        yield return new WaitForSeconds(0.5f);
        Image[] ts = selfParent.GetComponentsInChildren<Image>();
        int dx = 0;
        foreach (Image item in ts)
        {
            item.transform.SetSiblingIndex(dx);
            dx++;
        }

       // print(" ---------剩余牌数---------- " + ts.Length + "   " + selfParent.GetComponent<RectTransform>().localPosition);
       if(dx > 0)
        {
            SortAnimPoker(ts, 0);
        }
        else
        {
            //发送查询
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("Win");
            protocol.AddInt(GameMgr.instance.team);
            NetMgr.srvConn.Send(protocol);
        }
    }

    //胜利结束
    private void RecvWin(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        int winTeam = proto.GetInt(start, ref start);
        if(winTeam == GameMgr.instance.team)//胜利
        {
            PanelMgr.instance.OpenPanel<WinPanel>("", 1);
        }
        else//失败
        {
            PanelMgr.instance.OpenPanel<WinPanel>("", 0);
        }
    }

    //同步出过的牌到各个客户端
    private void PokerToClient(string[] list,int rot,int locX,int locY)
    {
        for (int i = 0; i < list.Length-1; i++)
        {
            GameObject ob = Instantiate(initPoker);
            ob.transform.SetParent(cardsParent);
            Sprite sp = pokerSpriteDic[list[i]];
            ob.GetComponent<Image>().sprite = sp;
            ob.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            ob.name = list[i];
            ob.GetComponent<RectTransform>().localScale = new Vector3(0f, 0f, 0f);
            ob.GetComponent<RectTransform>().localEulerAngles = new Vector3(60, 0, rot+i*15);
            ob.transform.SetParent(cardsParent);
            ob.transform.DOLocalMove(new Vector3(locX+i*15, locY+i*15, 0), 0.2f);
            ob.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f);
        }
    }

    //为实例化出来的牌添加按钮事件
    private void AddButtonOb(GameObject obj)
    {
        Button bt = obj.AddComponent<Button>();
        EventTriggerListener.Get(bt.gameObject).onDown = OnEnterBtn;
        /*
        bt.onClick.AddListener(() =>
        {
            float vcY = bt.gameObject.GetComponent<RectTransform>().localPosition.y;
            if (vcY == -155)
            {
                bt.transform.DOLocalMoveY(-135f, 0.2f);
                if (!cardsDict.ContainsKey(obj.name))
                {
                    cardsDict.Add(obj.name, bt.gameObject);
                }
            }
            else
            {
                bt.transform.DOLocalMoveY(-155f, 0.2f);
                if (cardsDict.ContainsKey(obj.name))
                {
                    cardsDict.Remove(obj.name);
                }
            }
        });*/
    }

    //监听按钮事件
    private void OnEnterBtn(GameObject obj)
    {
        Button bt = obj.GetComponent<Button>();
        Vector3 vc3 = bt.gameObject.GetComponent<RectTransform>().localPosition;
        float vcY = vc3.y;
        if (vcY == -155)
        {
            bt.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(vc3.x, -135f, vc3.z);
            if (!cardsDict.ContainsKey(obj.name))
            {
                cardsDict.Add(obj.name, bt.gameObject);
            }
        }
        else
        {
            bt.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(vc3.x, -155f, vc3.z);
            if (cardsDict.ContainsKey(obj.name))
            {
                cardsDict[obj.name] = null;
                cardsDict.Remove(obj.name);
            }
        }
        print("点击Poker ===========  " + obj.name + "    " + cardsDict.Count);
    }

    //出牌后返回函数
    private void RecvPlay_Card(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        int nowPlayCardTeam = proto.GetInt(start, ref start);
        string upCards = proto.GetString(start, ref start);
        int maxCard = proto.GetInt(start, ref start);

        GameMgr.instance.nowPlayCardTeam = nowPlayCardTeam;
        GameMgr.instance.upTeamPlayCards = upCards;
        GameMgr.instance.maxCardTeam = maxCard;
        string PlayEdCards = proto.GetString(start, ref start);
        int rot = proto.GetInt(start, ref start);
        int locX = proto.GetInt(start, ref start);
        int locY = proto.GetInt(start, ref start);
        string pokerNum = proto.GetString(start, ref start);
        if (GameMgr.instance.nowPlayCardTeam == GameMgr.instance.team)
        {
            countDownText.text = "15";
            AddTimerSysTask(15, countDownText);
            playCardBtn.gameObject.SetActive(true);
            if (GameMgr.instance.nowPlayCardTeam != GameMgr.instance.maxCardTeam)
            {
                cannotAffordBtn.gameObject.SetActive(true);
            }

            //发送查询
            ProtocolBytes protocol1 = new ProtocolBytes();
            protocol1.AddString("Cd");
            protocol1.AddInt(nowPlayCardTeam);
            NetMgr.srvConn.Send(protocol1);
        }

        string[] arrPokerNames = PlayEdCards.Split(';');
        string[] arrPokerNums = pokerNum.Split(';');
        PokerToClient(arrPokerNames, rot, locX, locY);
        PokerNumToClient(arrPokerNums);
    }

    //通知所有客户端剩余牌数
    private void PokerNumToClient(string[] list)
    {
        int tm = GameMgr.instance.team;
        int tmNext = -1;
        if (tm == 1) tmNext = 2;
        else if (tm == 2) tmNext = 3;
        else tmNext = 1;
        string str1 = null;
        string str2 = null;
        int int1 = -1;
        int int2 = -1;

        if(int.Parse(list[0]) == tm)
        {
            if(int.Parse(list[2]) == tmNext)
            {
                int1 = 5;
                int2 = 3;
            }else if(int.Parse(list[4]) == tmNext)
            {
                int1 = 3;
                int2 = 5;
            }
        }else if (int.Parse(list[2]) == tm)
        {
            if (int.Parse(list[0]) == tmNext)
            {
                int1 = 5;
                int2 = 1;
            }
            else if (int.Parse(list[4]) == tmNext)
            {
                int1 = 1;
                int2 = 5;
            }
        }
        else if (int.Parse(list[4]) == tm)
        {
            if (int.Parse(list[0]) == tmNext)
            {
                int1 = 3;
                int2 = 1;
        }
            else if (int.Parse(list[2]) == tmNext)
            {
                int1 = 1;
                int2 = 3;
            }
        }
        str1 = list[int1];
        str2 = list[int2];
        team_1.transform.GetChild(0).GetComponent<Text>().text = "剩余：" + str1;
        team_2.transform.GetChild(0).GetComponent<Text>().text = "剩余：" + str2;
    }

    //出牌后重新排序手中牌
    private void SortAnimPoker(Image[] ts, int index)
    {
        int tol = ts.Length;
        int py = tol / 2;
        //ts[index].SetSiblingIndex(index);
        ts[index].transform.DOLocalMove(new Vector3(-40 * py + index * 40, -155, 0), 0.05f).OnComplete(() => {
            if (index < tol - 1)
            {
                SortAnimPoker(ts, index + 1);
            }
            else
            {

            }
        });
    }

    //必要出倒计时通知
    private void RecvCd(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        int team = proto.GetInt(start, ref start);
        if(team != GameMgr.instance.team)
        {
            GetCountDown(team, 15);
        }
    }

    //清理客户端数据
    private void ClearClientData()
    {
        //发送查询
        ProtocolBytes protocol1 = new ProtocolBytes();
        protocol1.AddString("ClearUI");
        protocol1.AddInt(GameMgr.instance.team);
        NetMgr.srvConn.Send(protocol1);

        ClearServerData();
        OnCloseClick();
    }

    public void OnCloseClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("LeaveRoom");
        NetMgr.srvConn.Send(protocol, OnCloseBack);
    }


    public void OnCloseBack(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if (ret == 0)
        {
            

            PanelMgr.instance.OpenPanel<TipPanel>("", "退出成功!");
            PanelMgr.instance.OpenPanel<RoomListPanel>("");
            //Close();
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出失败！");
        }
    }

    //通知清理服务端数据
    private void ClearServerData()
    {
        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("ClearSer");
        NetMgr.srvConn.Send(protocol);
    }

    //退出房間后通知其他客戶端
    private void RecvClearUI(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        int team = proto.GetInt(start, ref start);

        print("--------------team --------GameMgr.instance.team------ " + team + " " + GameMgr.instance.team);
        if(team != GameMgr.instance.team)
        {
            if(team == 1)
            {
                if(GameMgr.instance.team == 2)
                {
                    team_1.transform.GetChild(2).GetComponent<Text>().text = "";
                }
                else
                {
                    team_2.transform.GetChild(2).GetComponent<Text>().text = "";
                }
            }
            else if (team == 2)
            {
                if (GameMgr.instance.team == 1)
                {
                    team_2.transform.GetChild(2).GetComponent<Text>().text = "";
                }
                else
                {
                    team_1.transform.GetChild(2).GetComponent<Text>().text = "";
                }
            }
            else
            {
                if (GameMgr.instance.team == 1)
                {
                    team_1.transform.GetChild(2).GetComponent<Text>().text = "";
                }
                else
                {
                    team_2.transform.GetChild(2).GetComponent<Text>().text = "";
                }
            }
        }
        else
        {
            team_1.transform.GetChild(2).GetComponent<Text>().text = "";
            team_2.transform.GetChild(2).GetComponent<Text>().text = "";
        }

        GameMgr.instance.team = -1;
        GameMgr.instance.nowTeamMove = 1;
        GameMgr.instance.isrefBool = false;
        GameMgr.instance.isLandlord = false;
        GameMgr.instance.isLanded = -1;
        GameMgr.instance.countDownNum = 15;
        GameMgr.instance.nowPlayCardTeam = -1;
        GameMgr.instance.upTeamPlayCards = "0";
        GameMgr.instance.maxCardTeam = -1;
        GameMgr.instance.isMuled = false;
        GameMgr.instance.mulNum = 0;
        GameMgr.instance.teamName.Clear();

        landBtn_Yes.gameObject.SetActive(false);
        landBtn_No.gameObject.SetActive(false);
        mul_0Btn.gameObject.SetActive(false);
        mul_1Btn.gameObject.SetActive(false);
        mul_2Btn.gameObject.SetActive(false);
        mul_3Btn.gameObject.SetActive(false);
        playCardBtn.gameObject.SetActive(false);
        cannotAffordBtn.gameObject.SetActive(false);

        readyPokerBtn.gameObject.SetActive(true);
        readyPokerBtn.GetComponent<Image>().color = Color.grey;
        readyPokerBtn.transform.GetChild(0).GetComponent<Text>().text = "请准备";
        countDownBtn.transform.GetChild(0).GetComponent<Text>().text = "倍数：25";
        team_1.GetComponent<Image>().sprite = vars.landlordBc;
        team_2.GetComponent<Image>().sprite = vars.landlordBc;
        team_1.transform.GetChild(0).GetComponent<Text>().text = "剩余：0";
        team_2.transform.GetChild(0).GetComponent<Text>().text = "剩余：0";

        Image[] ts = selfParent.GetComponentsInChildren<Image>();
        Image[] ts1 = cardsParent.GetComponentsInChildren<Image>();
        foreach (Image item in ts)
        {
            Destroy(item.gameObject);
        }
        foreach (Image item in ts1)
        {
            Destroy(item.gameObject);
        }

        if (tId > 0) DelTimerSysTask();
    }

    /***********************定时任务**********************/
    private void AddTimerSysTask(int timeIndex,Text tex)
    {
        if (tId > 0) DelTimerSysTask();
        GameMgr.instance.countDownNum = timeIndex;
        tId = TimerSys.Instance.AddTimeTask(() => {
            GameMgr.instance.countDownNum--;
            if(GameMgr.instance.countDownNum == 0)
            {
                //print("%%%%%%%%%%%%%%%%%%%%%%%%  " + cannotAffordBtn.gameObject.activeSelf + "  " + mul_0Btn.gameObject.activeSelf);
                if (landBtn_No.gameObject.activeSelf)
                {
                    LandBtn_No();
                }
                else if (cannotAffordBtn.gameObject.activeSelf || playCardBtn.gameObject.activeSelf)
                {
                    PlayCard(1);
                }
                else if(mul_0Btn.gameObject.activeSelf)
                {
                    MulBtn(5);
                }
                tex.text = "";
            }
            else
            {
                tex.text = GameMgr.instance.countDownNum.ToString();
            }
        }, 1000, PETimeUnit.MilliSecond, timeIndex);
    }

    private void DelTimerSysTask()
    {
        bool ret = TimerSys.Instance.DelTimeTask(tId);
        countDownText.text = "";
        team_1.transform.GetChild(1).GetComponent<Text>().text = "";
        team_2.transform.GetChild(1).GetComponent<Text>().text = "";

    }
    /***********************定时任务**********************/
}
