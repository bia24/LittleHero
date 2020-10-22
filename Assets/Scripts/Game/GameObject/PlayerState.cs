using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 与模型角色对应的状态
/// </summary>
public enum PlayerAnim
{
    /// <summary>
    /// 站立
    /// </summary>
    Idle,
    /// <summary>
    /// 行走
    /// </summary>
    Walk,
    /// <summary>
    /// 奔跑
    /// </summary>
    Run,
    /// <summary>
    /// 一段攻击
    /// </summary>
    AttackFirst,
    /// <summary>
    /// 二段攻击
    /// </summary>
    AttackSecond,
    /// <summary>
    /// 三段攻击
    /// </summary>
    AttackThird,
    /// <summary>
    /// 站立跳跃状态
    /// </summary>
    Jump,
    /// <summary>
    /// 冲刺攻击状态
    /// </summary>
    RunAttack,
    /// <summary>
    /// 技能攻击1状态
    /// </summary>
    SkillAttackOne,
    /// <summary>
    /// 技能攻击2状态
    /// </summary>
    SkillAttackTwo,
    /// <summary>
    /// 受击动画
    /// </summary>
    BeAttack,
    /// <summary>
    /// 倒地动画
    /// </summary>
    ToFloor
}

/// <summary>
/// 跳跃类型    
/// </summary>
public enum JumpStyle
{
    /// <summary>
    /// 站立跳跃
    /// </summary>
    Idle,
    /// <summary>
    /// 行走跳跃
    /// </summary>
    Walk,
    /// <summary>
    /// 跑步跳跃
    /// </summary>
    Run
}
/// <summary>
/// 玩家特殊状态
/// </summary>
public enum PlayerState
{
    None,
    /// <summary>
    /// 受击打 发生位移
    /// </summary>
    Transform,
    /// <summary>
    /// 死亡
    /// </summary>
    Die
}

public enum PlayerSkill
{
    /// <summary>
    /// 一段攻击
    /// </summary>
    AttackFirst,
    /// <summary>
    /// 二段攻击
    /// </summary>
    AttackSecond,
    /// <summary>
    /// 三段攻击
    /// </summary>
    AttackThird,
    /// <summary>
    /// 奔跑攻击
    /// </summary>
    AttackRun,
    /// <summary>
    /// 跳跃攻击
    /// </summary>
    AttackJump,
    /// <summary>
    /// 大招攻击
    /// </summary>
    AttackAce
}