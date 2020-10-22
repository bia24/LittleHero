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

[Serializable]
public class KeyboardSettings
{
    public List<KeyboardEntity> keyboardEntities;
}

[Serializable]
public class KeyboardEntity
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
}

[Serializable]
public class CharacterUIInfos
{
    public List<CharacterUI> characterUIEntities;
}

[Serializable]
public class CharacterUI
{
    public int id; //角色id
    public string name;
    public string iconName;

    public CharacterUI(int i,string n,string iN)
    {
        id = i;
        n = name;
        iconName = iN;
    }
}

[Serializable]
public class BattleParam
{
    public int levelUpExp;
    public int levelMax;
    public float expUpRate;
    public int perEnemyExp;
    public int perEnemyMp;
}


[Serializable]
public class BattleCharacterInfos
{
    public List<BattleCharacter> battleCharacterEntities;
}


[Serializable]
public class BattleCharacter
{
    public int id;
    public string name; //名称
    public string headIconName;
    public string objectName; //预制体名称
    public int hp;
    public int mp;
    public int pdefense;
    public int mdefense;
    public float hpUpRate;//成长率
    public float pdefenseUpRate;
    public float mdefenseUpRate;
    public float damageUpRate;
    /// <summary>
    /// 一段攻击
    /// </summary>
    public int skill1_id; 
    /// <summary>
    /// 二段攻击
    /// </summary>
    public int skill2_id;
    /// <summary>
    /// 三段攻击
    /// </summary>
    public int skill3_id;
    /// <summary>
    /// 冲刺攻击
    /// </summary>
    public int skill4_id;
    /// <summary>
    /// 大招攻击
    /// </summary>
    public int skill5_id;
    /// <summary>
    /// 跳跃攻击
    /// </summary>
    public int skill6_id;
}


[Serializable]
public class SkillInfos
{
    public List<Skill> skills;
}

[Serializable]
public class Skill:ICloneable
{
    public int id;
    public string name;
    public string attackType; //魔法或物理
    public string effect; //效果：普通受击、击退、悬空、倒地
    public int damage;
    public float x;
    public float y;
    public string disType;
    public int mpCost; //蓝量消耗
    /// <summary>
    /// 能量值消耗
    /// </summary>
    public int powerCost;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

[Serializable]
public class LevelInfos
{
    public List<Level> levels;
}

[Serializable]
public class Level
{
    public int id;
    public string bgName;
    public string soundType;
}

/// <summary>
/// 实时玩家战斗信息
/// </summary>
public class RealtimeBattleInfo
{
    public string iconName;
    public int hp;
    public int hpMax;
    public int mp;
    public int mpMax;
    public int power;
    public int pdefense;
    public int mdefense;
    public int level;
    public int exp;
}

/// <summary>
/// 士兵产出计划
/// </summary>

[Serializable]
public class EnemyGenPlan
{
    public List<EachDifficult> eachDifficults;
}


/// <summary>
/// 每一个难度
/// </summary>
[Serializable]
public class EachDifficult
{
    public string difficulty;
    public List<EachLevel> eachLevels;
}

/// <summary>
/// 每一关
/// </summary>
[Serializable]
public class EachLevel
{
    public int level;
    public List<EachTime> eachTimes;
}
/// <summary>
/// 每一次
/// </summary>
[Serializable]
public class EachTime
{
    public int number;
    public List<EnemyDetail> enemyDetails;
}
/// <summary>
/// 敌人详细
/// </summary>
[Serializable]
public class EnemyDetail
{
    public int characterId;
    public string pos;
}

[Serializable]
public class ComboInfos
{
    public List<ComboInfo> comboInfos;
}

[Serializable]
public class ComboInfo
{
    public int characterId;
    public List<Combo> combos;
}
[Serializable]
public class Combo: ICloneable
{
    public int comboId;
    public int skillId;
    public int n;
    public string name;
    public string key1;
    public string key2;
    public string key3;
    public string key4;
    public string key5;
    public string key6;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

[Serializable]
public class DialogueInfo
{
    public List<Dialogue> dialogues;
}

[Serializable]
public class Dialogue
{
    public int level;
    public int rank;
    public string type;
    public string iconName;
    public string context;
}

[Serializable]
public class Boundary
{
    public float moveXMin;
    public float moveXMax;
    public float moveYMin;
    public float moveYMax;
    public float cameraXMin;
    public float cameraXMax;
    public float triggerLeft;
    public float triggerOne;
    public float triggerTwo;
    public float triggerThree;
    public float triggerFour;
}

