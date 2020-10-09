using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;

/// <summary>
/// 本游戏的键盘按键枚举
/// </summary>
public enum GameKey
{
    Up,
    Down,
    Left,
    Right,
    Attack,
    Jump
}

/// <summary>
/// 键盘输入管理器
/// </summary>
public class InputManager :Singleton<InputManager>
{
    #region 从配置文件中读入，运行时会改变，保持内存和配置文件中一致(会对本地修改)
    private Dictionary<int,KeyboardEntity> keyboardEntities = new Dictionary<int, KeyboardEntity>();
    #endregion

    #region 从配置文件中读入，运行时不改变
    private Dictionary<int, Dictionary<int, Combo>> combos = new Dictionary<int, Dictionary<int, Combo>>();
    #endregion

    #region 创建管理器实例的时候获取，不摧毁，不改变
    private GameObject InputGo = null;
    #endregion

    #region 只读常量，不改变
    private readonly float COMBO_TIME = 0.5f;
    #endregion
    /// <summary>
    /// 输入框点击的上一个对象
    /// </summary>
    public GameObject LastInputObjectClicked { get; set; }

    public InputManager()
    {

        InputGo =  GameObject.Find("Input");

        if (InputGo == null)
            InputGo = new GameObject("Input");

        GameObject.DontDestroyOnLoad(InputGo);

    }

    /// <summary>
    /// 添加 输入框监控
    /// </summary>
    public void AddInputFieldListener()
    {
        if (InputGo.GetComponent<InputFieldListener>() != null)
            Debug.LogError("InputFieldListener shouldn't  be here!");
        InputGo.AddComponent<InputFieldListener>();
    }
    /// <summary>
    /// 
    /// </summary>
    public void RemoveInputFieldListener()
    {
        InputFieldListener target = InputGo.GetComponent<InputFieldListener>();
        if (target==null)
        {
            Debug.LogWarning("InputFieldListener not found");
            return;
        }
        target.RemoveListener();
    }
    /// <summary>
    /// 添加 战斗输入 监听
    /// </summary>
    public void AddBattleInputListener(int playerId)
    {
        InputBattleListener[] listeners = InputGo.GetComponents<InputBattleListener>();
        foreach(var t in listeners)
        {
            if (t.GetPlayerId() == playerId)
            {
                Debug.LogError("InputBattleListener can not be same playerId : "+playerId);
                return;
            }
        }
        InputBattleListener target= InputGo.AddComponent<InputBattleListener>();
        //用玩家id初始化
        target.Init(playerId);
    }

    /// <summary>
    /// 移除战斗输入监听
    /// </summary>
    public void RemoveBattleInputListener(int playerId)
    {
        InputBattleListener[]listeners = InputGo.GetComponents<InputBattleListener>();
        if (listeners.Length == 0)
        {
            Debug.LogWarning("InputBattleListener not found");
            return;
        }
        
        for(int i = 0; i < listeners.Length; i++)
        {
            if (listeners[i].GetPlayerId() == playerId)
            {
                listeners[i].RemoveListener(); //删除指定id的监听器
                return;
            }
        }

        Debug.LogError("InputBattleListener can not find according to this playerId : "+ playerId);
    }

    /// <summary>
    /// 初始化键盘设置的回调函数
    /// </summary>
    public void InitKeyboardSettingsCallBack(string context)
    {
        if (string.IsNullOrEmpty(context))
        {
            Debug.LogError("Init keyboard config error ! Null config string");
        }

        try
        {
            KeyboardSettings kss = JsonUtility.FromJson<KeyboardSettings>(context);
            foreach(var k in kss.keyboardEntities)
            {
                keyboardEntities.Add(k.id, k);
            }
        }catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void InitComboInfosCallBack(string context)
    {
        if (string.IsNullOrEmpty(context))
        {
            Debug.LogError("Init ComboInfos config error ! Null config string");
        }

        try
        {
            ComboInfos cis = JsonUtility.FromJson<ComboInfos>(context);
            foreach (ComboInfo ci in cis.comboInfos)
            {
                Dictionary<int, Combo> t_0 = new Dictionary<int, Combo>();

                foreach(Combo c in ci.combos)
                {
                    t_0.Add(c.comboId,c);
                }

                combos.Add(ci.characterId, t_0);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// 设置游戏键位
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public void SetGameKey(int playerId,GameKey target,string keyValue )
    {
        KeyboardEntity ke = null;
        if(!keyboardEntities.TryGetValue(playerId,out ke))
        {
            Debug.LogError("player id can not match one keyboard entity");
        }

        switch (target)
        {
            case GameKey.Up:
                ke.up = keyValue;
                break;
            case GameKey.Down:
                ke.down = keyValue;
                break;
            case GameKey.Left:
                ke.left = keyValue;
                break;
            case GameKey.Right:
                ke.right = keyValue;
                break;
            case GameKey.Attack:
                ke.attack = keyValue;
                break;
            case GameKey.Jump:
                ke.jump = keyValue;
                break;
        }
    }

    /// <summary>
    /// 获得玩家id，指定键位上的键值
    /// </summary>
    /// <param name="PlayerId"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public string GetGamekey(int PlayerId,GameKey target)
    {
        KeyboardEntity ke = null;
        if (!keyboardEntities.TryGetValue(PlayerId, out ke))
        {
            Debug.LogError("player id can not match one keyboard entity");
        }

        string res = null;

        switch (target)
        {
            case GameKey.Up:
                res = ke.up;
                break;
            case GameKey.Down:
                res = ke.down;
                break;
            case GameKey.Left:
                res = ke.left;
                break;
            case GameKey.Right:
                res = ke.right;
                break;
            case GameKey.Attack:
                res = ke.attack;
                break;
            case GameKey.Jump:
                res = ke.jump;
                break;
        }

        return res;
    }

    /// <summary>
    /// 返回键盘配置
    /// </summary>
    /// <returns></returns>
    public KeyboardEntity GetPlayerKeyboardSetting(int playerId)
    {
        KeyboardEntity res = null;
        if(!keyboardEntities.TryGetValue(playerId,out res))
        {
            Debug.LogError("can not find KeyboardEntity according this playerId : "+playerId);
        }
        return res;
    }

    /// <summary>
    /// 返回角色连招表索引
    /// </summary>
    /// <param name="characterId"></param>
    /// <returns></returns>
    public  Dictionary<int,Combo> GetComboDic(int characterId)
    {
        Dictionary<int, Combo> res = null;
        if(!combos.TryGetValue(characterId,out res))
        {
            Debug.LogError("can not find combo list according this characterId : "+ characterId);
        }
        return res;
    }

    /// <summary>
    /// 连击间隔时间
    /// </summary>
    /// <returns></returns>
    public float GetCombointermissionTime()
    {
        return COMBO_TIME;
    }



}
