using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using UnityEngine.UI;
using DG.Tweening;

public class UIDarkPanel : UIBase
{
    Image image = null;

    public override void Show()
    {
        base.Show();
        image=GetComponent<Image>();
        image.DOFade(0,0);
        image.DOFade(1f, 1f).SetUpdate(UpdateType.Fixed,false).OnComplete(()=> {
            EventCenter.Instance.SendEvent(SGEventType.UIDarkPanelFinish, null); 
            image.DOFade(0, 1.5f).SetUpdate(UpdateType.Fixed, false).OnComplete(() =>
            {
                Hide(null);
            });
        });
    }
}
