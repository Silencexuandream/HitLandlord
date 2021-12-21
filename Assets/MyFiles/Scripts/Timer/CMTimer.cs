// ========================================================
// 描述：
// 作者：城门
// 邮箱：136569631@qq.com
// 创建时间：2019/05/28 15:22:08
// 版本：v 1.0
// ========================================================
/* ==============================================================================
 * 功能描述：CMTimer 计时器 
 * 创 建 者：Administrator
 * 邮箱：136569631@qq.com
 * 版本：v 1.0
 * 创建日期：2019/5/28 14:29:18
 * ==============================================================================*/


using System;
using System.Collections.Generic;

public class CMTimer
{
    private DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    private double nowTime;

    private Action<string> taskLog;
    /***********************时间定时**********************/
    private static readonly string obj = "lock";
    private int tId;
    private List<int> tIdList = new List<int>();
    private List<int> rectTidList = new List<int>();

    private List<PETimeTask> tempTimeList = new List<PETimeTask>();
    private List<PETimeTask> taskTimeList = new List<PETimeTask>();
    /***********************时间定时**********************/

    /***********************帧定时**********************/
    private int frameCounter;
    private List<PEFrameTask> tempFrameList = new List<PEFrameTask>();
    private List<PEFrameTask> taskFrameList = new List<PEFrameTask>();
    /***********************帧定时**********************/

    public CMTimer(){
        tIdList.Clear();
        rectTidList.Clear();

        tempTimeList.Clear();
        taskTimeList.Clear();

        tempFrameList.Clear();
        taskFrameList.Clear();
    }

    public void Update()
    {
        CheckTimeTask();
        CheckFrameTask();

        if (rectTidList.Count > 0)
        {
            RecycleTid();
        }
    }

    private void CheckTimeTask()
    {
        //加入缓存区的定时任务
        for (int tempIndex = 0; tempIndex < tempTimeList.Count; tempIndex++)
        {
            taskTimeList.Add(tempTimeList[tempIndex]);
        }
        tempTimeList.Clear();


        //遍历检测条件是否达到条件
        nowTime = GetUTCMilliSeconds();
        for (int i = 0; i < taskTimeList.Count; i++)
        {
            PETimeTask task = taskTimeList[i];
            //nowTime 和 task.destTime 作比较
            if (nowTime.CompareTo(task.destTime) < 0)
            {
                continue;
            }
            else
            {
                Action cb = task.callBack;
                try
                {
                    if (cb != null)
                    {
                        cb();
                    }
                }
                catch (Exception e)
                {
                    LogInfo(e.ToString());
                }

                //移除已经完成的任务
                if (task.count == 1)
                {
                    taskTimeList.RemoveAt(i);
                    i--;
                    rectTidList.Add(task.tId);
                }
                else
                {
                    if (task.count != 0)
                    {
                        task.count -= 1;
                    }
                    task.destTime += task.delay;
                }

            }
        }
    }

    private void CheckFrameTask()
    {
        //加入缓存区的定时任务
        for (int tempIndex = 0; tempIndex < tempFrameList.Count; tempIndex++)
        {
            taskFrameList.Add(tempFrameList[tempIndex]);
        }
        tempFrameList.Clear();

        frameCounter += 1;
        //遍历检测条件是否达到条件
        for (int i = 0; i < taskFrameList.Count; i++)
        {
            PEFrameTask task = taskFrameList[i];
            if (frameCounter < task.destFrame)
            {
                continue;
            }
            else
            {
                Action cb = task.callBack;
                try
                {
                    if (cb != null)
                    {
                        cb();
                    }
                }
                catch (Exception e)
                {
                    LogInfo(e.ToString());
                }

                //移除已经完成的任务
                if (task.count == 1)
                {
                    taskFrameList.RemoveAt(i);
                    i--;
                    rectTidList.Add(task.tId);
                }
                else
                {
                    if (task.count != 0)
                    {
                        task.count -= 1;
                    }
                    task.destFrame += task.delay;
                }

            }
        }
    }

    #region 时间定时任务
    public int AddTimeTask(Action callBack, double delay, PETimeUnit timeUnit = PETimeUnit.MilliSecond, int count = 1)
    {
        LogInfo("Add Time Task.");

        if (timeUnit != PETimeUnit.MilliSecond)
        {
            switch (timeUnit)
            {
                case PETimeUnit.Second:
                    delay = delay * 1000;
                    break;
                case PETimeUnit.Minute:
                    delay = delay * 1000 * 60;
                    break;
                case PETimeUnit.Hour:
                    delay = delay * 1000 * 60 * 60;
                    break;
                case PETimeUnit.Day:
                    delay = delay * 1000 * 60 * 60 * 24;
                    break;
                default:
                    LogInfo("Add Task TimeUnit Type Error...");
                    break;
            }
        }

        nowTime = GetUTCMilliSeconds();
        int tid = GetTid();
        tempTimeList.Add(new PETimeTask(tid, callBack, nowTime + delay, delay, count));
        tIdList.Add(tid);
        return tid;
    }

    public bool DelTimeTask(int tId)
    {
        bool exist = false;
        for (int i = 0; i < taskTimeList.Count; i++)
        {
            PETimeTask task = taskTimeList[i];
            if (task.tId == tId)
            {
                taskTimeList.RemoveAt(i);
                for (int j = 0; j < tIdList.Count; j++)
                {
                    if (tIdList[j] == tId)
                    {
                        tIdList.RemoveAt(j);
                        break;
                    }
                }
                exist = true;
                break;
            }
        }

        if (!exist)
        {
            for (int i = 0; i < tempTimeList.Count; i++)
            {
                PETimeTask task = tempTimeList[i];
                if (task.tId == tId)
                {
                    tempTimeList.RemoveAt(i);
                    for (int j = 0; j < tIdList.Count; j++)
                    {
                        if (tIdList[j] == tId)
                        {
                            tIdList.RemoveAt(j);
                            break;
                        }
                    }
                    exist = true;
                    break;
                }
            }
        }

        return exist;
    }

    public bool ReplaceTimeTask(int tId, Action callBack, double delay, PETimeUnit timeUnit = PETimeUnit.MilliSecond, int count = 1)
    {
        if (timeUnit != PETimeUnit.MilliSecond)
        {
            switch (timeUnit)
            {
                case PETimeUnit.Second:
                    delay = delay * 1000;
                    break;
                case PETimeUnit.Minute:
                    delay = delay * 1000 * 60;
                    break;
                case PETimeUnit.Hour:
                    delay = delay * 1000 * 60 * 60;
                    break;
                case PETimeUnit.Day:
                    delay = delay * 1000 * 60 * 60 * 24;
                    break;
                default:
                    LogInfo("Add Task TimeUnit Type Error...");
                    break;
            }
        }

        nowTime = GetUTCMilliSeconds();
        PETimeTask newTask = new PETimeTask(tId, callBack, nowTime + delay, delay, count);

        bool isRep = false;
        for (int i = 0; i < taskTimeList.Count; i++)
        {
            if (taskTimeList[i].tId == tId)
            {
                taskTimeList[i] = newTask;
                isRep = true;
                break;
            }
        }

        if (!isRep)
        {
            for (int i = 0; i < tempTimeList.Count; i++)
            {
                if (tempTimeList[i].tId == tId)
                {
                    tempTimeList[i] = newTask;
                    isRep = true;
                    break;
                }
            }
        }

        return isRep;
    }
    #endregion

    #region 帧定时任务
    public int AddFrameTask(Action callBack, int delay, int count = 1)
    {
        int tid = GetTid();
        tempFrameList.Add(new PEFrameTask(tid, callBack, frameCounter + delay, delay, count));
        tIdList.Add(tid);
        return tid;
    }

    public bool DelFrameTask(int tId)
    {
        bool exist = false;
        for (int i = 0; i < taskFrameList.Count; i++)
        {
            PEFrameTask task = taskFrameList[i];
            if (task.tId == tId)
            {
                taskFrameList.RemoveAt(i);
                for (int j = 0; j < tIdList.Count; j++)
                {
                    if (tIdList[j] == tId)
                    {
                        tIdList.RemoveAt(j);
                        break;
                    }
                }
                exist = true;
                break;
            }
        }

        if (!exist)
        {
            for (int i = 0; i < tempFrameList.Count; i++)
            {
                PEFrameTask task = tempFrameList[i];
                if (task.tId == tId)
                {
                    tempFrameList.RemoveAt(i);
                    for (int j = 0; j < tIdList.Count; j++)
                    {
                        if (tIdList[j] == tId)
                        {
                            tIdList.RemoveAt(j);
                            break;
                        }
                    }
                    exist = true;
                    break;
                }
            }
        }

        return exist;
    }

    public bool ReplaceFrameTask(int tId, Action callBack, int delay, int count = 1)
    {
        PEFrameTask newTask = new PEFrameTask(tId, callBack, frameCounter + delay, delay, count);

        bool isRep = false;
        for (int i = 0; i < taskFrameList.Count; i++)
        {
            if (taskFrameList[i].tId == tId)
            {
                taskFrameList[i] = newTask;
                isRep = true;
                break;
            }
        }

        if (!isRep)
        {
            for (int i = 0; i < tempFrameList.Count; i++)
            {
                if (tempFrameList[i].tId == tId)
                {
                    tempFrameList[i] = newTask;
                    isRep = true;
                    break;
                }
            }
        }

        return isRep;
    }
    #endregion

    #region 工具函数
    public void SetLog(Action<string> log)
    {
        taskLog = log;
    }

    private void LogInfo(string info)
    {
        if (taskLog != null)
        {
            taskLog(info);
        }
    }

    private int GetTid()
    {
        lock (obj)
        {
            tId += 1;

            //安全代码，以防万一
            while (true)
            {
                if (tId == int.MaxValue)
                {
                    tId = 0;
                }
                bool used = false;
                for (int i = 0; i < tIdList.Count; i++)
                {
                    if (tId == tIdList[i])
                    {
                        used = true;
                        break;
                    }
                }
                if (!used)
                {
                    break;
                }
                else
                {
                    tId += 1;
                }
            }

        }
        return tId;
    }

    private void RecycleTid()
    {
        for (int i = 0; i < rectTidList.Count; i++)
        {
            int tid = rectTidList[i];
            for (int j = 0; j < tIdList.Count; j++)
            {
                tIdList.RemoveAt(j);
                break;
            }
        }

        rectTidList.Clear();
    }

    private double GetUTCMilliSeconds()
    {
        TimeSpan ts = DateTime.UtcNow - startDateTime;
        return ts.TotalMilliseconds;
    }
    #endregion

}

