// ========================================================
// 描述：计时系统
// 作者：城门
// 邮箱：136569631@qq.com
// 创建时间：2019/05/28 09:26:53
// 版本：v 1.0
// ========================================================
using System;
using System.Collections.Generic;
using UnityEngine;

//开发目标：
//时间定时，帧定时
//定时任务可循环，可取消，可替换
//使用简单，调用方便

public class TimerSys : MonoBehaviour {
    public static TimerSys Instance;
    private CMTimer cmT;
    private bool isCanUpdate = false;

    public void InitSys()
    {
        Instance = this;
        cmT = new CMTimer();
        cmT.SetLog((string info) =>
        {
            Debug.Log("CMTimer：" + info);
        });
        Debug.Log("TimerSys Init Done.");
        isCanUpdate = true;
    }

    private void Update()
    {
        if (isCanUpdate)
        {
            cmT.Update();
        }
    }

    #region 时间定时任务
    public int AddTimeTask(Action callBack, float delay, PETimeUnit timeUnit = PETimeUnit.MilliSecond, int count = 1)
    {
       return cmT.AddTimeTask(callBack, delay, timeUnit, count);
    }

    public bool DelTimeTask(int tId)
    {
        return cmT.DelTimeTask(tId);
    }

    public bool ReplaceTimeTask(int tId, Action callBack, float delay, PETimeUnit timeUnit = PETimeUnit.MilliSecond, int count = 1)
    {
        return cmT.ReplaceTimeTask(tId, callBack, delay, timeUnit, count);
    }
    #endregion

    #region 帧定时任务
    public int AddFrameTask(Action callBack, int delay, int count = 1)
    {
        return cmT.AddFrameTask(callBack, delay, count);
    }

    public bool DelFrameTask(int tId)
    {
        return cmT.DelFrameTask(tId);
    }

    public bool ReplaceFrameTask(int tId, Action callBack, int delay, int count = 1)
    {
        return cmT.ReplaceFrameTask(tId, callBack, delay, count);
    }
    #endregion
}
