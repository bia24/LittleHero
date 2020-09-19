using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using DG.Tweening;
using UnityEngine.UI;
using System.Text;

/// <summary>
/// 加载界面
/// </summary>
public class UILoading : UIBase
{
    /// <summary>
    /// 面板显示调用函数
    /// </summary>
    public override void Show()
    {
        base.Show();

        //LoadingIcon 闪烁特效
        GetWidget("LoadingIcon").transform.DOScale(1.05f, 0.3f).SetLoops(-1, LoopType.Yoyo);

    }

    private void Update()
    {
        //Loading...
        int n = Time.frameCount / 30 % 3 ;
        StringBuilder sb = new StringBuilder("Loading");
        switch (n)
        {
            case 0:sb.Append(" ."); break;
            case 1: sb.Append(" . ."); break;
            case 2: sb.Append(" . . ."); break;
        }
        GetWidget<Text>("LoadingText").text = sb.ToString();

        CheckLoadTask();//检测loading任务，完成发送触发事件
   
    }

    /// <summary>
    /// 检测当前loading任务是否已经完成
    /// </summary>
    private void CheckLoadTask()
    {
        //关闭本界面
        if (LoaderManager.Instance.EmptyLoadTask())
        {
            Hide(null);
            EventCenter.Instance.SendEvent(SGEventType.LoadingFinish, null);
        }
    }

    
}
