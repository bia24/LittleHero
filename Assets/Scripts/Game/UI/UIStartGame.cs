using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;


/// <summary>
/// “开始游戏”界面
/// </summary>
public class UIStartGame : UIBase
{
    /// <summary>
    /// 第一次显示标志
    /// </summary>
    private bool isFirstShow = true;
    /// <summary>
    /// 增加 组件 的 交互 事件
    /// </summary>
    public override void Init()
    {
        base.Init();

        AddUIEvent("PressTip", EventTriggerType.PointerClick, OnPressTipClick);
        AddUIEvent("PlayerNumber", EventTriggerType.PointerEnter, OnEnter);
        AddUIEvent("PlayerNumber", EventTriggerType.PointerExit, OnExit);
        AddUIEvent("PlayerNumber", EventTriggerType.PointerClick, OnPressPlayerNumber);
        AddUIEvent("Difficulty", EventTriggerType.PointerEnter, OnEnter);
        AddUIEvent("Difficulty", EventTriggerType.PointerExit, OnExit);
        AddUIEvent("Difficulty", EventTriggerType.PointerClick, OnPressDifficulty);
        AddUIEvent("GameSettings", EventTriggerType.PointerEnter, OnEnter);
        AddUIEvent("GameSettings", EventTriggerType.PointerExit, OnExit);
        AddUIEvent("GameSettings", EventTriggerType.PointerClick, OnPressGameSettings);
        AddUIEvent("Finish", EventTriggerType.PointerEnter, OnEnter);
        AddUIEvent("Finish", EventTriggerType.PointerExit, OnExit);
        AddUIEvent("Finish", EventTriggerType.PointerClick, OnPressFinish);
        AddUIEvent("Exit", EventTriggerType.PointerClick, OnPressExit);

        isFirstShow = true;
    }
    /// <summary>
    /// 重载“显示”方法
    /// </summary>
    public override void Show()
    {
        base.Show();

        //请求本panel所需要的数据
        EventCenter.Instance.SendEvent(SGEventType.PlayerNumberGet,
            new EventData(new EventCallBack(PlayerNumberShowListener),null));
        EventCenter.Instance.SendEvent(SGEventType.DifficultyGet, 
            new EventData(new EventCallBack(DifficultyShowListener),null));

        if (isFirstShow)//第一次显示，“pressTip引导”
        {
            //隐藏
            GetWidget("PlayerNumber").SetActive(false);
            GetWidget("Difficulty").SetActive(false);
            GetWidget("Finish").SetActive(false);
            GetWidget("GameSettings").SetActive(false);
            //显示
            GetWidget("PressTip").SetActive(isFirstShow);
            isFirstShow = false;
            //"PressTip"Button 闪烁特效
            GetWidget("PressTip").transform.DOScale(1f, 0f);
            GetWidget("PressTip").transform.DOScale(1.05f, 0.3f).SetLoops(-1, LoopType.Yoyo);
            GetWidget<Text>("PressTip").DOColor(new Color(0.9811f, 0.6621f, 0.0509f, 1f), 0f);
            GetWidget<Text>("PressTip").DOColor(new Color(0.7f, 0.24f, 0, 0.66f), 0.3f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            //显示
            GetWidget("PlayerNumber").SetActive(true);
            GetWidget("Difficulty").SetActive(true);
            GetWidget("Finish").SetActive(true);
            GetWidget("GameSettings").SetActive(true);
            //隐藏
            GetWidget("PressTip").SetActive(false);
        }
    }

   
    /// <summary>
    /// 给 组件 添加 "事件中心" 的 监听回调
    /// </summary>
    protected override void AddListenerToEventCenter()
    {
        EventCenter.Instance.RegistListener(SGEventType.PlayerNumberShow,PlayerNumberShowListener);
        EventCenter.Instance.RegistListener(SGEventType.DifficultyShow, DifficultyShowListener);
    }

    /// <summary>
    /// 面板隐藏 时，取消 事件回调
    /// </summary>
    protected override void CancelListenerFromEventCenter()
    {
        EventCenter.Instance.RemoveListener(SGEventType.PlayerNumberShow, PlayerNumberShowListener);
        EventCenter.Instance.RemoveListener(SGEventType.DifficultyShow, DifficultyShowListener);
    }

    /// <summary>
    /// On "Press Any Button" click
    /// 都是一个Panel中的组件进行通知，没必要使用事件中心
    /// </summary>
    private void OnPressTipClick(BaseEventData data)
    {
        //隐藏
        GetWidget("PressTip").SetActive(false);
        //显示
        GetWidget("PlayerNumber").SetActive(true);
        GetWidget("Difficulty").SetActive(true);
        GetWidget("Finish").SetActive(true);
        GetWidget("GameSettings").SetActive(true);
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay,
            new EventData(new SoundParam(UIController.Instance.GetAudioSource(),"Click01"),null));
    }

    /// <summary>
    /// On "玩家人数" click
    /// </summary>
    private void OnPressPlayerNumber(BaseEventData data)
    {
        //切换
        EventCenter.Instance.SendEvent(SGEventType.PlayerNumberChange, null);
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay,
            new EventData(new SoundParam(UIController.Instance.GetAudioSource(), "Click02"), null));
    }

    /// <summary>
    /// “玩家人数”回调显示，人数在data中封装
    /// </summary>
    /// <param name="data"></param>
    private void PlayerNumberShowListener(EventData data)
    {
        try
        {
            int pn = (int)data.Param;
            string pnString = (pn == 1) ? "1 Player" : "2 Players";
            GetWidget("PlayerNumber").GetComponent<Text>().text = pnString;
        }catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// On "难度" click
    /// </summary>
    /// <param name="data"></param>
    private void OnPressDifficulty(BaseEventData data)
    {
        //切换
        EventCenter.Instance.SendEvent(SGEventType.DifficultyChange , null);
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay,
            new EventData(new SoundParam(UIController.Instance.GetAudioSource(), "Click02"), null));
    }

    /// <summary>
    /// "难度"显示回调函数
    /// </summary>
    /// <param name="data"></param>
    private void DifficultyShowListener(EventData data)
    {
        try
        {
            string diff = (string)data.Param;
            GetWidget("Difficulty").GetComponent<Text>().text = diff;
        }catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// On "游戏设置按钮"click
    /// </summary>
    private void OnPressGameSettings(BaseEventData data)
    {
        //切换
        EventCenter.Instance.SendEvent(SGEventType.UIGameSettings,null);
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay,
            new EventData(new SoundParam(UIController.Instance.GetAudioSource(), "Click02"), null));
        //大小恢复
        (data as PointerEventData).pointerEnter.transform.DOScale(1f, 0.5f);
        //隐藏
        Hide(null);
    }

    /// <summary>
    /// On "设置完毕" click
    /// </summary>
    private void OnPressFinish(BaseEventData data)
    {
        //切换

        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay,
            new EventData(new SoundParam(UIController.Instance.GetAudioSource(), "Click01"), null));
        //大小变换
        (data as PointerEventData).pointerEnter.transform.DOScale(1f, 0.5f);
    }
    /// <summary>
    /// On "退出" click
    /// </summary>
    private void OnPressExit(BaseEventData data)
    {
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay,
            new EventData(new SoundParam(UIController.Instance.GetAudioSource(), "Click02"), null));
        //退出
        Application.Quit();
    }

    /// <summary>
    /// 当鼠标悬浮在button上的通知回调函数
    /// </summary>
    private void OnEnter(BaseEventData data)
    {
        (data as PointerEventData).pointerEnter.transform.DOScale(1.2f, 0.5f); 
    }

    /// <summary>
    /// 当鼠标不再悬浮在button上的通知回调函数
    /// </summary>
    private void OnExit(BaseEventData data)
    {
        (data as PointerEventData).pointerEnter.transform.DOScale(1f, 0.5f);
    }
}
