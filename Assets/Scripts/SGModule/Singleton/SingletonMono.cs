using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SGModule
{
    /// <summary>
    /// 泛型单例类。子类继承自Mono，默认挂载在GoMgr上
    /// </summary>
    public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        private static T _instance = null;

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = GameObject.Find("SingletonMgr");
                    if (go == null)
                    {
                        go = new GameObject("SingletonMgr");
                        GameObject.DontDestroyOnLoad(go);
                    }
                    _instance = go.AddComponent<T>();
                }
                return _instance;
            }
        }
    }

}