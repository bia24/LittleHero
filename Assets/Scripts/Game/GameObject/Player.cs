using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

/// <summary>
/// 玩家基类
/// </summary>
public abstract class Player : MonoBehaviour, IAnim,IAttacker,IStrengthenSkill,IParticleMoveSpeed
{
    #region 产生玩家是进行初始化构造的数据
    /// <summary>
    /// 玩家id
    /// </summary>
    private int playerId;
    /// <summary>
    /// 玩家的实时战斗信息
    /// </summary>
    private RealtimeBattleInfo realInfo;
    /// <summary>
    /// 玩家状态到动画名称的映射
    /// </summary>
    private Dictionary<PlayerAnim, string> stateAnimNames = new Dictionary<PlayerAnim, string>();
    /// <summary>
    /// 玩家技能枚举到技能id的映射
    /// </summary>
    private Dictionary<PlayerSkill, int> skillMapping = new Dictionary<PlayerSkill, int>();
    /// <summary>
    /// 本模型的动画状态机
    /// </summary>
    private Animator animator = null;
    /// <summary>
    /// 本模型的音源
    /// </summary>
    public AudioSource AudioSource { get; set; }
    /// <summary>
    /// 能量罩粒子特效产生位置
    /// </summary>
    public Transform PowerParticlePos { get; set; }
    /// <summary>
    /// 能量罩特效
    /// </summary>
    public Transform PowerParticle { get; set; }
    
    #endregion

    #region 常量
    /// <summary>
    /// 奔跑速度放大倍数，上下
    /// </summary>
    private readonly float RUN_SPEED_SCALE_UPDOWN = 1.5F;
    protected virtual float GetRunSpeedScaleUpDown()
    {
        return RUN_SPEED_SCALE_UPDOWN;
    }
    /// <summary>
    /// 奔跑速度放大倍数，左右
    /// </summary>
    private readonly float RUN_SPEED_SCALE_LEFTRIGHT = 2.0F;
    protected virtual float GetRunSpeedScaleLeftRight()
    {
        return RUN_SPEED_SCALE_LEFTRIGHT;
    }
    /// <summary>
    /// 站立跳跃移动速度放大倍数
    /// </summary>
    private readonly float IDLE_JUMP_SPEED_SCALE = 0.2F;
    protected virtual float GetIdleJumpSpeedScale()
    {
        return IDLE_JUMP_SPEED_SCALE;
    }
    /// <summary>
    /// 行走跳跃移动速度放大倍数
    /// </summary>
    private readonly float WALK_JUMP_SPEED_SCALE = 0.4F;
    protected virtual float GetWalkJumpSpeedScale()
    {
        return WALK_JUMP_SPEED_SCALE;
    }
    /// <summary>
    /// 奔跑跳跃移动速度放大倍数
    /// </summary>
    private readonly float RUN_JUMP_SPEED_SCALE = 0.8F;
    protected virtual float GetRunJumpSpeedScale()
    {
        return RUN_JUMP_SPEED_SCALE;
    }
    /// <summary>
    /// 基础移动速度
    /// </summary>
    private readonly float BASE_MOVE_SPEED = 1.6F;
    protected virtual float GetBaseMoveSpeed()
    {
        return BASE_MOVE_SPEED;
    }
    /// <summary>
    /// 冲刺攻击移动速率缩放
    /// </summary>
    private readonly float RUN_ATTACK_MOVE_SPEED_SCALE = 0.8f;
    protected virtual float GetRunAttackMoveSpeedScale()
    {
        return RUN_ATTACK_MOVE_SPEED_SCALE;
    }
    /// <summary>
    /// 受击位移时间
    /// </summary>
    private readonly float BEATTACK_TRANSFORM_TIME = 0.45F;
    protected virtual float GetBeAtttackTransformTime()
    {
        return BEATTACK_TRANSFORM_TIME; 
    }
    /// <summary>
    /// 受击位移速度倍率
    /// </summary>
    private readonly float BEATTACK_TRANSFORM_SPEED_SCALE = 0.8F;
    protected virtual float GetBeAttackTransformSpeedScale()
    {
        return BEATTACK_TRANSFORM_SPEED_SCALE;
    }
    /// <summary>
    /// 俯冲攻击位移速率缩放
    /// </summary>
    private readonly float SKY_ATTACK_TRANSFORM_SPEED_SCALE = 0.8F;
    protected virtual float GetSkyAttackTransformSpeedScale()
    {
        return SKY_ATTACK_TRANSFORM_SPEED_SCALE;
    }
    /// <summary>
    /// 俯冲攻击位移时间
    /// </summary>
    private readonly float SKY_ATTACK_TRANSFORM_TIME = 0.5F;
    protected virtual float GetSkyAttackTransformTime()
    {
        return SKY_ATTACK_TRANSFORM_TIME;
    }
    #endregion

    #region 判断变量
    /// <summary>
    /// 跳跃前的动作
    /// </summary>
    protected JumpStyle JumpStyle { get; set; }
    /// <summary>
    /// 受到攻击的方向
    /// </summary>
    protected Vector3 BeAttackDir { get; set; }
    /// <summary>
    /// 玩家自身的特殊状态
    /// </summary>
    protected PlayerState PlayerState { get; set; }
   
    #endregion

    #region 玩家成长值
    /// <summary>
    /// 血量成长
    /// </summary>
    protected float HpUpRate { get; set; }
    /// <summary>
    /// 物理防御成长
    /// </summary>
    protected float PdefenseUpRate { get; set; }
    /// <summary>
    /// 魔法防御成长
    /// </summary>
    public float MdefenseUpRate { get; set; }
    /// <summary>
    /// 伤害成长
    /// </summary>
    public float DamageUpRate { get; set; }
    #endregion

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(int playerId)
    {
        //本玩家的角色信息初始化;
        this.playerId = playerId;
        JumpStyle = JumpStyle.Idle;
        BeAttackDir = Vector3.left;
        PlayerState = PlayerState.None;
        //玩家动画状态初始化
        stateAnimNames.Clear();
        InitAnimStateName();
        //技能映射初始化，必须在角色动画回调方法绑定之前
        skillMapping.Clear();
        InitSkillMapping();
        //实时玩家战斗信息构造
        RealtimePlayerInfoConstruct();
        //成长值初始化
        PlayerUpRateInit();
        //动画状态机获取
        animator = GetComponent<Animator>();
        //初始化状态机速度
        animator.speed = 1;
        //音源获取
        AudioSource = GetComponent<AudioSource>();
        if (AudioSource == null)
            AudioSource = gameObject.AddComponent<AudioSource>();
        //能量罩位置获取
        if (PowerParticlePos == null)
            PowerParticlePos = transform.Find("QiChangPos");
        //能量特效索引初始化
        PowerParticle = null;
        //角色动画回调方法绑定
        EventCenter.Instance.SendEvent(SGEventType.AnimCallbackRigister, new EventData(null, gameObject));
        //监听事件绑定
        RegisterEvent();
        //发送player初始化完成事件。(UI收到事件后可发出显示请求)
        EventCenter.Instance.SendEvent(SGEventType.PlayerInitFinished, new EventData(this,null));
        //将自己注册进战场
        BattleController.Instance.AddPlayer(this);
    }

    private void RegisterEvent()
    {
        EventCenter.Instance.RegistListener(SGEventType.ComboId, ComboIdListener);
        EventCenter.Instance.RegistListener(SGEventType.Command, CommandListener);
        EventCenter.Instance.RegistListener(SGEventType.CommandNoneDir, CommandNoneDirListener);
        EventCenter.Instance.RegistListener(SGEventType.BattleGetPlayers, GetPlayersListener);
        EventCenter.Instance.RegistListener(SGEventType.AttackJudge, BeAttack);
        EventCenter.Instance.RegistListener(SGEventType.BattleAttackSuccess, AttackSuccessListener);
        EventCenter.Instance.RegistListener(SGEventType.BattleResetPlayerState, PlayerStateResetListener);
    }



    /// <summary>
    /// 事件解绑
    /// </summary>
    private void CancelEvent()
    {
        EventCenter.Instance.RemoveListener(SGEventType.ComboId, ComboIdListener);
        EventCenter.Instance.RemoveListener(SGEventType.Command, CommandListener);
        EventCenter.Instance.RemoveListener(SGEventType.CommandNoneDir, CommandNoneDirListener);
        EventCenter.Instance.RemoveListener(SGEventType.BattleGetPlayers, GetPlayersListener);
        EventCenter.Instance.RemoveListener(SGEventType.AttackJudge, BeAttack);
        EventCenter.Instance.RemoveListener(SGEventType.BattleAttackSuccess, AttackSuccessListener);
        EventCenter.Instance.RemoveListener(SGEventType.BattleResetPlayerState, PlayerStateResetListener);
    }

    private void PlayerStateResetListener(EventData data)
    {
        BattleController.Instance.ResetPlayerState(this);
    }

    /// <summary>
    /// 初始化动画状态名称
    /// </summary>
    protected abstract void InitAnimStateName();
   
    protected void AddAnimName(PlayerAnim state,string name)
    {
        stateAnimNames.Add(state, name);
    }

    /// <summary>
    /// 初始化技能映射
    /// </summary>
    protected abstract void InitSkillMapping();

    protected void AddSkillMapping(PlayerSkill skill,int id)
    {
        skillMapping.Add(skill, id);
    }

    /// <summary>
    /// 获得技能id
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    protected int GetSkillId(PlayerSkill type)
    {
        int id = default;
        if(!skillMapping.TryGetValue(type,out id))
        {
            Debug.LogError("can not find this skillId  : "+ type.ToString());
        }
        return id;
    }
    /// <summary>
    /// 是否正在运行目标状态的动画
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    protected bool IsInAnim(PlayerAnim state)
    {
        string name = null;
        if (!stateAnimNames.TryGetValue(state, out name))
            Debug.LogError("can not find this state anim name");
        return AnimationController.Instance.IsInStateAnim(name, this);
    }
    /// <summary>
    /// 获得指定动画的长度
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    protected float GetAnimLength(PlayerAnim state)
    {
        string name = null;
        if (!stateAnimNames.TryGetValue(state, out name))
            Debug.LogError("can not find this state anim name");
        return AnimationController.Instance.GetAnimtionClipLength(this, name);
    }
    /// <summary>
    /// 是否正在转换动画
    /// </summary>
    /// <returns></returns>
    protected bool IsInTransforming()
    {
        return AnimationController.Instance.IsInTransforming(this);
    }
    /// <summary>
    /// 是否正在转换指定动画
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected bool IsInTransforming(string name)
    {
        return AnimationController.Instance.IsInTransforming(this,name);
    }
    /// <summary>
    /// 获得状态bool参数的值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected bool GetStateBool(string name)
    {
        return AnimationController.Instance.GetStateBool(this,name);
    }
    /// <summary>
    /// 获得当前动画的播放归一化时间
    /// </summary>
    /// <returns></returns>
    protected float GetCurrentTime()
    {
        return AnimationController.Instance.GetAnimCurrentRuningTime(this);
    }

    /// <summary>
    /// 本玩家依据角色生成战斗信息
    /// </summary>
    /// <param name="data"></param>
    private void RealtimePlayerInfoConstruct()
    {
        int characterId = GameController.Instance.GetCharacterId(playerId);
        BattleCharacter bc = BattleController.Instance.GetBattleCharacter(characterId);
        realInfo = new RealtimeBattleInfo();

        realInfo.iconName = bc.headIconName; //玩家头像
        realInfo.hp = bc.hp;
        realInfo.hpMax = bc.hp;
        realInfo.mp = 0;
        realInfo.mpMax = bc.mp;
        realInfo.level = 1;
        realInfo.exp = 0;
        realInfo.power = 0;
        realInfo.mdefense = bc.mdefense;
        realInfo.pdefense = bc.pdefense;
    }
    /// <summary>
    /// 成长值初始化
    /// </summary>
    private void PlayerUpRateInit()
    {
     
        int characterId = GameController.Instance.GetCharacterId(playerId);
        BattleCharacter bc = BattleController.Instance.GetBattleCharacter(characterId);

        HpUpRate = bc.hpUpRate;
        PdefenseUpRate = bc.pdefenseUpRate;
        MdefenseUpRate = bc.mdefenseUpRate;
        DamageUpRate = bc.damageUpRate;
    }

    /// <summary>
    /// 获得id
    /// </summary>
    /// <returns></returns>
    public int GetPlayerId()
    {
        return playerId;
    }
    /// <summary>
    /// 获得头像
    /// </summary>
    /// <param name="data"></param>
    public Sprite GetHeadIcon()
    {
        return TextureManager.Instance.GetTexture<Sprite>(realInfo.iconName);
    }
    /// <summary>
    /// 获得hp值
    /// </summary>
    /// <param name="data"></param>
    public float GetHpRate()
    {
       return (float)realInfo.hp / realInfo.hpMax;
    }
    /// <summary>
    /// 获得mp值
    /// </summary>
    /// <param name="data"></param>
    public float GetMpRate()
    {
        return (float)realInfo.mp / realInfo.mpMax;
    }
    /// <summary>
    /// 获得能量值监听
    /// </summary>
    /// <param name="data"></param>
    public  int GetPowerNumber()
    {
        return realInfo.power;
    }
    /// <summary>
    /// 获得等级
    /// </summary>
    /// <param name="data"></param>
    public int GetLevel()
    {
        return realInfo.level;
    }
    /// <summary>
    /// 等级提升
    /// </summary>
    protected void LevelUp()
    {
        realInfo.level++;
        //等级提升需要改变人物的相关属性
        int characterId = GameController.Instance.GetCharacterId(playerId);
        BattleCharacter bcInfo = BattleController.Instance.GetBattleCharacter(characterId);
        //1.血量上限提升&血量补充满
        int max = (int)(GetHpMax() * bcInfo.hpUpRate);//血量上限是实时数据，不需要pow
        SetHp(max);
        SetHpMax(max);
        //2.物理防御和魔法防御提升
        SetMdefense((int)(GetMdefense() * bcInfo.mdefenseUpRate));
        SetPdefense((int)(GetPdefense() * bcInfo.pdefenseUpRate));
        //发送通知
        EventCenter.Instance.SendEvent(SGEventType.BattlePlayerLevelChange, new EventData(null,gameObject));
        //产生升级特效
        BattleController.Instance.ShowLevelUpParicle(gameObject);
    }
    /// <summary>
    /// 获得实时hp值
    /// </summary>
    /// <returns></returns>
    public int GetHp()
    {
        return realInfo.hp;
    }
    /// <summary>
    /// 获得实时的mp值
    /// </summary>
    /// <returns></returns>
    public int GetMp()
    {
        return realInfo.mp;
    }
    /// <summary>
    /// 获得蓝量最大值
    /// </summary>
    /// <returns></returns>
    private int GetMpMax()
    {
        return realInfo.mpMax;
    }
    /// <summary>
    /// 获得血量最大值
    /// </summary>
    /// <returns></returns>
    private int GetHpMax()
    {
        return realInfo.hpMax;
    }
    /// <summary>
    /// 设置血量的最大值
    /// </summary>
    /// <param name="value"></param>
    private void SetHpMax(int value)
    {
        realInfo.hpMax = value;
        //发送通知
        EventCenter.Instance.SendEvent(SGEventType.BattlePlayerHpChange,new EventData(null,gameObject));
    }
    /// <summary>
    /// 设置血量
    /// </summary>
    /// <param name="value"></param>
    public void SetHp(int value)
    {
        realInfo.hp = value;
        //发送通知
        EventCenter.Instance.SendEvent(SGEventType.BattlePlayerHpChange,new EventData(null,gameObject));
    }
    /// <summary>
    /// 设置蓝量
    /// </summary>
    /// <param name="value"></param>
    public void SetMp(int value)
    {
        realInfo.mp = value;
        //发送通知
        EventCenter.Instance.SendEvent(SGEventType.BattlePlayerMpChange, new EventData(null, gameObject));
    }

    /// <summary>
    /// 经验值增加
    /// </summary>
    public void AddExp()
    {
        if (GetLevel() == BattleController.Instance.GetLevelMax())
            return;// 已经到达了最高等级，不进行经验变化

        realInfo.exp += BattleController.Instance.GetPerExpEnemy();
        int maxExp = BattleController.Instance.GetLevelUpNeedExp(GetLevel());

        while (GetLevel() < BattleController.Instance.GetLevelMax()&&
            realInfo.exp>=maxExp) //当前等级小于等级上限且经验值超过当前经验值上限时
        {
            realInfo.exp -= maxExp;
            LevelUp(); //升一级
            maxExp = BattleController.Instance.GetLevelUpNeedExp(GetLevel());//更新经验上限
        }

    }

    /// <summary>
    /// 蓝量增加，蓝量满了需要增加一颗power
    /// </summary>
    public void AddMagic()
    {
        int perMp = BattleController.Instance.GetPerMpEnemy();

        int mp = GetMp() + perMp;

        while (mp >= GetMpMax()
            &&GetPower()<BattleController.Instance.GetMaxPowerNumber()) //若增加了mp之后，总mp大于最大值mp，增加一颗能量
        {
            AddPower();//增加一颗能量
            mp -= GetMpMax();
        }

        SetMp(Mathf.Clamp(mp, 0, GetMpMax()));
     
    }

    /// <summary>
    /// 增加能量值
    /// </summary>
    private void AddPower()
    {
        int n = GetPower() + 1;
        SetPower(n);
    }

    /// <summary>
    /// 依据技能id 减少能量值
    /// </summary>
    protected void ReducePower(int skillId)
    {
        Skill skill = BattleController.Instance.GetSkill(skillId);
        int n = GetPower() - skill.powerCost;
        SetPower(n);
    }

    /// <summary>
    /// 设置能量的值
    /// </summary>
    /// <param name="n"></param>
    private void SetPower(int n)
    {
        realInfo.power = n;
        //发送通知
        EventCenter.Instance.SendEvent(SGEventType.BattlePlayerPowerChange, new EventData(null, gameObject));
        //显示能量罩特效
        BattleController.Instance.ShowPowerParticle(this);
    }
    /// <summary>
    /// 获得能量值
    /// </summary>
    /// <returns></returns>
    private int GetPower()
    {
        return realInfo.power;
    }
    /// <summary>
    /// 获得实时物理防御
    /// </summary>
    /// <returns></returns>
    public int GetPdefense()
    {
        return realInfo.pdefense;
    }
    /// <summary>
    /// 获得实时魔法防御
    /// </summary>
    /// <returns></returns>
    public int GetMdefense()
    {
        return realInfo.mdefense;
    }
    /// <summary>
    /// 设置物理防御
    /// </summary>
    /// <param name="value"></param>
    private void SetPdefense(int value)
    {
        realInfo.pdefense = value;
    }

    /// <summary>
    /// 设置魔法防御
    /// </summary>
    /// <param name="value"></param>
    private void SetMdefense(int value)
    {
        realInfo.mdefense = value;
    }


    /// <summary>
    /// 输入命令监听
    /// </summary>
    /// <param name="data"></param>
    private void CommandListener(EventData data)
    {
        int playerId = (int)data.Param;
        if (playerId != GetPlayerId()) return;//不是本玩家的指令

        GameKey key = (GameKey)data.Param2;
        switch (key)
        {
            case GameKey.Attack:
                Attack();
                break;
            case GameKey.Jump:
                Jump();
                break;
            default:
                Move(key);
                break;
        }
    }

    /// <summary>
    /// 输入连击id监听
    /// </summary>
    /// <param name="data"></param>
    private  void ComboIdListener(EventData data)
    {
        int playerId = (int)data.Param;
        if (playerId != GetPlayerId()) return;//不是本玩家的指令
        int comboId = (int)data.Param2;
        Combo(comboId);
    }
    /// <summary>
    /// 持续按键命令监听，只有方向键
    /// </summary>
    /// <param name="data"></param>
    private  void CommandNoneDirListener(EventData data)
    {
        int playerId = (int)data.Param;
        if (playerId != GetPlayerId()) return;//不是本玩家的指令
        Idle();
    }
    /// <summary>
    /// 获取战场上的玩家监听
    /// </summary>
    /// <param name="data"></param>
    private void GetPlayersListener(EventData data)
    {
        //data中分装的是一个回调方法
        EventCallBack ecb = data.Param as EventCallBack;
        ecb?.Invoke(new EventData(null,gameObject));
    }

    /// <summary>
    /// 玩家攻击方法
    /// </summary>
    protected virtual  void Attack()
    {
        if (IsInTransforming())//状态转移时不接受指令
            return;
        if (IsInAnim(PlayerAnim.Idle))//如果是站立状态
        {
            //魔法值判定和能量值判定
            if (!MagicJudge(GetSkillId(PlayerSkill.AttackFirst))||!PowerJudge(GetSkillId(PlayerSkill.AttackFirst)))
                return;
            //魔法值和能量消耗
            ReduceMp(GetSkillId(PlayerSkill.AttackFirst));
            ReducePower(GetSkillId(PlayerSkill.AttackFirst));
            //关闭站立动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, false));
            //开启攻击 1状态
            EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack1", gameObject));
        }
        else if (IsInAnim(PlayerAnim.Walk)) //如果是行走状态
        {
            //魔法值判定和能量值判定
            if (!MagicJudge(GetSkillId(PlayerSkill.AttackFirst)) || !PowerJudge(GetSkillId(PlayerSkill.AttackFirst)))
                return;
            //魔法值和能量消耗
            ReduceMp(GetSkillId(PlayerSkill.AttackFirst));
            ReducePower(GetSkillId(PlayerSkill.AttackFirst));
            //关闭 行走 动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", gameObject, false));
            //开启攻击 1状态
            EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack1", gameObject));
        }
        else if (IsInAnim(PlayerAnim.AttackFirst) && GetCurrentTime() < 0.85) //如果是一段攻击的状态，当一段攻击 运行到0.85时，可接受连击
        {
            //魔法值判定和能量值判定
            if (!MagicJudge(GetSkillId(PlayerSkill.AttackSecond)) || !PowerJudge(GetSkillId(PlayerSkill.AttackSecond)))
                return;
            //魔法值和能量消耗
            ReduceMp(GetSkillId(PlayerSkill.AttackSecond));
            ReducePower(GetSkillId(PlayerSkill.AttackSecond));

            EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack2", gameObject));
        }
        else if (IsInAnim(PlayerAnim.AttackSecond) && GetCurrentTime() < 0.85)
        {
            //魔法值判定和能量值判定
            if (!MagicJudge(GetSkillId(PlayerSkill.AttackThird)) || !PowerJudge(GetSkillId(PlayerSkill.AttackThird)))
                return;

            //魔法值和能量消耗
            ReduceMp(GetSkillId(PlayerSkill.AttackThird));
            ReducePower(GetSkillId(PlayerSkill.AttackThird));

            EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack3", gameObject));
        }
        else if (IsInAnim(PlayerAnim.Run))//如果是跑步状态，冲刺攻击
        {
            //魔法值判定和能量值判定
            if (!MagicJudge(GetSkillId(PlayerSkill.AttackRun)) || !PowerJudge(GetSkillId(PlayerSkill.AttackRun)))
                return;

            //魔法值和能量消耗
            ReduceMp(GetSkillId(PlayerSkill.AttackRun));
            ReducePower(GetSkillId(PlayerSkill.AttackRun));

            //关闭 跑步 动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Running", gameObject, false));
            //触发 攻击5
            EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("RunAttack", gameObject));
            //移动物体冲刺距离
            float endTime = GetAnimLength(PlayerAnim.RunAttack) * 0.9f;
            Vector3 dir = transform.localScale.x > 0 ? Vector3.left : Vector3.right;
            StartCoroutine("AnimTranslate", new AnimTranslateParam(endTime, GetRunAttackMoveSpeedScale(),dir));
        }
    }
    /// <summary>
    /// 跳跃方法
    /// </summary>
    protected virtual void Jump()
    {
        if (IsInTransforming()) //当正在转移，说明有其它指令，不进行跳跃
            return;

        if (!(IsInAnim(PlayerAnim.Idle) || IsInAnim(PlayerAnim.Walk) || IsInAnim(PlayerAnim.Run)))
            return;

        if (IsInAnim(PlayerAnim.Idle))
        {
            //关闭站立动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, false));
            //站立跳跃状态
            JumpStyle = JumpStyle.Idle;
        }
        else if (IsInAnim(PlayerAnim.Walk))
        {
            //关闭 行走 动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", gameObject, false));
            JumpStyle = JumpStyle.Walk;
        }
        else
        {
            //关闭 跑步 动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Running", gameObject, false));
            JumpStyle = JumpStyle.Run;
        }

        //触发 跳跃
        EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Jump", gameObject));
    }
    /// <summary>
    /// 移动方法
    /// </summary>
    /// <param name="key"></param>
    protected virtual void Move(GameKey key)
    {
        //1.若站立
        if (IsInAnim(PlayerAnim.Idle) && !IsInTransforming() && !GetStateBool("Running"))
        {
            //关闭 站立 动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, false));
            //打开 步行 动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", gameObject, true));
        }
        //2.若走路
        if ((IsInAnim(PlayerAnim.Walk) && !IsInTransforming()) || IsInTransforming("Player1_Idle -> Player1_Walk"))
        {
            switch (key)
            {
                case GameKey.Up: //y
                    Translate(new Vector3(0, 1, 0),Time.timeScale);
                    break;
                case GameKey.Down: //-y
                    Translate(new Vector3(0, -1, 0), Time.timeScale);
                    break;
                case GameKey.Left: //-x
                    Translate(new Vector3(-1, 0, 0), Time.timeScale);
                    Rotate(false);
                    break;
                case GameKey.Right: //x
                    Translate(new Vector3(1, 0, 0), Time.timeScale);
                    Rotate(true);
                    break;
            }
        }
        //3.若跑步
        if ((IsInAnim(PlayerAnim.Run) && !IsInTransforming()) || IsInTransforming("Player1_Idle -> Player1_Run"))
        {
            switch (key)
            {
                case GameKey.Up: //y
                    Translate(new Vector3(0, 1, 0), GetRunSpeedScaleUpDown()* Time.timeScale);
                    break;
                case GameKey.Down: //-y
                    Translate(new Vector3(0, -1, 0), GetRunSpeedScaleUpDown() * Time.timeScale);
                    break;
                case GameKey.Left: //-x
                    Translate(new Vector3(-1, 0, 0), GetRunSpeedScaleLeftRight() * Time.timeScale);
                    Rotate(false);
                    break;
                case GameKey.Right: //x
                    Translate(new Vector3(1, 0, 0), GetRunSpeedScaleLeftRight() * Time.timeScale);
                    Rotate(true);
                    break;
            }
        }

        if (IsInAnim(PlayerAnim.Jump) && !IsInTransforming())
        {
            float speedScale = 0f;
            switch (JumpStyle)
            {
                case JumpStyle.Idle:
                    speedScale = GetIdleJumpSpeedScale();
                    break;
                case JumpStyle.Walk:
                    speedScale = GetWalkJumpSpeedScale();
                    break;
                case JumpStyle.Run:
                    speedScale = GetRunJumpSpeedScale();
                    break;
            }
            switch (key)
            {
                case GameKey.Left: //-x
                    Translate(new Vector3(-1, 0, 0), speedScale);
                    Rotate(false);
                    break;
                case GameKey.Right: //x
                    Translate(new Vector3(1, 0, 0), speedScale);
                    Rotate(true);
                    break;
            }
        }
    }
    /// <summary>
    /// 站立方法
    /// </summary>
    protected virtual void Idle()
    {
        if (IsInAnim(PlayerAnim.Idle))
            return;
        if (IsInAnim(PlayerAnim.Walk) && !IsInTransforming()) //若在走路
        {
            //关闭行走动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", gameObject, false));
            //打开站立动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, true));
        }
        if (IsInAnim(PlayerAnim.Run) && !IsInTransforming() && !GetStateBool("RunAttack"))//若在跑步，且没有在转换，且没有run攻击trigger
        {
            //关闭跑步动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Running", gameObject, false));
            //打开站立动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, true));
        }
    }
    /// <summary>
    /// 连击方法
    /// </summary>
    /// <param name="comboIndex"></param>
    protected virtual void Combo(int comboIndex)
    {
    }
   


    /// <summary>
    /// 获得动画控制器
    /// </summary>
    /// <returns></returns>
    public Animator GetAnimator()
    {
        return this.animator;
    }

    /// <summary>
    /// 获得游戏物体的动画回调集合
    /// </summary>
    /// <returns></returns>
    public abstract List<AnimCallBackEntity> GetCallBacks();

    /// <summary>
    /// 位移
    /// </summary>
    /// <param name="dir"></param>
    protected void Translate(Vector3 dir, float scale = 1.0f)
    {
        transform.Translate(dir * GetBaseMoveSpeed()  *Time.fixedDeltaTime* scale, Space.Self);
    }
    /// <summary>
    /// 旋转,出场x为负，朝右
    /// </summary>
    /// <param name="isRight"></param>
    protected void Rotate(bool isRight)
    {
        Vector3 scale = transform.localScale;
        if (isRight) //向右移动
            scale.x = scale.x > 0 ? scale.x * -1 : scale.x;
        else//向左移动
            scale.x = scale.x < 0 ? scale.x * -1 : scale.x;

        transform.localScale = scale;
    }

  

    #region 回调方法
    /// <summary>
    /// 动作结束，转为站立动作
    /// </summary>
    public void FinishedToIdle()
    {
        //若本回调没被调用，说明在本事件时间节点之前，就完成了动画“转换”
        if (IsInTransforming()) //若在转变，说明动画“转换”持续到了本事件的时间节点，在此之前就触发了其他事件，不做处理
            return;
        //否则
        //恢复站立动画
        EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, true));
    }
    /// <summary>
    /// 攻击动作声音播放
    /// </summary>
    protected void AttackSound(string param)
    {
        string[] ps = param.Split('|');
        string type = ps[0];
        float volumeScale = float.Parse(ps[1]);
        SoundParam p = new SoundParam(this.AudioSource, type, false,false,true, volumeScale);
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData (p, null));
    }
    /// <summary>
    /// 攻击伤害招式判定
    /// </summary>
    protected void AttackJudge(int skillId)
    {
        Skill skill = BattleController.Instance.GetSkill(skillId);
        Skill newSkill = skill.Clone() as Skill;
        //强化技能伤害
        newSkill= StrengthenSkill(newSkill);
        //发送新的技能信息
        BattleController.Instance.AttackJudge(newSkill, transform,this);
    }
    /// <summary>
    /// 依据等级，强化技能伤害
    /// </summary>
    /// <param name="skill"></param>
    public virtual Skill StrengthenSkill( Skill skill)
    {
        //伤害还没有和等级相关，发出伤害此时 关联
        skill.damage = (int)(Mathf.Pow(DamageUpRate, GetLevel()) * skill.damage); //skill中的是基础伤害。需要pow
        return skill;
    }
    /// <summary>
    /// 收到攻击的回调方法
    /// </summary>
    protected  void BeAttack(EventData data)
    {
        //param1 skill;sender 攻击者;param2 真正的攻击对象
        GameObject attacker = data.Sender;
        if (attacker.GetComponent<Enemy>() == null) //这是玩家自己的攻击，不做处理
            return;

        Skill skill = data.Param as Skill;
        string type = skill.attackType;
        int damage = skill.damage;
        string effect = skill.effect;
        float x = skill.x;
        float y = skill.y;
        string disType = skill.disType;
       
        IAttacker attackObject=data.Param2 as IAttacker;

        //技能是否命中判断
        if (!SkillDistanceJudge(attackObject, transform, disType, x, y))
            return;

        //若是特效粒子的攻击，这里回收粒子
        if (attackObject.IsParticleAttack())
            attackObject.GetParticle().Revert();

        //伤害判定
        DamageCalculate(type, damage);

        //死亡判定
        if (DieJudge())
        {
            //若死亡
            return;
        }

        //受击面向获得
        BeAttackDir = GetBeAttackDir(attackObject, transform);


        //动画判断
        AnimStateJudge(effect);

        //攻击效果表现
        DoBeAttackEffect(effect, BeAttackDir);

    }
    #endregion

    #region 协程回调
    protected IEnumerator AnimTranslate(AnimTranslateParam param)
    {
        //开启位移状态
        PlayerState = PlayerState.Transform;
        float normalizeTime  = param.normalizeTime;
        float speedScale = param.speedScale;
        Vector3 dir = param.dir;
      
        float timeCount = 0;
        while (timeCount<normalizeTime)
        {
            if (BattleController.Instance.GetGameState()==GameState.Running)
            {
                timeCount += Time.deltaTime;
                Translate(dir, speedScale*Time.timeScale);
            }
            yield return null;
        }
        //取消位移状态
        PlayerState = PlayerState.None;
    }
    #endregion

    /// <summary>
    /// 蓝量计算判定
    /// </summary>
    /// <returns></returns>
    protected bool MagicJudge(int skillId)
    {
        Skill skill= BattleController.Instance.GetSkill(skillId);
        int mp = GetMp();

        if (mp < skill.mpCost)
            return false;//蓝量不够消耗
        else
            return true;
    }
    /// <summary>
    /// 依据技能id减少蓝量
    /// </summary>
    /// <param name="skillId"></param>
    protected void ReduceMp(int skillId)
    {
        Skill skill = BattleController.Instance.GetSkill(skillId);
        int mp = GetMp();
        SetMp(mp - skill.mpCost);
    }
    /// <summary>
    /// 能量值判定，大招和跳跃攻击需要一颗能量值
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    protected bool PowerJudge(int skillId)
    {
        Skill skill = BattleController.Instance.GetSkill(skillId);
        int power = GetPower();

        if (power < skill.powerCost)
            return false;//能量值不够消耗
        else
            return true;
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
        return (Mathf.Abs(beAttacker.localPosition.x - attackObject.GetAttackerLocalPostion().x) <= x && Mathf.Abs(beAttacker.localPosition.y - attackObject.GetAttackerLocalPostion().y) <= y);
    }


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
    /// 获得被攻击者的面向
    /// </summary>
    /// <param name="attackObject"></param>
    /// <param name="beAttacker"></param>
    /// <returns></returns>
    protected Vector3 GetBeAttackDir(IAttacker attackObject, Transform beAttacker)
    {
        return Vector3.Normalize(new Vector3(attackObject.GetAttackerLocalPostion().x - beAttacker.localPosition.x, 0, 0));
    }
    /// <summary>
    /// 受击时依据攻击效果，播放正确的动画
    /// </summary>
    /// <param name="effect"></param>
    protected  void AnimStateJudge(string effect)
    {
        switch (effect)
        {
            case "NormalAttacked": //普通效果攻击，播放beAttack动画即可
                BeNormalAttackedAnimPlay();
                break;
            case "Transform"://位移攻击
                BeNormalAttackedAnimPlay(); //和普通攻击一样播放动画
                break;
            case "ToFloor":
                BeToFloorAtttackAnimPlay(); //倒地攻击播放动画
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 收到普通攻击动画播放
    /// </summary>
    protected virtual void BeNormalAttackedAnimPlay()
    {
    }

    protected virtual void BeToFloorAtttackAnimPlay()
    {
    }

    /// <summary>
    /// 攻击效果表现
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="BeAttackDir"></param>
    protected  void  DoBeAttackEffect(string effect, Vector3 BeAttackDir)
    {
        if (effect.Equals("Transform")) //位移攻击，朝受击方向的反方向移动即可
        {
            StartCoroutine("AnimTranslate",new AnimTranslateParam(GetBeAtttackTransformTime(),
                GetBeAttackTransformSpeedScale(),Vector3.Normalize(-1*BeAttackDir)));
        }
        else if(effect.Equals("ToFloor"))  //倒地攻击
        {
            //nothing
        }
        else if(effect.Equals("Up"))
        {
            Debug.LogWarning("can not match this enemy attack type :  "+ effect);
        }
    }
    /// <summary>
    /// 玩家攻击击打成功的监听
    /// </summary>
    private void AttackSuccessListener(EventData data)
    {
        Player player = data.Param as Player;
        if (player != this) return;//不是本玩家击打成功不做处理

        //攻击成功 ，经验值增加
        player.AddExp();
        //攻击成功，蓝量增加
        player.AddMagic();
    }

    /// <summary>
    /// 检测玩家是否运动到了场景边界，在逻辑update之后最后检测
    /// </summary>
    private void LateUpdate()
    {
        //相机移动
        CameraMoveTest();
        //玩家移动限制区域
        BoundaryClamp();
       
    }

    /// <summary>
    /// 相机移动函数
    /// </summary>
    private void CameraMoveTest()
    {
        if (playerId != 0)
            return;//只对玩家主机1有效

        CameraPos next = default;

        if(transform.localPosition.x<=BattleController.Instance.GetTriggerOne()
            && BattleController.Instance.GetCurrentCameraPos() == CameraPos.Mid)
        {
            //左移
            next = CameraPos.Left;
            EventCenter.Instance.SendEvent(SGEventType.BattleCameraSet, new EventData(next, null));
        }
        else if(transform.localPosition.x>=BattleController.Instance.GetTriggerTwo()
            &&BattleController.Instance.GetCurrentCameraPos()==CameraPos.Left)
        {
            //归中
            next = CameraPos.Mid;
            EventCenter.Instance.SendEvent(SGEventType.BattleCameraSet, new EventData(next, null));
        }
        else if(transform.localPosition.x>=BattleController.Instance.GetTriggerFour()
            &&BattleController.Instance.GetCurrentCameraPos()==CameraPos.Mid)
        {
            //右移
            next = CameraPos.Right;
            EventCenter.Instance.SendEvent(SGEventType.BattleCameraSet, new EventData(next, null));
        }
        else if(transform.localPosition.x<=BattleController.Instance.GetTriggerThree()
            && BattleController.Instance.GetCurrentCameraPos() == CameraPos.Right)
        {
            //归中
            next = CameraPos.Mid;
            EventCenter.Instance.SendEvent(SGEventType.BattleCameraSet, new EventData(next, null));
        }

    }

    private void BoundaryClamp()
    {
        float x = Mathf.Clamp(transform.localPosition.x,BattleController.Instance.GetMoveXMin(),BattleController.Instance.GetMoveXMax());
        float y = transform.localPosition.y;
        if (!IsInAnim(PlayerAnim.Jump) && !IsInAnim(PlayerAnim.SkillAttackTwo))
        {
            y = Mathf.Clamp(transform.localPosition.y,BattleController.Instance.GetMoveYMin(),BattleController.Instance.GetMoveYMax());
        }
        transform.localPosition = new Vector3(x,y,0);
    }


    /// <summary>
    /// 死亡判定&处理方法
    /// </summary>
    public bool DieJudge()
    {
        if (GetHp() > 0)
            return false;
        else
        {
            //1.发送玩家死亡通知
            EventCenter.Instance.SendEvent(SGEventType.BattlePlayerDie,new EventData(null,gameObject));
            //2.取消所有监听
            CancelEvent();
            //3.进行倒地动画播放
            EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("ToFloor", gameObject));
            //4.死亡特效开启
            BattleController.Instance.ShowDieParticle(gameObject);
            //5.设置死亡状态
            PlayerState = PlayerState.Die;
            //4.将自己移除战场列表
            BattleController.Instance.RemovePlayer(this);
            //6. 协程开启回收
            StartCoroutine("DelayRevert", 1f);
            return true;
        }
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
    /// 玩家死亡后的回收工作
    /// </summary>
    protected virtual void Revert()
    {
        //1.能量罩特效回收
        if (PowerParticle != null)
        {
            BattleController.Instance.RevertPowerParticle(this);
        }
        //2.死亡特效回收
        BattleController.Instance.RevertDieParticle(gameObject);
        //3.升级特效回收(若还可见)
        Transform levelUpPos= transform.Find("LevelUpPos");
        for(int i = 0; i < levelUpPos.childCount; i++)
        {
            GameObject p = levelUpPos.GetChild(i).gameObject;
           if (p.activeSelf)
            {
                BattleController.Instance.RevertLevelUpParticle(p);
            }
        }
        //3.本模型回收
        int characterId = GameController.Instance.GetCharacterId(GetPlayerId());
        BattleController.Instance.RevertGameObject(characterId,gameObject);
       
    }

    /// <summary>
    /// 给倒地动画添加中断动画的回调
    /// </summary>
    protected void BreakToFloorToDie()
    {
        if (PlayerState == PlayerState.Die)
        {
            //若处于死亡状态了
            GetAnimator().speed = 0;//暂停当前动画
        }
    }

    public Vector3 GetAttackerLocalPostion()
    {
        return transform.localPosition;
    }

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
    /// 若本单位会产生移动特效，其速度获取
    /// </summary>
    /// <returns></returns>
    public virtual  float GetParticleMoveSpeed()
    {
        return 0f;
    }


    private void OnDestroy()
    {
        //取消所有事件的绑定
        CancelEvent();
        //从战场集合中删除自己
        BattleController.Instance.RemovePlayerRaw(this);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}

/// <summary>
/// 进行动画协程调用的参数
/// </summary>
public struct AnimTranslateParam
{
    public float normalizeTime;
    public float speedScale;
    public Vector3 dir;

    public AnimTranslateParam(float endNormalizeTime,float speedScale,Vector3 dir)
    {
        normalizeTime = endNormalizeTime;
        this.speedScale = speedScale;
        this.dir = dir;
    }
}






