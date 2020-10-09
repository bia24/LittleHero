using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;

/// <summary>
/// 敌人生成器
/// </summary>
public class EnemyGenerator : MonoBehaviour
{

    /// <summary>
    /// 开关
    /// </summary>
    private bool trigger;

    private int currentTimes;

    private void Awake()
    {
        //参数初始化
        trigger = true;
        currentTimes = 1; 
    }



    private void OffEnemyGenTrigger()
    {
        trigger = false;
    }

    private void OnEnemyGenTrigger()
    {
        trigger = true;
    }

    private void CreateEnemy()
    {
        Dictionary<int, List<EnemyDetail>> plan = BattleController.Instance.GetCurrentGenEnemyPlan();

        if (currentTimes > plan.Count)
        {
            //本关卡已经产完兵了
            trigger = false;
            currentTimes = 1;
            return;
        }

        List<EnemyDetail> e = plan[currentTimes++];//获得本波次的产兵计划

        foreach (var detail in e)
        {
            EventCenter.Instance.SendEvent(SGEventType.CreateEnemy, new EventData(detail, null));
        }
    }


    //敌人生成循环
    private void Update()
    {
        if (trigger) //由trigger控制
        {
            CreateEnemy();
        }
    }

}
