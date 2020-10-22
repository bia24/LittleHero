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
        /// <summary>
        /// 战场指引   
        /// </summary>
        UIBattleDirTip=1012,
        /// <summary>
        /// 游戏暂停面板被唤出
        /// </summary>
        UIPausePanel=1013,
        /// <summary>
        /// 在游戏进行中唤出游戏设置菜单
        /// </summary>
        UIGameSettingOnGaming=1014,
        /// <summary>
        /// 战斗面板消失
        /// </summary>
        UIBattlePanelHide=1015,
        /// <summary>
        /// 黑屏渐变面板完成工作
        /// </summary>
        UIDarkPanelFinish=1016,
        /// <summary>
        /// 黑屏渐变UI唤出
        /// </summary>
        UIDarkPanel=1017,
        /// <summary>
        /// 战斗结束对话框
        /// </summary>
        UIBattleEnd=1018,



       //声音从2000开始
       /// <summary>
       /// 主bgm播放
       /// </summary>
        SoundBGM =2000,
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
       /// <summary>
       /// esc 按键被按下
       /// </summary>
       EscKeyDown=5014,
      





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
        /// <summary>
        /// 战斗中血量变化
        /// </summary>
        BattlePlayerHpChange =7006,
        /// <summary>
        /// 战斗中魔法值变化
        /// </summary>
        BattlePlayerMpChange=7007,
        /// <summary>
        /// 战斗等级变化
        /// </summary>
        BattlePlayerLevelChange = 7008,
        /// <summary>
        /// 战斗中能量变化
        /// </summary>
        BattlePlayerPowerChange = 7009,
        /// <summary>
        /// 战斗中成功攻击敌人
        /// </summary>
        BattleAttackSuccess=7010,
        /// <summary>
        /// 设置战场上的相机
        /// </summary>
        BattleCameraSet=7012,
        /// <summary>
        /// 敌人死亡通知
        /// </summary>
        BattleEnemyDie=7013,
        /// <summary>
        /// 玩家死亡
        /// </summary>
        BattlePlayerDie=7014,
        /// <summary>
        /// 战斗暂停
        /// </summary>
        BattlePause=7015,
        /// <summary>
        /// 战斗暂停恢复
        /// </summary>
        BattlePauseExit=7016,
        /// <summary>
        /// 产生下一波兵
        /// </summary>
        NextTimeEnemyGenerate=7018,
        /// <summary>
        /// boss死了
        /// </summary>
        BattleBossDie=7019,
        /// <summary>
        /// 重置玩家状态
        /// </summary>
        BattleResetPlayerState=7020,
        /// <summary>
        /// 对话出现
        /// </summary>
        BattleDialogueAppear=7021,
        /// <summary>
        /// 对话退出
        /// </summary>
        BattleDialogueExit= 7022,


        //动画相关从8000开始
        /// <summary>
        /// 动画回调注册
        /// </summary>
        AnimCallbackRigister = 8000,
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
