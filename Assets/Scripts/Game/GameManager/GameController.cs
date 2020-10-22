using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;

public class GameController :Singleton<GameController>
{

    public void Init()
    {
        //加载游戏配置相关的配置文件
        //游戏角色信息
        DataManager.Instance.LoadData("UICharacter", GameManager.Instance.GameCharacterInfoLoadCallBack, false);
        //游戏中对话信息加载
        DataManager.Instance.LoadData("Dialogues", GameManager.Instance.GameDialogueLoadCallBack, false);
        //事件监听
        EventCenter.Instance.RegistListener(SGEventType.GameRuntimeDataInit, GameManagerInit);
        EventCenter.Instance.RegistListener(SGEventType.UIPlayerNumberButtomClick, SetPlayerNumber);
        EventCenter.Instance.RegistListener(SGEventType.UIDifficultyButtomClick, SetGameDifficulty);
        EventCenter.Instance.RegistListener(SGEventType.UIChangeCharacterClick, CharacterChangeListener);
    }

    /// <summary>
    /// 游戏管理器runtime数据初始化
    /// </summary>
    /// <param name="data"></param>
    private void GameManagerInit(EventData data)
    {
        bool allInit = (bool)data.Param;
        GameManager.Instance.InitRuntimeData(allInit);
    }

    /// <summary>
    /// 获得游戏人数
    /// </summary>
    /// <returns></returns>
    public int GetPlayerNumber()
    {
        return GameManager.Instance.GetPlayerNumber();
    }
    
    /// <summary>
    /// 玩家人数设置，设置完毕发送人数变更通知
    /// </summary>
    /// <param name="data"></param>
    private void SetPlayerNumber(EventData data)
    {
        //更改玩家人数
        GameManager.Instance.SetPlayerNumber();
        //获得最新的玩家人数
        int pn = GameManager.Instance.GetPlayerNumber();
        if (pn == 1)
        {
            //更改后为单人游戏，将玩家2的角色置为0
            GameManager.Instance.SetPlayerCharacterId(1, 0);
        }
        else
        {
            //更改后为双人游戏，将玩家2的角色置为1
            GameManager.Instance.SetPlayerCharacterId(1, 1);
        }
        EventCenter.Instance.SendEvent(SGEventType.PlayerNumberChange, null);
    }

    /// <summary>
    /// 获取当前游戏难度
    /// </summary>
    /// <param name="data"></param>
    public DifficultyType GetGameDifficulty()
    {
        return GameManager.Instance.GetDifficulty();
    }

    /// <summary>
    /// "难度" 切换 回调函数
    /// 以Easy，Normal，Hard的顺序切换
    /// </summary>
    /// <param name="data"></param>
    private void SetGameDifficulty(EventData data)
    {
        DifficultyType dt = GameManager.Instance.GetDifficulty();
        switch (dt)
        {
            case DifficultyType.Easy:
                GameManager.Instance.SetDifficulty(DifficultyType.Normal);
                EventCenter.Instance.SendEvent(SGEventType.DifficultyChange, new EventData(DifficultyType.Normal.ToString(), null));
                break;
            case DifficultyType.Normal:
                GameManager.Instance.SetDifficulty(DifficultyType.Hard);
                EventCenter.Instance.SendEvent(SGEventType.DifficultyChange, new EventData(DifficultyType.Hard.ToString(), null));
                break;
            case DifficultyType.Hard:
                GameManager.Instance.SetDifficulty(DifficultyType.Easy);
                EventCenter.Instance.SendEvent(SGEventType.DifficultyChange, new EventData(DifficultyType.Easy.ToString(), null));
                break;
        }
    }
   
    /// <summary>
    /// 角色 更换点击 监听
    /// </summary>
    private void CharacterChangeListener(EventData data)
    {
        int playerId = (int)data.Param;

        int characterId = GameManager.Instance.GetCharacterId(playerId);
        int max = GameManager.Instance.GetNumberOfCharacterUIInfos();

        if (characterId == max - 1)
            characterId = 1;
        else
            characterId++;

        //设置新的角色id
        GameManager.Instance.SetPlayerCharacterId(playerId, characterId);
        //发送更改了玩家对应角色映射
        EventCenter.Instance.SendEvent(SGEventType.PlayerChooseCharacterChange, new EventData(playerId, null));
        
    }

    /// <summary>
    /// 返回玩家所选角色的UI信息
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
   public CharacterUI GetCharacterUIInfo(int playerId)
    {
        return GameManager.Instance.GetCharacterUIInfo(playerId);
    }
    /// <summary>
    /// 获得玩家的角色Id
    /// </summary>
    /// <param name="playerId">玩家id</param>
    /// <returns></returns>
    public int GetCharacterId(int playerId)
    {
        return GameManager.Instance.GetCharacterId(playerId);
    }

    /// <summary>
    /// 获得当前对话
    /// </summary>
    /// <returns></returns>
    public Dialogue GetCurrentDialogue()
    {
        int level = BattleController.Instance.GetCurrentLevel();
        int rank = GameManager.Instance.GetCurrentDialogueRank();
        return GameManager.Instance.GetDialogue(level, rank);
    }
    /// <summary>
    /// 设置下一个对话计数，返回是否到下一关卡了
    /// </summary>
    public bool NextDialogueRank()
    {
        int level = BattleController.Instance.GetCurrentLevel();
        int max = GameManager.Instance.GetDialogueListCount(level);//获得本关卡所有的对话数目
        int rank = GameManager.Instance.GetCurrentDialogueRank();
        if (rank == max - 1)
        {
            //下一关了，重置顺序
            GameManager.Instance.SetDialogueRank(0);
            return true;
        }
        else
        {
            GameManager.Instance.SetDialogueRank(rank + 1);
            return false;
        }
    }

    /// <summary>
    /// 停止时间
    /// </summary>
    public void StopTime()
    {
        Time.timeScale = 0;
    }
    /// <summary>
    /// 恢复时间
    /// </summary>
    public void ResumeTime()
    {
        Time.timeScale = 1;
    }
    /// <summary>
    /// 在场景切换时，需要清除缓存，避免空指针
    /// </summary>
    public void ClearCache()
    {
        //对象池要清空。因为场景中有对象会被清除，有null指针
        PoolManager.Instance.ClearPool();
        //AssetManager中的换粗你也要清空。也会有null指针对象被加载过
       AssetManager.Instance.ClearAssetsCaches();
    }


}
