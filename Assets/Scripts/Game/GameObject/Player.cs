using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

/// <summary>
/// 玩家基类
/// </summary>
public abstract class Player : MonoBehaviour,IAnim
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
    private Dictionary<State,string> stateAnimNames = new Dictionary<State, string>();
    /// <summary>
    /// 本模型的动画状态机
    /// </summary>
    private Animator animator = null;
    /// <summary>
    /// 本模型的音源
    /// </summary>
    private AudioSource audioSource = null;
    #endregion

    #region 常量
    /// <summary>
    /// 奔跑速度放大倍数，上下
    /// </summary>
    protected readonly float  RUN_SPEED_SCALE_UPDOWN=1.5F;
    /// <summary>
    /// 奔跑速度放大倍数，左右
    /// </summary>
    protected readonly float RUN_SPEED_SCALE_LEFTRIGHT = 2.0F;
    /// <summary>
    /// 站立跳跃移动速度放大倍数
    /// </summary>
    protected readonly float IDLE_JUMP_SPEED_SCALE = 0.2F;
    /// <summary>
    /// 行走跳跃移动速度放大倍数
    /// </summary>
    protected readonly float WALK_JUMP_SPEED_SCALE = 0.3F;
    /// <summary>
    /// 奔跑跳跃移动速度放大倍数
    /// </summary>
    protected readonly float RUN_JUMP_SPEED_SCALE = 0.5F;
    /// <summary>
    /// 基础移动速度
    /// </summary>
    private readonly float BASE_MOVE_SPEED = 0.04F;
    #endregion

    #region 判断变量
    protected JumpStyle jumpStyle = JumpStyle.Idle;
    #endregion

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(int playerId)
    {
        //本玩家的角色信息初始化;
        this.playerId = playerId;
        jumpStyle = JumpStyle.Idle;
        //玩家动画状态初始化
        stateAnimNames.Clear();
        InitAnimStateName();
        //实时玩家战斗信息构造
        RealtimePlayerInfoConstruct();
        //动画状态机获取
        animator = GetComponent<Animator>();
        //音源获取
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        //角色动画回调方法绑定
        EventCenter.Instance.SendEvent(SGEventType.AnimCallbackRigister, new EventData(null, gameObject));
        //监听事件绑定
        EventCenter.Instance.RegistListener(SGEventType.ComboId, ComboIdListener);
        EventCenter.Instance.RegistListener(SGEventType.Command, CommandListener);
        EventCenter.Instance.RegistListener(SGEventType.CommandNoneDir, CommandNoneDirListener);
        EventCenter.Instance.RegistListener(SGEventType.BattleGetPlayers, GetPlayersListener);
        //发送player初始化完成事件。(UI收到事件后可发出显示请求)
        EventCenter.Instance.SendEvent(SGEventType.PlayerInitFinished, new EventData(this,null));
    }
    /// <summary>
    /// 初始化动画状态名称
    /// </summary>
    protected abstract void InitAnimStateName();
   
    protected void AddAnimName(State state,string name)
    {
        stateAnimNames.Add(state, name);
    }

    /// <summary>
    /// 是否正在运行目标状态的动画
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    protected bool IsInState(State state)
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
    protected float GetAnimLength(State state)
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
        //data 中参数存着BattleCharacter
        int characterId = GameController.Instance.GetCharacterId(playerId);
        BattleCharacter bc = BattleController.Instance.GetBattleCharacter(characterId);
        realInfo = new RealtimeBattleInfo();

        realInfo.iconName = bc.headIconName; //玩家头像
        realInfo.hp = bc.hp;
        realInfo.hpMax = bc.hp;
        realInfo.mp = bc.mp;
        realInfo.mpMax = bc.mp;
        realInfo.level = 1;
        realInfo.exp = 0;
        realInfo.power = 1;
        realInfo.mdefense = bc.mdefense;
        realInfo.pdefense = bc.pdefense;
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
    public float GetHp()
    {
       return (float)realInfo.hp / realInfo.hpMax;
    }
    /// <summary>
    /// 获得mp值
    /// </summary>
    /// <param name="data"></param>
    public float GetMp()
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
    /// 获得等级监听
    /// </summary>
    /// <param name="data"></param>
    public int GetLevel()
    {
        return realInfo.level;
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
    }
    /// <summary>
    /// 跳跃方法
    /// </summary>
    protected virtual void Jump()
    {
    }
    /// <summary>
    /// 移动方法
    /// </summary>
    /// <param name="key"></param>
    protected virtual void Move(GameKey key)
    {
    }
    /// <summary>
    /// 站立方法
    /// </summary>
    protected virtual void Idle()
    {
    }
    /// <summary>
    /// 连击方法
    /// </summary>
    /// <param name="comboIndex"></param>
    protected virtual void Combo(int comboIndex)
    {
    }
    /// <summary>
    /// 当玩家实体对象返回对象池时，对本脚本的监听做解绑操作
    /// </summary>
    public void Revert()
    {
        EventCenter.Instance.RemoveListener(SGEventType.ComboId, ComboIdListener);
        EventCenter.Instance.RemoveListener(SGEventType.Command, CommandListener);
        EventCenter.Instance.RegistListener(SGEventType.CommandNoneDir, CommandNoneDirListener);
        EventCenter.Instance.RegistListener(SGEventType.BattleGetPlayers, GetPlayersListener);
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
        transform.Translate(dir * GetMoveSpeed() * scale, Space.Self);
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

    /// <summary>
    /// 获得移动基础速率
    /// </summary>
    /// <returns></returns>
    protected virtual float GetMoveSpeed()
    {
        return BASE_MOVE_SPEED;
    }

    #region 回调方法
    /// <summary>
    /// 动作结束，转为站立动作
    /// </summary>
    protected void FinishedToIdle()
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
        SoundParam p = new SoundParam(this.audioSource, type, true,false,true, volumeScale);
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData (p, null));
    }
    /// <summary>
    /// 攻击伤害招式判定
    /// </summary>
    protected void AttackJudge(int skillId)
    {
        BattleController.Instance.AttackJudge(skillId);
    }
    #endregion
    
    #region 协程回调
    protected IEnumerator AnimTranslate(AnimTranslateParam param)
    {
        float normalizeTime  = param.normalizeTime;
        float speedScale = param.speedScale;
        bool isRight = transform.localScale.x < 0 ? true : false;
        Vector3 dir = isRight ? Vector3.right : Vector3.left;
        float timeCount = 0;
        while (timeCount<normalizeTime)
        {
            timeCount += Time.deltaTime;
            Translate(dir, speedScale);
            yield return null;
        }
    }
    #endregion
}

/// <summary>
/// 进行动画协程调用的参数
/// </summary>
public struct AnimTranslateParam
{
    public float normalizeTime;
    public float speedScale;

    public AnimTranslateParam(float endNormalizeTime,float speedScale)
    {
        normalizeTime = endNormalizeTime;
        this.speedScale = speedScale;
    }
}

