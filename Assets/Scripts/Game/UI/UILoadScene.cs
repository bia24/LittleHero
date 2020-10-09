using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using DG.Tweening;
using UnityEngine.UI;

public class UILoadScene : UIBase
{
    public override void Show()
    {
        base.Show();

        //设置特效
        GetWidget("Handle").transform.DOScale(0.9f, 0f);
        GetWidget("Handle").transform.DOScale(1.05f, 0.2f).SetLoops(-1, LoopType.Yoyo);
        //初始化进度条
        GetWidget<Slider>("Slider").value = 0.0f;
    }

    protected override void AddListenerToEventCenter()
    {
        EventCenter.Instance.RegistListener(SGEventType.SceneLoadProcess, ShowProcessListener);
    }
    protected override void CancelListenerFromEventCenter()
    {
        EventCenter.Instance.RemoveListener(SGEventType.SceneLoadProcess, ShowProcessListener);
    }

    private void ShowProcessListener(EventData data)
    {
        float p = (float)data.Param;
        GetWidget<Slider>("Slider").value = Mathf.Clamp(p, 0.0f, 1.0f);
    }
}
