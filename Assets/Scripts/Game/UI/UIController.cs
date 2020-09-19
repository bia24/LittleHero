using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
/// <summary>
/// 控制UI “显示”的事件回调绑定
/// 总是需要这么一个地方用来绑定显示回调的，回调用调用UIManager中的show方法。
/// 不然无法“显示”，只有“显示”一个panel才能初始化它
/// </summary>
public class UIController:Singleton<UIController>
{
   public void Init()
    {
        EventCenter.Instance.RegistListener(SGEventType.UIGameStartPanel,ShowGameStartPanelListener);
        EventCenter.Instance.RegistListener(SGEventType.UIGameSettings, ShowGameSettingsListener);
        EventCenter.Instance.RegistListener(SGEventType.UILoadingPanel, ShowLoadingPanelListener);
        EventCenter.Instance.RegistListener(SGEventType.UIWarnningPanel, ShowWarnningPanelListener);
        
    }

    /// <summary>
    /// 获得UI的默认AudioSource
    /// </summary>
    /// <returns></returns>
    public AudioSource GetAudioSource()
    {
        return UIManager.Instance.Source;
    }

    /// <summary>
    /// 游戏开始面板唤出
    /// </summary>
    /// <param name="data"></param>
    private void ShowGameStartPanelListener(EventData data)
    {
        UIManager.Instance.ShowPanel<UIStartGame>("GameStartPanel", UIManager.UILayer.Bottom);
    }

    /// <summary>
    /// 游戏设置面板唤出
    /// </summary>
    /// <param name="data"></param>
    private void ShowGameSettingsListener(EventData data)
    {
        UIManager.Instance.ShowPanel<UIGameSettings>("GameSettingsPanel", UIManager.UILayer.Bottom);
    }

    /// <summary>
    /// 加载界面被唤出
    /// </summary>
    /// <param name="data"></param>
    private void ShowLoadingPanelListener(EventData data)
    {
        UIManager.Instance.ShowPanel<UILoading>("LoadingPanel", UIManager.UILayer.Top);
    }

    /// <summary>
    /// 警告界面被唤出
    /// </summary>
    /// <param name="data"></param>
    private void ShowWarnningPanelListener(EventData data)
    {
        UIManager.Instance.ShowPanel<UIWarnning>("WarnningPanel",UIManager.UILayer.Top);
        UIManager.Instance.GetPanel<UIWarnning>("WarnningPanel").SetWarnningContext(data.Param as string);
    }


}


