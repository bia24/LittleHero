using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;

/// <summary>
/// 敌人基类
/// </summary>
public abstract class Enemy : MonoBehaviour, IAnim, IAttacker, IStrengthenSkill, IParticleMoveSpeed
{
    /// <summary>
    /// AI的状态
    /// </summary>
    public enum AIState
    {
        Idle,
        Walk,
        Attack,
        BeAttack,
        ToFloor,
        BeAttackTransform,
        BeAttackUp,
        Die
    }


    /// <summary>
    /// 角色id，区别敌人的代表的角色
    /// </summary>
    private int characterId;
    public int GetCharacterId() { return characterId; }
    /// <summary>
    /// 实时的战斗信息
    /// </summary>
    private RealtimeBattleInfo realInfo;
    /// <summary>
    /// 当前代表的AI状态类
    /// </summary>
    protected AIStateBase State { get; set; }
    /// <summary>
    /// 是否激活
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// 本模型的状态机
    /// </summary>
    private Animator Animator { get; set; }
    /// <summary>
    /// 音源
    /// </summary>
    private AudioSource AudioSource { get; set; }

    /// <summary>
    /// 动态获取的战场上的玩家list
    /// </summary>
    protected List<Transform> players = new List<Transform>();
    /// <summary>
    /// 难度系数因子
    /// </summary>
    protected float HardFactor = 1.0f;
    /// <summary>
    /// 受击面向，动态变化
    /// </summary>
    public Vector3 BeAttackDir { get; set; }
    /// <summary>
    /// 被击打的连续计数
    /// </summary>
    protected int ContinueBeAttackCount { get; set; }

    #region 一些常量
    /// <summary>
    /// 查找距离
    /// </summary>
    private readonly float BASE_SEARCH_DIS = 0.8f;
    public virtual float GetSearchDistance()
    {
        return BASE_SEARCH_DIS * HardFactor;
    }
    /// <summary>
    /// 发现敌人的反应时间
    /// </summary>
    private readonly float REACTION_TIME = 0.5F;
    public virtual float GetReactionTime()
    {
        return REACTION_TIME * (2 - HardFactor);
    }
    /// <summary>
    /// 站立发憷时间
    /// </summary>
    private readonly float IDLE_TIME = 1F;
    public virtual float GetIdleTime()
    {
        return IDLE_TIME * (2 - HardFactor);
    }
    /// <summary>
    /// 原地触发攻击概率
    /// </summary>
    private readonly float ATTACK_PR = 0.1F;
    public virtual float GetAttackPR()
    {
        return ATTACK_PR * HardFactor;
    }
    /// <summary>
    /// 原地攻击触发的帧间隔
    /// </summary>
    private readonly int ATTACK_FRAME_RATE = 100;
    public virtual int GetAttackFrameRate()
    {
        return (int)(ATTACK_FRAME_RATE * (2 - HardFactor));
    }
    /// <summary>
    /// 两次攻击的间隔时间
    /// </summary>
    private readonly float ATTACK_INTERVAL = 2.5F;
    public virtual float GetAttackInterval()
    {
        return ATTACK_INTERVAL * (2 - HardFactor);
    }
    /// <summary>
    /// 基础移动速率
    /// </summary>
    private readonly float MOVE_SPEED_BASE = 2F;
    public virtual float GetMoveSpeed()
    {
        return MOVE_SPEED_BASE * HardFactor;
    }
    /// <summary>
    /// 每次移动的时间
    /// </summary>
    private readonly float MOVE_TIME = 6F;
    public virtual float GetMoveTime()
    {
        return MOVE_TIME;
    }
    /// <summary>
    /// 增加移动时间的随机性
    /// </summary>
    private readonly float MOVE_TIME_PR_LOWER = 0.5f;
    public virtual float GetMoveTimePRLower()
    {
        return MOVE_TIME_PR_LOWER;
    }
    /// <summary>
    /// 被攻击后位移的持续时间
    /// </summary>
    private readonly float BEATTACK_TRANSFORM_TIME = 0.5F;
    public virtual float GetBeAttackTransformTime()
    {
        return BEATTACK_TRANSFORM_TIME * (2 - HardFactor);
    }
    /// <summary>
    /// 倒地时间
    /// </summary>
    private readonly float TOFLOOR_TIME = 1F;
    public virtual float GetFloorTime()
    {
        return TOFLOOR_TIME * (2 - HardFactor);
    }

    #endregion


    public Animator GetAnimator()
    {
        return Animator;
    }
    /// <summary>
    /// 动画事件绑定回调
    /// </summary>
    /// <returns></returns>
    public abstract List<AnimCallBackEntity> GetCallBacks();


    public void Init(int characterId)
    {
        //初始化本对象的实时战斗信息
        this.characterId = characterId;
        //受击面向初始化
        BeAttackDir = Vector3.right;
        //初始化难度系数
        InitDegreeOfDifficulty();
        //生成实时敌人战斗信息
        RealtimeEnemyInfoConstruct();
        //状态机获取
        Animator = GetComponent<Animator>();
        //角色动画回调方法绑定
        EventCenter.Instance.SendEvent(SGEventType.AnimCallbackRigister, new EventData(null, gameObject));
        //事件中心事件注册
        RegisterEvent();
        EventCenter.Instance.RegistListener(SGEventType.BattlePauseExit, GamePauseExitListener);
        //音源获取
        AudioSource = GetComponent<AudioSource>();
        if (AudioSource == null)
            AudioSource = gameObject.AddComponent<AudioSource>();
        //战场player集合清空
        players.Clear();
        //将自己加入到战场敌人集合中
        BattleController.Instance.AddEnemy(this);
        //初始化当前状态类
        State = StateFactory.Instance.GetStateInstance(AIState.Idle);
        State.OnEnter(this);
        //被击打计数归0
        ContinueBeAttackCount = 0;
        //激活
        Active = BattleController.Instance.GetGameState() == GameState.Running ? true : false;
    }

    /// <summary>
    /// 事件中心事件注册
    /// </summary>
    private void RegisterEvent()
    {
        EventCenter.Instance.RegistListener(SGEventType.AttackJudge, BeAttack);
        EventCenter.Instance.RegistListener(SGEventType.BattlePause, GamePauseListener);
        EventCenter.Instance.RegistListener(SGEventType.BattlePauseExit, GamePauseExitListener);
    }

    /// <summary>
    /// 取消所有事件中心的注册
    /// </summary>
    private void CancleEvent()
    {
        EventCenter.Instance.RemoveListener(SGEventType.AttackJudge, BeAttack);
        EventCenter.Instance.RemoveListener(SGEventType.BattlePause, GamePauseListener);
        EventCenter.Instance.RemoveListener(SGEventType.BattlePauseExit, GamePauseExitListener);
    }


    /// <summary>
    /// 初始化难度系数
    /// </summary>
    private void InitDegreeOfDifficulty()
    {
        DifficultyType type = GameController.Instance.GetGameDifficulty();
        switch (type)
        {
            case DifficultyType.Easy:
                HardFactor = 0.6f;
                break;
            case DifficultyType.Normal:
                HardFactor = 0.8f;
                break;
            case DifficultyType.Hard:
                HardFactor = 1.1f;
                break;
        }
    }
    /// <summary>
    /// 构造实时战斗信息
    /// </summary>
    private void RealtimeEnemyInfoConstruct()
    {
        //data 中参数存着BattleCharacter
        BattleCharacter bc = BattleController.Instance.GetBattleCharacter(GetCharacterId());
        realInfo = new RealtimeBattleInfo();

        realInfo.iconName = bc.headIconName; //头像
        realInfo.hp = (int)(bc.hp * HardFactor);
        realInfo.hpMax = realInfo.hp;
        realInfo.mdefense = (int)(bc.mdefense * HardFactor);
        realInfo.pdefense = (int)(bc.pdefense * HardFactor);
    }

    //循环敌人AI
    private void Update()
    {
        if (!Active) return;

        State.Update(this);  //当前状态的更新
    }
    /// <summary>
    /// 设置敌人的AI状态
    /// </summary>
    /// <param name="state"></param>
    public void SetState(AIState state, bool restart = false)
    {
        if (state != State.GetName() || restart == true) //若想切换的状态和当前状态不一致或者前序重置，直接切换，不等到下一帧，使设状态与初始化同帧
        {
            State.OnExit(this);
            State = StateFactory.Instance.GetStateInstance(state);
            State.OnEnter(this); //直接在设置的时候就初始化
        }
    }

    /// <summary>
    /// 查找当前战场上的玩家信息
    /// </summary>
    /// <returns></returns>
    public List<Transform> SearchTarget()
    {
        //每次查找目标前清空
        players.Clear();
        //查找，完成后集合中拥有当前最新的玩家信息
        BattleController.Instance.GetCurrentPlayer(new EventCallBack(GetPlayersCallback));
        return players;
    }

    /// <summary>
    /// 返回一个在搜索范围内的最近player
    /// </summary>
    /// <param name="players"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public virtual Transform GetMinDisPlayerWithingDis(List<Transform> players)
    {
        float min = float.MaxValue;
        Transform res = null;
        foreach (var temp in players)
        {
            float dis = Vector3.Distance(new Vector3(transform.localPosition.x, transform.localPosition.y, 0),
                new Vector3(temp.localPosition.x, temp.localPosition.y, 0));
            if (dis < min)
            {
                min = dis;
                res = temp.transform;
            }
        }
        if (min < GetSearchDistance())
            return res;
        else
            return null;
    }

    /// <summary>
    /// 获得"最近"的那个玩家
    /// </summary>
    /// <param name="players"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public virtual Transform GetMinDisPlayer(List<Transform> players)
    {
        float min = float.MaxValue;
        Transform res = null;
        foreach (var temp in players)
        {
            float dis = Vector3.Distance(new Vector3(temp.localPosition.x, temp.localPosition.y, 0),
                new Vector3(transform.localPosition.x, transform.localPosition.y, 0));
            if (dis < min)
            {
                min = dis;
                res = temp.transform;
            }
        }
        return res;
    }

    /// <summary>
    /// 回调
    /// </summary>
    /// <param name="data"></param>
    private void GetPlayersCallback(EventData data)
    {
        players.Add(data.Sender.transform);
    }

    /// <summary>
    /// 是否正在转换动画
    /// </summary>
    /// <returns></returns>
    public bool IsInTransforming()
    {
        return AnimationController.Instance.IsInTransforming(this);
    }


    public virtual void Move(Vector3 dir)
    {
        transform.Translate(dir * GetMoveSpeed()*Time.fixedDeltaTime*Time.timeScale, Space.Self);
    }

    public virtual void Rotate(Vector3 dir)
    {
        bool isRight = dir.x > 0 ? true : false;
        float x = transform.localScale.x;
        if (x < 0)//right
            x = isRight ? x : x * -1;
        else//left
            x = isRight ? x * -1 : x;
        transform.localScale = new Vector3(x, transform.localScale.y, 0.0f);
    }

    /// <summary>
    /// 获得物理防御
    /// </summary>
    /// <returns></returns>
    protected int GetPdefense()
    {
        return realInfo.pdefense;
    }

    /// <summary>
    /// 获得魔法防御
    /// </summary>
    /// <returns></returns>
    protected int GetMdefense()
    {
        return realInfo.mdefense;
    }


    /// <summary>
    /// 获得血量
    /// </summary>
    /// <returns></returns>
    protected int GetHp()
    {
        return realInfo.hp;
    }
    /// <summary>
    /// 获得血量百分比
    /// </summary>
    /// <returns></returns>
    public float GetHpRate()
    {
        return (float)realInfo.hp / realInfo.hpMax;
    }

    /// <summary>
    /// 设置血量
    /// </summary>
    protected void SetHp(int hp)
    {
        realInfo.hp = hp;

    }


    

    #region 回调方法
    /// <summary>
    /// 回到站立状态的回调
    /// </summary>
    protected void FinishedToIdle()
    {
        if (IsInTransforming()) //说明已经转换了。
            return;
        SetState(AIState.Idle);
    }
    /// <summary>
    /// 攻击判定回调
    /// </summary>
    /// <param name="skillId"></param>
    protected void AttackJudge(int skillId)
    {
        Skill s = BattleController.Instance.GetSkill(skillId);
        //创建副本
        Skill newSkill = s.Clone() as Skill;
        //伤害还未乘以难度系数
        newSkill = StrengthenSkill(newSkill);
        //发送新的技能信息
        BattleController.Instance.AttackJudge(newSkill, transform, this);
    }

    /// <summary>
    /// 技能强化
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public virtual Skill StrengthenSkill(Skill skill)
    {
        skill.damage = (int)(skill.damage * HardFactor);
        return skill;
    }
    /// <summary>
    /// 收到攻击的回调方法
    /// </summary>
    /// <param name="data"></param>
    private void BeAttack(EventData data)
    {
        //param1 skill;
        //sender 攻击者发出者
        //param2 真正的攻击对象

        GameObject attacker = data.Sender;
        if (attacker.GetComponent<Player>() == null)//这是敌人的指令，不做处理
            return;


        Skill skill = data.Param as Skill;
        string type = skill.attackType;
        int damage = skill.damage;
        string effect = skill.effect;
        float x = skill.x;
        float y = skill.y;
        string disType = skill.disType;


        IAttacker attackObject = data.Param2 as IAttacker;

        //技能是否命中判断
        if (!SkillDistanceJudge(attackObject, transform, disType, x, y))
            return;

        //若是特效粒子的攻击，这里回收粒子
        if (attackObject.IsParticleAttack())
        {
            attackObject.GetParticle().Revert();
        }


        //伤害判定
        DamageCalculate(type, damage);
        //一次成功攻击发出通知
        EventCenter.Instance.SendEvent(SGEventType.BattleAttackSuccess, new EventData(attacker.GetComponent<Player>(), gameObject));
        //受击面向获得
        BeAttackDir = GetBeAttackDir(attackObject, transform);
        //死亡判定以及方法
        if (DieJudge())
            return;
        //动画判断
        AnimStateJudge(effect);
       
        
    }
    #endregion


    /// <summary>
    /// 伤害计算
    /// </summary>
    /// <param name="type"></param>
    /// <param name="damage"></param>
    protected virtual void DamageCalculate(string type, int damage)
    {
        //在实时战斗信息中的值，都已经乘以难度系数了
        int defense = 0;
        switch (type)
        {
            case "Physics":
                defense = GetPdefense();
                break;
            case "Magic":
                defense = GetMdefense();
                break;
        }

        damage = Mathf.Clamp(damage - defense, 0, damage);
        int hp = GetHp() - damage;
        SetHp(hp);
    }
    /// <summary>
    /// 动画判断
    /// </summary>
    /// <param name="effect"></param>
    protected virtual void AnimStateJudge(string effect)
    {
        //此次被攻击是否无动画表现效果
        if (IsThisBeAttackNoEffect())
            return;

        switch (effect)
        {
            case "NormalAttacked": //普通攻击
                if (State.GetName() != AIState.ToFloor && State.GetName() != AIState.BeAttackTransform &&
                    State.GetName() != AIState.Die && State.GetName() != AIState.BeAttackUp)
                {
                    SetState(AIState.BeAttack, true);
                }
                break;
            case "Transform": //位移攻击
                if (State.GetName() != AIState.ToFloor && State.GetName() != AIState.BeAttackTransform &&
                    State.GetName() != AIState.Die && State.GetName() != AIState.BeAttackUp)
                {
                    SetState(AIState.BeAttackTransform);
                }
                break;
            case "ToFloor": //倒地
                if (State.GetName() != AIState.ToFloor && State.GetName() != AIState.BeAttackTransform &&
                    State.GetName() != AIState.Die && State.GetName() != AIState.BeAttackUp)
                {
                    SetState(AIState.ToFloor);
                }
                break;
            case "Up": //浮空
                if (State.GetName() != AIState.BeAttackTransform && State.GetName() != AIState.Die && State.GetName() != AIState.BeAttackUp)
                {
                    SetState(AIState.BeAttackUp);
                }
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 技能是否放到的检测
    /// </summary>
    protected bool SkillDistanceJudge(IAttacker attackObject, Transform beAttacker, string disType, float x, float y)
    {
        bool res = false;
        switch (disType)
        {
            case "Forward":
                if (!LookAtJudge(attackObject, beAttacker))
                    return false; //朝向不满足
                if (!IsInSkillDistance(attackObject, beAttacker, x, y))
                    return false;//不在技能范围内
                res = true;
                break;
            case "Range":
                if (!IsInSkillDistance(attackObject, beAttacker, x, y))
                    return false;//不在技能范围内
                res = true;
                break;
            default:
                Debug.LogWarning("can not match skill dis type  : " + disType);
                break;
        }
        return res;
    }
    /// <summary>
    /// 判断朝向
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="beAttacker"></param>
    /// <returns></returns>
    protected bool LookAtJudge(IAttacker attackObject, Transform beAttacker)
    {
        bool isRight = attackObject.GetAttackerLocalScale().x < 0 ? true : false;
        float deltaX = beAttacker.localPosition.x - attackObject.GetAttackerLocalPostion().x; //x轴上的差

        bool res = isRight ? (deltaX >= 0 ? true : false) : (deltaX <= 0 ? true : false);
        return res;
    }
    /// <summary>
    /// 是否在技能可触范围内
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="beAttacker"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected bool IsInSkillDistance(IAttacker attackObject, Transform beAttacker, float x, float y)
    {
        return (Mathf.Abs(beAttacker.localPosition.x - attackObject.GetAttackerLocalPostion().x) <= x
            && Mathf.Abs(beAttacker.localPosition.y - attackObject.GetAttackerLocalPostion().y) <= y);
    }
    /// <summary>
    /// 获得被攻击者的面向
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="beAttacker"></param>
    /// <returns></returns>
    protected Vector3 GetBeAttackDir(IAttacker attackObject, Transform beAttacker)
    {
        return Vector3.Normalize(new Vector3(attackObject.GetAttackerLocalPostion().x - beAttacker.localPosition.x, 0, 0));
    }

    private void LateUpdate()
    {
        BoundaryClamp();
    }

    /// <summary>
    /// 边界检测
    /// </summary>
    private void BoundaryClamp()
    {
        float x = Mathf.Clamp(transform.localPosition.x, BattleController.Instance.GetMoveXMin(), BattleController.Instance.GetMoveXMax());

        float y = Mathf.Clamp(transform.localPosition.y, BattleController.Instance.GetMoveYMin(), BattleController.Instance.GetMoveYMax());

        transform.localPosition = new Vector3(x, y, 0);
    }

    /// <summary>
    /// 死亡判定，以及处理方法
    /// </summary>
    public virtual bool DieJudge()
    {
        if (GetHp() > 0)
            return false;
        else
        {
            //设置该敌人死亡
            SetOneEnemyDie();
            BattleController.Instance.RemoveEnemy(this);
            return true;
        }
    }
    /// <summary>
    /// 设置一个敌人死亡
    /// </summary>
    public void SetOneEnemyDie()
    {
        //死亡。
        //1. 状态进入死亡状态，触发死亡动画
        SetState(AIState.Die);
        //2. 取消事件中心注册
        CancleEvent();
        //3. 发送本死亡信息
        EventCenter.Instance.SendEvent(SGEventType.BattleEnemyDie, new EventData(null, gameObject));
        //4. 死亡特效播放
        BattleController.Instance.ShowDieParticle(gameObject);
        //5. 协程回收
        StartCoroutine("DelayRevert", 1f);
    }


    private IEnumerator DelayRevert(float delay)
    {
        float timeCount = 0f;

        while (timeCount < delay)
        {
            if (BattleController.Instance.GetGameState()==GameState.Running)
            {
                timeCount += Time.fixedDeltaTime*Time.timeScale;
            }
            yield return null;
        }
        //对象回收
        Revert();
    }


    /// <summary>
    /// 返回对象池
    /// </summary>
    protected void Revert()
    {
        State.OnExit(this);
        State = null;
        //死亡特效回收(先回收子物体)
        BattleController.Instance.RevertDieParticle(gameObject);
        //gameobject revert to pool manager(最后回收玩家物体)
        BattleController.Instance.RevertGameObject(GetCharacterId(), gameObject);
    }


    /// <summary>
    /// 攻击动画产生特效回调
    /// </summary>
    protected virtual void AttackParticleCreate(string param)
    {
        string[] ps = param.Split('|');
        int skillId = int.Parse(ps[0]);
        string particleName = ps[1];

        BattleController.Instance.ShowAttackMoveParticle(gameObject, particleName, skillId);
    }

    /// <summary>
    /// 本敌人的攻击点
    /// </summary>
    /// <returns></returns>
    public Vector3 GetAttackerLocalPostion()
    {
        return transform.localPosition;
    }
    /// <summary>
    /// 本敌人的局部缩放
    /// </summary>
    /// <returns></returns>
    public Vector3 GetAttackerLocalScale()
    {
        return transform.localScale;
    }

    public bool IsParticleAttack()
    {
        return false;
    }

    public MoveParticleBase GetParticle()
    {
        return null;
    }

    /// <summary>
    /// 若本单位会产生移动特效，其速度获取
    /// </summary>
    /// <returns></returns>
    public virtual float GetParticleMoveSpeed()
    {
        return 0f;
    }
    /// <summary>
    /// 若敌人有多个攻击类型，获得每个技能触发的概率
    /// </summary>
    /// <returns></returns>
    public virtual int GetAttackTypeNumber()
    {
        return 1;
    }
    /// <summary>
    /// 为了避免一直被攻击，当被击打多次时，可以霸体的机会
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsThisBeAttackNoEffect()
    {
        return false;
    }
    /// <summary>
    /// 游戏暂停
    /// </summary>
    /// <param name="data"></param>
    private void GamePauseListener(EventData data)
    {
        Active = false;
    }
    /// <summary>
    /// 游戏暂停恢复
    /// </summary>
    /// <param name="data"></param>
    private void GamePauseExitListener(EventData data)
    {
        Active = true;
    }

    private void OnDestroy()
    {
        //销毁时取消所有绑定
        CancleEvent();
        //从战场集合中删除，因为会一直排序渲染。但是不用回收了。因为对象池会清空
        BattleController.Instance.RemoveEnemyRaw(this);
    }
    /// <summary>
    /// 获得依附的对象
    /// </summary>
    /// <returns></returns>
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    /// <summary>
    /// 攻击动作声音播放
    /// </summary>
    protected void AttackSound(string param)
    {
        string[] ps = param.Split('|');
        string type = ps[0];
        float volumeScale = float.Parse(ps[1]);
        SoundParam p = new SoundParam(this.AudioSource, type, false, false, true, volumeScale);
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(p, null));
    }
    /// <summary>
    /// 行走状态的结束判断条件
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool WalkingCondition(Transform target)
    {
        return Vector3.Distance(transform.localPosition, target.transform.localPosition) > GetSearchDistance();
    }
    

   
}


