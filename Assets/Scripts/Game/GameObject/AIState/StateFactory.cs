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
            //case Enemy.AIState.Idle:
            //    res = InstanceManager.Instance.GetInstance<AIStateBase>(typeof(Idle));
            //    break;

        }

        return res;
    }
}
