using System.Collections;
using System.Collections.Generic;
using SGModule;
using UnityEngine;

public class YuanChen : Enemy
{
    public override List<AnimCallBackEntity> GetCallBacks()
    {
        List<AnimCallBackEntity> res = new List<AnimCallBackEntity>();
        //动作结束回到站立 动作回调
        res.Add(new AnimCallBackEntity("Enemy2_Attack", "FinishedToIdle", 1.0f, AnimEventParamType.Null)); //exitime 1.0
        res.Add(new AnimCallBackEntity("Enemy2_BeAttacked", "FinishedToIdle", 1.0f, AnimEventParamType.Null)); //exitime 1.0
        res.Add(new AnimCallBackEntity("Enemy2_Up", "FinishedToIdle", 0.8f, AnimEventParamType.Null)); //exitime 0.85 
         //声音回调
         res.Add(new AnimCallBackEntity("Enemy2_Attack", "AttackSound", 0.2f, AnimEventParamType.String, "Enemy2_Attack|1.0"));

        //攻击判定
        BattleCharacter bc = BattleController.Instance.GetBattleCharacter(GetCharacterId());

        res.Add(new AnimCallBackEntity("Enemy2_Attack", "AttackParticleCreate", 0.3f, 
            AnimEventParamType.String,bc.skill1_id.ToString()+"|"+ "Enemy2_Particle")); //int skill id+name

        return res;
    }



    public override float GetParticleMoveSpeed()
    {
        return 4f;
    }


    /// <summary>
    /// 发现敌人的反应时间
    /// </summary>
    private readonly float REACTION_TIME = 1F;
    public override float GetReactionTime()
    {
        return REACTION_TIME * (2 - HardFactor);
    }

    /// <summary>
    /// 站立发憷时间
    /// </summary>
    private readonly float IDLE_TIME = 2F;
    public override float GetIdleTime()
    {
        return IDLE_TIME * (2 - HardFactor);
    }

   

  

    /// <summary>
    /// 基础移动速率
    /// </summary>
    private readonly float MOVE_SPEED_BASE = 2F;
    public override float GetMoveSpeed()
    {
        return MOVE_SPEED_BASE * HardFactor;
    }

    /// <summary>
    /// 每次移动的时间
    /// </summary>
    private readonly float MOVE_TIME = 3F;
    public override float GetMoveTime()
    {
        return MOVE_TIME;
    }
    /// <summary>
    /// 增加移动时间的随机性
    /// </summary>
    private readonly float MOVE_TIME_PR_LOWER = 0.8f;
    public override float GetMoveTimePRLower()
    {
        return MOVE_TIME_PR_LOWER;
    }

    private readonly float Y_MIN_DISTANCE = 0.2F;
    /// <summary>
    /// 获得Y轴最短判断距离
    /// </summary>
    /// <returns></returns>
    private float GetMinYDistance()
    {
        return Y_MIN_DISTANCE;
    }

    /// <summary>
    /// 获得纵坐标最近，且符号条件的玩家
    /// </summary>
    /// <param name="players"></param>
    /// <returns></returns>
    public override Transform GetMinDisPlayerWithingDis(List<Transform> players)
    {
        float min = float.MaxValue;
        Transform res = null;
        foreach (var temp in players)
        {
            float dis = Mathf.Abs(temp.localPosition.y - transform.localPosition.y);
            if (dis < min)
            {
                min = dis;
                res = temp.transform;
            }
        }


        if (YuanchenEnemyAttackCondition(res))
            return res;
        else
            return null;
    }

    private bool YuanchenEnemyAttackCondition(Transform target)
    {
        return Mathf.Abs(target.position.y - transform.position.y) < GetMinYDistance();
    }

    /// <summary>
    /// 获得纵坐标最近的那个敌人
    /// </summary>
    /// <param name="players"></param>
    /// <returns></returns>
    public override Transform GetMinDisPlayer(List<Transform> players)
    {
        float min = float.MaxValue;
        Transform res = null;
        foreach (var temp in players)
        {
            float dis = Mathf.Abs(temp.localPosition.y - transform.localPosition.y);
            if (dis < min)
            {
                min = dis;
                res = temp.transform;
            }
        }
        return res;
    }
    /// <summary>
    /// 远程的移动条件，满足条件继续行走
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public override bool WalkingCondition(Transform target)
    {
        return Mathf.Abs(target.localPosition.y - transform.localPosition.y) > GetMinYDistance();
    }

   

}
