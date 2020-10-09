using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;


/// <summary>
/// 动画管理器的逻辑控制
/// </summary>
public class AnimationController :Singleton<AnimationController>
{
   public void Init()
    {
        //注册事件
        EventCenter.Instance.RegistListener(SGEventType.AnimCallbackRigister, RigisterAnimCallback);
        EventCenter.Instance.RegistListener(SGEventType.AnimSetTrigger, SetTriggerListener);
        EventCenter.Instance.RegistListener(SGEventType.AnimSetBool, SetBoolListener);
    }

    /// <summary>
    /// 注册指定实体的所有动画回调
    /// </summary>
    /// <param name="data"></param>
    private void RigisterAnimCallback(EventData data)
    {
        IAnim anim = data.Sender.GetComponent<IAnim>();
        AnimManager.Instance.BindAnimCallBack(anim);
    }

    private void SetTriggerListener(EventData data)
    {
        string paramName = data.Param as string;
        AnimManager.Instance.SetTrigger(data.Sender.GetComponent<IAnim>().GetAnimator(), paramName);
    }

    private void SetBoolListener(EventData data)
    {
        string paramName = data.Param as string;
        bool value = (bool)data.Param2;
        AnimManager.Instance.SetBool(data.Sender.GetComponent<IAnim>().GetAnimator(),paramName,value);
    }

    /// <summary>
    /// 是否正在运行指定动画
    /// </summary>
    /// <param name="animName"></param>
    /// <param name="anim"></param>
    /// <returns></returns>
    public bool IsInStateAnim(string animName,IAnim anim)
    {
        Animator animator = anim.GetAnimator();
        return AnimManager.Instance.IsInThisAnimState(animator, 0, animName);
    }

    public float GetAnimCurrentRuningTime(IAnim anim)
    {
        Animator animator = anim.GetAnimator();
        return AnimManager.Instance.GetCurrentNormalizeTime(animator, 0);
    }

    public bool IsInTransforming(IAnim anim)
    {
        Animator animator = anim.GetAnimator();
        return AnimManager.Instance.IsInTransforming(animator,0);
    }

    public bool IsInTransforming(IAnim anim,string name)
    {
        Animator animator = anim.GetAnimator();
        return AnimManager.Instance.IsInTransforming(animator, 0,name);
    }

    public bool GetStateBool(IAnim anim,string name)
    {
        Animator animator = anim.GetAnimator();
        return AnimManager.Instance.GetAnimtorBoolParam(animator, name);
    }

    public float GetAnimtionClipLength(IAnim anim, string name)
    {
        Animator animator = anim.GetAnimator();
        return AnimManager.Instance.GetAnimtionClip(animator, 0, name).length;
    }
}

