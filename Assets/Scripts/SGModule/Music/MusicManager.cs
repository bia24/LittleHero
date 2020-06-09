using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule
{
    /// <summary>
    /// 简单的音乐管理器
    /// </summary>
    public class MusicManage:Singleton<MusicManage>
    {
        /// <summary>
        /// 播放环境音效的source
        /// </summary>
        private AudioSource mainSource;
        /// <summary>
        /// clip缓存，优化访问，防止resources资源切换场景时卸载
        /// </summary>
        private Dictionary<string, AudioClip> clipsCache = new Dictionary<string, AudioClip>();
        /// <summary>
        /// Clip在resources文件夹中的默认路径
        /// </summary>
        private static string audioResourcePath = "Audio";
      

        public  MusicManage()
        {
            //防止Cemra被摧毁或不存在，自己见一个AudioListener对象
            GameObject go = GameObject.Find("Audio");
            if (go == null)
            {
                go = new GameObject("Audio");
                GameObject.DontDestroyOnLoad(go);
            }
            if (Camera.main != null && Camera.main.GetComponent<AudioListener>() != null)
                Object.Destroy(Camera.main.GetComponent<AudioListener>());
            go.AddComponent<AudioListener>();

            mainSource = go.AddComponent<AudioSource>();
            mainSource.loop = true; //场景 播放器是循环的
        }

        /// <summary>
        /// 设置一个source 的默认音量
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        public void SetVolume(AudioSource source,float value)
        {
            source.volume = value;
        }
        
        public void Play(AudioSource source,string clipName,float volumeScale=1f,bool isBreak = false)
        {
            //获取音乐片段
            AudioClip clip = null;
            if(!clipsCache.TryGetValue(clipName,out clip))
            {
                clip = AssetManager.instance.LoadObject<AudioClip>(audioResourcePath+"/"+clipName);
                if (clip == null)
                {
                    Debug.LogError("can not find AudioClip : "+clipName);
                    return;
                }
                clipsCache.Add(clipName, clip);
            }
            //播放音乐
            if (isBreak) //中断前者播放
            {
                source.PlayOneShot(clip,volumeScale);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 停止声音源的播放，相当于结束。下次play会从0开始
        /// </summary>
        /// <param name="source"></param>
        public void  Stop(AudioSource source)
        {
            if (source.isPlaying)
                source.Stop();
        }

        /// <summary>
        /// 暂停播放声音，下次开始从中断处继续。
        /// </summary>
        /// <param name="source"></param>
        public void Pause(AudioSource source)
        {
            if (source.isPlaying)
                source.Pause();
        }
    }
}
