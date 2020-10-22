using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0618
namespace SGModule
{
    /// <summary>
    /// 异步加载器
    /// </summary>
    public class AsyncLoader : Loader
    {
        /// <summary>
        /// 下载器引用
        /// </summary>
        private WWW www;
       

        public override void StartTask(LoaderInParam param)
        {
            base.StartTask(param);
            StartCoroutine("Task", param);
        }

        //异步加载器输入路径为Application.persistent/ResourcesPath+".txt"
        IEnumerator Task(object param)
        {
            LoaderInParam p = param as LoaderInParam;

            www = new WWW(p.Url);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log("asyncloading from "+p.Url+" faild , error mesage : "+www.error);
                yield break;
            }


            //回调函数执行
            p.Callback.Invoke(www.text);
            //向数据管理器中加入缓存
            DataManager.Instance.AddDataCache(p.Url, www.text);
            //下载器完成任务
            FinishTask();
        }
    }

}
