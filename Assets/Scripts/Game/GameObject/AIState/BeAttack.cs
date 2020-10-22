using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class BeAttack : AIStateBase
{
    public BeAttack()
    {
        state = Enemy.AIState.BeAttack;
    }

    public override void OnEnter(Enemy enemy)
    {
        //开启受击打状态动画
        EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("BeAttack", enemy.gameObject));
    }

    public override void OnExit(Enemy enemy)
    {
        //将自己revert回工厂
        StateFactory.Instance.Revert(this);
    }

    public override void Update(Enemy enemy)
    {
        
    }
}
