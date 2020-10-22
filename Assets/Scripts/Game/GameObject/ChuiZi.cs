using System.Collections;
using System.Collections.Generic;
using SGModule;
using UnityEngine;

public class ChuiZi : Enemy
{
    public override List<AnimCallBackEntity> GetCallBacks()
    {
        List<AnimCallBackEntity> res = new List<AnimCallBackEntity>();
        //动作结束回到站立 动作回调
        res.Add(new AnimCallBackEntity("Enemy3_Attack", "FinishedToIdle", 1.0f, AnimEventParamType.Null)); //exitime 1.0
        res.Add(new AnimCallBackEntity("Enemy3_BeAttacked", "FinishedToIdle", 1.0f, AnimEventParamType.Null)); //exitime 1.0
        res.Add(new AnimCallBackEntity("Enemy3_Up", "FinishedToIdle", 0.8f, AnimEventParamType.Null)); //exitime 0.85 
          //声音回调
        res.Add(new AnimCallBackEntity("Enemy3_Attack", "AttackSound", 0.5f, AnimEventParamType.String, "Enemy3_Attack|1.0"));
        //攻击判定
        BattleCharacter bc = BattleController.Instance.GetBattleCharacter(GetCharacterId());
        res.Add(new AnimCallBackEntity("Enemy3_Attack", "AttackJudge", 0.4f, AnimEventParamType.Int, bc.skill1_id)); //int skill id
        return res;
    }


    /// <summary>
    /// 发现敌人的反应时间
    /// </summary>
    private readonly float REACTION_TIME = 0.2F;
    public override float GetReactionTime()
    {
        return REACTION_TIME * (2 - HardFactor);
    }

    /// <summary>
    /// 站立发憷时间
    /// </summary>
    private readonly float IDLE_TIME = 0.8F;
    public override float GetIdleTime()
    {
        return IDLE_TIME * (2 - HardFactor);
    }

    /// <summary>
    /// 原地触发攻击概率
    /// </summary>
    private readonly float ATTACK_PR = 0.3F;
    public override float GetAttackPR()
    {
        return ATTACK_PR * HardFactor;
    }

    /// <summary>
    /// 原地攻击触发的帧间隔
    /// </summary>
    private readonly int ATTACK_FRAME_RATE = 80;
    public override int GetAttackFrameRate()
    {
        return (int)(ATTACK_FRAME_RATE * (2 - HardFactor));
    }

    /// <summary>
    /// 两次攻击的间隔时间
    /// </summary>
    private readonly float ATTACK_INTERVAL = 1.2F;
    public override float GetAttackInterval()
    {
        return ATTACK_INTERVAL * (2 - HardFactor);
    }

    /// <summary>
    /// 基础移动速率
    /// </summary>
    private readonly float MOVE_SPEED_BASE = 3F;
    public override float GetMoveSpeed()
    {
        return MOVE_SPEED_BASE * HardFactor;
    }

    /// <summary>
    /// 每次移动的时间
    /// </summary>
    private readonly float MOVE_TIME = 4F;
    public override float GetMoveTime()
    {
        return MOVE_TIME;
    }
    /// <summary>
    /// 增加移动时间的随机性
    /// </summary>
    private readonly float MOVE_TIME_PR_LOWER = 0.5f;
    public override float GetMoveTimePRLower()
    {
        return MOVE_TIME_PR_LOWER;
    }

    /// <summary>
    /// 被攻击后位移的持续时间
    /// </summary>
    private readonly float BEATTACK_TRANSFORM_TIME = 0.4F;
    public override float GetBeAttackTransformTime()
    {
        return BEATTACK_TRANSFORM_TIME * (2 - HardFactor);
    }

    /// <summary>
    /// 倒地时间
    /// </summary>
    private readonly float TOFLOOR_TIME = 0.8F;
    public override float GetFloorTime()
    {
        return TOFLOOR_TIME * (2 - HardFactor);
    }



    /// <summary>
    /// 动画判断 ，boss 在攻击时霸体
    /// </summary>
    /// <param name="effect"></param>
    protected override void AnimStateJudge(string effect)
    {

        //此次被攻击是否无动画表现效果
        if (IsThisBeAttackNoEffect())
            return;

        switch (effect)
        {
            case "NormalAttacked": //普通攻击
                if (State.GetName() != AIState.ToFloor && State.GetName() != AIState.BeAttackTransform &&
                    State.GetName() != AIState.Die && State.GetName() != AIState.BeAttackUp
                   && State.GetName() != AIState.Attack)
                {
                    SetState(AIState.BeAttack, true);
                }
                break;
            case "Transform": //位移攻击
                if (State.GetName() != AIState.ToFloor && State.GetName() != AIState.BeAttackTransform &&
                    State.GetName() != AIState.Die && State.GetName() != AIState.BeAttackUp
                    && State.GetName() != AIState.Attack)
                {
                    SetState(AIState.BeAttackTransform);
                }
                break;
            case "ToFloor": //倒地
                if (State.GetName() != AIState.ToFloor && State.GetName() != AIState.BeAttackTransform &&
                    State.GetName() != AIState.Die && State.GetName() != AIState.BeAttackUp
                    && State.GetName() != AIState.Attack)
                {
                    SetState(AIState.ToFloor);
                }
                break;
            case "Up": //浮空
                if (State.GetName() != AIState.BeAttackTransform && State.GetName() != AIState.Die && State.GetName() != AIState.BeAttackUp
                    && State.GetName() != AIState.Attack)
                {
                    SetState(AIState.BeAttackUp);
                }
                break;
            default:
                break;
        }
    }

    protected override bool IsThisBeAttackNoEffect()
    {
        if (ContinueBeAttackCount < 3)
        {
            ContinueBeAttackCount++;
            return false;
        }
        else
        {
            ContinueBeAttackCount = 0;
            return true;
        }
    }
}
