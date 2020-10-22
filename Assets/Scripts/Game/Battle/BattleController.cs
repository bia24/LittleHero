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
        DataManager.Instance.LoadData("Boundary", BattleManager.Instance.BattleGroundParamLoadCallBack, false);

        //添加监听
        EventCenter.Instance.RegistListener(SGEventType.BattleStart, BattleManagerInitListener);
        EventCenter.Instance.RegistListener(SGEventType.CreateEnemy, CreateEnemyListener);
        EventCenter.Instance.RegistListener(SGEventType.BattleCameraSet, SetCameraListener);
        EventCenter.Instance.RegistListener(SGEventType.EscKeyDown, GamePauseOrRusemeListener);
        EventCenter.Instance.RegistListener(SGEventType.BattlePause, SetGamePauseListener);
        EventCenter.Instance.RegistListener(SGEventType.BattlePauseExit, SetGameRunningListener);
        EventCenter.Instance.RegistListener(SGEventType.UIDarkPanelFinish, ChangeToNextLevel);
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
        SetLevel(1);
        //设置相机位置
        BattleManager.Instance.SetCameraPos(CameraPos.Mid);
        //UI唤出，必须在战斗开始之前
        EventCenter.Instance.SendEvent(SGEventType.UIBattlePanel, null);
        //添加战斗输入监控
        AddInputBattleListener();
        //添加顺序渲染组件
        AddSortRenderMono();
        //添加boss死亡监听
        AddBossDieListener();
        //战斗开始
        BattleStart();
    }

    /// <summary>
    /// 战斗开始业务逻辑
    /// </summary>
    private void BattleStart()
    {
        //游戏状态设置为运行态
        BattleManager.Instance.GameState = GameState.Running;
        //控制BattleManager产生玩家
        int pn = GameController.Instance.GetPlayerNumber();
        for (int id=0;id< pn; id++)
           CreatPlayer(id);
        //控制BattleManager产生敌人
        BattleManager.Instance.CreateEnemyGenerator();
        //控制对话开始.这里会暂停
        EventCenter.Instance.SendEvent(SGEventType.UIDialogue,null);
    }

    /// <summary>
    /// 战斗结束，或者中途退出时的清理工作
    /// </summary>
    public void BattleExit()
    {
        //将产生的东西清除
        //1.取消战场按键监听
        RemoveInputBattleListener();
        //2.销毁敌人产生器
        BattleManager.Instance.DestroyEnemyGenerator();
        //3.调用manager中的退出方法
        BattleManager.Instance.BattleExit();
        //4.游戏时间缩放恢复
        GameController.Instance.ResumeTime();
        //5.UI面板退出
        EventCenter.Instance.SendEvent(SGEventType.UIBattlePanelHide, null);
        //6.删除渲染组件
        RemoveRenderMono();
        //7.删除boss死亡监听
        RemoveOnBossDieListener();
    }

    /// <summary>
    /// 添加boss死亡监听
    /// </summary>
    private void AddBossDieListener()
    {
        GameObject go = new GameObject("OnBossDieListener");
        go.AddComponent<OnBossDieListener>();
    }
    /// <summary>
    /// 移除boss死亡监听
    /// </summary>
    private void RemoveOnBossDieListener()
    {
        GameObject go = GameObject.Find("OnBossDieListener");
        GameObject. Destroy(go);
    }

    /// <summary>
    /// 添加渲染组件
    /// </summary>
    private void AddSortRenderMono()
    {
        GameObject go = new GameObject("SortRender");
        go.AddComponent<BattleSortRender>();
    }
    /// <summary>
    /// 移除渲染组件
    /// </summary>
    private void RemoveRenderMono()
    {
        GameObject go = GameObject.Find("SortRender");
        GameObject.Destroy(go);
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
    /// 取消战斗监听脚本
    /// </summary>
    private void RemoveInputBattleListener()
    {
        int pn = GameController.Instance.GetPlayerNumber();
        for (int id = 0; id < pn; id++)
        {
            EventCenter.Instance.SendEvent(SGEventType.InputBattleListenerRemove, new EventData(id, null));
        }
    }
         

    /// <summary>
    /// 配置关卡内容
    /// </summary>
    private void SetLevel(int level)
    {
        BattleManager.Instance.SetLevel(level);

        int currentLevel = BattleManager.Instance.GetCurrentLevel();
        Level info = BattleManager.Instance.GetLevelInfo(currentLevel);

        string bg = info.bgName;
        string sound = info.soundType;
        //设置背景
        BattleManager.Instance.SetBackground(TextureManager.Instance.GetTexture<Sprite>(bg));
        //设置声音
        EventCenter.Instance.SendEvent(SGEventType.SoundBGM, new EventData(sound, null));
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
    /// 获得技能
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public Skill GetSkill(int skillId)
    {
        return BattleManager.Instance.GetSkill(skillId);
    }

    /// <summary>
    ///  攻击判定
    /// </summary>
    /// <param name="skill">技能</param>
    /// <param name="attacker">攻击发出者</param>
    /// <param name="attackObject">计算攻击的相关参数获取</param>
    public void AttackJudge(Skill skill,Transform attacker,IAttacker attackObject)
    {
        EventCenter.Instance.SendEvent(SGEventType.AttackJudge, new EventData(skill, attacker.gameObject, attackObject));
    }


    /// <summary>
    /// 获得当前升级所需要的经验
    /// </summary>
    public int GetLevelUpNeedExp(int playerLevel)
    {
        float rate= Mathf.Pow(BattleManager.Instance.GetPlayerLevelUpExpRate(), playerLevel);
        int expNeed =(int)rate * BattleManager.Instance.GetPlayerLevelUpExp();//比例乘以就初始化所需经验
        return expNeed;
    }
    /// <summary>
    /// 获得等级上限
    /// </summary>
    /// <returns></returns>
    public int GetLevelMax()
    {
        return BattleManager.Instance.GetPlayerLevelMax();
    }
    /// <summary>
    /// 击打经验
    /// </summary>
    /// <returns></returns>
    public int GetPerExpEnemy()
    {
        return BattleManager.Instance.GetPerEnemyExp();
    }
    /// <summary>
    /// 击打蓝量恢复
    /// </summary>
    /// <returns></returns>
    public int GetPerMpEnemy()
    {
        return BattleManager.Instance.GetPerEnemyMp();
    }
    /// <summary>
    /// 获得能量的最大数目
    /// </summary>
    /// <returns></returns>
    public int GetMaxPowerNumber()
    {
        return BattleManager.Instance.GetPowerMax();
    }

    public void ShowPowerParticle(Player target)
    {
        int n = target.GetPowerNumber(); //获得能量值
        if (n == 0)
        {
            if (target.PowerParticle != null)
            {
                PoolManager.Instance.RevertPool(BattleManager.Instance.GetPowerParticleResourcesFullPath(), target.PowerParticle.gameObject);
                target.PowerParticle = null;
            }
        }
        else
        {
            if (target.PowerParticle != null)
                return;
            else
                //获得特效
                target.PowerParticle = PoolManager.Instance.getPrefab(BattleManager.Instance.GetPowerParticleResourcesFullPath(), "PowerParticle", target.PowerParticlePos, false).transform;
        }
    }
    /// <summary>
    /// 回收能量罩特效
    /// </summary>
    /// <param name="p"></param>
    public void RevertPowerParticle(Player target)
    {
        if (target.PowerParticle != null)
        {
            PoolManager.Instance.RevertPool(BattleManager.Instance.GetPowerParticleResourcesFullPath(), target.PowerParticle.gameObject);
            target.PowerParticle = null;
        }
    }

    public float GetMoveXMin()
    {
        return BattleManager.Instance.MoveXMin;
    }

    public float GetMoveXMax()
    {
        return BattleManager.Instance.MoveXMax;
    }

    public float GetMoveYMin()
    {
        return BattleManager.Instance.MoveYMin;
    }

    public float GetMoveYMax()
    {
        return BattleManager.Instance.MoveYMax;
    }

    public float GetCameraXMin()
    {
        return BattleManager.Instance.CameraXMin;
    }

    public float GetCameraXMax()
    {
        return BattleManager.Instance.CameraXMax;
    }

    public float GetTriggerOne()
    {
        return BattleManager.Instance.TriggerOne;
    }

    public float GetTriggerTwo()
    {
        return BattleManager.Instance.TriggerTwo;
    }

    public float GetTriggerThree()
    {
        return BattleManager.Instance.TriggerThree;
    }

    public float GetTriggerFour()
    {
        return BattleManager.Instance.TriggerFour;
    }

    public CameraPos GetCurrentCameraPos()
    {
        return BattleManager.Instance.GetCameraPos();
    }

    private void SetCameraListener(EventData data)
    {
        CameraPos next = (CameraPos)data.Param;
        CameraPos current = BattleManager.Instance.GetCameraPos();
        if (next == current)
            return;
        BattleManager.Instance.SetCameraPos(next);
    }

    /// <summary>
    /// 将一个游戏对象返回至对象池
    /// </summary>
    /// <param name="characterId"></param>
    /// <param name="go"></param>
    public void RevertGameObject(int characterId,GameObject go)
    {
        BattleCharacter bc = GetBattleCharacter(characterId);
        string goFullPath = BattleManager.Instance.GetGameObjectResourcesPath() + "/"+bc.objectName;
        PoolManager.Instance.RevertPool(goFullPath,go);
    }
    /// <summary>
    /// 将一个死亡特效返回对象池
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="go"></param>
    public void RevertDieParticle(GameObject go)
    {
        GameObject particle = go.transform.Find("DiePos").Find("DieParticle").gameObject;
        PoolManager.Instance.RevertPool(BattleManager.Instance.GetDieParticleResourcesFullPath(), particle);
    }
    /// <summary>
    /// 显示死亡特效
    /// </summary>
    /// <param name="data"></param>
    public void ShowDieParticle(GameObject target)
    {
        Transform diePos= target.transform.Find("DiePos");
         PoolManager.Instance.getPrefab(BattleManager.Instance.GetDieParticleResourcesFullPath(), "DieParticle", diePos,false);
    }
    /// <summary>
    /// 特效产生方法
    /// </summary>
    /// <param name="producer">特效产生者，player或enemy</param>
    /// <param name="particleName"></param>
    /// <param name="skillId"></param>
    public void ShowAttackMoveParticle(GameObject producer,string particleName,int skillId)
    {
        //获得战场原点。特效粒子产生后不能依赖于产生者。自己独立。
        Transform basePoint = producer.transform.parent;
        //取出粒子特效，将它放置在战场上原点的子组件上
        GameObject particle= PoolManager.Instance.getPrefab(BattleManager.Instance.GetParticleResourcesRootPath() + "/" + particleName, particleName,
            basePoint, false);
        //设置粒子特效的位置为模型粒子产生点的位置
        particle.transform.position = producer.transform.Find("ParticlePos").position;
        //获得施法者的面向
        bool isRight = producer.transform.localScale.x < 0 ? true : false;
        //设置粒子特效的缩放(面向)为此时产生者的缩放
        Vector3 scale = particle.transform.localScale;
        scale.x = isRight ? (scale.x < 0 ? scale.x : scale.x * -1) : (scale.x > 0 ? scale.x : scale.x * -1);
        particle.transform.localScale = scale;

        MoveParticleBase p = particle.GetComponent<MoveParticleBase>();
        if (p == null)
            p = particle.AddComponent<MoveParticleBase>();

        p.Init(isRight?Vector3.right:Vector3.left,skillId, producer, particleName);
    }

    public void RevertAttackMoveParticle(string particleName,GameObject go)
    {
        PoolManager.Instance.RevertPool(BattleManager.Instance.GetParticleResourcesRootPath() + "/" + particleName,go);
    }

    /// <summary>
    /// 显示升级特效
    /// </summary>
    /// <param name="target"></param>
    public void ShowLevelUpParicle(GameObject target)
    {
        Transform levelUpPos = target.transform.Find("LevelUpPos");
       GameObject go=  PoolManager.Instance.getPrefab(BattleManager.Instance.GetParticleResourcesRootPath()+"/"+ "LevelUpParticle",
           "LevelUpParticle", levelUpPos, false);
        LevelUpParticle script = go.GetComponent<LevelUpParticle>();
        if(script==null)
            script= go.AddComponent<LevelUpParticle>(); ;
        //初始化
        script.Init(target.GetComponent<Player>());
    }
    /// <summary>
    /// 回收升级特效
    /// </summary>
    /// <param name="target"></param>
    public void RevertLevelUpParticle(GameObject particle)
    {
        PoolManager.Instance.RevertPool(BattleManager.Instance.GetParticleResourcesRootPath() + "/" + "LevelUpParticle",
           particle);
    }

    /// <summary>
    /// 添加玩家到战场
    /// </summary>
    /// <param name="p"></param>
    public void AddPlayer(Player p)
    {
        BattleManager.Instance.playerInBattle.Add(p);
    }

    public void RemovePlayer(Player p)
    {
        BattleManager.Instance.playerInBattle.Remove(p);
        if (BattleManager.Instance.playerInBattle.Count == 0)//玩家没了，敌人还存在
        {
            EventCenter.Instance.SendEvent(SGEventType.UIBattleEnd, new EventData(false, null, "小英雄，请重新来过"));
            //声音
            EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
                    new SoundParam(BattleManager.Instance.audioSource, "Lose_Sound", false), null));
        }
    }

    public void RemovePlayerRaw(Player p)
    {
        BattleManager.Instance.playerInBattle.Remove(p);
    }

    /// <summary>
    /// 添加可移动的特效到集合
    /// </summary>
    /// <param name="particle"></param>
    public void AddParticle(IAttacker particle)
    {
        BattleManager.Instance.moveParticleInBattle.Add(particle);
    }
    /// <summary>
    /// 移除可移动的特效到集合
    /// </summary>
    /// <param name="particle"></param>
    public void RemoveParicle(IAttacker particle)
    {
        BattleManager.Instance.moveParticleInBattle.Remove(particle);
    }

    /// <summary>
    /// 添加到战场敌人集合
    /// </summary>
    /// <param name="enemy"></param>
    public void AddEnemy(Enemy enemy)
    {
        BattleManager.Instance.enemyInBattle.Add(enemy);
    }
    /// <summary>
    /// 从战场中将敌人移除
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemy(Enemy enemy)
    {
        //从战场中移除
        List<Enemy> list = BattleManager.Instance.enemyInBattle;
        list.Remove(enemy);
        //从移除的角色id上判断是否为每一关的boss
        int id = enemy.GetCharacterId();
        if (id == BattleManager.Instance.BOSS1_ID &&GetCurrentLevel()==1||
            id == BattleManager.Instance.BOSS2_ID&&GetCurrentLevel()==2 ||
            id == BattleManager.Instance.BOSS3_ID&&GetCurrentLevel()==3)
        {
            //boss死亡
            EventCenter.Instance.SendEvent(SGEventType.BattleBossDie,new EventData(id,null,new EventCallBack(OnBossDieFinished)));
            //声音
            EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
                    new SoundParam(BattleManager.Instance.audioSource, "BossDie", false), null));

            //其余小兵也死亡清空战场
            while (list.Count != 0)
            {
                list[0].SetOneEnemyDie();
                list.RemoveAt(0);
            }

            //特效若存在删除
            while (BattleManager.Instance.moveParticleInBattle.Count != 0)
            {
                BattleManager.Instance.moveParticleInBattle[0].GetParticle().Revert();
            }
           
        }
        else
        {
            //battle是否需要切换、产兵、结束进行判断
            int n = list.Count;
            if (n == 0)
                EventCenter.Instance.SendEvent(SGEventType.NextTimeEnemyGenerate, null);
        }
    }
    /// <summary>
    /// 不触发移除事件的移除
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemyRaw(Enemy enemy)
    {
        BattleManager.Instance.enemyInBattle.Remove(enemy);
    }

    private void OnBossDieFinished(EventData  data)
    {
        int id = (int)data.Param;//bossId
        if (id != BattleManager.Instance.BOSS3_ID )
        {
            //普通boss
            //UI黑屏渐变动画开启，由动画完成后进行回调
            EventCenter.Instance.SendEvent(SGEventType.UIDarkPanel,null);
        }
        else 
        {
            EventCenter.Instance.SendEvent(SGEventType.UIBattleEnd, new EventData(true,null,"恭喜你，真不愧是小英雄"));
            //声音
            EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
                    new SoundParam(BattleManager.Instance.audioSource, "Win_Sound",false), null));
        }
    }

    private void ChangeToNextLevel(EventData data)
    {
        
        int level = BattleManager.Instance.GetCurrentLevel();
        //设置新关卡
        SetLevel(++level);
        //唤出对话框
        EventCenter.Instance.SendEvent(SGEventType.UIDialogue,null);
        //重置玩家状态
        EventCenter.Instance.SendEvent(SGEventType.BattleResetPlayerState, null);
        //相机归位
        BattleManager.Instance.SetCameraPos(CameraPos.Mid);
    }


    
    /// <summary>
    /// 玩家初始化状态
    /// </summary>
    /// <param name="p"></param>
    public void ResetPlayerState(Player p)
    {
        //初始化位置信息
        BattleManager.Instance.ResetPlayerTransform(p);
    }

    /// <summary>
    /// 获得战场中的敌人数目
    /// </summary>
    /// <returns></returns>
    public int GetEnemyCount()
    {
        return BattleManager.Instance.enemyInBattle.Count;
    }

    public void OnlyOneEnemyVisibleJudge()
    {
        Enemy e = BattleManager.Instance.enemyInBattle[0];
        Vector3 viewPos= BattleManager.Instance.GetBattleCamera().WorldToViewportPoint(e.transform.position);
        if (viewPos.x <= 0)
        {
            EventCenter.Instance.SendEvent(SGEventType.UIBattleDirTip, new EventData(-1, null));
        }
        else if (viewPos.x > 0 && viewPos.x <= 1)
        {
            EventCenter.Instance.SendEvent(SGEventType.UIBattleDirTip, new EventData(0, null));
        }
        else
        {
            EventCenter.Instance.SendEvent(SGEventType.UIBattleDirTip, new EventData(1, null));
        }
    }
    
    /// <summary>
    /// ecs按钮按下的监听
    /// </summary>
    /// <param name="data"></param>
    private void GamePauseOrRusemeListener(EventData data)
    {
        if (BattleManager.Instance.GameState == GameState.Running)
        {
            EventCenter.Instance.SendEvent(SGEventType.BattlePause,null);
        }
        else
        {
            EventCenter.Instance.SendEvent(SGEventType.BattlePauseExit, null);
        }
    }

    /// <summary>
    /// 设置游戏暂停监听
    /// </summary>
    /// <param name="data"></param>
    private void SetGamePauseListener(EventData data)
    {
        GameController.Instance.StopTime();
        BattleManager.Instance.GameState = GameState.Pause;
    }
    /// <summary>
    /// 设置游戏恢复监听
    /// </summary>
    /// <param name="data"></param>
    private void SetGameRunningListener(EventData data)
    {
        GameController.Instance.ResumeTime();
        BattleManager.Instance.GameState = GameState.Running;
    }

    /// <summary>
    /// 获得当前游戏的状态
    /// </summary>
    /// <returns></returns>
    public GameState GetGameState()
    {
        return BattleManager.Instance.GameState;
    }

    
   
}

public struct SlowDownParam
{
    public float time;
    public int bossId;
    public EventCallBack callback;
    public SlowDownParam(float time,int bossId, EventCallBack callback)
    {
        this.time = time;
        this.bossId = bossId;
        this.callback = callback;
    }
}
