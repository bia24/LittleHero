using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

    /// <summary>
    /// 简单的音乐管理器
    /// </summary>
    public class MusicManager:Singleton<MusicManager>
    {
        /// <summary>
        /// 播放环境音效的source
        /// </summary>
        private static AudioSource _mainSource;
        public  AudioSource MainSource { get { return _mainSource; } }
        /// <summary>
        /// clip缓存，优化访问，防止resources资源切换场景时卸载
        /// </summary>
        private Dictionary<string, AudioClip> clipsCache = new Dictionary<string, AudioClip>();
        /// <summary>
        /// Clip在resources文件夹中的默认路径
        /// </summary>
        private static string audioResourcePath = "Sound";
        /// <summary>
        /// 音乐音量
        /// </summary>
        private float soundVolume;
        /// <summary>
        /// 音乐开关
        /// </summary>
        private bool soundTrigger;
        /// <summary>
        /// 音效音量
        /// </summary>
        private float soundEffectVolume;
        /// <summary>
        /// 音效开关
        /// </summary>
        private bool soundEffectTrigger;
        /// <summary>
        /// 声音类型 到 名称 的映射 ，从配置文件中初始化
        /// </summary>
        private Dictionary<string, string> soundName = new Dictionary<string, string>();



        public  MusicManager()
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

            _mainSource = go.AddComponent<AudioSource>();

        }

        /// <summary>
        /// 从json配置文件中读入，并存在字典中，该内容因为不会动态修改，所以不需要在persistent中
        /// </summary>
        /// <param name="context"></param>
        public void InitSoundNameConfigCallBack(string context)
        {
            Sound s = JsonUtility.FromJson<Sound>(context);

            foreach (SoundEntity se in s.soundEntitisList)
            {
                soundName.Add(se.type, se.name);
            }
        }
        /// <summary>
        /// 音乐配置文件读取回调函数
        /// </summary>
        /// <param name="context"></param>
        public void InitSoundSettingsConfigCallBack(string context)
        {
            SoundSettingsEntity sse = JsonUtility.FromJson<SoundSettingsEntity>(context);
            soundVolume = sse.soundVolume;
            soundEffectVolume = sse.soundEffectVolume;
            soundTrigger = sse.soundTrigger == 0 ? false : true;
            soundEffectTrigger = sse.soundEffectTrigger == 0 ? false : true;
        }


        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="source"></param>
        /// <param name="clipType"></param>
        /// <param name="volumeScale"></param>
        /// <param name="isBreakOther"></param>
        public void Play(AudioSource source,string clipType,bool isBreakOther = false,bool isLoop=false)
        {
            //获取音乐名称
            string clipName = null;

            if (!soundName.TryGetValue(clipType, out clipName) || clipName.Equals(""))
                Debug.LogError("can not find clipName: "+ clipType);


            //获取音乐片段
            AudioClip clip = null;
            if(!clipsCache.TryGetValue(clipName,out clip))
            {
                clip = AssetManager.Instance.LoadObject<AudioClip>(audioResourcePath+"/"+clipName);
                if (clip == null)
                {
                    Debug.LogError("can not find AudioClip : "+clipName);
                    return;
                }
                clipsCache.Add(clipName, clip);
            }
            //若循环播放，循环播放认为是bgm音乐
            if (isLoop)
            {
                source.Stop();
                //循环播放只能通过替换默认clip实现
                source.loop = true;
                source.clip = clip;
                if (soundTrigger)
                {
                    source.volume = soundVolume;
                }
                else
                {
                    source.volume = 0.0f;
                }
                source.Play();
                return;
            }

            //音效播放
            source.loop = false;
            float volume = soundEffectTrigger == true ? soundEffectVolume : 0.0f;
            if (isBreakOther) //中断前者播放
            {
                source.Stop();
                source.PlayOneShot(clip, volume);
            }
            else
            {
                source.PlayOneShot(clip, volume);
            }
        }

        /// <summary>
        /// 获得音量值
        /// </summary>
        /// <returns></returns>
        public float GetSoundVolume()
        {
            return soundVolume;
        }
        /// <summary>
        /// 设置音量
        /// </summary>
        /// <returns></returns>
        public void  SetSoundVolume(float v)
        {
            soundVolume = Mathf.Clamp(v, 0.0f, 1.0f);
        }

        /// <summary>
        /// 将音乐音量应用到播放器
        /// </summary>
        public void ApplySoundVolume()
        {
            _mainSource.volume = soundVolume;
        }

        /// <summary>
        /// 获得声音开关
        /// </summary>
        /// <returns></returns>
        public bool GetSoundTrigger()
        {
            return soundTrigger;
        }

        /// <summary>
        /// 音乐开关，控制bgm
        /// </summary>
        /// <param name="t"></param>
        public void SetSoundTrigger(bool t)
        {
            if (soundTrigger == t)
                return;

            soundTrigger = t;

            if (soundTrigger)
            {
                _mainSource.volume = soundVolume;
            }
            else
            {
                _mainSource.volume = 0.0f;
            }
        }

        /// <summary>
        /// 获得音效值
        /// </summary>
        /// <returns></returns>
        public float GetSoundEffectVolume()
        {
            return soundEffectVolume;
        }

        /// <summary>
        /// 设置音效值
        /// </summary>
        /// <param name="v"></param>
        public void SetSoundEffectVolume(float v)
        {
            soundEffectVolume = Mathf.Clamp(v, 0.0f, 1.0f);
        }
        /// <summary>
        /// 获得音效开关
        /// </summary>
        /// <returns></returns>
       public bool GetSoundEffectTrigger()
        {
            return soundEffectTrigger;
        }
        /// <summary>
        /// 音效开关
        /// </summary>
        /// <param name="t"></param>
        public void SetSoundEffectTrigger(bool t)
        {
            soundEffectTrigger = t;
        }

        /// <summary>
        ///获得声音配置实体
        /// </summary>
        /// <returns></returns>
        public SoundSettingsEntity GetSoundSettings()
        {
            int st = soundTrigger == true ? 1 : 0;
            int set = soundEffectTrigger == true ? 1 : 0;
            return new SoundSettingsEntity(soundVolume,soundEffectVolume,st,set);
        }
       

    }
