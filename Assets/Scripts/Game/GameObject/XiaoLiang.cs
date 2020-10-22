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
    private readonly float MOVE_SPEED = 1.5f;

    protected override float GetBaseMoveSpeed()
    {
        return MOVE_SPEED;
    }
    #endregion

    public override List<AnimCallBackEntity> GetCallBacks()
    {
        List<AnimCallBackEntity> res = new List<AnimCallBackEntity>();
        //动作结束回到站立 动作回调
        res.Add(new AnimCallBackEntity("Player2_Attack1", "FinishedToIdle", 0.9f, AnimEventParamType.Null)); //exittime 0.95
        res.Add(new AnimCallBackEntity("Player2_Attack2", "FinishedToIdle", 0.9f, AnimEventParamType.Null));//exittime 0.95
        res.Add(new AnimCallBackEntity("Player2_Attack3", "FinishedToIdle", 0.9f, AnimEventParamType.Null));//exittime 0.95
        res.Add(new AnimCallBackEntity("Player2_Attack5", "FinishedToIdle", 0.9f, AnimEventParamType.Null));//exittime 0.95 ,run attack
        res.Add(new AnimCallBackEntity("Player2_Jump", "FinishedToIdle", 0.9f, AnimEventParamType.Null));//exittime 0.95 
        res.Add(new AnimCallBackEntity("Player2_Attack4", "FinishedToIdle", 0.9f, AnimEventParamType.Null));
        res.Add(new AnimCallBackEntity("Player2_Attack6", "FinishedToIdle", 0.9f, AnimEventParamType.Null));
        res.Add(new AnimCallBackEntity("Player2_BeAttacked", "FinishedToIdle", 0.75f, AnimEventParamType.Null));//exittime 0.8
        res.Add(new AnimCallBackEntity("Player2_ToFloor", "FinishedToIdle", 0.75f, AnimEventParamType.Null));//exittime 0.8
        res.Add(new AnimCallBackEntity("Player2_ToFloor", "BreakToFloorToDie", 0.7f, AnimEventParamType.Null));//中断停止当前动画，不会回到idle

        //声音回调
        res.Add(new AnimCallBackEntity("Player2_Attack1", "AttackSound", 0.5f, AnimEventParamType.String, "Player2_Attack_First|1.0"));
        res.Add(new AnimCallBackEntity("Player2_Attack2", "AttackSound", 0.5f, AnimEventParamType.String, "Player2_Attack_Second|1.0"));
        res.Add(new AnimCallBackEntity("Player2_Attack3", "AttackSound", 0.5f, AnimEventParamType.String, "Player2_Attack_Third|1.0"));
        res.Add(new AnimCallBackEntity("Player2_Attack5", "AttackSound", 0.5f, AnimEventParamType.String, "Player2_Run_Attack|1.0"));
        res.Add(new AnimCallBackEntity("Player2_Attack4", "AttackSound", 0.1f, AnimEventParamType.String, "Player2_SkillAttack_UpDown|1.0"));
        res.Add(new AnimCallBackEntity("Player2_Attack6", "AttackSound", 0.5f, AnimEventParamType.String, "Player2_SkillAttack_Sky|1.0"));

        //攻击判定
        res.Add(new AnimCallBackEntity("Player2_Attack1", "AttackParticleCreate", 0.5f, AnimEventParamType.String, 
            GetSkillId(PlayerSkill.AttackFirst).ToString()+"|"+ "Player2_Particle01"));
        res.Add(new AnimCallBackEntity("Player2_Attack2", "AttackJudge", 0.5f, AnimEventParamType.Int, GetSkillId(PlayerSkill.AttackSecond)));
        res.Add(new AnimCallBackEntity("Player2_Attack3", "AttackJudge", 0.5f, AnimEventParamType.Int, GetSkillId(PlayerSkill.AttackThird)));
        res.Add(new AnimCallBackEntity("Player2_Attack5", "AttackJudge", 0.2f, AnimEventParamType.Int, GetSkillId(PlayerSkill.AttackRun)));
        res.Add(new AnimCallBackEntity("Player2_Attack4", "AttackJudge", 0.5f, AnimEventParamType.Int, GetSkillId(PlayerSkill.AttackAce)));
        res.Add(new AnimCallBackEntity("Player2_Attack6", "AttackJudge", 0.5f, AnimEventParamType.Int, GetSkillId(PlayerSkill.AttackJump)));
        return res;
    }

    /// <summary>
    /// 初始化本角色的状态与动画名称
    /// </summary>
    protected override void InitAnimStateName()
    {
        AddAnimName(PlayerAnim.Idle, "Player2_Idle");
        AddAnimName(PlayerAnim.Walk, "Player2_Walk");
        AddAnimName(PlayerAnim.Jump, "Player2_Jump");
        AddAnimName(PlayerAnim.Run, "Player2_Run");
        AddAnimName(PlayerAnim.AttackFirst, "Player2_Attack1");
        AddAnimName(PlayerAnim.AttackSecond, "Player2_Attack2");
        AddAnimName(PlayerAnim.AttackThird, "Player2_Attack3");
        AddAnimName(PlayerAnim.RunAttack, "Player2_Attack5");
        AddAnimName(PlayerAnim.SkillAttackOne, "Player2_Attack4");
        AddAnimName(PlayerAnim.SkillAttackTwo, "Player2_Attack6");
        AddAnimName(PlayerAnim.BeAttack, "Player2_BeAttacked");
        AddAnimName(PlayerAnim.ToFloor, "Player2_ToFloor");
    }
    
    protected override void InitSkillMapping()
    {
        int characterId = GameController.Instance.GetCharacterId(GetPlayerId());
        BattleCharacter bc = BattleController.Instance.GetBattleCharacter(characterId);

        AddSkillMapping(PlayerSkill.AttackFirst, bc.skill1_id);
        AddSkillMapping(PlayerSkill.AttackSecond, bc.skill2_id);
        AddSkillMapping(PlayerSkill.AttackThird, bc.skill3_id);
        AddSkillMapping(PlayerSkill.AttackRun, bc.skill4_id);
        AddSkillMapping(PlayerSkill.AttackAce, bc.skill5_id);
        AddSkillMapping(PlayerSkill.AttackJump, bc.skill6_id);
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
                if (IsInAnim(PlayerAnim.Idle) || IsInTransforming("Player2_Walk -> Player2_Idle"))
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, false));
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Running", gameObject, true));
                }
                break;
            case 2://上挑技能攻击
                if ((IsInAnim(PlayerAnim.Idle) && (GetStateBool("Idleing"))) || IsInTransforming("Player2_Walk -> Player2_Idle"))
                {
                    if (!MagicJudge(GetSkillId(PlayerSkill.AttackAce))||!PowerJudge(GetSkillId(PlayerSkill.AttackAce)))
                        return;

                    //魔法值和能量消耗
                    ReduceMp(GetSkillId(PlayerSkill.AttackAce));
                    ReducePower(GetSkillId(PlayerSkill.AttackAce));

                    EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, false));
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("SkillAttack1", gameObject));
                }
                else if ((IsInAnim(PlayerAnim.Walk) && (GetStateBool("Walking"))) || IsInTransforming("Player2_Idle -> Player2_Walk"))
                {
                    if (!MagicJudge(GetSkillId(PlayerSkill.AttackAce)) || !PowerJudge(GetSkillId(PlayerSkill.AttackAce)))
                        return;
                    //魔法值和能量消耗
                    ReduceMp(GetSkillId(PlayerSkill.AttackAce));
                    ReducePower(GetSkillId(PlayerSkill.AttackAce));

                    EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", gameObject, false));
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("SkillAttack1", gameObject));
                }
                else if (IsInAnim(PlayerAnim.AttackThird) && GetCurrentTime() < 0.85)
                {
                    if (!MagicJudge(GetSkillId(PlayerSkill.AttackAce)) || !PowerJudge(GetSkillId(PlayerSkill.AttackAce)))
                        return;
                    //魔法值和能量消耗
                    ReduceMp(GetSkillId(PlayerSkill.AttackAce));
                    ReducePower(GetSkillId(PlayerSkill.AttackAce));

                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("SkillAttack1", gameObject));
                }
                break;

            case 3:
            case 4://俯冲攻击
                if (IsInAnim(PlayerAnim.Jump) && GetCurrentTime() < 0.76 && GetCurrentTime() > 0.26 && !IsInTransforming())
                {
                    if (!MagicJudge(GetSkillId(PlayerSkill.AttackJump)) || !PowerJudge(GetSkillId(PlayerSkill.AttackJump)))
                        return;
                    //魔法值和能量消耗
                    ReduceMp(GetSkillId(PlayerSkill.AttackJump));
                    ReducePower(GetSkillId(PlayerSkill.AttackJump));

                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("SkillAttack2", gameObject));
                    Vector3 dir = transform.localScale.x > 0 ? Vector3.left : Vector3.right;
                    StartCoroutine("AnimTranslate", new AnimTranslateParam(GetSkyAttackTransformTime(), GetSkyAttackTransformSpeedScale(), dir));
                }
                break;

        }
    }
    protected override void BeNormalAttackedAnimPlay()
    {
        if (IsInAnim(PlayerAnim.ToFloor) || PlayerState == PlayerState.Transform ||
           PlayerState == PlayerState.Die || IsInTransforming("AnyState -> Player2_ToFloor") ||
           IsInTransforming("Player2_ToFloor -> Player2_Idle")) //若正处于倒地动画或角色受击位移状态或死亡状态，不处理动画
            return;

        //bug:)同一帧中多个trigger都触发了， 无法消除多余trigger

        if (GetStateBool("Idleing"))//站立bool可以包括所有转换至idle的情况(也包括idle回调之后)
        {
            //关闭站立动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, false));
        }
        else if (GetStateBool("Walking"))
        {
            //关闭行走动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", gameObject, false));
        }
        else if (GetStateBool("Running"))
        {
            //关闭奔跑动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Running", gameObject, false));
        }
        //发送受击的trigger即可
        EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("BeAttack", gameObject));
    }

    protected override void BeToFloorAtttackAnimPlay()
    {
        if (PlayerState == PlayerState.Die || PlayerState == PlayerState.Transform || IsInAnim(PlayerAnim.ToFloor) ||
             IsInTransforming("AnyState -> Player2_ToFloor")||IsInTransforming("Player2_ToFloor -> Player2_Idle"))
            return;//不做处理
        if (GetStateBool("Idleing"))//站立bool可以包括所有转换至idle的情况(也包括idle回调之后)
        {
            //关闭站立动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", gameObject, false));
        }
        else if (GetStateBool("Walking"))
        {
            //关闭行走动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", gameObject, false));
        }
        else if (GetStateBool("Running"))
        {
            //关闭奔跑动画
            EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Running", gameObject, false));
        }
        //发送倒地的trigger即可
        EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("ToFloor", gameObject));
    }

   
    public override float GetParticleMoveSpeed()
    {
        return 4f;
    }

}
