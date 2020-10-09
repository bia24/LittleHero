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
        /// 初始化加载界面
        /// </summary>
        UILoadingPanel=1002,
        /// <summary>
        /// 警示面板
        /// </summary>
        UIWarnningPanel=1003,
        /// <summary>
        ///角色选择面板
        /// </summary>
        UICharacterChoose=1004,
        /// <summary>
        /// 切换场景加载界面
        /// </summary>
        UILoadScenePanel=1005,
        /// <summary>
        /// 切换场景界面隐藏
        /// </summary>
        UILoadScenePanelHide=1006,
        /// <summary>
        /// 战斗面板界面唤出
        /// </summary>
        UIBattlePanel=1007,
        /// <summary>
        /// 难度按钮被点击
        /// </summary>
        UIDifficultyButtomClick=1008,
        /// <summary>
        /// 玩家人数按钮被点击
        /// </summary>
        UIPlayerNumberButtomClick=1009,
        /// <summary>
        /// 战斗对话界面显示
        /// </summary>
        UIDialogue=1010,
        /// <summary>
        /// 更换角色点击
        /// </summary>
        UIChangeCharacterClick=1011,
      
       
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
       /// 音乐 音量改变
       /// </summary>
       SoundVolumeChange=2006,
       /// <summary>
       /// 音效 改变
       /// </summary>
       SoundEffectVolumeChange=2007,



       //游戏设置从3000开始
       /// <summary>
       /// 游戏人数 变更
       /// </summary>
       PlayerNumberChange=3001,
       /// <summary>
       /// 游戏难度 变更
       /// </summary>
       DifficultyChange=3002,
       /// <summary>
       /// 下一段对话的id
       /// </summary>
        NextDialogue=3006,
        /// <summary>
        /// 对话顺序已经到了下一level了
        /// </summary>
        DialogueRankReset=3007,
        /// <summary>
        /// 游戏管理器运行时数据初始化
        /// </summary>
        GameRuntimeDataInit=3008,
        /// <summary>
        /// 玩家所选角色更改
        /// </summary>
        PlayerChooseCharacterChange=3009,


       //游戏控制逻辑从4000开始
       /// <summary>
       /// UILoading界面加载完成
       /// </summary>
        LoadingFinish =4000,

       //输入控制从5000开始
       /// <summary>
       /// 输入框监听添加
       /// </summary>
       InputFieldListenerAdd=5001,
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
       InputConfigSave=5007,
       /// <summary>
       /// 输入框监听脚本移除
       /// </summary>
       InputFieldListenerRemove=5008,
       /// <summary>
       /// 战斗输入监听添加
       /// </summary>
       InputBattleListenerAdd=5009,
       /// <summary>
       /// 战斗输入监听移除
       /// </summary>
       InputBattleListenerRemove=5010,
       /// <summary>
       /// 输入监听到了连击id
       /// </summary>
       ComboId=5011,
       /// <summary>
       /// 输入监听到了指令
       /// </summary>
       Command=5012,
       /// <summary>
       /// 方向空指令
       /// </summary>
       CommandNoneDir=5013,
      





        //场景加载 从6000开始
        /// <summary>
        /// 加载场景
        /// </summary>
        SceneLoad = 6000,
        /// <summary>
        /// 场景加载进度
        /// </summary>
        SceneLoadProcess=6001,
        /// <summary>
        ///  场景加载完成
        /// </summary>
        SceneLoadDone =6002,


        //战斗相关从 7000开始
        /// <summary>
        /// 战斗场景启动
        /// </summary>
        BattleStart=7000,
        /// <summary>
        /// 战斗管理器初始化
        /// </summary>
        BattleManagerInit=7001,
        /// <summary>
        /// 玩家模型创建初始化事件
        /// </summary>
        PlayerInitFinished=7002,
        /// <summary>
        /// 生成敌人
        /// </summary>
        CreateEnemy=7003,
        /// <summary>
        /// 获得战场上的玩家信息
        /// </summary>
        BattleGetPlayers=7004,
        /// <summary>
        /// 攻击判定
        /// </summary>
        AttackJudge=7005,


        //动画相关从8000开始
        /// <summary>
        /// 动画回调注册
        /// </summary>
        AnimCallbackRigister=8000,
        /// <summary>
        /// 动画转换 trigger
        /// </summary>
        AnimSetTrigger=8001,
        /// <summary>
        /// 动画转化 bool
        /// </summary>
        AnimSetBool=8002
        









    }
}
