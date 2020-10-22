using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;


public class InputBattleListener : MonoBehaviour
{
    #region 脚本创建时进行初始化 init 一次
    private int playerId;
    /// <summary>
    /// 连击的间隔时间
    /// </summary>
    private float comboTime;
    /// <summary>
    /// 检测开关
    /// </summary>
    private bool listenerTrigger = false;
    /// <summary>
    /// 退出按钮开关
    /// </summary>
    private bool escButtonTrigger = false;
    /// <summary>
    /// 玩家输入按键的缓存
    /// </summary>
    private List<GameKey> playerInputCache = new List<GameKey>();
    /// <summary>
    /// 检测用户输入的变量
    /// </summary>
    private PlayerInput playerInput;
   
    /// <summary>
    /// 玩家输入间隔累加
    /// </summary>
    private float inputDeltaTime;
 
    #endregion
    /// <summary>
    /// 用于检测combo存在性的缓存
    /// </summary>
    private List<int> checkCache = new List<int>();



    public void Init(int playerId)
    {
        //参数设置
        this.playerId = playerId;
        comboTime = InputController.Instance.GetCombointermissionTime();

        //初始化
        inputDeltaTime= 0.0f;
        ClearInputCache();

        playerInput = new PlayerInput();

        //绑定监听
        RegisterEvent();
        //激活开关，依据当前游戏是否运行态 确定是否激活。
        bool active = BattleController.Instance.GetGameState() == GameState.Running ? true : false;
        SetTrigger(active);
        escButtonTrigger = true;
    }

    private void RegisterEvent()
    {
        //绑定监听
        EventCenter.Instance.RegistListener(SGEventType.BattlePause, SetTriggerOffListener);
        EventCenter.Instance.RegistListener(SGEventType.BattlePauseExit, SetTriggerOnListener);
        EventCenter.Instance.RegistListener(SGEventType.BattleDialogueAppear, SetTriggerOffListener);
        EventCenter.Instance.RegistListener(SGEventType.BattleDialogueAppear, SetEcsTriggerOffLisener);
        EventCenter.Instance.RegistListener(SGEventType.BattleDialogueExit, SetEcsTriggerOnLisener);
        EventCenter.Instance.RegistListener(SGEventType.BattleDialogueExit, SetTriggerOnListener);
        EventCenter.Instance.RegistListener(SGEventType.UIDarkPanel, SetEcsTriggerOffLisener);
    }

    private void RemoveEvent()
    {
        //取消监听
        EventCenter.Instance.RemoveListener(SGEventType.BattlePause, SetTriggerOffListener);
        EventCenter.Instance.RemoveListener(SGEventType.BattlePauseExit, SetTriggerOnListener);
        EventCenter.Instance.RemoveListener(SGEventType.BattleDialogueAppear, SetTriggerOffListener);
        EventCenter.Instance.RemoveListener(SGEventType.BattleDialogueAppear, SetEcsTriggerOffLisener);
        EventCenter.Instance.RemoveListener(SGEventType.BattleDialogueExit, SetEcsTriggerOnLisener);
        EventCenter.Instance.RemoveListener(SGEventType.BattleDialogueExit, SetTriggerOnListener);
        EventCenter.Instance.RemoveListener(SGEventType.UIDarkPanel, SetEcsTriggerOffLisener);
    }

    /// <summary>
    /// 退出按钮的监控
    /// </summary>
    /// <param name="data"></param>
    private void SetEcsTriggerOffLisener(EventData data)
    {
        escButtonTrigger = false;
    }
    /// <summary>
    /// 退出按钮的监控
    /// </summary>
    /// <param name="data"></param>
    private void SetEcsTriggerOnLisener(EventData data)
    {
        escButtonTrigger = true;
    }

    /// <summary>
    /// 游戏暂停时的监听
    /// </summary>
    private void SetTriggerOffListener(EventData data)
    {
        SetTrigger(false);
    }

    private void SetTriggerOnListener(EventData data)
    {
        SetTrigger(true);
    }

    /// <summary>
    /// 设置监听开关
    /// </summary>
    /// <param name="data"></param>
    private void SetTrigger(bool trigger)
    {
        listenerTrigger = trigger;
    }
   
    /// <summary>
    /// 获得本脚本监听的玩家id
    /// </summary>
    /// <returns></returns>
    public int GetPlayerId()
    {
        return this.playerId;
    }

    private void Update()
    {
        if (listenerTrigger)
        {
            //间隔时间累加
            InputDeltaTimeSumUp();

            if (Input.anyKeyDown) //检测开启，按下按键。优先检测是否有按键按下
            {
                
                if (GetKeyDownCode(out playerInput))//该输入有效，是本脚本监听的id玩家的指定键位按下 输入；
                    //同一帧中监听到了多个同一玩家按键按下，只发送一个。不支持多按键按下
                {
                   int comboIndex = -1;
                   if(MakeCombo(playerInput,out comboIndex))
                    {
                        //产生了有效combo，本次input本身代表的动作不发送command，只将comboIndex发送出去
                        EventCenter.Instance.SendEvent(SGEventType.ComboId,new EventData(playerInput.playerId,null,comboIndex));
                    }
                    else
                    {
                        if(!IsDirectionKeyDown(playerInput))//若不是方向键按下，在此发送命令。方向键交给getkey处理
                             EventCenter.Instance.SendEvent(SGEventType.Command, new EventData(playerInput.playerId, null, playerInput.inputKey));
                    }
                }
            }
            if (GetKeyCode(out playerInput))// 检测玩家是否按下、持续按下了方向键，方向键都在这里处理

            {
                //发送该input指令，表示方向键被按下或持续按下
                EventCenter.Instance.SendEvent(SGEventType.Command, new EventData(playerInput.playerId, null, playerInput.inputKey));
            }
            else
            {
                //发送方向键空指令。表示本帧中方向键未被按下或持续按下
                EventCenter.Instance.SendEvent(SGEventType.CommandNoneDir, new EventData(playerId, null));
            }
        }

        //-------------------------------------


        //esc键被按下，且是主机的监听器，唤出游戏暂停面板
        if (Input.GetKeyDown(KeyCode.Escape)&&playerId==0&&escButtonTrigger==true)
        {
            //唤出UI
            EventCenter.Instance.SendEvent(SGEventType.EscKeyDown, null);
            //声音
            EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
               new SoundParam(UIManager.Instance.Source, "Click03"), null));
        }
        


    }

    /// <summary>
    /// 清除输入缓存
    /// </summary>
    /// <param name="playerId"></param>
    private void ClearInputCache()
    {
        playerInputCache.Clear();
    }
   
    /// <summary>
    /// 添加玩家输入。若招式最后一招，则返回true，否则都返回false
    /// </summary>
    /// <param name="input"></param>
    private bool MakeCombo(PlayerInput input,out int comboIndex)
    {
        //获取角色的连击索引
        Dictionary<int, Combo> characterCombo= InputController.Instance.GetComboDic(input.playerId);


        //先检测间隔时间是否合格
        if (!CheckComboTime(input))
        {
            ClearInputCache();//若不在规定时间内，之前的招式抛弃。
        }

        //检测连击序列，检测完成后，checkCache有了本次检测结果
        CheckCombo(input,characterCombo);

        if (checkCache.Count == 0)
        {
            //不存在匹配的结果，分两种情况
            if (playerInputCache.Count == 0)
            {
                //这是招式起手，本次输入无效，不作为连招，结束
                ResetInputDeltaTime();
                comboIndex = -1;
                return false;
            }
            else
            {
                //本招之前已经有招式了，只是无法形成连招。之前的输入可以舍弃了。
               //但是本招式可能可作为起手式。检测一次
                ClearInputCache();//清空输入缓存
                CheckCombo(input,characterCombo);
                if (checkCache.Count == 0)
                {
                    //该招式也无法做起手式，结束
                    ResetInputDeltaTime();
                    comboIndex = -1;
                    return false;
                }
                else
                {
                    //该招式可以作为起手式，加入缓存
                    playerInputCache.Add(input.inputKey);
                }
            }
        }
        else if(checkCache.Count==1)
        {
            //checkCache.Count为1的情况需要判断是否已经完成了连招
            if(CheckComboFinished(input, characterCombo))
            {
                //连击完成了
                //输入缓存清空
                ClearInputCache();
                ResetInputDeltaTime();
                comboIndex = checkCache[0];//返回连击索引 checkCache[0]
                return true;
            }
            else
            {
                //其余情况，连招还没完成，加入本次输入
                playerInputCache.Add(input.inputKey);
            }
        }
        else
        {
            //其余情况，加入本次输入
            playerInputCache.Add(input.inputKey);
        }
        //重置输入的间隔时间，不管本招式是否有效
        ResetInputDeltaTime();
        comboIndex = -1;
        return false;
    }
   /// <summary>
   /// 检测玩家输入的具体键值
   /// </summary>
   /// <param name="playerInput"></param>
   /// <returns></returns>
    private bool GetKeyDownCode(out PlayerInput playerInput)
    {

        foreach (GameKey gk in Enum.GetValues(typeof(GameKey)))
        {
            KeyCode code = InputController.Instance.GetGameKey(playerId, gk);
            if (Input.GetKeyDown(code))
            {
                playerInput.playerId = playerId;
                playerInput.inputKey = gk;
                return true;
            }
        }

        playerInput.playerId = default;
        playerInput.inputKey = default;
        return false;
    }
    /// <summary>
    /// 检测玩家持续输入的具体按键
    /// </summary>
    /// <param name="playerInput"></param>
    /// <returns></returns>
    private bool GetKeyCode(out PlayerInput playerInput)
    {

        //up
        KeyCode code = InputController.Instance.GetGameKey(playerId, GameKey.Up);
        if (Input.GetKey(code))
        {
            playerInput.inputKey = GameKey.Up;
            playerInput.playerId = playerId;
            return true;
        }
        //down
        code = InputController.Instance.GetGameKey(playerId, GameKey.Down);
        if (Input.GetKey(code))
        {
            playerInput.inputKey = GameKey.Down;
            playerInput.playerId = playerId;
            return true;
        }
        //left
        code = InputController.Instance.GetGameKey(playerId, GameKey.Left);
        if (Input.GetKey(code))
        {
            playerInput.inputKey = GameKey.Left;
            playerInput.playerId = playerId;
            return true;
        }
        //right
        code = InputController.Instance.GetGameKey(playerId, GameKey.Right);
        if (Input.GetKey(code))
        {
            playerInput.inputKey = GameKey.Right;
            playerInput.playerId = playerId;
            return true;
        }

        playerInput.playerId = default;
        playerInput.inputKey = default;
        return false;
    }
   

    /// <summary>
    /// 增加输入的间隔时间
    /// </summary>
    private void InputDeltaTimeSumUp()
    {
        inputDeltaTime += Time.fixedDeltaTime;
    }
    /// <summary>
    /// 重置玩家的输入间隔时间
    /// </summary>
    /// <param name="playerId"></param>
    private void ResetInputDeltaTime()
    {
        inputDeltaTime = 0.0f;
    }
    /// <summary>
    /// 依据玩家id返回输入间隔时间
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    private float GetInputDeltaTime()
    {
        return inputDeltaTime;
    }
    /// <summary>
    /// 检查此次输入是否在连击时间内
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private bool CheckComboTime(PlayerInput input)
    {

        if (playerInputCache.Count == 0) //如果输入缓存中没有元素，则符合标准
            return true;
        else
        {
            if (GetInputDeltaTime() <= comboTime)
                return true; //符合连击时间
        }

        return false;
    }
    /// <summary>
    /// 检查此次输入是否在连招表中
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private void CheckCombo(PlayerInput input, Dictionary<int,Combo> characterCombo)
    {
        //缓存清空
        checkCache.Clear();

        //输入缓存和此次输入的code组成的序列与连击表中的比对
        playerInputCache.Add(input.inputKey);//先加入用于检测

        foreach (KeyValuePair<int,Combo> kv in characterCombo)
        {
            string propertyName = null;
            bool match = true;
            for(int i = 0; i < playerInputCache.Count; i++)
            {
                propertyName = "key" + (i + 1).ToString();
                Type type = kv.Value.GetType();
                System.Reflection.FieldInfo p = type.GetField(propertyName);
                string target= p.GetValue(kv.Value) as string;
                if (!target.Equals(playerInputCache[i].ToString()))//连击表中指定位置与当前输入序列不匹配
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                checkCache.Add(kv.Key);//如果是匹配的，该连击项目加入缓存
            }
        }

        playerInputCache.RemoveAt(playerInputCache.Count-1);//检测完成，将该输入移除
        
    }

    /// <summary>
    /// 检测是否完成了连击
    /// </summary>
    /// <param name="input"></param>
    /// <param name="checkCache"></param>
    /// <returns></returns>
    private bool CheckComboFinished(PlayerInput input, Dictionary<int, Combo> characterCombo)
    {
        int comboId = checkCache[0];//仅一个
        if (characterCombo[comboId].n == playerInputCache.Count + 1)
            return true;
        else
            return false;
    }
    /// <summary>
    /// 判断一个用户输入是否为方向键
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private bool IsDirectionKeyDown(PlayerInput input)
    {
        GameKey key = input.inputKey;
        return (GameKey.Up == key || GameKey.Down == key || GameKey.Left == key || GameKey.Right == key);
    }

    public void RemoveListener()
    {
        //摧毁本脚本
        Destroy(this);
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }
}
