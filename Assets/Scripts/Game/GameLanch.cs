using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

/// <summary>
/// 游戏启动脚本，唯一需要提前挂载在GO上的脚本
/// </summary>
public class GameLanch : MonoBehaviour
{
    private static GameLanch _instance =null;
    
    public static GameLanch Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance ==null) _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitGame();
        
    }

    /// <summary>
    /// 模块功能初始化
    /// </summary>
    private void InitGame()
    {
        Debug.Log("-----游戏初始化-----开始");

        

        DataManager.Instance.Init();// 主配置文件 在本SG框架中只从Resources中加载(同步)，快速开发
                                    //主配置文件加载完成，其它模块配置文件才有路径可以加载
                                 
        //各个模块业务功能的初始化
        //ui模块
        UIController.Instance.Init();
        //音乐模块
        SoundController.Instance.Init();
        //游戏管理模块
        GameController.Instance.Init();
        //输入模块
        InputController.Instance.Init();


        //绑定初始化加载完成回调
        EventCenter.Instance.RegistListener(SGEventType.LoadingFinish,LanchGame);

        //显示加载界面
        EventCenter.Instance.SendEvent(SGEventType.UILoadingPanel, null);
    }
  
    /// <summary>
    /// 游戏启动
    /// </summary>
    private void LanchGame(EventData data)
    {
        //取消绑定初始化加载完成的回调，避免后续继续使用UILoading界面发生响应。
        EventCenter.Instance.RemoveListener(SGEventType.LoadingFinish, LanchGame);
        Debug.Log("-----游戏初始化-----完成");

        //ui  trigger
        EventCenter.Instance.SendEvent(SGEventType.UIGameStartPanel,null);
        //sound bgm trigger
        EventCenter.Instance.SendEvent(SGEventType.SoundBGM, null);
        Debug.Log("-----游戏启动----->>>>>>>");
    }

   
    
}
