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

    private Dictionary<int,KeyboardEntity> keyboardEntities = new Dictionary<int, KeyboardEntity>();
     

    private InputListener inputMono = null;

    /// <summary>
    /// 输入框点击的上一个对象
    /// </summary>
    public GameObject LastInputObjectClicked { get; set; }

    public InputManager()
    {
        GameObject go=  GameObject.Find("Input");
        if (go == null)
            go = new GameObject("Input");

        GameObject.DontDestroyOnLoad(go);

        inputMono= go.AddComponent<InputListener>();
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

    /// <summary>
    /// 设置游戏键位
    /// </summary>
    /// <param name="id"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public void SetGameKey(int id,GameKey target,string keyValue )
    {
        KeyboardEntity ke = null;
        if(!keyboardEntities.TryGetValue(id,out ke))
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

    public string GetGamekey(int id,GameKey target)
    {
        KeyboardEntity ke = null;
        if (!keyboardEntities.TryGetValue(id, out ke))
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
    /// 返回键盘配置的 复制
    /// </summary>
    /// <returns></returns>
    public List<KeyboardEntity> GetKeyboardSettings()
    {
        List<KeyboardEntity> res = new List<KeyboardEntity>();
        foreach(KeyValuePair<int,KeyboardEntity> kep  in keyboardEntities)
        {
            KeyboardEntity temp = kep.Value.Clone() as KeyboardEntity;
            res.Add(temp);
        }
        return res;
    }

    public void SetKeyboardSettings(List<KeyboardEntity> entities)
    {

    }

}
