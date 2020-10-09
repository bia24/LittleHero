using System.Collections;
using System.Collections.Generic;
using SGModule;
using UnityEngine;

public class JinZhan : Enemy
{
    public override List<AnimCallBackEntity> GetCallBacks()
    {
        List<AnimCallBackEntity> res = new List<AnimCallBackEntity>();
        //动作结束回到站立 动作回调
        res.Add(new AnimCallBackEntity("Enemy1_Attack", "FinishedToIdle", 1.0f, AnimEventParamType.String,"Attack")); //exitime 1.0
        
        //声音回调
        //res.Add(new AnimCallBackEntity("Player1_Attack1", "AttackSound", 0.5f, AnimEventParamType.String, "Player1_Attack_First|1.0"));
        
        return res;
    }

    protected override void InitAnimStateName()
    {
        AddAnimName(State.Idle, "Enemy1_Idle");
        AddAnimName(State.Walk, "Enemy1_Walk");
        AddAnimName(State.AttackFirst, "Enemy1_Attack");
        AddAnimName(State.BeAttackNormal, "Enemy1_BeAttacked");
        AddAnimName(State.BeAttackFloor, "Enemy1_ToFloor");
        AddAnimName(State.BeAttackOver, "Enemy1_ToFloor");
        AddAnimName(State.BeAttackTransform, "Enemy1_BeAttacked");
        AddAnimName(State.BeDie, "Enemy1_ToFloor");
    }

    protected override void AnimJudge(string effect)
    {
        
    }


}
