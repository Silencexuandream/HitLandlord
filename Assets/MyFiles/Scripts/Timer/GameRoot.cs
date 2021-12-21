// ========================================================
// 描述：启动入口
// 作者：城门
// 邮箱：136569631@qq.com
// 创建时间：2019/05/28 09:26:53
// 版本：v 1.0
// ========================================================

using UnityEngine;
using UnityEngine.UI;

public class GameRoot : MonoBehaviour {
    private Button btnAdd;
    private Button btnDel;
    private Button btnRep;

    private Button btnAdd_Frame;
    private Button btnDel_Frame;
    private Button btnRep_Frame;

    private int tId;

    private void Start()
    {
        btnAdd = GameObject.Find("Canvas/btn_Add").GetComponent<Button>();
        btnAdd.onClick.AddListener(AddTimerSysTask);
        btnDel = GameObject.Find("Canvas/btn_Del").GetComponent<Button>();
        btnDel.onClick.AddListener(DelTimerSysTask);
        btnRep = GameObject.Find("Canvas/btn_Rep").GetComponent<Button>();
        btnRep.onClick.AddListener(ReplaceTimerSysTask);

        btnAdd_Frame = GameObject.Find("Canvas/btn_Add_Frame").GetComponent<Button>();
        btnAdd_Frame.onClick.AddListener(AddFrameSysTask);
        btnDel_Frame = GameObject.Find("Canvas/btn_Del_Frame").GetComponent<Button>();
        btnDel_Frame.onClick.AddListener(DelFrameSysTask);
        btnRep_Frame = GameObject.Find("Canvas/btn_Rep_Frame").GetComponent<Button>();
        btnRep_Frame.onClick.AddListener(ReplaceFrameSysTask);
        Debug.Log("Game Start.");

        TimerSys timerSys = GetComponent<TimerSys>();
        timerSys.InitSys();

        
    }

    #region 时间定时测试
    private void AddTimerSysTask()
    {
        //tId = TimerSys.Instance.AddTimeTask(FuncA, 500, PETimeUnit.MilliSecond, 0);
        tId = TimerSys.Instance.AddTimeTask(()=> {
            Debug.Log("Delay Add Log" + tId);
        }, 500, PETimeUnit.MilliSecond, 0);
    }

    private void DelTimerSysTask()
    {
        bool ret = TimerSys.Instance.DelTimeTask(tId);
        Debug.Log("Del Log " + ret);
    }

    private void ReplaceTimerSysTask()
    {
        bool ret = TimerSys.Instance.ReplaceTimeTask(tId, ()=> {
            Debug.Log("Delay Replace Log");
        }, 2000);
        Debug.Log("Replace Log " + ret);
    }
    #endregion


    #region 帧定时测试
    private void AddFrameSysTask()
    {
        tId = TimerSys.Instance.AddFrameTask(() => {
            Debug.Log("Delay Add  Frame Log" + tId);
        }, 50, 0);
    }

    private void DelFrameSysTask()
    {
        bool ret = TimerSys.Instance.DelFrameTask(tId);
        Debug.Log("Del Frame Log " + ret);
    }

    private void ReplaceFrameSysTask()
    {
        bool ret = TimerSys.Instance.ReplaceFrameTask(tId, ()=> {
            Debug.Log("Delay Frame Replace Log ");
        }, 50);
        Debug.Log("Replace Frame Log " + ret);
    }
    #endregion
}
