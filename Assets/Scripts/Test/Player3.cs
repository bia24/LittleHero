using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class Player3 : AnimCallBackController
{
    public override void Init()
    {
    }

    public override void FillEntities()
    {
        entities.Add(new AnimCallBackEntity("Player1_Idle", "Show1", 0.5f, AnimEventParamType.String, "hello"));
    }

    private void Show1(string context)
    {
        Debug.Log(gameObject.name+"  " +context);
    }
}
