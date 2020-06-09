using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    /// <summary>
    /// 同步下载器，本质上是一个空类
    /// </summary>
    public class SyncLoader : Loader
    {
       
        public override void StartTask(LoaderInParam param)
        {
            Debug.Log("Start SyncLoading asset from " + param.Url);
            //从resources文件夹中加载
            string context = AssetManager.instance.LoadObject<TextAsset>(param.Url).text;
            if (context == null)
            {
                Debug.LogError("syncloading from "+param.Url+" failed!!");
                return;
            }
            Debug.Log("SyncLoading asset finished ");

            //调用回调函数
            param.Callback.Invoke(context);
            //向数据管理器中加入缓存
            DataManager.instance.AddDataCache(param.Url,context );
            //本下载器任务完成
            FinishTask();
            
        }

    }
}
