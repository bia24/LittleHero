using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDirTip : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        int n = BattleController.Instance.GetEnemyCount();
        if (n== 1)
        {
           BattleController.Instance.OnlyOneEnemyVisibleJudge();
        }
    }
}
