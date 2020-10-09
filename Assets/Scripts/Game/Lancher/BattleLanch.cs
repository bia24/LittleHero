using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

/// <summary>
/// 战斗场景启动器
/// </summary>
public class BattleLanch : MonoBehaviour
{
    private void Start()
    {
        EventCenter.Instance.SendEvent(SGEventType.BattleStart,null);
    }
}
