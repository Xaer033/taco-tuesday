using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GhostGen
{
    public class NotificationDispatcher  
    {
        private Dictionary<string, List<Action<Hashtable>>> _eventDictionary;
   
        public NotificationDispatcher()
        {
            _eventDictionary = new Dictionary<string, List<Action<Hashtable>>>();
        }

        public void AddListener(string eventKey, Action<Hashtable> callback)
        {
            Assert.IsNotNull(callback);
            if (callback == null) { return; }

            List<Action<Hashtable>> callbackList = null;
            if (!_eventDictionary.TryGetValue(eventKey, out callbackList))
            {
                callbackList = new List<Action<Hashtable>>();
                _eventDictionary.Add(eventKey, callbackList);
            }
            callbackList.Add(callback);
        }

        public void RemoveListener(string eventKey, Action<Hashtable> callback)
        {
            Assert.IsNotNull(callback);
            if (callback == null) { return; }

            List<Action<Hashtable>> callbackList = null;
            if (_eventDictionary.TryGetValue(eventKey, out callbackList))
            {
                int index = callbackList.FindIndex((x) => x == callback);
                callbackList.RemoveAt(index);
            }
        }

        public void RemoveAllListeners(string eventKey)
        {
            List<Action<Hashtable>> callbackList = null;
            if (_eventDictionary.TryGetValue(eventKey, out callbackList))
            {
                callbackList.Clear();
            }
        }

        public void DispatchEvent(string eventKey, Hashtable eventData = null)
        {
            List<Action<Hashtable>> callbackList = null;
            if (_eventDictionary.TryGetValue(eventKey, out callbackList))
            {
                int length = callbackList.Count;
                for(int i = 0; i < length; ++i)
                {
                    callbackList[i].Invoke(eventData);
                }
            }
        }
    }
}