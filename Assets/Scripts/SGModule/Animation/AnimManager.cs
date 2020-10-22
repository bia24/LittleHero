using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SGModule
{

    /// <summary>
    /// 注册动画回调所需内容的获取接口
    /// </summary>
    public interface IAnim
    {
        /// <summary>
        /// 返回一个游戏实体的动画控制器
        /// </summary>
        /// <returns></returns>
        Animator GetAnimator();
        /// <summary>
        /// 返回一个游戏实体的动画回调实体集合
        /// </summary>
        /// <returns></returns>
        List<AnimCallBackEntity> GetCallBacks();
    }

    /// <summary>
    /// 使用 Animtor控制器进行动画的调用
    /// </summary>
    public class AnimManager : Singleton<AnimManager>
    {

        /// <summary>
        /// 依据游戏角色物体和回调控制器(mono)实现动态动画绑定
        /// note：由于发布后，动画片段在内存重新加载时就会重置，所以不能用角色类型来判断是否绑定过
        /// note：直接在动画片段上查找是否有绑定函数，没有再绑定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        public void BindAnimCallBack(IAnim target) 
        {
            //绑定动画回调函数
            //获得动画状态机中的所有动画片段引用
            AnimationClip[] clips = target.GetAnimator().runtimeAnimatorController.animationClips;
            List<AnimCallBackEntity> entities =target.GetCallBacks();
          
            foreach (var e in entities)
            {
                for(int i = 0; i < clips.Length; i++)
                {
                    AnimationClip clip = clips[i];
                    if (clip.name.Equals(e.AnimName))//当动画片段的名称相同，这是需要绑定的目标
                    {
                        //这里要加一步判断，判断是否该动画片段中已经绑定了同名方法，有则不再重复绑定
                        AnimationEvent[] events = clip.events;
                        bool isExits = false;
                        for(int j = 0; j < events.Length; j++)
                        {
                            //查找该片段的所有事件中，是否有同名事件了。
                            if (events[j].functionName.Equals(e.FunName))
                            {
                                isExits = true;
                                break;
                            }
                        }
                        if (isExits)//若存在相同的同名事件方法
                        {
                            break;
                        }
                        AnimationEvent ae = new AnimationEvent();
                        ae.time = clips[i].length * e.NormalizeTime; //用单位比例时间获得实际时间
                        ae.functionName = e.FunName;
                        switch (e.Type)
                        {
                            case AnimEventParamType.Int:
                                ae.intParameter = e.IntParam;
                                break;
                            case AnimEventParamType.Float:
                                ae.floatParameter = e.FloatParam;
                                break;
                            case AnimEventParamType.Object:
                                ae.objectReferenceParameter = e.ObjectParam;
                                break;
                            case AnimEventParamType.String:
                                ae.stringParameter = e.StringParam;
                                break;
                            case AnimEventParamType.Null:
                                break;
                        }
                        //给动画片段添加事件，只在运行时有效果
                        clips[i].AddEvent(ae);
                        break;
                    }
                }
            }
         
        }


        public void SetBool(Animator target,string paramName,bool value)
        {
            target.SetBool(paramName, value);
        }

        public void SetInt(Animator target, string paramName, int value)
        {
            target.SetInteger(paramName, value);
        }

        public void SetFloat(Animator target, string paramName, float value)
        {
            target.SetFloat(paramName, value);
        }

        public void SetTrigger(Animator target, string paramName)
        {
            target.SetTrigger(paramName);
        }
        /// <summary>
        /// 是否状态机正在运行name的动画
        /// </summary>
        /// <param name="target"></param>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsInThisAnimState(Animator target,int index,string name)
        {
            return target.GetCurrentAnimatorStateInfo(index).IsName(name);
        }

        public float GetCurrentNormalizeTime(Animator target, int index)
        {
            return target.GetCurrentAnimatorStateInfo(index).normalizedTime;
        }

        public bool IsInTransforming(Animator target, int index)
        {
            return target.IsInTransition(index);
        }

        public bool IsInTransforming(Animator target, int index, string name)
        {
            return target.GetAnimatorTransitionInfo(0).IsName(name);
        }

        public bool GetAnimtorBoolParam(Animator target,string name)
        {
            return target.GetBool(name);
        }

        public  AnimationClip GetAnimtionClip(Animator target,int index, string name)
        {
            AnimationClip[] sources = target.runtimeAnimatorController.animationClips;
            foreach (var t in sources)
            {
                if (t.name.Equals(name))
                {
                    return t;
                }
            }
            return null;
        }

        public void ResetAllTrigger(Animator target)
        {
            AnimatorControllerParameter[] paramArray = target.parameters;
            foreach (var param in paramArray)
            {
                if (param.type == AnimatorControllerParameterType.Trigger)
                {
                    target.ResetTrigger(param.name);
                }
            }
        }
    }

    
}
