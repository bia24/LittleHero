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
    public List<DifficultyParam> difficultyParams;
}

[Serializable]
public class DifficultyParam
{
    public string type; //难度类型 easy、normal、difficult
    public float pdefenseUpRate;
    public float mdefenseUpRate;
    public float damageUpRate;
    public float reduceRate;//效果减少比例
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
    public string name;
    public string headIconName;
    public string objectName;
    public int hp;
    public int mp;
    public int pdefense;
    public int mdefense;
    public float hpUpRate;//成长率
    public float pdefenseUpRate;
    public float mdefenseUpRate;
    public float damageUpRate;
    public int skill1_id; //出招动作1绑定的技能数值
    public int skill2_id;
    public int skill3_id;
    public int skill4_id;
    public int skill5_id;
}


[Serializable]
public class SkillInfos
{
    public List<Skill> skills;
}

[Serializable]
public class Skill
{
    public int id;
    public string name;
    public string attackType; //魔法或物理
    public string effect; //效果：普通受击、击退、悬空、倒地
    public int damage;
    public float transformDis;//位移距离
    public float x;
    public float y;
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

