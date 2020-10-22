using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class LevelUpParticle : MonoBehaviour
{
    private readonly float exitTime = 1.0f;
    float timeCount=0f;
    private readonly float playSoundTime = 0.1f;
    bool hasPlaySound = false;
    AudioSource audioSource = null;
    bool trigger = false;

    public void Init(Player target)
    {
        audioSource = target.AudioSource;
        timeCount = 0;
        hasPlaySound = false;
        trigger = true;
        RegisterEvent();
    }
  
    private void Update()
    {
        if (!trigger)
            return;

        if (timeCount < exitTime)
        {
            timeCount += Time.deltaTime;
        }
        else
        {
            BattleController.Instance.RevertLevelUpParticle(gameObject);
            RemoveEventListener();
            trigger = false;
        }
        if(timeCount > playSoundTime && !hasPlaySound)
        {
            //播放特效声音
            //声音
            EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
                new SoundParam(audioSource, "Appear",false,false,false), null));
            //设置trigger
            hasPlaySound = true;
        }
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

    private void OnDestroy()
    {
        RemoveEventListener();
    }
}
