using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class ToFloor : AIStateBase
{
    float timeCount = 0.0f;

    public ToFloor()
    {
        state = Enemy.AIState.ToFloor;
    }

    public override void OnEnter(Enemy enemy)
    {
        //开启倒地动画
        EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("ToFloor", enemy.gameObject));
        //时间初始化
        timeCount = 0.0f;
    }

    public override void OnExit(Enemy enemy)
    {
        //将自己revert回工厂
        StateFactory.Instance.Revert(this);
    }

    public override void Update(Enemy enemy)
    {
       
        if (timeCount < enemy.GetFloorTime())
        {
            timeCount += Time.deltaTime;
        }
        else
        {
            enemy.SetState(Enemy.AIState.Idle);
        }
    }
}
