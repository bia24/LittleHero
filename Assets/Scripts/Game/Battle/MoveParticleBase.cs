using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class MoveParticleBase : MonoBehaviour,IAttacker
{
    Vector3 moveDir;
    bool trigger;
    Skill newSkill;
    float speed;
    protected GameObject attacker;
    Vector3 startLocalPos;
    string particleName;
    readonly float exitTIME = 5F;
    float timeCount = 0f;

    public void Init(Vector3 moveDir,int skillId,GameObject attacker,string particleName)
    {
        trigger =  BattleController.Instance.GetGameState()==GameState.Running?true:false;
        timeCount = 0f;
        this.moveDir = moveDir;
        this.attacker = attacker;
        this.particleName = particleName;
        startLocalPos = attacker.transform.localPosition;
        //技能强化
        newSkill = BattleController.Instance.GetSkill(skillId).Clone() as Skill;
        newSkill = attacker.GetComponent<IStrengthenSkill>().StrengthenSkill(newSkill);
        speed = attacker.GetComponent<IParticleMoveSpeed>().GetParticleMoveSpeed();
        //注册事件
        RegisterEvent();
        //将自己添加到战场集合
        BattleController.Instance.AddParticle(this);
    }

    private void RegisterEvent()
    {
        EventCenter.Instance.RegistListener(SGEventType.BattlePause, GamePauseListener);
        EventCenter.Instance.RegistListener(SGEventType.BattlePauseExit, GamePauseExitListener);
    }

    private void RemoveEventListener()
    {
        EventCenter.Instance.RemoveListener(SGEventType.BattlePause, GamePauseListener);
        EventCenter.Instance.RemoveListener(SGEventType.BattlePauseExit, GamePauseExitListener);
    }

    private void GamePauseListener(EventData data)
    {
        trigger = false;
    }

    private void GamePauseExitListener(EventData data)
    {
        trigger = true;
    }


    private void Update()
    {
        if (trigger)
        {
            transform.Translate(moveDir* speed*Time.fixedDeltaTime*Time.timeScale, Space.World);
            BattleController.Instance.AttackJudge(newSkill, attacker.transform,this);
            timeCount += Time.deltaTime;
            if(timeCount> exitTIME)
            {
                //若超过了存在时间，将本特效回收
                Revert();
            }
        }
    }


    public virtual void Revert()
    {
        trigger = false;
        //取消事件注册
        RemoveEventListener();
        BattleController.Instance.RevertAttackMoveParticle(particleName, gameObject);
        //将自己移除战场集合
        BattleController.Instance.RemoveParicle(this);
    }
   /// <summary>
   /// 获得动态的粒子判断局部坐标
   /// </summary>
   /// <returns></returns>
    public Vector3 GetAttackerLocalPostion()
    {
        return new Vector3(transform.localPosition.x,startLocalPos.y,0);
    }

    public Vector3 GetAttackerLocalScale()
    {
        return transform.localScale; //在初始化的时候本粒子的缩放已经和发出者同步。因此可以直接拿来
    }

    public bool IsParticleAttack()
    {
        return true;
    }

    public MoveParticleBase GetParticle()
    {
        return this;
    }

    //若中途摧毁了
    private void OnDestroy()
    {
        RemoveEventListener();//取消事件注册
        BattleController.Instance.RemoveParicle(this);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
