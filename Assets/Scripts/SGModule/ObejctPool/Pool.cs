using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    /// <summary>
    /// 对象池
    /// </summary>
    public class Pool
    {
        private List<GameObject> list = new List<GameObject>();
        /// <summary>
        /// 用作初始化对象transform的记录
        /// </summary>
        public Vector3 InitPos { get; set; }
        public Quaternion InitRot { get; set; }
        public Vector3 InitScale { get; set; }

        public Pool(Transform initTransform) {
            //初始化
            InitPos = initTransform.localPosition;
            InitRot = initTransform.localRotation;
            InitScale = initTransform.localScale;
        }

        public GameObject getObject()
        {
            if (Empty()) { Debug.LogError("null pool"); return null; }
            GameObject go = list[0];
            list.RemoveAt(0);
            return go;
        }

        public void addObejct(GameObject obj)
        {
            list.Add(obj);
        }

        public bool Empty()
        {
            return list.Count <= 0 ? true:false;
        }
    }
}
