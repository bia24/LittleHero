using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class BeAttackTransform : AIStateBase
{
    float tranformTime = 0.0f;

    Vector3 dir = Vector3.right;

    public BeAttackTransform()
    {
        state = Enemy.AIState.BeAttackTransform;
    }

    public override void OnEnter(Enemy enemy)
    {
        //开启受击打状态动画
        EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("BeAttack", enemy.gameObject));
        //初始化计时器
        tranformTime = 0.0f;
        //初始化被攻击的方向，移动方向是被击方向的反方向
        dir = new Vector3(enemy.BeAttackDir.x * -1, 0, 0);
    }

    public override void OnExit(Enemy enemy)
    {
        //将自己revert回工厂
        StateFactory.Instance.Revert(this);
    }

    public override void Update(Enemy enemy)
    {
        if (tranformTime > enemy.GetBeAttackTransformTime())
            return;

        tranformTime += Time.deltaTime;
        
        enemy.Move(dir);
    }

   
}
