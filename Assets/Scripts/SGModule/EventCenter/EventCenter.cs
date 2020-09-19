using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    /// <summary>
    /// 消息中心
    /// </summary>
    public class EventCenter:Singleton<EventCenter>
    {
        /// <summary>
        /// 事件中心回调函数索引
        /// </summary>
        private Dictionary<SGEventType, EventCallBack> callBacks   = new Dictionary<SGEventType, EventCallBack>();

        /// <summary>
        /// 事件监听注册
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void RegistListener(SGEventType type,EventCallBack callback)
        {
            if(callback==null)
            {
                Debug.LogWarning("call back is null");
                return;
            }

            if (!callBacks.ContainsKey(type))
            {
                //不存在指定事件的key，创建新key-value
                callBacks.Add(type, callback);
            }
            else
            {
                //存在指定事件的key，不能用其他变量存委托，
                //然后+=，因为委托非原地+，会导致字典中的委托没变化
                callBacks[type] += callback;
            }           
        }

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void SendEvent(SGEventType type,EventData data)
        {
            EventCallBack callback = null;

            if (callBacks.TryGetValue(type,out callback))
            {
                callback?.Invoke(data);
            }
        }

        /// <summary>
        /// 删除指定事件类型中的监听回调 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void RemoveListener(SGEventType type,EventCallBack callback)
        {
            if (callBacks.ContainsKey(type))
            {
                if (callBacks[type] == null)
                    return;
                callBacks[type] -= callback;
            }
        }

        /// <summary>
        /// 删除指定事件的所有监听回调
        /// </summary>
        /// <param name="type"></param>
        public void RemoveEvent(SGEventType type)
        {
            if (callBacks.ContainsKey(type))
            {
                callBacks.Remove(type);
            }
        }
    }
}
