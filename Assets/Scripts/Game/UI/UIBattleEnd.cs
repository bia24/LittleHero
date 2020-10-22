using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIBattleEnd : UIBase
{
    public override void Init()
    {
        base.Init();
        AddUIEvent("MainMenu", EventTriggerType.PointerClick, OnMainMenuClick);
        AddUIEvent("Exits", EventTriggerType.PointerClick, OnExitClick);

    }


    public void ShowText(bool isWin,string context)
    {
        GetWidget<Text>("TextTip").text = context;
        GetWidget<Text>("TextTip").color = isWin ? Color.green : Color.red;
    }

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
}
