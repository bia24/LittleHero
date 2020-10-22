using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class StateFactory:Singleton<StateFactory>
{
    public AIStateBase GetStateInstance(Enemy.AIState state)
    {
        AIStateBase res = null;
        switch (state)
        {
            case Enemy.AIState.Idle:
                res= InstanceManager.Instance.GetInstance<AIStateBase>(typeof(Idle));
                break;
            case Enemy.AIState.Walk:
                res = InstanceManager.Instance.GetInstance<AIStateBase>(typeof(Walk));
                break;
            case Enemy.AIState.Attack:
                res = InstanceManager.Instance.GetInstance<AIStateBase>(typeof(Attack));
                break;
            case Enemy.AIState.BeAttack:
                res = InstanceManager.Instance.GetInstance<AIStateBase>(typeof(BeAttack));
                break;
            case Enemy.AIState.BeAttackTransform:
                res = InstanceManager.Instance.GetInstance<AIStateBase>(typeof(BeAttackTransform));
                break;
            case Enemy.AIState.ToFloor:
                res = InstanceManager.Instance.GetInstance<AIStateBase>(typeof(ToFloor));
                break;
            case Enemy.AIState.BeAttackUp:
                res = InstanceManager.Instance.GetInstance<AIStateBase>(typeof(BeAttackUp));
                break;
            case Enemy.AIState.Die:
                res = InstanceManager.Instance.GetInstance<AIStateBase>(typeof(Die));
                break;
        }

        return res;
    }
    /// <summary>
    /// 回池子
    /// </summary>
    /// <param name="state"></param>
    public void Revert(AIStateBase state)
    {
        InstanceManager.Instance.Revert(state.GetType(),state);
    }
}
