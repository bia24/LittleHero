using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

namespace SGModule
{
    /// <summary>
    /// 以panel为单位的UI基类，挂载在panel上
    /// </summary>
    public class UIBase:MonoBehaviour
    {
        /// <summary>
        /// 面板出现/隐藏的形式
        /// </summary>
        protected enum ShowType
        {
            /// <summary>
            /// 普通
            /// </summary>
            Normal,
            /// <summary>
            /// 淡出
            /// </summary>
            Fade
        }

        /// <summary>
        /// panel上的控件名字索引 
        /// </summary>
        private Dictionary<string, GameObject> widgets = new Dictionary<string, GameObject>();

        /// <summary>
        /// 用于子物体的淡出效果
        /// </summary>
        private CanvasGroup group = null;

        /// <summary>
        /// 出现和消失的类型
        /// </summary>
        private ShowType showType=ShowType.Normal;

        /// <summary>
        /// 面板初始化
        /// </summary>
        public virtual void Init()
        {
            //递归获得所有子物体，加入集合中
            FindChildren(gameObject.transform, widgets);

            //给panel增加group组件
            group = gameObject.AddComponent<CanvasGroup>(); 
        }

        /// <summary>
        /// 递归查找所有子物体
        /// </summary>
        private void FindChildren(Transform parent,Dictionary<string,GameObject>outList)
        {
            if (parent.childCount == 0)
                return; 
            for(int i = 0; i < parent.childCount; i++)
            {
                GameObject child = parent.GetChild(i).gameObject;
                outList.Add(child.name, child);
                //递归查找
                FindChildren(child.transform, outList);
            }
        }

        /// <summary>
        /// 获取一个组件上的指定脚本：Button、Image等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected T GetWidget<T>(string name) where T:MonoBehaviour
        {
            GameObject widget = null;
            if (!widgets.TryGetValue(name, out widget))
                return null;
            return widget.GetComponent<T>();
        }

        /// <summary>
        /// 给ui添加交互事件，需要使用EventTrigger
        /// </summary>
        protected void AddUIEvent(string name, EventTriggerType type,UnityAction<BaseEventData> callback)
        {
            GameObject go = null;
            if(!widgets.TryGetValue(name,out go))
            {
                Debug.LogError(name + " ui gameobject not exist");
                return;
            }

            if (callback == null) return;

            EventTrigger ET = go.GetComponent<EventTrigger>();
            if (ET == null)
            {
               ET= go.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(callback);

            ET.triggers.Add(entry);
        }

        /// <summary>
        /// 面板显示出来的调用函数，可重载
        /// </summary>
        public virtual void  Show()
        {
            //依据显示类型进行显示
            gameObject.SetActive(true);
            switch (showType)
            {
                case ShowType.Normal:
                    break;
                case ShowType.Fade:
                    group.DOFade(0f, 0f);
                    group.DOFade(1f, 3f).SetUpdate(true);
                    break;
            }

            //在事件中心中 注册监听
            AddListenerToEventCenter();
        }

        /// <summary>
        /// 面板隐藏的调用函数
        /// </summary>
        /// <param name="action">由于隐藏可能存在的延迟性，可能需要完成后的回调函数，针对fade型隐藏使用</param>
        public virtual void Hide(UnityAction action)
        {
            //隐藏面板
            switch (showType)
            {
                case ShowType.Normal:
                    gameObject.SetActive(false);
                    action?.Invoke();
                    break;
                case ShowType.Fade:
                    group.DOFade(1f, 0f);
                    group.DOFade(0f, 1f).SetUpdate(true).OnComplete(()=>
                    {
                        action?.Invoke();
                        gameObject.SetActive(false);
                    }
                    );
                    break;
            }
          
            CancelListenerFromEventCenter();
        }

        public bool IsActiveInHierachy()
        {
            return gameObject.activeInHierarchy;
        }

        /// <summary>
        /// 向事件中心注册监听
        /// </summary>
        protected virtual void AddListenerToEventCenter()
        {
        }

        /// <summary>
        /// 取消事件中心的监听
        /// </summary>
        protected virtual void CancelListenerFromEventCenter()
        {
        }

    }
}
