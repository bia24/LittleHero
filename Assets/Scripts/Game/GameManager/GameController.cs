using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;

public class GameController :Singleton<GameController>
{

    public void Init()
    {
        //加载游戏配置相关的配置文件
        DataManager.Instance.LoadData("GameSettings", GameManager.Instance.GameSettingsInitCallBack, false);
        //事件监听
        EventCenter.Instance.RegistListener(SGEventType.PlayerNumberChange,PlayerNumberChangeListener);
        EventCenter.Instance.RegistListener(SGEventType.PlayerNumberGet, PlayerNumberGetListener);
        EventCenter.Instance.RegistListener(SGEventType.DifficultyGet, DifficultyGetListener);
        EventCenter.Instance.RegistListener(SGEventType.DifficultyChange, DiffcultyChangeListener);
    }

  
    /// <summary>
    /// "玩家人数"显示请求回调方法
    /// </summary>
    /// <param name="data"></param>
    private void PlayerNumberGetListener(EventData data)
    {
        int pn = GameManager.Instance.GetPlayerNumber();
        EventCallBack cb = data.Param as EventCallBack;
        cb?.Invoke(new EventData(pn,null));
    }
    
    /// <summary>
    /// "玩家人数"变更回调方法
    /// </summary>
    /// <param name="data"></param>
    private void PlayerNumberChangeListener(EventData data)
    {
        GameManager.Instance.SetPlayerNumber();
        EventCenter.Instance.SendEvent(SGEventType.PlayerNumberShow, new EventData(GameManager.Instance.GetPlayerNumber(), null));
    }

    /// <summary>
    /// "难度" 获取请求回调函数
    /// </summary>
    /// <param name="data"></param>
    private void DifficultyGetListener(EventData data)
    {
        try
        {
            string diff = GameManager.Instance.GetDifficulty().ToString();
            EventCallBack cb = data.Param as EventCallBack;
            cb?.Invoke(new EventData(diff, null));
        }catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// "难度" 切换 回调函数
    /// 以Easy，Normal，Hard的顺序切换
    /// </summary>
    /// <param name="data"></param>
    private void DiffcultyChangeListener(EventData data)
    {
        DifficultyType dt = GameManager.Instance.GetDifficulty();
        switch (dt)
        {
            case DifficultyType.Easy:
                GameManager.Instance.SetDifficulty(DifficultyType.Normal);
                EventCenter.Instance.SendEvent(SGEventType.DifficultyShow, new EventData(DifficultyType.Normal.ToString(), null));
                break;
            case DifficultyType.Normal:
                GameManager.Instance.SetDifficulty(DifficultyType.Hard);
                EventCenter.Instance.SendEvent(SGEventType.DifficultyShow, new EventData(DifficultyType.Hard.ToString(), null));
                break;
            case DifficultyType.Hard:
                GameManager.Instance.SetDifficulty(DifficultyType.Easy);
                EventCenter.Instance.SendEvent(SGEventType.DifficultyShow, new EventData(DifficultyType.Easy.ToString(), null));
                break;
        }
    }
}
