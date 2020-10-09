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
        EventCenter.Instance.RegistListener(SGEventType.UICharacterChoose, ShowCharacterChooseListener);
        EventCenter.Instance.RegistListener(SGEventType.UILoadScenePanel, ShowLoadScenePanelListener);
        EventCenter.Instance.RegistListener(SGEventType.UILoadScenePanelHide, LoadScenePanelHideListener);
        EventCenter.Instance.RegistListener(SGEventType.UIBattlePanel, ShowBattlePanelListener);
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
        bool allInit = (bool)data.Param;
        //游戏开始界面唤出，表示游戏重新或刚开始，初始化游戏运行数据
        EventCenter.Instance.SendEvent(SGEventType.GameRuntimeDataInit, new EventData(allInit,null));
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

    /// <summary>
    /// 角色选择面板 回调
    /// </summary>
    private void ShowCharacterChooseListener(EventData data)
    {
        UIManager.Instance.ShowPanel<UICharacterChoose>("CharacterChoose", UIManager.UILayer.Bottom);
    }

    /// <summary>
    /// 场景切换界面唤出 监听
    /// </summary>
    /// <param name="data"></param>
    private void ShowLoadScenePanelListener(EventData data)
    {
        UIManager.Instance.ShowPanel<UILoadScene>("LoadScenePanel",UIManager.UILayer.Top);
    }
    /// <summary>
    /// 场景切换界面隐藏
    /// </summary>
    /// <param name="data"></param>
    private void LoadScenePanelHideListener(EventData data)
    {
        UIManager.Instance.HidePanel("LoadScenePanel", null);
    }
    /// <summary>
    /// 战斗界面 唤出 监听
    /// </summary>
    private void ShowBattlePanelListener(EventData data)
    {
        UIManager.Instance.ShowPanel<UIBattlePanel>("BattlePanel",UIManager.UILayer.Mid);
    }
}


