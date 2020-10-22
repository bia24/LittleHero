using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

/// <summary>
/// 敌人死亡状态
/// </summary>
public class Die : AIStateBase
{

    public Die()
    {
        state = Enemy.AIState.Die;
    }


    public override void OnEnter(Enemy enemy)
    {
        //开启倒地(死亡)动画
        EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("ToFloor", enemy.gameObject));
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
