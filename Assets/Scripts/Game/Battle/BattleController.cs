using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;

/// <summary>
/// 战斗 逻辑控制
/// </summary>
public class BattleController : Singleton<BattleController>
{
    public void Init()
    {
        //加载配置文件
        DataManager.Instance.LoadData("BattleParam", BattleManager.Instance.BattleParamLoadCallBack, false);
        DataManager.Instance.LoadData("BattleCharacter", BattleManager.Instance.BattleCharacterInfosLoadCallBack, false);
        DataManager.Instance.LoadData("Skill", BattleManager.Instance.SkillInfosLoadCallBack, false);
        DataManager.Instance.LoadData("LevelInfo", BattleManager.Instance.LevelInfoLoadCallBack, false);
        DataManager.Instance.LoadData("EnemyGenPlan", BattleManager.Instance.EnemyGenPlanLoadCallBack, false);
        //添加监听
        EventCenter.Instance.RegistListener(SGEventType.BattleStart, BattleManagerInitListener);
        EventCenter.Instance.RegistListener(SGEventType.CreateEnemy, CreateEnemyListener);

    }

    /// <summary>
    /// 战斗管理器初始化
    /// </summary>
    /// <param name="data"></param>
    private void BattleManagerInitListener(EventData data)
    {
        //战斗管理器相关参数的初始化
        BattleManager.Instance.BattleManagerRefInit();
        //关卡配置初始化
        ConfigLevelInit();
        //设置相机位置
        BattleManager.Instance.SetCameraPos(CameraPos.Mid);
        //UI唤出，必须在战斗开始之前
        EventCenter.Instance.SendEvent(SGEventType.UIBattlePanel, null);
        //添加战斗输入监控
        AddInputBattleListener();
        //战斗开始
        BattleStart();
    }
   

    /// <summary>
    /// 战斗开始业务逻辑
    /// </summary>
    private void BattleStart()
    {
        //控制BattleManager产生玩家
        int pn = GameController.Instance.GetPlayerNumber();
        for (int id=0;id< pn; id++)
           CreatPlayer(id);
        //控制BattleManager产生敌人
        BattleManager.Instance.CreateEnemyGenerator();
        //控制对话开始
        //EventCenter.Instance.SendEvent(SGEventType.UIDialogue,null);
    }
    /// <summary>
    /// 添加战斗输入监听脚本
    /// </summary>
    private void AddInputBattleListener()
    {
        int pn = GameController.Instance.GetPlayerNumber();
        //依据玩家数目添加战斗输入监听
        for(int id = 0; id < pn; id++)
        {
            EventCenter.Instance.SendEvent(SGEventType.InputBattleListenerAdd, new EventData(id,null));
        }
    }

    /// <summary>
    /// 配置关卡内容
    /// </summary>
    private void ConfigLevelInit()
    {
        BattleManager.Instance.SetLevel(1);

        int level = BattleManager.Instance.GetCurrentLevel();

        Level l = BattleManager.Instance.GetLevelInfo(level);

        string soundType = l.soundType;
        string bgName = l.bgName;

        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundBGM, new EventData(soundType, null));
        //背景
        Sprite sprite = TextureManager.Instance.GetTexture<Sprite>(bgName);
        BattleManager.Instance.SetBackground(sprite);
    }


    private void CreateEnemyListener(EventData data)
    {
        EnemyDetail ed = data.Param as EnemyDetail;
        Produce pos = (Produce)Enum.Parse(typeof(Produce), ed.pos);
        Enemy e= BattleManager.Instance.GenerateEnemy(ed.characterId, pos);
        //初始化脚本
        e.Init(ed.characterId);
    }

    /// <summary>
    /// 依据玩家id创建战斗实例
    /// </summary>
    /// <param name="playerId"></param>
    private void CreatPlayer(int playerId)
    {
        int characterId = GameController.Instance.GetCharacterId(playerId);
        Player p=BattleManager.Instance.CreatPlayer(characterId);
        //初始化 实例
        p.Init(playerId);
    }

    /// <summary>
    /// 获得战斗的角色信息
    /// </summary>
    /// <param name="characterId">角色id</param>
    /// <returns></returns>
   public BattleCharacter GetBattleCharacter(int characterId)
    {
        return BattleManager.Instance.GetBattleCharacter(characterId);
    }

    /// <summary>
    /// 返回当前难度和关卡的产兵计划
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, List<EnemyDetail>> GetCurrentGenEnemyPlan()
    {
        DifficultyType diff = GameController.Instance.GetGameDifficulty();
        return BattleManager.Instance.GetCurrentGenEnemyPlan(diff);
    }

    /// <summary>
    /// 得到当前关卡
    /// </summary>
    /// <returns></returns>
    public int GetCurrentLevel()
    {
        return BattleManager.Instance.GetCurrentLevel();
    }

    /// <summary>
    /// 获得当前战场上的玩家脚本
    /// </summary>
    /// <returns></returns>
    public void GetCurrentPlayer(EventCallBack cb)
    {
        EventCenter.Instance.SendEvent(SGEventType.BattleGetPlayers, new EventData(cb, null));
    }

    /// <summary>
    /// 攻击判定
    /// </summary>
    /// <param name="skillId"></param>
    public void AttackJudge(int skillId)
    {
        Skill skill = BattleManager.Instance.GetSkill(skillId);
        EventCenter.Instance.SendEvent(SGEventType.AttackJudge, new EventData(skill, null));
    }

}
