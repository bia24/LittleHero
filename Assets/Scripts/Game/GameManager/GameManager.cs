using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SGModule;

/// <summary>
/// 游戏难易控制枚举
/// </summary>
public enum DifficultyType
{
    Easy,
    Normal,
    Hard
}
/// <summary>
/// 玩家id和角色id的映射参数
/// </summary>
public struct PlayerCharacterID
{
    public int playerId;
    public int CharacterID;
}

/// <summary>
/// 本游戏中需要使用到的数据的管理类，可以理解为model（data）的持有者，一个外界沟通的中介
/// </summary>
public class GameManager :Singleton<GameManager>
{
    #region 初始化配置文件中读入，游戏中不改变的配置信息
    /// <summary>
    /// 角色游戏信息
    /// </summary>
    private Dictionary<int, CharacterUI> characterUIInfos = new Dictionary<int, CharacterUI>();
    /// <summary>
    /// 关卡-对话索引
    /// </summary>
    private Dictionary<int, List<Dialogue>> dialogues = new Dictionary<int, List<Dialogue>>();
    #endregion

    #region 非配置文件中读入的游戏设置，运行时初始化，会随游戏进程改变
    /// <summary>
    /// 难度
    /// </summary>
    private DifficultyType difficulty;
    /// <summary>
    /// 玩家人数
    /// </summary>
    private int playerNumber;
    /// <summary>
    /// 玩家所选角色
    /// </summary>
    private Dictionary<int, int> playerIdToCharacterId = new Dictionary<int, int>();
    /// <summary>
    /// 对话计数器
    /// </summary>
    private int dialogueRank;
    #endregion

    /// <summary>
    /// 初始化运行时所需的数据
    /// </summary>
    public void InitRuntimeData(bool allInit)
    {
        if (allInit) //可选初始化
        {
            //游戏难度初始化
            difficulty = DifficultyType.Easy;
            //玩家人数初始化
            playerNumber = 1;
        }

        // 玩家id到角色的映射初始化
        ResetPlayerIdToCharacterId(playerNumber);
        //对话计数器初始化
        dialogueRank = 0;
    }

   
    /// <summary>
    /// 重置玩家所选角色映射
    /// </summary>
    public void ResetPlayerIdToCharacterId(int playerNumber)
    {
        //默认是1个玩家，因此玩家2选择NoneUI显示
        playerIdToCharacterId.Clear();
        playerIdToCharacterId.Add(0, 1);
        if (playerNumber < 2)
        {
            playerIdToCharacterId.Add(1, 0);
        }
        else
        {
            playerIdToCharacterId.Add(1, 1);
        }
    }

    /// <summary>
    /// 游戏角色信息加载回调函数
    /// </summary>
    /// <param name="context"></param>
    public void GameCharacterInfoLoadCallBack(string context)
    {
        CharacterUIInfos cis = JsonUtility.FromJson<CharacterUIInfos>(context);
        
        foreach(CharacterUI c in cis.characterUIEntities)
        {
            characterUIInfos.Add(c.id, c);
        }
    }
    /// <summary>
    /// 游戏对话加载回调函数
    /// </summary>
    /// <param name="context"></param>
    public void GameDialogueLoadCallBack(string context)
    {
        
        DialogueInfo d = null;
        try
        {
            d = JsonUtility.FromJson<DialogueInfo>(context);
        }catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
       
        dialogues.Add(1, new List<Dialogue>());
        dialogues.Add(2, new List<Dialogue>());
        dialogues.Add(3, new List<Dialogue>());
        foreach(var t in d.dialogues)
        {
            dialogues[t.level].Add(t); //添加所有对话
        }
        for(int i = 1; i <= 3; i++)
        {
            //排序
            dialogues[i].Sort((x, y) => { if (x.rank < y.rank) return -1; else if (x.rank == y.rank) return 0; else return 1; });
        }
      
    }
    /// <summary>
    /// 获得游戏难度
    /// </summary>
    /// <returns></returns>
    public DifficultyType GetDifficulty()
    {
        return difficulty;
    }
    /// <summary>
    /// 设置难度
    /// </summary>
    /// <param name="type"></param>
    public void SetDifficulty(DifficultyType type)
    {
        difficulty = type;
    }

    /// <summary>
    /// 获得玩家人数
    /// </summary>
    /// <returns></returns>
    public int GetPlayerNumber()
    {
        return playerNumber;
    }

    /// <summary>
    /// 玩家人数 变更
    /// </summary>
    public void SetPlayerNumber()
    {
        playerNumber = 3 - playerNumber;
    }

  /// <summary>
  /// 依据玩家id返回其角色信息
  /// </summary>
  /// <param name="playerId">玩家Id</param>
  /// <returns></returns>
    public CharacterUI GetCharacterUIInfo(int playerId)
    {
        int characterId = GetCharacterId(playerId);

        CharacterUI res = null;
        if (!characterUIInfos.TryGetValue(characterId, out res))
            Debug.LogError("can not find this characterUIInfo : "+ characterId);
        return res;
    }
    /// <summary>
    /// 获得玩家可选的角色的UI信息数目
    /// </summary>
    /// <returns></returns>
    public int GetNumberOfCharacterUIInfos()
    {
        return characterUIInfos.Count;
    }
    /// <summary>
    /// 依据玩家id 查找角色id
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public int GetCharacterId(int playerId)
    {
        int cId = default;
        if (!this.playerIdToCharacterId.TryGetValue(playerId, out cId))
            Debug.LogError("can not find this characterId: "+playerId);
        return cId;
    }

    /// <summary>
    /// 根据玩家id 切换其所选的角色
    /// </summary>
    public void SetPlayerCharacterId(int playerId,int characterId)
    {
        if ((playerId < 0 || playerId > 1) || (characterId < 0 || characterId >= characterUIInfos.Count))
            Debug.LogError("非法的 playerId 或者characterId");
        playerIdToCharacterId[playerId] = characterId;
    }
    /// <summary>
    /// 获得当前对话计数
    /// </summary>
    /// <returns></returns>
    public int GetCurrentDialogueRank()
    {
        return this.dialogueRank;
    }
    /// <summary>
    /// 设置当前对话计数
    /// </summary>
    /// <param name="count"></param>
    public void  SetDialogueRank(int rank)
    {
        this.dialogueRank = rank;
    }
    /// <summary>
    /// 得到某个关卡的对话集合数目
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int GetDialogueListCount(int level)
    {
        return dialogues[level].Count;
    }
    /// <summary>
    /// 获得指定关卡指定次序的对话信息
    /// </summary>
    /// <param name="level"></param>
    /// <param name="rank"></param>
    /// <returns></returns>
    public Dialogue GetDialogue(int level,int rank)
    {
        return dialogues[level][rank];
    }
}
