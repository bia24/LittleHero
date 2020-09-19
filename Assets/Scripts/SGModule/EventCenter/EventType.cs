using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    //消息中心的类型
    public enum SGEventType
    {
        /// <summary>
        /// 空
        /// </summary>
        Null=0,

        //UI 从1000开始
        /// <summary>
        ///游戏开始平面显示
        /// </summary>
        UIGameStartPanel = 1000,
        /// <summary>
        /// 游戏设置菜单
        /// </summary>
        UIGameSettings=1001,
        /// <summary>
        /// 加载界面
        /// </summary>
        UILoadingPanel=1002,
        /// <summary>
        /// 警示面板
        /// </summary>
        UIWarnningPanel=1003,
      
       
       //声音从2000开始
       /// <summary>
       /// 主bgm播放
       /// </summary>
       SoundBGM=2000,
       /// <summary>
       /// 普通音乐播放
       /// </summary>
       SoundPlay=2001,
       /// <summary>
       /// 设置背景音乐音量
       /// </summary>
       SetSound=2002,
       /// <summary>
       /// 音乐 开关
       /// </summary>
       SoundTrigger=2003,
       /// <summary>
       /// 音效开关
       /// </summary>
       SoundEffectTrigger=2004,
       /// <summary>
       /// 音乐配置获取
       /// </summary>
       SoundSettingsGet=2005,
       /// <summary>
       /// 音乐 音量改变
       /// </summary>
       SoundVolumeChange=2006,
       /// <summary>
       /// 音效 改变
       /// </summary>
       SoundEffectVolumeChange=2007,



       //游戏设置从3000开始
       /// <summary>
       /// 游戏人数获取
       /// </summary>
       PlayerNumberGet=3000,
       /// <summary>
       /// 游戏人数 变更
       /// </summary>
       PlayerNumberChange=3001,
       /// <summary>
       /// 游戏人数 显示
       /// </summary>
       PlayerNumberShow=3002,
       /// <summary>
       /// 游戏难度 获取
       /// </summary>
       DifficultyGet=3003,
       /// <summary>
       /// 游戏难度 显示
       /// </summary>
       DifficultyShow=3004,
       /// <summary>
       /// 游戏难度 变更
       /// </summary>
       DifficultyChange=3005,

       //游戏控制逻辑从4000开始
       LoadingFinish=4000,

       //输入控制从5000开始
       /// <summary>
       /// 键位配置文件获取
       /// </summary>
       KeyboardConfigGet=5000,
       /// <summary>
       /// 键位输入框被点击
       /// </summary>
       InputFieldClick=5002,
       /// <summary>
       /// 恢复键位输入框状态
       /// </summary>
       ResetInputFieldState=5003,
       /// <summary>
       /// 输入框按键输入监控开启
       /// </summary>
       InputFieldOn=5004,
       /// <summary>
       /// 输入框按键值显示
       /// </summary>
       InputFieldShow=5005,
       /// <summary>
       /// 通知上一个输入框引用为null
       /// </summary>
       SetLastFieldRefNull=5006,
       /// <summary>
       /// 存储配置
       /// </summary>
       InputConfigSave=5007


       



    }
}
