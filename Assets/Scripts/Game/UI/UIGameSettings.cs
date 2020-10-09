using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

/// <summary>
/// 游戏设置 面板的 ui
/// </summary>
public class UIGameSettings : UIBase
{
    /// <summary>
    /// 存储所有输入框文本引用(可见的)
    /// </summary>
    private List<Text> inputFields = new List<Text>();
    /// <summary>
    /// 输入框字体 字数限制
    /// </summary>
    private readonly int MAX_FONT_NUMBER = 5;
    /// <summary>
    /// 大字体
    /// </summary>
    private readonly int BIG_FONT_SIZE = 38;
    /// <summary>
    /// 小字体
    /// </summary>
    private readonly int SMALL_FONT_SIZE = 20;
    /// <summary>
    /// 重载初始化方法
    /// </summary>
    public override void Init()
    {
        base.Init();

        //组件响应事件绑定
        AddUIEvent("Cancel", EventTriggerType.PointerClick, OnCancelClick);
        AddUIEvent("Save", EventTriggerType.PointerClick, OnSaveClick);

        GetWidget<Toggle>("SoundToggleOn").onValueChanged.AddListener(OnSoundToggleChange);
        GetWidget<Toggle>("SoundEffectToggleOn").onValueChanged.AddListener(OnSoundEffectToggleChange);
        GetWidget<Slider>("Slider").onValueChanged.AddListener(OnSoundVolumeChange);
        GetWidget<Slider>("EffectSlider").onValueChanged.AddListener(OnSoundEffectVolumeChange);

        AddUIEvent("Bg_Up", EventTriggerType.PointerClick, OnInputFieldClick);
        AddUIEvent("Bg_Down", EventTriggerType.PointerClick, OnInputFieldClick);
        AddUIEvent("Bg_Left", EventTriggerType.PointerClick, OnInputFieldClick);
        AddUIEvent("Bg_Right", EventTriggerType.PointerClick, OnInputFieldClick);
        AddUIEvent("Bg_Attack", EventTriggerType.PointerClick, OnInputFieldClick);
        AddUIEvent("Bg_Jump", EventTriggerType.PointerClick, OnInputFieldClick);

        AddUIEvent("Bg_Up_Player2", EventTriggerType.PointerClick, OnInputFieldClick);
        AddUIEvent("Bg_Down_Player2", EventTriggerType.PointerClick, OnInputFieldClick);
        AddUIEvent("Bg_Left_Player2", EventTriggerType.PointerClick, OnInputFieldClick);
        AddUIEvent("Bg_Right_Player2", EventTriggerType.PointerClick, OnInputFieldClick);
        AddUIEvent("Bg_Attack_Player2", EventTriggerType.PointerClick, OnInputFieldClick);
        AddUIEvent("Bg_Jump_Player2", EventTriggerType.PointerClick, OnInputFieldClick);
    }

    protected override void SetShowType()
    {
       
    }
    /// <summary>
    /// 动态加载组件初始化
    /// </summary>
    protected override void DynamicInit()
    {
        //动态创建Player2控制面板
        GameObject go = UIManager.Instance.InstantiateUIPrefab("ControlPanel");

        //递归修改名字，并将该物体及子物体都加入到集合中
        SetChildrenName(go.transform, "_Player2");

        //位置修改
        go.transform.SetParent(transform, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMax = new Vector2(1, 0.5f); ;
        rt.anchorMin = new Vector2(1, 0.5f);
        rt.anchoredPosition = new Vector2(-1 * rt.anchoredPosition.x, rt.anchoredPosition.y);
        //标签lable修改，widgets中已经包含该物体及子物体所有引用
        GetWidget<Text>("PlayerLabel_Player2").text = "Player2";
    }

    /// <summary>
    /// 重载显示
    /// </summary>
    public override void Show()
    {
        base.Show();

        //组件显示初始化**
        //输入框初始化
        InputFieldVisibleInit();
        //声源设置初始化
        SoundSettingsInit();
        //键盘配置初始化
        KeyboardConfigInit();

        //通知发送**
        //通知添加 输入框输入 监听
        EventCenter.Instance.SendEvent(SGEventType.InputFieldListenerAdd, null);
    }

    protected override void AddListenerToEventCenter()
    {
        EventCenter.Instance.RegistListener(SGEventType.InputFieldShow, InputFieldShowListener);
    }

    protected override void CancelListenerFromEventCenter()
    {
        EventCenter.Instance.RemoveListener(SGEventType.InputFieldShow, InputFieldShowListener);
    }

    /// <summary>
    /// 音乐 音量大小改变 回调 
    /// </summary>
    /// <param name="v"></param>
    private void OnSoundVolumeChange(float v)
    {
        EventCenter.Instance.SendEvent(SGEventType.SoundVolumeChange, new EventData(v, null));
    }
    /// <summary>
    /// 音效 值大小改变 回调
    /// </summary>
    /// <param name="v"></param>
    private void OnSoundEffectVolumeChange(float v)
    {
        EventCenter.Instance.SendEvent(SGEventType.SoundEffectVolumeChange, new EventData(v, null));
    }

    /// <summary>
    /// 玩家数目获取
    /// </summary>
    /// <param name="data"></param>
   private void  InputFieldVisibleInit()
    {
        int playerNumber = GameController.Instance.GetPlayerNumber();
        bool visible = playerNumber > 1 ? true : false;
        //人数影响面板可见性
        GetWidget("ControlPanel_Player2").SetActive(visible);

        //从人数影响text集合的数目
        //将显示的inputfield引用保存，方便使用
        Text[] temp = transform.GetComponentsInChildren<Text>();
        inputFields.Clear();
        foreach (var t in temp)
        {
            if (t.gameObject.tag.Equals("Input")&&t.gameObject.activeInHierarchy)
                inputFields.Add(t);
        }
    }

    /// <summary>
    /// 音源配置请求 回调
    /// </summary>
    /// <param name="data"></param>
    private void SoundSettingsInit()
    {
        SoundSettingsEntity sse = SoundController.Instance.GetSoundSettings();
        GetWidget<Toggle>("SoundToggleOn").isOn = sse.soundTrigger == 1 ? true : false;
        GetWidget<Toggle>("SoundEffectToggleOn").isOn = sse.soundEffectTrigger == 1 ? true : false;
        GetWidget<Slider>("Slider").value = sse.soundVolume;
        GetWidget<Slider>("EffectSlider").value = sse.soundEffectVolume;
    }

    private void OnInputFieldClick(BaseEventData data)
    {
        PointerEventData pdata = data as PointerEventData;
        GameObject target = pdata.pointerPress;
        //颜色变化
        foreach(var t in inputFields)
        {
            GameObject g= GetInputFieldBg(t).gameObject;
            if (g != target)
                g.GetComponent<Image>().color = Color.white;
            else
                g.GetComponent<Image>().color = Color.green;
        }
        //发送点击通知给事件中心
        EventCenter.Instance.SendEvent(SGEventType.InputFieldClick,new EventData(null,target));
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
            new SoundParam(UIManager.Instance.Source, "Click03"),null));
    } 

    /// <summary>
    /// 输入框 按键显示监听
    /// </summary>
    /// <param name="data"></param>
    private void InputFieldShowListener(EventData data)
    {
        KeyCode code = (KeyCode)data.Param;
        string value = code.ToString();
        GameObject tar = data.Sender;
        tar.GetComponent<Image>().color = Color.white;

        foreach(var t in inputFields)
        {
            if (t.text.Equals(value))
                t.text = "";
        }
        tar.transform.parent.GetComponentsInChildren<Text>()[1].text = value;
        EventCenter.Instance.SendEvent(SGEventType.SetLastFieldRefNull, null);
        CheckInputFieldFont();
    }


    /// <summary>
    /// 键盘配置 显示 初始化
    /// </summary>
    /// <param name="data"></param>
    private void KeyboardConfigInit()
    {
        GameObject player2 = GetWidget("ControlPanel_Player2");
        //玩家1设置
        KeyboardEntity ke1 = InputController.Instance.GetPlayerKeyboardSetting(0);
        GetWidget<Text>("Text_Up").text = ke1.up;
        GetWidget<Text>("Text_Down").text = ke1.down;
        GetWidget<Text>("Text_Left").text = ke1.left;
        GetWidget<Text>("Text_Right").text = ke1.right;
        GetWidget<Text>("Text_Attack").text = ke1.attack;
        GetWidget<Text>("Text_Jump").text = ke1.jump;

        //玩家2设置
        if (player2.activeSelf)
        {
            KeyboardEntity ke2 = InputController.Instance.GetPlayerKeyboardSetting(1);
            
            GetWidget<Text>("Text_Up_Player2").text = ke2.up;
            GetWidget<Text>("Text_Down_Player2").text = ke2.down;
            GetWidget<Text>("Text_Left_Player2").text = ke2.left;
            GetWidget<Text>("Text_Right_Player2").text = ke2.right;
            GetWidget<Text>("Text_Attack_Player2").text = ke2.attack;
            GetWidget<Text>("Text_Jump_Player2").text = ke2.jump;
        }

        //检查字体
        CheckInputFieldFont();

        //初始化颜色：白色
        foreach(var t in inputFields)
        {
            GetInputFieldBg(t).color = Color.white;
        }

        //检测是否重复
        for(int i = 0; i < inputFields.Count; i++)
        {
            Text t = inputFields[i];
            if (t.text.Equals(""))
                continue;
            for(int j = i + 1; j < inputFields.Count; j++)
            {
                Text m = inputFields[j];
                if (t.text.Equals(m.text))
                {
                    m.text = "";
                    break;
                }
            }
        }
        

    }

    /// <summary>
    /// 字体大小检查
    /// </summary>
    private void CheckInputFieldFont()
    {
        foreach(var t in inputFields)
        {
            if (t.text.Length >= MAX_FONT_NUMBER)
                t.fontSize = SMALL_FONT_SIZE;
            else
                t.fontSize = BIG_FONT_SIZE;
        }
    }
    

    /// <summary>
    /// “取消” 按钮响应事件
    /// </summary>
    /// <param name="data"></param>
    private void OnCancelClick(BaseEventData data)
    {
        //点击声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
                new SoundParam(UIManager.Instance.Source, "Click02"), null));
        //移除InputFieldListener监听
        EventCenter.Instance.SendEvent(SGEventType.InputFieldListenerRemove, null);
        //隐藏
        Hide(null);
        //显示主菜单
        EventCenter.Instance.SendEvent(SGEventType.UIGameStartPanel, new EventData(false,null));
    }

    /// <summary>
    /// “保存”按钮响应事件
    /// </summary>
    /// <param name="data"></param>
    private void OnSaveClick(BaseEventData data)
    {
        //检测是否input keyboard有无效设置
        bool isValid = false;
        foreach(var t in inputFields)
        {
            if (t.text.Equals(""))
            {
                GetInputFieldBg(t).color = Color.red;
                isValid = true;
            }
        }
        //弹出警告框
        if (isValid)
        {
            EventCenter.Instance.SendEvent(SGEventType.UIWarnningPanel, new EventData("Invaild key setting!!!",null));
            //声音
            EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
                new SoundParam(UIManager.Instance.Source, "Click03"), null));
            //声音
            EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
                new SoundParam(UIManager.Instance.Source, "Appear"), null));
        }
        else
        {
            //发送存储请求
            EventCenter.Instance.SendEvent(SGEventType.InputConfigSave, new EventData(inputFields, null));
            //声音
            EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
                new SoundParam(UIManager.Instance.Source, "Click01"), null));
            //移除InputFieldListener监听
            EventCenter.Instance.SendEvent(SGEventType.InputFieldListenerRemove, null);
            //回到主菜单
            Hide(null);
            EventCenter.Instance.SendEvent(SGEventType.UIGameStartPanel, new EventData(false,null));
        }
    }

    /// <summary>
    /// Sound  开关发生变化
    /// </summary>
    /// <param name="trigger"></param>
    private void OnSoundToggleChange(bool trigger)
    {
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
           new SoundParam(UIController.Instance.GetAudioSource(), "Click02"), null));
        EventCenter.Instance.SendEvent(SGEventType.SoundTrigger,new EventData(trigger,null));
        
    }

    /// <summary>
    /// Sound Effect 开关发送变化
    /// </summary>
    /// <param name="trigger"></param>
    private void OnSoundEffectToggleChange(bool trigger)
    {
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
          new SoundParam(UIController.Instance.GetAudioSource(), "Click02"), null));
        EventCenter.Instance.SendEvent(SGEventType.SoundEffectTrigger, new EventData(trigger, null));
    }
  
    /// <summary>
    /// 获得输入框的背景图片
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private Image GetInputFieldBg(Text t)
    {
        return t.transform.parent.GetComponentInChildren<Image>();
    }

}
