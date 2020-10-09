using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    public class EventData
    {
        /// <summary>
        /// 参数封装
        /// </summary>
        public object Param { get; set; }
        /// <summary>
        /// 事件发送方
        /// </summary>
        public GameObject Sender { get; set; }
        /// <summary>
        /// 参数2封装
        /// </summary>
        public object Param2 { get; set; }

        public EventData(object param,GameObject sender,object param2=null)
        {
            Param = param;
            Sender = sender;
            Param2 = param2;
        }
         
    }
}
