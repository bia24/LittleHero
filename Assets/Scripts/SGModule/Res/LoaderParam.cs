using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
  /// <summary>
  /// 加载器需要传入的参数
  /// </summary>
    public class LoaderInParam 
    {
        public string Url { get;}
        public LoadCompleteCallBack Callback { get; }

        public LoaderInParam(string url,LoadCompleteCallBack callback)
        {
            Url = url;
            Callback = callback;
        }
    }

   
}
