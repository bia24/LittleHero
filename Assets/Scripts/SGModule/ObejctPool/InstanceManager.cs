using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SGModule
{
    public class InstanceManager :Singleton<InstanceManager>
    {
        /// <summary>
        /// 脚本实例对象池
        /// </summary>
        private Dictionary<Type, List<object>> pool = new Dictionary<Type, List<object>>();

        public T GetInstance<T>(Type type) where T:class
        {
            List<object> res = null;
            if(!pool.TryGetValue(type,out res)) //没有该key
            {
                pool.Add(type, new List<object>());
                res = pool[type];
            }

            if(res.Count==0)//没有实例对象了
            {
                res.Add(Activator.CreateInstance(type));
            }

            object instance = res[0];
            res.Remove(0);

            return instance as T;
        }

        public void Revert(Type type,object o)
        {
            List<object> res = null;
            if (!pool.TryGetValue(type, out res)) //没有该key
            {
                Debug.LogError("can not find this instance pool : "+type.ToString());
            }

            res.Add(o);
        }

        public void Clear()
        {
            pool.Clear();
        }
    }
}
