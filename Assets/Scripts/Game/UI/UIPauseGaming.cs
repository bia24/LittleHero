using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using UnityEngine.EventSystems;

public class UIPauseGaming : UIBase
{
    /// <summary>
    /// 重载初始化方法
    /// </summary>
    public override void Init()
    {
        base.Init();

        AddUIEvent("GameSetting", EventTriggerType.PointerClick, OnGameSettingClick);
        AddUIEvent("MainMenu", EventTriggerType.PointerClick, OnMainMenuClick);
        AddUIEvent("Goon", EventTriggerType.PointerClick, OnPlayClick);
        AddUIEvent("Exits", EventTriggerType.PointerClick, OnExitClick);
        AddUIEvent("Cancel", EventTriggerType.PointerClick, OnCancelClick);
    }

    /// <summary>
    /// 游戏中点击了游戏设置按钮
    /// </summary>
    /// <param name="data"></param>
    private void OnGameSettingClick( BaseEventData data)
    {
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
           new SoundParam(UIManager.Instance.Source, "Click02"), null));
        //唤出游戏中的游戏设置菜单
        EventCenter.Instance.SendEvent(SGEventType.UIGameSettingOnGaming, null);
    }
    /// <summary>
    /// 游戏中点击了主菜单按钮
    /// </summary>
    /// <param name="data"></param>
    private void OnMainMenuClick(BaseEventData data)
    {
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
           new SoundParam(UIManager.Instance.Source, "Click02"), null));
        //隐藏自己
        Hide(null);
        //战场管理器退出工作
        BattleController.Instance.BattleExit();
        //通知切换场景
        EventCenter.Instance.SendEvent(SGEventType.SceneLoad, new EventData("GameStart", null));
    }
    /// <summary>
    /// 继续游戏点击
    /// </summary>
    /// <param name="data"></param>
    private void OnPlayClick(BaseEventData data)
    {
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
           new SoundParam(UIManager.Instance.Source, "Click02"), null));
        //隐藏自己
        Hide(null);
        //发出恢复游戏通知
        EventCenter.Instance.SendEvent(SGEventType.BattlePauseExit, null);

    }
    /// <summary>
    /// 退出游戏点击
    /// </summary>
    /// <param name="data"></param>
    private void OnExitClick(BaseEventData data)
    {
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
           new SoundParam(UIManager.Instance.Source, "Click02"), null));
        //隐藏自己
        Hide(null);
        //战场管理器退出工作
        BattleController.Instance.BattleExit();
        //应用程序退出
        Application.Quit();
    }
    /// <summary>
    /// 退出暂停面板按钮点击
    /// </summary>
    /// <param name="data"></param>
    private void OnCancelClick(BaseEventData data)
    {
        //隐藏自己
        Hide(null);
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
           new SoundParam(UIManager.Instance.Source, "Click02"), null));
        //发出恢复游戏通知
        EventCenter.Instance.SendEvent(SGEventType.BattlePauseExit,null);
    }

}
