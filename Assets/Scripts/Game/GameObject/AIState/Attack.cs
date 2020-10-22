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
        int typeNumber = enemy.GetAttackTypeNumber();
        float value;
        switch (typeNumber)
        {
            case 1:
                //开启攻击动画
                EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack", enemy.gameObject));
                break;
            case 3:
                value = Random.Range(0f, 1f);
                if (value < 1 / 4f)
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack", enemy.gameObject));
                }
                else if (value >= 1 / 4f && value < 2 / 4f)
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack2", enemy.gameObject));
                }
                else
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack3", enemy.gameObject));
                }
                break;
            case 4:
                 value = Random.Range(0f, 1f);
                if (value < 1 / 5f)
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack", enemy.gameObject));
                }
                else if (value >= 1 / 5f && value < 2 / 5f)
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack2", enemy.gameObject));
                }
                else if(value>=2/5f&&value<3/5f)
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack3", enemy.gameObject));
                }
                else
                {
                    EventCenter.Instance.SendEvent(SGEventType.AnimSetTrigger, new EventData("Attack4", enemy.gameObject));
                }
                break;
        }


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
