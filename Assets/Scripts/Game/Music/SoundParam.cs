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

    public SoundParam(AudioSource s,string t,bool isBreak=true,bool isLoop=false)
    {
        source = s;
        type = t;
        this.isBreak = isBreak;
        this.isLoop = isLoop;
    }
}
