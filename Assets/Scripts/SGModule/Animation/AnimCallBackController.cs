using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    /// <summary>
    /// 回调动画绑定和调用的基类
    /// </summary>
    public abstract class AnimCallBackController : MonoBehaviour
    {
        /// <summary>
        /// 初始化控制器
        /// </summary>
        public abstract void Init();
        /// <summary>
        /// 填充本角色类型的动画回调实体
        /// </summary>
        public abstract void FillEntities();
        /// <summary>
        /// 动画回调参数实体类，用来传递信息
        /// </summary>
        protected List<AnimCallBackEntity> entities = new List<AnimCallBackEntity>();



        /// <summary>
        /// 得到所有动画回调函数
        /// </summary>
        /// <returns></returns>
        public virtual List<AnimCallBackEntity> GetCallBackList()
        {
            return entities;
        }


    }

}