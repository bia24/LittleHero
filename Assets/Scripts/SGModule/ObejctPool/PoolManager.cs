using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    /// <summary>
    /// 对象池管理器
    /// now：仅支持从resources中加载对象
    /// todo：支持从AB包中加载
    /// </summary>
    public class PoolManager:Singleton<PoolManager>
    {
        /// <summary>
        /// 对象名称-池子索引
        /// </summary>
        private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();
       
        /// <summary>
        /// 从对象池中获取一个对象
        /// </summary>
        /// <param name="resourcesName"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public GameObject getPrefab(string resourcesName,string rename,Transform parent=null,bool stayWorldPos=false)
        {
            GameObject go = null;
            if (hasPool(resourcesName)) //池子已经拥有
            {
                if (pools[resourcesName].Empty())
                {
                    //池子存在，但是没有对象可以使用了，实例化一个对象
                    go = AssetManager.Instance.LoadGameObject(resourcesName);
                    if (go == null)
                    {
                        Debug.LogError("prefab not exists in resources : "+resourcesName);
                        return null;
                    }
                }
                else 
                    go = pools[resourcesName].getObject();
            }
            else //池子不存在
            {
                go= AssetManager.Instance.LoadGameObject(resourcesName);
                if (go == null)
                {
                    Debug.LogError("prefab not exists in resources : " + resourcesName);
                    return null;
                }
                //初始化池子
                pools.Add(resourcesName, new Pool(go.transform));
            }
            //对获得的对象进行transform初始化，赋值为prefab中的数据
            if (parent != null)
            {
                go.transform.SetParent(parent, stayWorldPos);
            }
            go.transform.localPosition = pools[resourcesName].InitPos;
            go.transform.localRotation = pools[resourcesName].InitRot;
            go.transform.localScale = pools[resourcesName].InitScale;

            go.SetActive(true);
            go.name = rename;
            return go;
        }

        /// <summary>
        /// 将一个对象返回到一个对象池中
        /// </summary>
        /// <param name="name"></param>
        /// <param name="go"></param>
        public void RevertPool(string resourcesName, GameObject go)
        {
            if(!hasPool(resourcesName))
            {
                Debug.LogError("not has this pool , create first");
                return;
            }
            go.SetActive(false);
            pools[resourcesName].addObejct(go);
        }

        /// <summary>
        /// 清空对象池。
        /// 在场景切换时，无法避免要destroy对象。因此为了避免空指针，
        /// 在切换前，清空对象池
        /// </summary>
        public void ClearPool()
        {
            pools.Clear();
        }

        private bool hasPool(string name)
        {
            return pools.ContainsKey(name);
        }
    }
}
