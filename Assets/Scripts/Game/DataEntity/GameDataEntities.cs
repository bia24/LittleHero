using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 本游戏中 音乐 的业务数据json转换实体
/// </summary>
[Serializable]   
public class Sound 
{
    public List<SoundEntity> soundEntitisList;

}
/// <summary>
/// 本游戏中 音乐类型到名称映射 的业务数据json转换实体
/// </summary>
[Serializable]
public class SoundEntity
{
    public string type;
    public string name;
}

/// <summary>
/// 本游戏中 音乐 配置 的转换实体
/// </summary>
[Serializable]
public class SoundSettingsEntity
{
    public Single soundVolume;
    public float soundEffectVolume;
    public int soundTrigger;
    public int soundEffectTrigger;

    public SoundSettingsEntity(float sv,float sev,int st,int set)
    {
        soundVolume = sv;
        soundEffectVolume = sev;
        soundTrigger = st;
        soundEffectTrigger = set;
    }
}

/// <summary>
/// 本游戏中 游戏设置 的业务数据json转换实体
/// </summary>
[Serializable]
public class GameSettings
{
    public string difficulty;
    public int playerNumber;
}

[Serializable]
public class KeyboardSettings
{
    public List<KeyboardEntity> keyboardEntities;
}

[Serializable]
public class KeyboardEntity:ICloneable
{
    public int id;
    public string up;
    public string down;
    public string left;
    public string right;
    public string attack;
    public string jump;
   
    public KeyboardEntity(int i,string u,string d,string l,string r,string a,string j)
    {
        id = i;
        up = u;
        down = d;
        left = l;
        right = r;
        attack = a;
        jump = j;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

