using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSortRender : MonoBehaviour
{

    private void LateUpdate()
    {
        RenderSpriteOrderSort();
    }

    private void RenderSpriteOrderSort()
    {
        //将战场中所有玩家和敌人、可移动特效加到排序集合中
        List<IAttacker> temp = new List<IAttacker>();
        foreach (Player p in BattleManager.Instance.playerInBattle)
        {
            temp.Add(p);
        }
        foreach (Enemy e in BattleManager.Instance.enemyInBattle)
        {
            temp.Add(e);
        }
        foreach (IAttacker a in BattleManager.Instance.moveParticleInBattle)
        {
            temp.Add(a);
        }


        //依据localtranform的值排序上下，并获得其上的所有spriteRender，并动态设置它们的order
        temp.Sort((a, b) =>
        {
            float ay = a.GetAttackerLocalPostion().y;
            float by = b.GetAttackerLocalPostion().y;
            if (ay < by)
                return -1;
            else if (ay == by)
                return 0;
            else
                return 1;
        });

        int orderCount = 0;

        //获得对象上的所有spriterender组件

        for (int i = temp.Count - 1; i >= 0; i--)
        {
            Transform target = temp[i].GetGameObject().transform;
            List<SpriteRenderer> list = new List<SpriteRenderer>();
            FindSpriteRendererRecursive(target, ref list);//获得自己和孩子身上所有的spriterender组件
            list.Sort((x, y) => {
                if (x.sortingOrder < y.sortingOrder) return -1;
                else if (x.sortingOrder == y.sortingOrder) return 0;
                else
                    return 1;
            });//对集合排序。第一位基准数
            int baseOrder = list[0].sortingOrder;

            for (int j = 0; j < list.Count; j++)
            {
                list[j].sortingOrder = list[j].sortingOrder - baseOrder + orderCount;
            }

            orderCount = list[list.Count - 1].sortingOrder + 1;
        }

    }

    private void FindSpriteRendererRecursive(Transform root, ref List<SpriteRenderer> list)
    {
        if (root == null || list == null)
            return;
        SpriteRenderer sr = root.GetComponent<SpriteRenderer>();
        if (sr != null)
            list.Add(sr);
        for (int i = 0; i < root.childCount; i++)
        {
            FindSpriteRendererRecursive(root.GetChild(i), ref list);
        }
    }

}
