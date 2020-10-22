using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class Idle : AIStateBase
{
    /// <summary>
    /// 反应时间
    /// </summary>
    private bool reaction = false;
    private float reactionTimeCount = 0.0f;
   /// <summary>
   /// 站立发呆时间
   /// </summary>
    private float idleTimeCount = 0.0f;
    /// <summary>
    /// 攻击间隔计时器，不会重置
    /// </summary>
    private float attackIntervalCount = 0.0f;
    
   public Idle()
    {
        //本类状态赋值
        state = Enemy.AIState.Idle;
    }

    public override void OnEnter(Enemy enemy)
    {
        //开启站立动画
        EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", enemy.gameObject, true));
        //时间初始化
        reaction = false;
        reactionTimeCount = 0;
        idleTimeCount = 0;
    }

    public override void OnExit(Enemy enemy)
    {
        //关闭站立动画
        EventCenter.Instance.SendEvent(SGEventType.AnimSetBool, new EventData("Idleing", enemy.gameObject, false));
        //将自己revert回工厂
        StateFactory.Instance.Revert(this);
    }

    public override void Update(Enemy enemy)
    {
        if (enemy.IsInTransforming())
            return;//如果正在转换，不进行逻辑判断。

        if (reaction) //若正处于反应至攻击状态
        {
            reactionTimeCount += Time.deltaTime;
            attackIntervalCount += Time.deltaTime;
            if(reactionTimeCount>enemy.GetReactionTime()&&attackIntervalCount>enemy.GetAttackInterval())
            {
                reaction = false;
                //切换到攻击状态
                enemy.SetState(Enemy.AIState.Attack); 
                //重置攻击间隔时间
                attackIntervalCount = 0.0f;
                return;
            }
            return;
        }
 
        List<Transform>players= enemy.SearchTarget();
        Transform target = enemy.GetMinDisPlayerWithingDis(players);
        if (target != null) //搜索范围内 玩家 不为空，开启反应至攻击状态
        {
            //面向调整
            enemy.Rotate(Vector3.Normalize(target.position-enemy.transform.position));
            reaction = true;
            return;
        }

        //否则，若还未达到寻路时间，原地随机攻击，暂停寻路时间；
        //发憷时间累加
        idleTimeCount += Time.deltaTime;
        //攻击间隔时间累加
        attackIntervalCount += Time.deltaTime;

        if (idleTimeCount < enemy.GetIdleTime()&&Time.frameCount%enemy.GetAttackFrameRate()==0)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            float value= Random.Range(0.0f, 1.0f);
            if (value < enemy.GetAttackPR())
            {
                reaction = true;
                return;
            }
        }
        else if(idleTimeCount > enemy.GetIdleTime())
        {
            enemy.SetState(Enemy.AIState.Walk);
            return;
        }
    }

   


}
