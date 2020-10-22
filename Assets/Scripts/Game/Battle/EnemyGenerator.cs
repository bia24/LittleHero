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
    /// 产兵开关
    /// </summary>
    private bool trigger;

    private int currentTimes;

    private float timeCount = 0f;

    private readonly float delay = 0.5f;

    public void Init()
    {
        //参数初始化
        trigger = false;
        currentTimes = 1;
        timeCount = 0f;
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        EventCenter.Instance.RegistListener(SGEventType.NextTimeEnemyGenerate, OnEnemyGenTrigger);
        EventCenter.Instance.RegistListener(SGEventType.BattleBossDie, OnBossDieListener);
        EventCenter.Instance.RegistListener(SGEventType.BattleDialogueExit, OnEnemyGenTrigger);
    }

    private void RemoveEvent()
    {
        EventCenter.Instance.RemoveListener(SGEventType.NextTimeEnemyGenerate, OnEnemyGenTrigger);
        EventCenter.Instance.RemoveListener(SGEventType.BattleBossDie, OnBossDieListener);
        EventCenter.Instance.RemoveListener(SGEventType.BattleDialogueExit, OnEnemyGenTrigger);
    }

   private  void OnBossDieListener(EventData data)
    {
        currentTimes = 1;
    }

    private void OnEnemyGenTrigger(EventData data)
    {
        trigger = true;
    }

    private void CreateEnemy()
    {
        Dictionary<int, List<EnemyDetail>> plan = BattleController.Instance.GetCurrentGenEnemyPlan();

        if (currentTimes > plan.Count)
        {
            //防止这关没有boss无法切入下一关
            Debug.LogWarning("this level has no enemy create any more");
            trigger = false;
            return;
        }

        List<EnemyDetail> e = plan[currentTimes++];//获得本波次的产兵计划

        foreach (var detail in e)
        {
            EventCenter.Instance.SendEvent(SGEventType.CreateEnemy, new EventData(detail, null));
        }

        trigger = false;//每次产兵成功trigger设为false。等待下一次开启
        timeCount = 0f;
       
    }

   

    //敌人生成循环
    private void Update()
    {
        if (trigger) //由trigger控制
        {
            if (timeCount < delay)
            {
                timeCount += Time.deltaTime;
                return;
            }
            CreateEnemy();
        }
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

}
