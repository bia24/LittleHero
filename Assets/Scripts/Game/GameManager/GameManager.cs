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
/// 本游戏中需要使用到的数据的管理类，可以理解为model（data）的持有者，一个外界沟通的中介
/// </summary>
public class GameManager :Singleton<GameManager>
{
   
    /// <summary>
    /// 难度
    /// </summary>
    private DifficultyType difficulty;
    /// <summary>
    /// 玩家人数
    /// </summary>
    private int playerNumber;

    public GameManager()
    {
    }


    public void GameSettingsInitCallBack(string context)
    {
        GameSettings gs = JsonUtility.FromJson<GameSettings>(context);

        playerNumber = Mathf.Clamp(gs.playerNumber, 1, 2);

        try
        {
            difficulty = (DifficultyType)Enum.Parse(typeof(DifficultyType), gs.difficulty);
        }catch(Exception e)
        {
            Debug.LogError(e.Message);
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
}
