using System.Collections;
using System.Collections.Generic;
using SGModule;
using UnityEngine;

public class JinZhan : Enemy
{
    public override List<AnimCallBackEntity> GetCallBacks()
    {
        List<AnimCallBackEntity> res = new List<AnimCallBackEntity>();
        //动作结束回到站立 动作回调
        res.Add(new AnimCallBackEntity("Enemy1_Attack", "FinishedToIdle", 1.0f, AnimEventParamType.Null)); //exitime 1.0
        res.Add(new AnimCallBackEntity("Enemy1_BeAttacked", "FinishedToIdle", 1.0f, AnimEventParamType.Null)); //exitime 1.0
        res.Add(new AnimCallBackEntity("Enemy1_Up", "FinishedToIdle", 0.8f, AnimEventParamType.Null)); //exitime 0.85 
        //声音回调
        res.Add(new AnimCallBackEntity("Enemy1_Attack", "AttackSound", 0.35f, AnimEventParamType.String, "Enemy1_Attack|1.0"));
        //攻击判定
         BattleCharacter bc = BattleController.Instance.GetBattleCharacter(GetCharacterId());
        res.Add(new AnimCallBackEntity("Enemy1_Attack", "AttackJudge", 0.4f, AnimEventParamType.Int, bc.skill1_id)); //int skill id
        return res;
    }

   


}
