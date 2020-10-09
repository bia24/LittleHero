using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;

[DisallowMultipleComponent]
public class InputFieldListener : MonoBehaviour
{
    private bool InputFieldTrigger { get; set; }

    private void Awake()
    {
        InputFieldTrigger = false;

        //注册输入框 键盘输入按钮 监听
        EventCenter.Instance.RegistListener(SGEventType.InputFieldOn, InputFieldOnListener);
    }

    private void InputFieldOnListener(EventData data)
    {
        InputFieldTrigger = true;
    }

    private void Update()
    {
        //设置按钮时检测
        if (InputFieldTrigger && Input.anyKeyDown)
        {
            KeyCode res = GetCurrentKeyCode();//得到当前按键值
            if (res != KeyCode.None)
            {
                InputFieldTrigger = false; //关闭按钮检测
                EventCenter.Instance.SendEvent(SGEventType.InputFieldShow,new EventData(res,InputManager.Instance.LastInputObjectClicked));
            }
        }

    }

    private KeyCode GetCurrentKeyCode()
    {
        foreach(KeyCode code in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(code) && ((int)code < 323 || (int)code > 329)) //排除鼠标按钮
                return code;        
        }
        return KeyCode.None;
    }

    /// <summary>
    /// 移除监听
    /// </summary>
    public void RemoveListener()
    {
        EventCenter.Instance.RemoveListener(SGEventType.InputFieldOn, InputFieldOnListener);
        Destroy(this);
    }
}
