using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI状态基类
/// </summary>
public abstract  class AIStateBase 
{
    protected Enemy.AIState state;
    /// <summary>
    /// 获得本状态所代表的状态
    /// </summary>
    /// <returns></returns>
    public Enemy.AIState GetName() { return state; }
    public abstract void OnEnter(Enemy enemy);
    public abstract void Update(Enemy enemy);
    public abstract void OnExit(Enemy enemy);

  
}
