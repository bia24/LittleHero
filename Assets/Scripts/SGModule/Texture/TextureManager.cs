using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SGModule
{
    public class TextureManager : Singleton<TextureManager>
    {
        private readonly string TEXURE_RESOURCES_PATH = "LoadTexture";
        //纹理缓存
        private Dictionary<string, Object> caches = new Dictionary<string, Object>();

        //获取指定纹理
        public T GetTexture<T>(string name) where T:Object 
        {
            Object res = null;
            if (!caches.TryGetValue(name, out res))
            {
                res = AssetManager.Instance.LoadObject(TEXURE_RESOURCES_PATH + "/" + name);
                if (res == null)
                {
                    Debug.LogError("load texture faild. Name : " + name);
                }
                caches.Add(name, res);
            }

            if (typeof(T).Equals(typeof(Sprite)))
            { //sprite 精灵
                Texture2D t2d = res as Texture2D;
                Sprite s = Sprite.Create(t2d,new Rect(0f,0f,t2d.width,t2d.height),new Vector2(0.5f,0.5f));
                return s as T;
            }
            else if(typeof(T).Equals(typeof(Texture)))
            { //纹理
                return res as T;
            }
            else
            {
                Debug.LogError("can not find this transform texure type");
            }

            return null;
        }
    }
}
