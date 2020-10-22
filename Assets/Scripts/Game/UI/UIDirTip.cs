using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIDirTip : MonoBehaviour
{
    Tweener t;

    private void Awake()
    {
        t = transform.DOScale(1.3f, 0.3f).SetLoops(-1, LoopType.Yoyo).SetAutoKill(false);
        t.Pause();
    }

    private void OnEnable()
    {
        t.Restart();
    }

    private void OnDisable()
    {
        t.Pause();
    }
}
