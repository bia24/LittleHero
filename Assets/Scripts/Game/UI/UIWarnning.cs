using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 警示ui面板
/// </summary>
public class UIWarnning : UIBase
{
    public override void Init()
    {
        base.Init();

        AddUIEvent("WarnningPanel", EventTriggerType.PointerClick, OnWarnningPanelClick);
        AddUIEvent("WarnningBG", EventTriggerType.PointerClick, OnWarnningPanelClick);
        AddUIEvent("Board", EventTriggerType.PointerClick, OnWarnningPanelClick);
    }
    /// <summary>
    /// 设置警告内容
    /// </summary>
    /// <param name="context"></param>
    public void SetWarnningContext(string context)
    {
        GetWidget<Text>("WarnningText").text = context;
    }

    private void OnWarnningPanelClick(BaseEventData data)
    {
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
           new SoundParam(UIManager.Instance.Source, "Click03"), null));
        Hide(null);
    }
}
