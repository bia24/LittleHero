using System.Collections;
using System.Collections.Generic;
using SGModule;
using UnityEngine;

public class XiaoLiang : Player
{
    #region 只读参数
    /// <summary>
    /// 基础移动速度
    /// </summary>
    private readonly float MOVE_SPEED = 0.03f;

    protected override float GetMoveSpeed()
    {
        return MOVE_SPEED;
    }
    /// <summary>
    /// 冲刺攻击移动速度倍数
    /// </summary>
    private readonly float RUN_ATTACK_MOVE_SPEED_SCALE = 0.8f;
    #endregion

    public override List<AnimCallBackEntity> GetCallBacks()
    {
        List<AnimCallBackEntity> res = new List<AnimCallBackEntity>();
        //动作结束回到站立 动作回调
        res.Add(new AnimCallBackEntity("Player2_Attack1", "FinishedToIdle", 0.9f, AnimEventParamType.Null)); //exitime 0.95
        res.Add(new AnimCallBackEntity("Player2_Attack2", "FinishedToIdle", 0.9f, AnimEventParamType.Null));//exitime 0.95
        res.Add(new AnimCallBackEntity("Player2_Attack3", "FinishedToIdle", 0.9f, AnimEventParamType.Null));//exitime 0.95
        res.Add(new AnimCallBackEntity("Player2_Attack5", "FinishedToIdle", 0.9f, AnimEventParamType.Null));//exitime 0.95 ,run attack
        res.Add(new AnimCallBackEntity("Player2_Jump", "FinishedToIdle", 0.9f, AnimEventParamType.Null));//exitime 0.95 
        res.Add(new AnimCallBackEntity("Player2_Attack4", "FinishedToIdle", 0.9f, AnimEventParamType.Null));
        res.Add(new AnimCallBackEntity("Player2_Attack6", "FinishedToIdle", 0.9f, AnimEventParamType.Null));
        return res;
    }


    /// <summary>
    /// 初始化本角色的状态与动画名称
    /// </summary>
    protected override void InitAnimStateName()
    {
        AddAnimName(State.Idle, "Player2_Idle");
        AddAnimName(State.Walk, "Player2_Walk");
        AddAnimName(State.Jump, "Player2_Jump");
        AddAnimName(State.Run, "Player2_Run");
        AddAnimName(State.AttackFirst, "Player2_Attack1");
        AddAnimName(State.AttackSecond, "Player2_Attack2");
        AddAnimName(State.AttackThird, "Player2_Attack3");
        AddAnimName(State.RunAttack, "Player2_Attack5");
        AddAnimName(State.SkillAttackOne, "Player2_Attack4");
        AddAnimName(State.SkillAttackTwo, "Player2_Attack6");
        //AddAnimName(State.BeAttackNormal, "Player2_BeAttacked");
        //AddAnimName(State.BeAttackFloor, "Player2_ToFloor");
        //AddAnimName(State.BeAttackOver, "Player2_ToFloor");
        //AddAnimName(State.BeAttackTransform, "Player2_BeAttacked");
        //AddAnimName(State.BeDie, "Player2_ToFloor");
    }

    protected override void Move(GameKey key)
    {
        //1.若站立
        if (IsInState(State.Idle) && !IsInTransforming() && !GetStateBool("Running"))
        {
            //关闭 站立 动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, false));
            //打开 步行 动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", gameObject, true));
        }

        //2.若走路
        if (IsInState(State.Walk) || IsInTransforming("Player2_Idle -> Player2_Walk"))
        {
            switch (key)
            {
                case GameKey.Up: //y
                    Translate(new Vector3(0, 1, 0));
                    break;
                case GameKey.Down: //-y
                    Translate(new Vector3(0, -1, 0));
                    break;
                case GameKey.Left: //-x
                    Translate(new Vector3(-1, 0, 0));
                    Rotate(false);
                    break;
                case GameKey.Right: //x
                    Translate(new Vector3(1, 0, 0));
                    Rotate(true);
                    break;
            }
        }

        //3.若跑步
        if (IsInState(State.Run) || IsInTransforming("Player2_Idle -> Player2_Run"))
        {
            switch (key)
            {
                case GameKey.Up: //y
                    Translate(new Vector3(0, 1, 0), RUN_SPEED_SCALE_UPDOWN);
                    break;
                case GameKey.Down: //-y
                    Translate(new Vector3(0, -1, 0), RUN_SPEED_SCALE_UPDOWN);
                    break;
                case GameKey.Left: //-x
                    Translate(new Vector3(-1, 0, 0), RUN_SPEED_SCALE_LEFTRIGHT);
                    Rotate(false);
                    break;
                case GameKey.Right: //x
                    Translate(new Vector3(1, 0, 0), RUN_SPEED_SCALE_LEFTRIGHT);
                    Rotate(true);
                    break;
            }
        }


        if (IsInState(State.Jump))
        {
            float speedScale = 0f;
            switch (jumpStyle)
            {
                case JumpStyle.Idle:
                    speedScale = IDLE_JUMP_SPEED_SCALE;
                    break;
                case JumpStyle.Walk:
                    speedScale = WALK_JUMP_SPEED_SCALE;
                    break;
                case JumpStyle.Run:
                    speedScale = RUN_JUMP_SPEED_SCALE;
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
    /// 当方向键无输入时，触发站立方法
    /// </summary>
    protected override void Idle()
    {
        if (IsInState(State.Idle))
            return;
        if (IsInState(State.Walk) && !IsInTransforming()) //若在走路
        {
            //关闭行走动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", gameObject, false));
            //打开站立动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, true));
        }
        if (IsInState(State.Run) && !IsInTransforming())//若在跑步
        {
            //关闭跑步动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Running", gameObject, false));
            //打开站立动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, true));
        }
    }

    /// <summary>
    /// 接受到了“攻击”指令的操作
    /// </summary>
    protected override void Attack()
    {
        if (IsInTransforming())//状态转移时不接受指令
            return;
        if (IsInState(State.Idle))//如果是站立状态
        {
            //关闭站立动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, false));
            //开启攻击 1状态
            EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack1", gameObject));
        }
        else if (IsInState(State.Walk)) //如果是行走状态
        {
            //关闭 行走 动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", gameObject, false));
            //开启攻击 1状态
            EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack1", gameObject));
        }
        else if (IsInState(State.AttackFirst) && GetCurrentTime() < 0.85) //如果是一段攻击的状态，当一段攻击 运行到0.85时，可接受连击
        {
            EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack2", gameObject));
        }
        else if (IsInState(State.AttackSecond) && GetCurrentTime() < 0.85)
        {
            EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack3", gameObject));
        }
        else if (IsInState(State.Run))//如果是跑步状态，冲刺攻击
        {
            //关闭 跑步 动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Running", gameObject, false));
            //触发 攻击5
            EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("RunAttack", gameObject));
            //移动物体冲刺距离
            float endTime = GetAnimLength(State.RunAttack) * 0.9f;
            StartCoroutine("AnimTranslate", new AnimTranslateParam(endTime, RUN_ATTACK_MOVE_SPEED_SCALE));
        }

    }

    /// <summary>
    /// 跳跃动作
    /// </summary>
    protected override void Jump()
    {
        if (IsInTransforming()) //当正在转移，说明有其它指令，不进行跳跃
            return;

        if (!(IsInState(State.Idle) || IsInState(State.Walk) || IsInState(State.Run)))
            return;

        if (IsInState(State.Idle))
        {
            //关闭站立动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, false));
            //站立跳跃状态
            jumpStyle = JumpStyle.Idle;
        }
        else if (IsInState(State.Walk))
        {
            //关闭 行走 动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", gameObject, false));
            jumpStyle = JumpStyle.Walk;
        }
        else
        {
            //关闭 跑步 动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Running", gameObject, false));
            jumpStyle = JumpStyle.Run;
        }

        //触发 跳跃
        EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Jump", gameObject));
    }



    /// <summary>
    /// 收到连击指令
    /// </summary>
    /// <param name="comboIndex"></param>
    protected override void Combo(int comboIndex)
    {

        switch (comboIndex)
        {
            case 0://奔跑
            case 1:
                //动画
                if (IsInState(State.Idle) || IsInTransforming("Player2_Walk -> Player2_Idle"))
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, false));
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Running", gameObject, true));
                }
                break;
            case 2://上挑技能攻击
                if (IsInState(State.Idle) || IsInTransforming("Player2_Walk -> Player2_Idle"))
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, false));
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("SkillAttack1", gameObject));
                }
                else if (IsInState(State.Walk) || IsInTransforming("Player2_Idle -> Player2_Walk"))
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", gameObject, false));
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("SkillAttack1", gameObject));
                }
                else if (IsInState(State.AttackThird) && GetCurrentTime() < 0.85)
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("SkillAttack1", gameObject));
                }
                break;

            case 3:
            case 4://俯冲攻击
                if (IsInState(State.Jump) && GetCurrentTime() < 0.76 && GetCurrentTime() > 0.26 && !IsInTransforming())
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("SkillAttack2", gameObject));
                    StartCoroutine("AnimTranslate", new AnimTranslateParam(0.5f, 0.8f));
                }
                break;

        }
    }
}
