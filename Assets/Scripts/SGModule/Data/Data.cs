using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 存储一些json 和数据对象的简单实体类
/// </summary>
namespace SGModule
{
    #region 数据路径
    /// <summary>
    /// 数据路径配置文件的实体
    /// </summary>
    [Serializable]
    public class DataPath
    {
        public List<DataPathEntity> DataPathList;

        public DataPath()
        {
            DataPathList = new List<DataPathEntity>();
        }
    }

    [Serializable]
    public class DataPathEntity
    {
        public string DataName;
        public string DataPath;
        public DataPathEntity(string name,string path)
        {
            DataName = name;
            DataPath = path;
        }
    }
    #endregion



}
