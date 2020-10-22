using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class BeAttackUp : AIStateBase
{

    public BeAttackUp()
    {
        state = Enemy.AIState.BeAttackUp;
    }

    public override void OnEnter(Enemy enemy)
    {
        //开启倒地动画
        EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("ToUp", enemy.gameObject));
     
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
