using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
 
namespace SGModule
{
    /// <summary>
    /// ui管理器单例。
    /// 1. 提供UI的加载，UI的索引字典
    /// 2. 向外提供接口，指定UI面板的show和hide。逻辑的话是业务层，应该再独立在上层，负责事件逻辑绑定。
    /// 3. 存储UI摆放的参数信息，单例供访问
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        /// <summary>
        /// UI预制体在Resources文件夹中的路径
        /// </summary>
        private static string prefabResoucePath = "UIPrefabs";
        /// <summary>
        /// UIGameObject预制体名称
        /// </summary>
        private static string UIRootObjectName = "UI";

        /// <summary>
        /// 所有Panel的索引
        /// </summary>
        private Dictionary<string, UIBase> panels = new Dictionary<string, UIBase>();

        /// <summary>
        /// UI显示的层级，放置在哪个预设好的位置
        /// </summary>
        public enum UILayer
        {
            Bottom,
            Mid,
            Top,
            /// <summary>
            /// 3D等非UI物体摆放层
            /// </summary>
            ThreeD
        }

        /// <summary>
        /// UI根引用
        /// </summary>
        private GameObject UIRootObject { get; }   //只读属性，只能在构造函数中进行初始化
        /// <summary>
        /// Canvas引用
        /// </summary>
        private Transform UICanvas { get; }
        /// <summary>
        /// 底层位置引用
        /// </summary>
        private Transform Bot { get; }
        /// <summary>
        /// 中间层位置，默认位置引用
        /// </summary>
        private Transform Mid { get; }
        /// <summary>
        /// 最上层位置引用
        /// </summary>
        private Transform Top { get; }
        /// <summary>
        /// 3D等物体位置引用
        /// </summary>
        private Transform ThreeD { get; }
        /// <summary>
        /// UI缩放器
        /// </summary>
        private CanvasScaler UIScaler { get; }
        /// <summary>
        /// 播放UI中的声音源source
        /// </summary>
        public AudioSource Source { get; }

        /// <summary>
        ///  当前屏幕的分辨率，只读属性，不会自动赋值
        /// </summary>
        public Vector2 CurrentScreenResolution => new Vector2(Screen.width, Screen.height);
        /// <summary>
        ///  参考分辨率
        /// </summary>
        public Vector2 RefScreenResolution => UIScaler.referenceResolution;

       
        public UIManager()
        {
            UIRootObject = AssetManager.Instance.LoadGameObject(prefabResoucePath+"/"+UIRootObjectName);
            UIRootObject.name = UIRootObjectName;
            GameObject.DontDestroyOnLoad(UIRootObject);

            UICanvas = UIRootObject.transform.Find("CanvasRoot");
            Bot = UICanvas.Find("Bot");
            Mid = UICanvas.Find("Mid");
            Top = UICanvas.Find("Top");
            ThreeD = UICanvas.Find("ThreeD");

            UIScaler = UICanvas.GetComponent<CanvasScaler>();
            Source = UIRootObject.GetComponent<AudioSource>();
            if (Source == null)
                Source = UIRootObject.AddComponent<AudioSource>();

            Screen.SetResolution((int)RefScreenResolution.x, (int)RefScreenResolution.y,false);

        }

        /// <summary>
        /// 显示一个UI面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public T ShowPanel<T>(string name,UILayer layer)where T:UIBase
        {
            UIBase p = null;
            if(!panels.TryGetValue(name,out p))
            {
                //还未加载过，从Resources中加载prefab
                string path = prefabResoucePath + "/" + name;
                //用Asset加载，不使用对象池
                GameObject go=AssetManager.Instance.LoadGameObject(path);
                go.name = name;
                p = go.AddComponent<T>();

                switch (layer)
                {
                    case UILayer.Bottom:
                        go.transform.SetParent(Bot, false);
                        break;
                    case UILayer.Mid:
                        go.transform.SetParent(Mid, false);
                        break;
                    case UILayer.Top:
                        go.transform.SetParent(Top, false);
                        break;
                    case UILayer.ThreeD:
                        go.transform.SetParent(ThreeD, false);
                        break;
                }
                panels.Add(name, p);
                p.Init();
            }

            p.Show();
            //设置该面板为最“上层”显示
            p.transform.SetAsLastSibling();

            return p as T;
        }

        /// <summary>
        /// 隐藏一个面板
        /// </summary>
        /// <param name="name"></param>
        public void HidePanel(string name, UnityAction callback = null)
        {
            UIBase p = null;
            if (!panels.TryGetValue(name, out p))
                return;
            if (!p.IsActiveInHierachy())
                return;
            p.Hide(callback);
        }

        /// <summary>
        /// 获取一个挂载有UIBase的panel，必须是已经加载过的，不然返回空
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetPanel<T>(string name) where T : UIBase
        {
            UIBase p = null;
            panels.TryGetValue(name, out p);
            return p as T;
        }
        /// <summary>
        /// 从Resources文件夹中实例一个UI预制体
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject InstantiateUIPrefab(string name)
        {
            GameObject go = AssetManager.Instance.LoadGameObject(prefabResoucePath+"/"+name);
            if (go != null)
                go.name = name;
            return go;
        }

    }
}
