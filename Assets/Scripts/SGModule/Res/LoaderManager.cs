using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule {

    /// <summary>
    /// 加载器的管理器
    /// </summary>
    public class LoaderManager : SingletonMono<LoaderManager>
    {
        /// <summary>
        /// 挂载下载器的空物体
        /// </summary>
        private GameObject loaderGo;
        /// <summary>
        /// 当前正在执行任务的下载器数目
        /// </summary>
        public int LoaderCount { get; set; }
       
        public enum LoaderType
        {
            Sync, // 同步
            Async //异步
        }

        private void Awake()
        {
            //init
            loaderGo = new GameObject("Loaders");
            DontDestroyOnLoad(loaderGo);
        }



        public Loader GetLoader(LoaderType type = LoaderType.Async)
        {
            Loader loader = null;
            //同步下载器
            if (type == LoaderType.Sync)
            {
                loader = loaderGo.AddComponent<SyncLoader>();
            }
            //异步下载器
            else
            {
                loader = loaderGo.AddComponent<AsyncLoader>();               
            }

            loader.Init();
            return loader;
        }
   

        public bool EmptyLoadTask()
        {
            return LoaderCount == 0 ? true : false;
        }
    }

}
