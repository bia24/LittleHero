using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace SGModule {
    /// <summary>
    /// 数据管理器。主要作用 ：
    /// 1. 从Resouces文件夹中获取初始化数据
    /// 2. 将数据存在本地persistent文件夹中
    /// 3. 缓存数据
    /// 4. 存储运行期间的数据，更新本地化数据
    /// </summary>
    public class DataManager :Singleton<DataManager>
    {
        /// <summary>
        /// 数据路径配置文件
        /// </summary>
        private static string dataPathConfigResourcePath = "DataPathConfig";
        /// <summary>
        /// 数据路径集合
        /// dataName-reousrcesPath
        /// </summary>
        private Dictionary<string, string> dataPaths = new Dictionary<string, string>();
        /// <summary>
        /// 数据缓存集合
        /// resourcesPath-dataContextString
        /// </summary>
        private Dictionary<string, string> dataCaches = new Dictionary<string, string>();

        /// <summary>
        /// 从加载器中缓存数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param"></param>
        public void AddDataCache(string url,string context)
        {
            string resourcesPath = PathUtils.GetResourcesPath(url);
            if (dataCaches.ContainsKey(resourcesPath))
                return;
        
            dataCaches.Add(resourcesPath, context);
        }

        public DataManager()
        {
            //初始化，从resources文件夹中读取数据名称-路径配置文件
            TextAsset t= AssetManager.instance.LoadObject<TextAsset>(dataPathConfigResourcePath);
            if (t == null)
            {
                Debug.LogError("DataPath Config load faild!");
                return;
            }

            DataPath d= JsonUtility.FromJson<DataPath>(t.text);
            foreach(DataPathEntity e in d.DataPathList)
            {
                dataPaths.Add(e.DataName, e.DataPath);
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public void LoadData(string dataName, LoadCompleteCallBack callback)
        {
            string path = null; //ResourcesPath 无扩展名
            if(!dataPaths.TryGetValue(dataName,out path))
            {
                Debug.LogError("load data failed! no exists data: " + dataName);
                return;
            }
            //回调函数不能是空的，必须要使用异步回调返回值
            if (callback == null)
            {
                Debug.LogError("LoadCompleteCallBack is null !");
                return;
            }

            //先去缓存区找
            string context = null;
            if(dataCaches.TryGetValue(path,out context))
            {

                callback.Invoke(context);
                return;
            }

            //缓存区不存在或者被清除，或者非第一次启动游戏(缓存区为空)
            //从本地文件夹中找

            //本地能找到
            string localPath = Application.persistentDataPath + "/" + path + ".txt";
            if (File.Exists(localPath))
            {
                //从persistent文件夹中用异步加载方式读取
                Loader l = LoaderManager.instance.GetLoader(LoaderManager.LoaderType.Async);
                LoaderInParam param = new LoaderInParam(localPath, callback);
                l.StartTask(param);
            }
            else //本地无法找到，从resources中加载初始数据
            {
                //从resources中同步读取
                Loader l = LoaderManager.instance.GetLoader(LoaderManager.LoaderType.Sync);
                l.StartTask(new LoaderInParam(path,callback)); //完成后，缓存中已经有数据

                //写入persistent文件夹
                string pathName = PathUtils.GetPathName(path);
                string fileName = PathUtils.GetFileName(path);
                StreamWriter wr = null;

                if (pathName.Equals(""))
                {
                    wr = new StreamWriter(Application.persistentDataPath + "/" + fileName + ".txt",false);
                    wr.Write(dataCaches[path]);
                }
                else
                {
                    Directory.CreateDirectory(Application.persistentDataPath + "/" + pathName);
                    wr = new StreamWriter(Application.persistentDataPath + "/" + path + ".txt",false);
                    wr.Write(dataCaches[path]);
                }

                wr.Dispose();
            }
        }

        /// <summary>
        /// 存储数据文件
        /// </summary>
        /// <param name="dataName"></param>
        /// <param name="data"></param>
        public void SaveData(string dataName,string data)
        {

            string resourcePath= null;
            if(!dataPaths.TryGetValue(dataName,out resourcePath))
            {
                Debug.LogError("save data failed! no exists data: " + dataName);
                return;
            }

            //因为重新保存到本地磁盘上，意味着缓存中的信息已过期，删除
            if (dataCaches.ContainsKey(resourcePath))
                dataCaches.Remove(resourcePath);

            //重新写入本地磁盘
            string localPath = Application.persistentDataPath + "/" + resourcePath + ".txt";
            string pathName = PathUtils.GetPathName(resourcePath);
            string fileName = PathUtils.GetFileName(resourcePath);

            if (!pathName.Equals("")&& !Directory.Exists(Application.persistentDataPath+"/"+pathName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + pathName);
            }

            StreamWriter wr = new StreamWriter(localPath, false);
            wr.Write(data);
            wr.Dispose();
            Debug.Log("Save data successed!");
        }


    }


}