using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Input 输入逻辑 处理
/// </summary>
public class InputController : Singleton<InputController>
{
    public void Init()
    {
        //读取默认键盘操作配置文件
        DataManager.Instance.LoadData("KeyboardSettings", InputManager.Instance.InitKeyboardSettingsCallBack, true);
        DataManager.Instance.LoadData("Combo", InputManager.Instance.InitComboInfosCallBack, false);

        //绑定事件
        EventCenter.Instance.RegistListener(SGEventType.InputFieldListenerAdd, InputFieldListenerAdd);
        EventCenter.Instance.RegistListener(SGEventType.InputFieldListenerRemove, InputFieldListenerRemove);
        EventCenter.Instance.RegistListener(SGEventType.InputBattleListenerAdd, InputBattleListenerAdd);
        EventCenter.Instance.RegistListener(SGEventType.InputBattleListenerRemove, InputBattleListenerRemove);
        EventCenter.Instance.RegistListener(SGEventType.InputFieldClick, InputFieldClickListener);
        EventCenter.Instance.RegistListener(SGEventType.SetLastFieldRefNull, SetNullLastRefListener);
        EventCenter.Instance.RegistListener(SGEventType.InputConfigSave, InputConfigSaveListener);
        
    }

    /// <summary>
    /// 挂载 InputFieldListener脚本，用来监听输入框修改
    /// </summary>
    /// <param name="data"></param>
    private void InputFieldListenerAdd(EventData data)
    {
        InputManager.Instance.AddInputFieldListener();
    }
    /// <summary>
    /// 删除InputFieldListener脚本
    /// </summary>
    /// <param name="data"></param>
    private void InputFieldListenerRemove(EventData data)
    {
        InputManager.Instance.RemoveInputFieldListener();
    }
    /// <summary>
    /// 挂载战斗输入监听脚本
    /// </summary>
    /// <param name="data"></param>
    private void InputBattleListenerAdd(EventData data)
    {
        int playerId = (int)data.Param;//获得需要添加的玩家id
        InputManager.Instance.AddBattleInputListener(playerId);
    }
    /// <summary>
    /// 移除监听 战斗输入
    /// </summary>
    /// <param name="data"></param>
    private void InputBattleListenerRemove(EventData data)
    {
        int playerId = (int)data.Param;
        InputManager.Instance.RemoveBattleInputListener(playerId);
    }
    /// <summary>
    /// 键盘配置获取 , 仅做初始化显示使用
    /// </summary>
    /// <param name="data"></param>
    public KeyboardEntity GetPlayerKeyboardSetting(int playerId)
    {
        return InputManager.Instance.GetPlayerKeyboardSetting(playerId);        
    }

    /// <summary>
    ///  返回角色的连招表索引，依据连招id索引
    /// </summary>
    /// <param name="characterId"></param>
    /// <returns></returns>
    public  Dictionary<int, Combo> GetComboDic(int playerId)
    {
        int characterId = GameController.Instance.GetCharacterId(playerId);
        return InputManager.Instance.GetComboDic(characterId);
    }

    /// <summary>
    ///键盘输入框文字改变 监听
    /// </summary>
    /// <param name="data"></param>
    private void InputFieldClickListener(EventData data)
    {
        GameObject cur = data.Sender;
        InputManager.Instance.LastInputObjectClicked = cur;
        //监控键盘按键Mono 通知
        EventCenter.Instance.SendEvent(SGEventType.InputFieldOn, null);
        
    }

    /// <summary>
    /// 设置上一个点击的输入框为null
    /// </summary>
    /// <param name="data"></param>
    private void SetNullLastRefListener(EventData data)
    {
        InputManager.Instance.LastInputObjectClicked = null;
    }

    /// <summary>
    /// 保存当前按钮配置
    /// </summary>
    /// <param name="data"></param>
    private void InputConfigSaveListener(EventData data)
    {
        bool onePlayer = true;
        //将InputManager中的数据更新为最新
        List<Text> inputFields = data.Param as List<Text>;
        foreach(var t in inputFields)
        {
            int id = 0;
            string gameKey = null;
            if (t.gameObject.name.EndsWith("_Player2"))
            {
                //id=1，玩家2
                id = 1;
                onePlayer = false;
            }
            //Text_xxx形式
            gameKey = t.name.Split('_')[1];
            GameKey key = (GameKey)Enum.Parse(typeof(GameKey), gameKey, true);
            InputManager.Instance.SetGameKey(id, key, t.text);
        }

        if (onePlayer)
        {
            //排除一种情况。单人配置存储后，本身是合法的。但是与双人配置冲突，此时自动调整双人配置，保证manager中的配置一直都是合法的
            List<string> bucket0 = new List<string>();
            List<string> bucket1 = new List<string>();
            foreach (GameKey gk in Enum.GetValues(typeof(GameKey)))
            {
                string key = InputManager.Instance.GetGamekey(0, gk);
                bucket0.Add(key);
            }

            foreach (GameKey gk in Enum.GetValues(typeof(GameKey)))
            {
                string key = InputManager.Instance.GetGamekey(1, gk);

                if (bucket0.Exists((v) => { return v.Equals(key); }))
                {
                    //这个gk 要改值
                    InputManager.Instance.SetGameKey(1, gk, "");
                }
                else
                {
                    bucket1.Add(key);
                }
            }

            foreach (GameKey gk in Enum.GetValues(typeof(GameKey)))
            {
                string key = InputManager.Instance.GetGamekey(1, gk);
                if (key.Equals(""))
                {
                    foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                    {
                        if (kc!=KeyCode.None
                            &&!bucket1.Exists((v) => { return v.Equals(kc.ToString()); }) &&
                            !bucket0.Exists((v) => { return v.Equals(kc.ToString()); }))
                        {
                            InputManager.Instance.SetGameKey(1, gk, kc.ToString());
                            bucket1.Add(kc.ToString());
                            break;
                        }
                    }
                }
            }
        }
        //将最新数据存储到persistent文件夹中，保持内存中和磁盘中一致

        List<KeyboardEntity> kbe = new List<KeyboardEntity>();
        for(int playerId = 0; playerId < 2; playerId++)
        {
            kbe.Add(GetPlayerKeyboardSetting(playerId));
        }
        KeyboardSettings kbs = new KeyboardSettings();
        kbs.keyboardEntities = kbe;
        string context = JsonUtility.ToJson(kbs,true);
        DataManager.Instance.SaveData("KeyboardSettings", context);
    }

    /// <summary>
    /// 返回玩家id 指定键位上的键值
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="gameKey"></param>
    /// <returns></returns>
    public KeyCode GetGameKey(int playerId,GameKey gameKey)
    {
        string value = InputManager.Instance.GetGamekey(playerId, gameKey);
        return (KeyCode)Enum.Parse(typeof(KeyCode), value);
    }

    /// <summary>
    /// 获得连击的间隔时间
    /// </summary>
    /// <returns></returns>
    public float GetCombointermissionTime()
    {
        return InputManager.Instance.GetCombointermissionTime();
    }
}
