using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// 角色选择界面加载
/// </summary>
public class UICharacterChoose : UIBase
{

    public override void Init()
    {
        base.Init();

        //UI控件事件绑定
        AddUIEvent("StartButton", EventTriggerType.PointerClick, OnStartButtonClick);
        AddUIEvent("Cancel", EventTriggerType.PointerClick, OnCancelClick);
        AddUIEvent("Tips", EventTriggerType.PointerClick, OnTipsClick);
        AddUIEvent("Tips_Player2", EventTriggerType.PointerClick, OnTipsClick);
        
    }

    public override void Show()
    {
        base.Show();

        //按钮闪烁特效
        GetWidget("Tips").transform.DOScale(1, 0f);
        GetWidget("Tips").transform.DOScale(1.05f, 0.3f).SetLoops(-1, LoopType.Yoyo).SetId("Tips");
        GetWidget<Text>("Tips").DOColor (new Color(0.9811f, 0.6621f, 0.0509f, 1f),0f);
        GetWidget<Text>("Tips").DOColor(new Color(0.7f, 0.24f, 0, 0.66f), 0.3f).SetLoops(-1, LoopType.Yoyo).SetId("TipsText"); ;

        GetWidget("Tips_Player2").SetActive(true);
        GetWidget("Tips_Player2").transform.DOScale(1, 0f);
        GetWidget("Tips_Player2").transform.DOScale(1.05f, 0.3f).SetLoops(-1, LoopType.Yoyo).SetId("TipsPlayer2"); ;
        GetWidget<Text>("Tips_Player2").DOColor(new Color(0.9811f, 0.6621f, 0.0509f, 1f), 0f);
        GetWidget<Text>("Tips_Player2").DOColor(new Color(0.7f, 0.24f, 0, 0.66f), 0.3f).SetLoops(-1, LoopType.Yoyo).SetId("TipsPlayer2Text"); ;

        //初始化选择面板
        InitCharaterChoosedUI();
    }

    protected override void AddListenerToEventCenter()
    {
        EventCenter.Instance.RegistListener(SGEventType.PlayerChooseCharacterChange, SetCharacterChoosedUI);
    }

    protected override void CancelListenerFromEventCenter()
    {
        EventCenter.Instance.RemoveListener(SGEventType.PlayerChooseCharacterChange, SetCharacterChoosedUI);
    }

    /// <summary>
    /// 初始化角色已选择UI
    /// </summary>
    private void InitCharaterChoosedUI()
    {
        int pn = GameController.Instance.GetPlayerNumber();
        if (pn < 2) 
        {
            GetWidget("Tips_Player2").SetActive(false); //单人模式，关闭角色2的更换按钮
        }
        //设置两个玩家的选择UI
        SetCharacterChoosedUI(new EventData(0, null));
        SetCharacterChoosedUI(new EventData(1, null));
    }

    /// <summary>
    /// 设置玩家已经选择UI
    /// </summary>
    /// <param name="data"></param>
    private void SetCharacterChoosedUI(EventData data)
    {
        int playerId = (int)data.Param; //获得玩家id
        CharacterUI c= GameController.Instance.GetCharacterUIInfo(playerId);
        Sprite s = TextureManager.Instance.GetTexture<Sprite>(c.iconName);
        if (playerId == 0)
        {
            GetWidget<Image>("HeadIcon").sprite = s;
            GetWidget<Text>("NameText").text = c.name;
        }
        else
        {
            GetWidget<Image>("HeadIcon_Player2").sprite = s;
            GetWidget<Text>("NameText_Player2").text = c.name;
        }
    }


    /// <summary>
    /// 开始按钮 点击
    /// </summary>
    /// <param name="data"></param>
    private void OnStartButtonClick(BaseEventData data)
    {
        //隐藏自己
        Hide(null);
        //关闭动画
        DOTween.Kill("Tips");
        DOTween.Kill("TipsText");
        DOTween.Kill("TipsPlayer2");
        DOTween.Kill("TipsPlayer2Text");
        // 通知场景切换
        EventCenter.Instance.SendEvent(SGEventType.SceneLoad, new EventData("NormalBattle",null));
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
           new SoundParam(UIManager.Instance.Source, "Click01"), null));
    }

    /// <summary>
    /// 取消按钮 点击
    /// </summary>
    /// <param name="data"></param>
    private void OnCancelClick(BaseEventData data)
    {
        //隐藏自己
        Hide(null);
        //关闭动画
        DOTween.Kill("Tips");
        DOTween.Kill("TipsText");
        DOTween.Kill("TipsPlayer2");
        DOTween.Kill("TipsPlayer2Text");
        //切换回游戏开始菜单
        EventCenter.Instance.SendEvent(SGEventType.UIGameStartPanel,new EventData(false,null));
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
           new SoundParam(UIManager.Instance.Source, "Click02"), null));
    }
    /// <summary>
    /// 提示player 闪烁点击
    /// </summary>
    /// <param name="data"></param>
    private void OnTipsClick(BaseEventData data)
    {
        GameObject target = (data as PointerEventData).pointerPress;
        //依据target的名字判断时哪个，然后发送通知
        if (!target.name.EndsWith("_Player2")) //玩家1
        {
            EventCenter.Instance.SendEvent(SGEventType.UIChangeCharacterClick, new EventData(0, null));
        }
        else
        {
            EventCenter.Instance.SendEvent(SGEventType.UIChangeCharacterClick, new EventData(1, null));
        }
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
                 new SoundParam(UIManager.Instance.Source, "Click03"), null));
    }
   
}
