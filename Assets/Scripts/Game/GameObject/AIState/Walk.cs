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
        target = GetMinDisPlayer(players, enemy);
        dir = target.localPosition - enemy.transform.localPosition;
        dir = new Vector3(dir.x, dir.y, 0f);
        dir = Vector3.Normalize(dir);
        //初始化移动时间
        moveTime = 0.0f;
        moveTimeMax = enemy.GetMoveTime() * Random.Range(enemy.GetMoveTimePRLower(), 1.0f);

    }

    public override void OnExit(Enemy enemy)
    {
        //关闭走路动画
        EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Walking", enemy.gameObject, false));
    }

    public override void Update(Enemy enemy)
    {
        if (!enemy.IsInState(State.Walk) || enemy.IsInTransforming())
            return;
        moveTime += Time.deltaTime;
        if (moveTime < moveTimeMax&&
            Vector3.Distance(enemy.transform.localPosition,target.transform.localPosition)>enemy.GetSearchDistance())
        {
            enemy.Move(dir);
        }
        else
        {
            //切换到站立状态
            enemy.CurrentState = Enemy.AIState.Idle;
        }
        
    }

    /// <summary>
    /// 获得最近的那个玩家
    /// </summary>
    /// <param name="players"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>
    private Transform GetMinDisPlayer(List<Transform> players,Enemy enemy)
    {
        float min = float.MaxValue;
        Transform res = null;
        foreach (var transform in players)
        {
            float dis = Vector3.Distance(new Vector3(transform.localPosition.x, transform.localPosition.y, 0),
                new Vector3(enemy.transform.localPosition.x, enemy.transform.localPosition.y, 0));
            if (dis < min)
            {
                min = dis;
                res = transform;
            }
        }
        return res;
    }


}
