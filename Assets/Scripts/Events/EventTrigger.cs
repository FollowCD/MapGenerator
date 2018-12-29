using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSEvent
{
    public class Event
    {
        public Event(EvtID id, object parm)
        {
            _Id = id;
            _Param = parm;
        }

        private EvtID _Id;
        private object _Param;

        public EvtID ID
        {
            get { return _Id; }
        }

        public object Param
        {
            set { _Param = value; }
            get { return _Param; }
        }
    }
    public delegate void EventAction(object param);

    public class EventTrigger:IDisposable
    {
        private Dictionary<EvtID, EventAction> _Listeners = new Dictionary<EvtID, EventAction>();
        private Queue<Event> _EventPool = new Queue<Event>();

        public void Reset()
        {
            _Listeners.Clear();
            _EventPool.Clear();
        }

        public void ListenEvent(EvtID id, EventAction action)
        {
            if (_Listeners.ContainsKey(id))
            {
                _Listeners[id] += action; 
            }
            else
            {
                _Listeners.Add(id,new EventAction(action));
            }
        }

        public void RemoveEvent(EvtID id, EventAction action)
        {
            if (_Listeners.ContainsKey(id))
            {
                _Listeners[id] -= action;
            }
        }

        public void Dispose()
        {
            this.Reset();
        }

        public void Trigger(EvtID id, object param)
        {
            Event evt = new Event(id,param);
            _EventPool.Enqueue(evt);
        }

        public void DispatchEvent()
        {
            if (_EventPool.Count > 0)
            {
                Event evt = _EventPool.Dequeue();

                if (_Listeners.ContainsKey(evt.ID))
                {
                    _Listeners[evt.ID].Invoke(evt.Param);
                }
            }
        }
    }
}
