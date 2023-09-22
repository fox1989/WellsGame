using Assets.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EventCenter : LogicNode
{
    private static EventCenter instance;
    public static EventCenter Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("eventCenter instance is null");
            }
            return instance;

        }
    }

    private Dictionary<int, List<ListenFunc>> listensDic = new Dictionary<int, List<ListenFunc>>();

    public delegate void ListenFunc(BaseEvent e);

    public void AddListen(BaseEvent baseEvent, ListenFunc func)
    {
        if (baseEvent == null || func == null)
            return;

        AddListen(baseEvent.EventKey, func);
    }


    public void AddListen(int inEventKey, ListenFunc func)
    {
        if (!listensDic.TryGetValue(inEventKey, out List<ListenFunc> funcs))
        {
            funcs = new List<ListenFunc>();
            listensDic[inEventKey] = funcs;
        }

        if (funcs.Contains(func))
        {
            Debug.LogError(inEventKey + " is have func" + func);
            return;
        }

        funcs.Add(func);
    }

    public void RemoveListen(BaseEvent baseEvent, ListenFunc func)
    {
        if (baseEvent == null || func == null)
            return;

        RemoveListen(baseEvent.EventKey, func);
    }

    public void RemoveListen(int inEventKey, ListenFunc func)
    {
        if (!listensDic.TryGetValue(inEventKey, out List<ListenFunc> funcs))
        {
            funcs = new List<ListenFunc>();
            listensDic[inEventKey] = funcs;
            return;
        }

        bool flag = funcs.Remove(func);

        if (!flag)
        {
            Debug.LogError(inEventKey + "remove func is fail" + func);

        }
    }


    public void SendEvent(BaseEvent inEvent)
    {
        if (listensDic.TryGetValue(inEvent.EventKey, out List<ListenFunc> funcs))
        {
            for (int i = 0; i < funcs.Count; i++)
            {
                if (funcs[i] != null)
                {
                    funcs[i].Invoke(inEvent);
                }
            }
        }
    }



}

/// <summary>
/// 事件基类TODO: 之后这里要池化
/// </summary>
public class BaseEvent
{
    public virtual int EventKey => -1;
}
