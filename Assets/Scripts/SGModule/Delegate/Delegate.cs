using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SGModule
{
    /// <summary>
    /// 事件中心通用回调函数
    /// </summary>
    /// <param name="eventData"></param>
    public delegate void EventCallBack(EventData eventData);

    /// <summary>
    /// 加载器完成任务时的回调函数
    /// </summary>
    /// <param name="loadData"></param>
    public delegate void LoadCompleteCallBack(string  context);
    
}