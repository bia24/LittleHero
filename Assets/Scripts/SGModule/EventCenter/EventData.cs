using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    public class EventData
    {
        /// <summary>
        /// 空参数类
        /// </summary>
        public static EventData nullArg = new EventData(null, null);
        /// <summary>
        /// 参数封装
        /// </summary>
        public object Param { get; set; }
        /// <summary>
        /// 事件发送方
        /// </summary>
        public GameObject Sender { get; set; }

        public EventData(object param,GameObject sender)
        {
            Param = param;
            Sender = sender;
        }
         
    }
}
