using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    public class AnimCallBackEntity
    {
        /// <summary>
        /// 动画片段名
        /// </summary>
        public string AnimName { get; set; }
        public int IntParam { get; set; }
        public float FloatParam { get; set; }
        public string StringParam { get; set; }
        public Object ObjectParam { get; set; }
        /// <summary>
        /// 参数类型
        /// </summary>
        public AnimEventParamType Type { get; set;}
        /// <summary>
        /// 回调函数名称
        /// </summary>
        public string FunName { get; set; }
        /// <summary>
        /// 回调时间 单位秒
        /// </summary>
        public float Time { get; set; }

        public AnimCallBackEntity(string animName,string funName,float time,AnimEventParamType type=AnimEventParamType.Null, object param=null)
        {
            Type = type;

            switch (type)
            {
                case AnimEventParamType.Int:
                    IntParam = (int)param;
                    break;
                case AnimEventParamType.Float:
                    FloatParam = (float)param;
                    break;
                case AnimEventParamType.Object:
                    ObjectParam = (Object)param;
                    break;
                case AnimEventParamType.String:
                    StringParam = (string)param;
                    break;
                case AnimEventParamType.Null:
                    break;
            }

            AnimName = animName;
            FunName = funName;
            Time = time;
        }

        
    }
}
