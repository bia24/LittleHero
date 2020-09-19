using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule {
    /// <summary>
    /// 资源管理器，加载资源到内存中并缓存
    /// now：本框架中，除了配置文件需要从persistent文件夹中加载，其它所有资源都从Resources中直接加载。适合快速开发一款游戏
    /// todo：ab资源的加载，框架需要重新修改，针对资源加载这方面。异步下载，异步加载等
    /// </summary>
    public class AssetManager : Singleton<AssetManager>
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private Dictionary<string, Object> caches = new Dictionary<string, Object>();

        /// <summary>
        /// 加载GameObject，并实例化到场景中
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public GameObject LoadGameObject(string path)
        {
            Object o = null;
            GameObject go = null;
            if (caches.TryGetValue(path, out o))
            {
                go = o as GameObject;
            }
            else
            {
                go= Resources.Load<GameObject>(path);
                if (go == null)
                {
                    Debug.LogError("Null GameObject load");
                    return null;
                }
                caches.Add(path, go);
            }
            return GameObject.Instantiate(go);
        }

        /// <summary>
        /// 加载类型T的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadObject<T>(string path) where T:Object
        {
            Object o = null;
            T res = null;
            if (caches.TryGetValue(path, out o))
            {
                res = o as T;
            }
            else
            {
                res = Resources.Load<T>(path);
                if (res == null) {
                    Debug.LogError("Null Object load");
                    return null;
                }
                caches.Add(path, res);
            }
            return res;
        }

        /// <summary>
        /// 加载Object类型通用资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Object LoadObject(string path)
        {
            Object o = null;
            if (!caches.TryGetValue(path, out o))
            {
                o = Resources.Load(path);
                if (o == null)
                {
                    Debug.LogError("Null Object load");
                    return null;
                }
                caches.Add(path, o);
            }
            return o;
        }
        
        /// <summary>
        /// 场景切换时，unity会对除了加载进的ab包资源不清空，其它资源都清空
        /// 如果缓存一直有引用，会造成这些资源一直存在内存
        /// 因此，为了节约内存，在场景切换前，要清空缓存。
        /// </summary>
        public void ClearAssetsCaches()
        {
            caches.Clear();
        }

    }

}