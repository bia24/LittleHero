using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    /// <summary>
    /// 普通泛型单例
    /// </summary>
    public class Singleton<T> where T : new()
    {
        private static T _instance = new T();

        /// <summary>
        /// 泛型T实例
        /// </summary>
        public static T Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// 受保护的构造方法，禁止被创建实例
        /// </summary>
        protected Singleton() { }


    }
}
