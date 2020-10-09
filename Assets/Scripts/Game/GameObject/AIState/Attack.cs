using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class Attack : AIStateBase
{
    public Attack()
    {
        state = Enemy.AIState.Attack;
    }

    public override void OnEnter(Enemy enemy)
    {
        //开启攻击动画
        EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack", enemy.gameObject));
    }

    public override void OnExit(Enemy enemy)
    {
        Debug.Log("Attack exit");
    }

    public override void Update(Enemy enemy)
    {
        Debug.Log("Attack update");
    }

}
