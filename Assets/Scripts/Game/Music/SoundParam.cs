using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//音乐播放的回调参数
public class SoundParam
{
   public string type;
   public  AudioSource source;
    public bool isBreak;
    public bool isLoop;
    public bool isCustomVolume;
    public float customVolumeScale;

    public SoundParam(AudioSource s,string t,bool isBreak=true,bool isLoop=false,bool isCustomVolume=false,float customVolumeScale=0.0f)
    {
        source = s;
        type = t;
        this.isBreak = isBreak;
        this.isLoop = isLoop;
        this.isCustomVolume = isCustomVolume;
        this.customVolumeScale = customVolumeScale;
    }
}

