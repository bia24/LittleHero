using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SGModule
{
    /// <summary>
    /// 使用 Animtor控制器进行动画的调用
    /// </summary>
    public class AnimManager : Singleton<AnimManager>
    {
        /// <summary>
        /// 是否绑定过回调函数的缓存查找索引
        /// </summary>
        private List<Type> hasBinded = new List<Type>();


        /// <summary>
        /// 依据游戏角色物体和回调控制器(mono)实现动态动画绑定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        public void BindAnimCallBack<T>(Animator target) where T:AnimCallBackController
        {
            //给目标上脚本，动画回调都是调用的这个脚本Mono上的函数
            AnimCallBackController c = target.gameObject.AddComponent<T>();
            //初始化该脚本
            c.Init();
            if (hasBinded.Contains(typeof(T)))
            {
                //说明该类型绑定过了，因为同一类型绑定的回调函数都是一致的，
                //且所有实例对象是共用animationclip的，所以不需要再绑定了，会造成回调函数重复
                //只要上述给它加上T类型的脚本即可。
                Debug.Log(target.gameObject.name + " bind animation callback finished. has been binded same callbacks");
                return;
            }
            //填充实体
            c.FillEntities();

            //绑定动画回调函数
            AnimationClip[] clips = target.runtimeAnimatorController.animationClips;
            List<AnimCallBackEntity> entities = c.GetCallBackList();
            foreach(var e in entities)
            {
                for(int i = 0; i < clips.Length; i++)
                {
                    if (clips[i].name.Equals(e.AnimName))
                    {
                        AnimationEvent ae = new AnimationEvent();
                        ae.time = e.Time;
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
                        clips[i].AddEvent(ae);
                        break;
                    }
                }
            }
            //将角色动画控制的T类型加入缓存，同一种类型的绑定一次即可
            hasBinded.Add(typeof(T));
            Debug.Log(target.gameObject.name + " bind animation callback finished.");
        }


        public void SetBooi(Animator target,string paramName,bool value)
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
    }
}
