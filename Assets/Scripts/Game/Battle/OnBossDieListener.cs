using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class OnBossDieListener : MonoBehaviour
{

    private void Awake()
    {
        EventCenter.Instance.RegistListener(SGEventType.BattleBossDie, OnBossDie);
    }

    private void OnBossDie(EventData data)
    {
        int bossId = (int)data.Param;
        EventCallBack callback = data.Param2 as EventCallBack;
        //当boss死了开启死亡慢放
        StartCoroutine("SlowDown", new SlowDownParam(1.2f, bossId, callback));
    }


    //只影响动画和脚本位移
    IEnumerator SlowDown(SlowDownParam slowdownParam)
    {
        float time = slowdownParam.time;
        int bossId = slowdownParam.bossId;
        EventCallBack callback = slowdownParam.callback;

        float timeCount = 0.0F;
        while (timeCount < time)
        {
            if (BattleController.Instance.GetGameState() == GameState.Running)
            {
                Time.timeScale = 0.2f;//当处于游戏运行的时候，时间放慢；否则，按照暂停的速率计算。
            }
            timeCount += Time.fixedDeltaTime*Time.timeScale;
            yield return null;
        }

        Time.timeScale = 1f;

        callback.Invoke(new EventData(bossId, null)); //结束后调用回调
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveListener(SGEventType.BattleBossDie, OnBossDie);
    }
}
