using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    /// <summary>
    /// 路径处理工具
    /// </summary>
    public static class PathUtils
    {
        /// <summary>
        /// 从resources文件路径中提取文件夹路径
        /// </summary>
        /// <param name="fullResourcesPath"></param>
        /// <returns></returns>
        public static string GetPathName(string fullResourcesPath)
        {
            if (string.IsNullOrEmpty(fullResourcesPath))
            {
                Debug.LogError("error null fullPath input");
            }
            //Resources.load()中resources/a/b等价于path：a/b
            int lastIndex = fullResourcesPath.LastIndexOf('/');
            if (lastIndex == -1)
                return "";
            return fullResourcesPath.Substring(0, lastIndex);
        }

        /// <summary>
        /// 从resources文件路径中提取文件名
        /// </summary>
        /// <param name="fullResourcesPath"></param>
        /// <returns></returns>
        public static string GetFileName(string fullResourcesPath)
        {
            if (string.IsNullOrEmpty(fullResourcesPath))
            {
                Debug.LogError("error null fullPath input");
            }

            int lastIndex = fullResourcesPath.LastIndexOf('/');
            if (lastIndex == -1)
                return fullResourcesPath;
            return fullResourcesPath.Substring(lastIndex + 1, fullResourcesPath.Length - lastIndex - 1);
        }

        /// <summary>
        /// 从加载器加载URL中提取resources文件路径
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetResourcesPath(string loaderUrl)
        {
            if (!loaderUrl.StartsWith(Application.persistentDataPath)) //resources 加载URL
                return loaderUrl;

            int lastPointIndex = loaderUrl.IndexOf('.');
            int length = Application.persistentDataPath.Length;

            return loaderUrl.Substring(length + 1, lastPointIndex - (length + 1));
        }
    }
}
