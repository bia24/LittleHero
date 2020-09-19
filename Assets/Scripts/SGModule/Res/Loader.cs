using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    /// <summary>
    /// 下载器基类
    /// </summary>
    public abstract class Loader : MonoBehaviour
    {
       
        /// <summary>
        /// 下载器开启任务
        /// </summary>
        /// <param name="url"></param>
        public virtual void StartTask(LoaderInParam param)
        {
            LoaderManager.Instance.LoaderCount++;
        }

        /// <summary>
        /// 下载器初始化
        /// </summary>
        public virtual void Init() { }

       
        /// <summary>
        /// 下载器完成任务
        /// </summary>
        public virtual void FinishTask()
        {
            Destroy(this);
            LoaderManager.Instance.LoaderCount--;
        }
        
    }
}
