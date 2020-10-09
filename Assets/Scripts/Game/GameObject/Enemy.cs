using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;

/// <summary>
/// 敌人基类
/// </summary>
public abstract class Enemy : MonoBehaviour,IAnim
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
        ToFloor
    }
    
    /// <summary>
    /// 角色id，区别敌人的代表的角色
    /// </summary>
    private int characterId;
    /// <summary>
    /// 实时的战斗信息
    /// </summary>
    private RealtimeBattleInfo realInfo;
    /// <summary>
    /// 当前状态
    /// </summary>
    public AIState CurrentState { get; set; }
    /// <summary>
    /// 当前代表的AI状态类
    /// </summary>
    private AIStateBase State { get; set; }
    /// <summary>
    /// 是否激活
    /// </summary>
    private bool Active { get; set; }
    /// <summary>
    /// 本模型的状态机
    /// </summary>
    private Animator Animator { get; set; }
    /// <summary>
    /// 音源
    /// </summary>
    private AudioSource AudioSource { get; set; }
    /// <summary>
    /// 角色状态到动画名称的映射
    /// </summary>
    private Dictionary<State, string> stateAnimNames = new Dictionary<State, string>();

    /// <summary>
    /// 动态获取的战场上的玩家list
    /// </summary>
    protected List<Transform> players = new List<Transform>();
    /// <summary>
    /// 难度系数因子
    /// </summary>
    protected float HardFactor = 1.0f;
  
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
    private readonly float REACTION_TIME = 0.3F;
    public virtual float GetReactionTime()
    {
        return REACTION_TIME * (2-HardFactor);
    }
    /// <summary>
    /// 站立发憷时间
    /// </summary>
    private readonly float IDLE_TIME = 2F;
    public virtual float GetIdleTime()
    {
        return IDLE_TIME * (2 - HardFactor);
    }
    /// <summary>
    /// 原地触发攻击概率
    /// </summary>
    private readonly float ATTACK_PR = 0.2F;
    public virtual float GetAttackPR()
    {
        return ATTACK_PR * HardFactor;
    }
    /// <summary>
    /// 原地攻击触发的帧间隔
    /// </summary>
    private readonly int ATTACK_FRAME_RATE = 120;
    public virtual int GetAttackFrameRate()
    {
        return ATTACK_FRAME_RATE;
    }
    /// <summary>
    /// 两次攻击的间隔时间
    /// </summary>
    private readonly float ATTACK_INTERVAL = 3F;
    public virtual float GetAttackInterval()
    {
        return ATTACK_INTERVAL;
    }
    /// <summary>
    /// 基础移动速率
    /// </summary>
    private readonly float MOVE_SPEED_BASE = 0.03F;
    public virtual float GetMoveSpeed()
    {
        return MOVE_SPEED_BASE;
    }
    /// <summary>
    /// 每次移动的时间
    /// </summary>
    private readonly float MOVE_TIME = 3F;
    public virtual float GetMoveTime()
    {
        return MOVE_TIME;
    }
    /// <summary>
    /// 增加移动时间的随机性
    /// </summary>
    private readonly float MOVE_TIME_PR_LOWER = 0.8f;
    public virtual float GetMoveTimePRLower()
    {
        return MOVE_TIME_PR_LOWER;
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

    /// <summary>
    /// 初始化动画状态名称
    /// </summary>
    protected abstract void InitAnimStateName();

    public void Init(int characterId)
    {
        //初始化本对象的实时战斗信息
        this.characterId = characterId;
        //初始化难度系数
        InitDegreeOfDifficulty();
       //生成实时敌人战斗信息
        RealtimeEnemyInfoConstruct();
        //状态机获取
        Animator = GetComponent<Animator>();
        //动画名称初始化
        stateAnimNames.Clear();
        InitAnimStateName();
        //角色动画回调方法绑定
        EventCenter.Instance.SendEvent(SGEventType.AnimCallbackRigister, new EventData(null, gameObject));
        //事件中心事件注册
        EventCenter.Instance.RegistListener(SGEventType.AttackJudge, BeAttack);
        //音源获取
        AudioSource = GetComponent<AudioSource>();
        if (AudioSource == null)
            AudioSource = gameObject.AddComponent<AudioSource>();
        //战场player集合清空
        players.Clear();
        //初始化当前状态
        CurrentState = AIState.Idle;
        //初始化当前状态类
        State = StateFactory.Instance.GetStateInstance(CurrentState);
        State.OnEnter(this);
        //激活
        Active = true;
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
                HardFactor = 0.8f;
                break;
            case DifficultyType.Normal:
                HardFactor = 1.0f;
                break;
            case DifficultyType.Hard:
                HardFactor = 1.2f;
                break;
        }
    }
    /// <summary>
    /// 构造实时战斗信息
    /// </summary>
    private void RealtimeEnemyInfoConstruct()
    {
        //data 中参数存着BattleCharacter
        BattleCharacter bc = BattleController.Instance.GetBattleCharacter(characterId);
        realInfo = new RealtimeBattleInfo();

        realInfo.iconName = bc.headIconName; //头像
        realInfo.hp = (int)(bc.hp * HardFactor);
        realInfo.hpMax = realInfo.hp;
        realInfo.mdefense =(int) (bc.mdefense*HardFactor);
        realInfo.pdefense = (int)(bc.pdefense*HardFactor);
    }

    //循环敌人AI
    private void Update()
    {
        if (!Active) return;

        if(CurrentState!=State.GetState())//若当前状态不一致，需要退出了
        {
            State.OnExit(this);
            State = StateFactory.Instance.GetStateInstance(CurrentState);
            State.OnEnter(this);//初始化新状态
        }
        State.Update(this);
    }

    /// <summary>
    /// 设置动画映射
    /// </summary>
    /// <param name="state"></param>
    /// <param name="name"></param>
    protected void AddAnimName(State state, string name)
    {
        stateAnimNames.Add(state, name);
    }

    /// <summary>
    /// 查找当前战场上的玩家信息
    /// </summary>
    /// <returns></returns>
    public  List<Transform> SearchTarget()
    {
        //每次查找目标前清空
        players.Clear();
        //查找，完成后结合中拥有当前最新的玩家信息
        BattleController.Instance.GetCurrentPlayer(new EventCallBack(GetPlayersCallback));
        return players;
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
    /// 是否正在运行目标状态的动画
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool IsInState(State state)
    {
        string name = null;
        if (!stateAnimNames.TryGetValue(state, out name))
            Debug.LogError("can not find this state anim name");
        return AnimationController.Instance.IsInStateAnim(name, this);
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
        transform.Translate(dir*GetMoveSpeed(),Space.Self);
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
    /// 设置血量
    /// </summary>
    protected void SetHp(int hp)
    {
        realInfo.hp = hp;
    }

    private void Revert()
    {
        //取消事件中心注册
        EventCenter.Instance.RemoveListener(SGEventType.AttackJudge, BeAttack);
    }


    #region 回调方法
    protected void FinishedToIdle(string state)
    {
        AIState aiState = (AIState)Enum.Parse(typeof(AIState), state);
        if (CurrentState != aiState) //说明已经转换了。不用动画来判断，因为动画比条件滞后
            return;
        CurrentState = AIState.Idle;
    }
    /// <summary>
    /// 收到攻击的回调方法
    /// </summary>
    /// <param name="data"></param>
    private void BeAttack(EventData data)
    {
        Skill skill = data.Param as Skill;
        string type = skill.attackType;
        int damage = skill.damage;
        string effect = skill.effect;
        float distance = skill.transformDis;
        //伤害判定
        DamageJudge(type, damage);
        //动画判断
        AnimJudge(effect);
    }
    #endregion


    /// <summary>
    /// 伤害计算
    /// </summary>
    /// <param name="type"></param>
    /// <param name="damage"></param>
    protected virtual  void DamageJudge(string type,int damage)
    {
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
    protected virtual void AnimJudge(string effect)
    {
    }


}
