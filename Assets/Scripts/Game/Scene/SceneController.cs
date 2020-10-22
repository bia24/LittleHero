using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class SceneController : Singleton<SceneController>
{
    /// <summary>
    /// 场景业务逻辑初始化
    /// </summary>
    public void Init()
    {
        //注册
        EventCenter.Instance.RegistListener(SGEventType.SceneLoad, SceneLoadListener);
        EventCenter.Instance.RegistListener(SGEventType.SceneLoadDone, SceneLoadDoneListener);
    }
    /// <summary>
    /// 场景加载 监听
    /// </summary>
    private void SceneLoadListener(EventData data)
    {
        string name = data.Param as string;
        //先清除缓存
        GameController.Instance.ClearCache();
        //通知ui唤出加载界面
        EventCenter.Instance.SendEvent(SGEventType.UILoadScenePanel, null);
        //通过场景管理器 开启场景加载 逻辑
        SceneManager.Instance.StartLoadScene(name);
    }

    /// <summary>
    /// 场景加载完成监听
    /// </summary>
    /// <param name="data"></param>
   private void SceneLoadDoneListener(EventData data)
    {
        string name = data.Param as string;
        //通知加载界面隐藏
        EventCenter.Instance.SendEvent(SGEventType.UILoadScenePanelHide,null);
    }
}
