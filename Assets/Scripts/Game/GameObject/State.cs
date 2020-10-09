using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏对象的状态动画索引
/// </summary>
public enum State
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
    /// 跳跃攻击状态
    /// </summary>
    JumpAttack,
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
    /// 普通受击状态
    /// </summary>
    BeAttackNormal,
    /// <summary>
    /// 位移受击状态
    /// </summary>
    BeAttackTransform,
    /// <summary>
    /// 浮空受击状态
    /// </summary>
    BeAttackOver,
    /// <summary>
    /// 倒地受击状态
    /// </summary>
    BeAttackFloor,
    /// <summary>
    /// 死亡状态
    /// </summary>
    BeDie
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
