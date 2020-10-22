using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class Walk : AIStateBase
{
    #region 动态变量
    /// <summary>
    /// 移动方向
    /// </summary>
    private Vector3 dir;
    /// <summary>
    /// 移动时间计数
    /// </summary>
    private float moveTime = 0.0f;
    /// <summary>
    /// 移动的时间最大值
    /// </summary>
    private float moveTimeMax = 0.0f;
    /// <summary>
    /// 目标
    /// </summary>
    private Transform target;
    #endregion

    public Walk()
    {
        state = Enemy.AIState.Walk;
    }

    public override void OnEnter(Enemy enemy)
    {
        //开启走路动画
        EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", enemy.gameObject, true));
        //更新行走方向
        List<Transform> players = enemy.SearchTarget();
        target = enemy.GetMinDisPlayer(players);

        if (target == null)//若无法找到玩家，说明玩家已经死亡
        {
            enemy.SetState(Enemy.AIState.Idle);
        }
        else
        {
            dir = target.localPosition - enemy.transform.localPosition;
            dir = new Vector3(dir.x, dir.y, 0f);
            dir = Vector3.Normalize(dir);
        }
        //初始化移动时间
        moveTime = 0.0f;
        Random.InitState(System.DateTime.Now.Millisecond);
        moveTimeMax = enemy.GetMoveTime() * Random.Range(enemy.GetMoveTimePRLower(), 1.0f);
    }

    public override void OnExit(Enemy enemy)
    {
        //关闭走路动画
        EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", enemy.gameObject, false));
        //将自己revert回工厂
        StateFactory.Instance.Revert(this);
    }

    public override void Update(Enemy enemy)
    {
        if (target == null) //目标玩家在锁定后死亡
        {
            //切换到站立状态
            enemy.SetState(Enemy.AIState.Idle);
            //结束
            return;
        }

        if (enemy.IsInTransforming())
            return;
        moveTime += Time.deltaTime;
        if (moveTime < moveTimeMax && enemy.WalkingCondition(target))
        {
            enemy.Move(dir);
            enemy.Rotate(dir);
        }
        else
        {
            //切换到站立状态
            enemy.SetState(Enemy.AIState.Idle);
        }
        
    }

   


}
