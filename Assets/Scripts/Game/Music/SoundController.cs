using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;
/// <summary>
/// 控制 音乐 播放的 业务逻辑类
/// </summary>
public class SoundController : Singleton<SoundController>
{


    public void Init()
    {
        //加载Manager的配置文件，默认为异步加载
        //从配置文件中加载类型->名称的映射
        DataManager.Instance.LoadData("SoundName", MusicManager.Instance.InitSoundNameConfigCallBack, false);
        //从配置文件中加载音乐相关配置
        DataManager.Instance.LoadData("SoundSettings",MusicManager.Instance.InitSoundSettingsConfigCallBack, true);

        //注册事件回调
        EventCenter.Instance.RegistListener(SGEventType.SoundBGM,PlayBGMListener);
        EventCenter.Instance.RegistListener(SGEventType.SoundPlay, PlaySoundListener);
        EventCenter.Instance.RegistListener(SGEventType.SoundTrigger, SetSoundTriggerListener);
        EventCenter.Instance.RegistListener(SGEventType.SoundEffectTrigger, SetSoundEffectTriggerListener);
        EventCenter.Instance.RegistListener(SGEventType.SoundSettingsGet, SoundSettingsGetListener);
        EventCenter.Instance.RegistListener(SGEventType.SoundVolumeChange, SoundVolumeChangeListener);
        EventCenter.Instance.RegistListener(SGEventType.SoundEffectVolumeChange, SoundEffectVolumeChangeListener);
    }

     

    /// <summary>
    /// 播放bgm专用方法
    /// </summary>
    /// <param name="data"></param>
    private void PlayBGMListener(EventData data)
    {
         MusicManager.Instance.Play(MusicManager.Instance.MainSource, "MainBGM", true,true);   
    }

    /// <summary>
    /// 在指定播放源上播放声音
    /// </summary>
    /// <param name="data"></param>
    public void PlaySoundListener(EventData data)
    {
        if(data==null)
           Debug.LogError("PlaySound error null EventData.");

     
        SoundParam param = data.Param as SoundParam;
        if (param == null)
            Debug.LogError("EventData convert SoundParam error");

        string soundType = param.type;
        AudioSource source = param.source;
        bool isBreak = param.isBreak;
        bool isLoop = param.isLoop;

        if(source==null )
             Debug.LogError("No audioSource get");

        MusicManager.Instance.Play(source, soundType , isBreak, isLoop);
      
    }

    /// <summary>
    /// 音乐开关设置  事件中心 回调
    /// </summary>
    /// <param name="data"></param>
    private void SetSoundTriggerListener(EventData data)
    {
        bool trigger = (bool)data.Param;
        bool current = MusicManager.Instance.GetSoundTrigger();
        if (trigger == current)
            return;

        MusicManager.Instance.SetSoundTrigger(trigger);
        SoundSettingsEntity sse = MusicManager.Instance.GetSoundSettings();
        string context = JsonUtility.ToJson(sse,true);
        DataManager.Instance.SaveData("SoundSettings", context);
    }
    /// <summary>
    /// 音效开关设置 事件中心  回调
    /// </summary>
    /// <param name="data"></param>
    private void SetSoundEffectTriggerListener(EventData data)
    {
        bool trigger = (bool)data.Param;
        bool current = MusicManager.Instance.GetSoundEffectTrigger();
        if (trigger == current)
            return;

        MusicManager.Instance.SetSoundEffectTrigger(trigger);
        SoundSettingsEntity sse = MusicManager.Instance.GetSoundSettings();
        string context = JsonUtility.ToJson(sse, true);
        DataManager.Instance.SaveData("SoundSettings", context);
    }

    /// <summary>
    /// 获得音源配置 回调
    /// </summary>
    /// <param name="data"></param>
    private void SoundSettingsGetListener(EventData data)
    {
        SoundSettingsEntity sse= MusicManager.Instance.GetSoundSettings();
        EventCallBack cb = data.Param as EventCallBack;
        cb?.Invoke(new EventData(sse, null));
    }

  /// <summary>
  /// 音乐音量改变回调
  /// </summary>
    private void SoundVolumeChangeListener(EventData data)
    {
        float v = (float)data.Param;
        float current = MusicManager.Instance.GetSoundVolume();
        if (Mathf.Abs(v - current) < 0.01f)
            return;

        bool t = MusicManager.Instance.GetSoundTrigger();
        MusicManager.Instance.SetSoundVolume(v);
        if (t)
            MusicManager.Instance.ApplySoundVolume();

        SoundSettingsEntity sse = MusicManager.Instance.GetSoundSettings();
        string context = JsonUtility.ToJson(sse,true);
        DataManager.Instance.SaveData("SoundSettings",context);
    }

    /// <summary>
    /// 音效改变 事件中心 监听
    /// </summary>
    /// <param name="data"></param>
    private void SoundEffectVolumeChangeListener(EventData data)
    {
        float v = (float)data.Param;
        float current = MusicManager.Instance.GetSoundEffectVolume();
        if (Mathf.Abs(v - current) < 0.01f)
            return;

        MusicManager.Instance.SetSoundEffectVolume(v);
        SoundSettingsEntity sse = MusicManager.Instance.GetSoundSettings();
        string context = JsonUtility.ToJson(sse, true);
        DataManager.Instance.SaveData("SoundSettings", context);
    }


}
