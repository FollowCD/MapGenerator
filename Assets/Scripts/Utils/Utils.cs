using System;
using System.Collections;
using System.Collections.Generic;
using NSEvent;
using UnityEngine;
using Event = NSEvent.Event;
using Random = UnityEngine.Random;

public class Utils
{
    private static EventTrigger _EventMgr = null;
    public static bool Init()
    {
        if (_EventMgr == null)
        {
            _EventMgr = new EventTrigger();
        }
        else
        {
            _EventMgr.Reset();
        }
        return true;
    }

    public static void CleanUp()
    {
        if (_EventMgr!=null)
        {
            _EventMgr.Dispose();
            _EventMgr = null;
        }
    }

    public static void ListenEvent(EvtID id, EventAction action)
    {
        if (_EventMgr!=null)
        {
            _EventMgr.ListenEvent(id,action);
        }
    }

    public static void RemoveEvent(EvtID id, EventAction action)
    {
        if (_EventMgr != null)
        {
            _EventMgr.RemoveEvent(id,action);
        }
    }

    public static void TriggerEvent(EvtID id, object param)
    {
        if (_EventMgr != null)
        {
            _EventMgr.Trigger(id,param);
        }
    }

    public static void UpdateFrame()
    {
        if (_EventMgr!=null)
        {
            _EventMgr.DispatchEvent();
        }
    }

    public static T PickRandom<T>(T[] array)
    {
        if (array == null)
        {
            return default(T);
        }

        if (array.Length == 1)
        {
            return array[0];
        }

        int idx = Random.Range(0, array.Length);
        return array[idx];
    }

    public static bool GetRandChance(float numerator, float denominator)
    {
        float random = Random.Range(0.0f, 1.0f);
        float result = numerator / denominator;
        return random < result;
    }

    public static string MakeName(string firstname, string nickname, string lastname)
    {
        string name = firstname;
        if (!string.IsNullOrEmpty(nickname))
        {
            name += "·";
            name += nickname;
        }

        if (!string.IsNullOrEmpty(lastname))
        {
            name += "·";
            name += lastname;
        }

        return name;
    }
}
