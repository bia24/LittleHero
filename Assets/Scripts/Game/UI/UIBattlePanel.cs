using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIBattlePanel : UIBase
{
    /// <summary>
    /// 对话框隐藏触发器
    /// </summary>
    private bool hideDialogueTrigger = false;

    public override void Init()
    {
        base.Init();

        AddUIEvent("Dialogue", EventTriggerType.PointerClick, OnDiaLogueClick);

    }

    public override void Show()
    {
        base.Show();
        //隐藏
        GetWidget("BattlePlayerHead").SetActive(false);
        GetWidget("BattlePlayerHead_Player2").SetActive(false);
        GetWidget("Time").SetActive(false);
        GetWidget("Forward").SetActive(false);
        GetWidget("Back").SetActive(false);
        GetWidget("BattleEnemyHead").SetActive(false);
        GetWidget("BattleEnemyHead_Player2").SetActive(false);
        GetWidget("Dialogue").SetActive(false);

    }

    protected override void AddListenerToEventCenter()
    {
        EventCenter.Instance.RegistListener(SGEventType.PlayerInitFinished,PlayerHeadPanelShow);
        EventCenter.Instance.RegistListener(SGEventType.UIDialogue, ShowDiaLoguePanel);
        EventCenter.Instance.RegistListener(SGEventType.DialogueRankReset, HideDialoguePanel);
    }

    protected override void CancelListenerFromEventCenter()
    {
        EventCenter.Instance.RemoveListener(SGEventType.PlayerInitFinished, PlayerHeadPanelShow);
        EventCenter.Instance.RemoveListener(SGEventType.UIDialogue, ShowDiaLoguePanel);
        EventCenter.Instance.RemoveListener(SGEventType.DialogueRankReset, HideDialoguePanel);
    }
    /// <summary>
    /// 玩家的头像面板显示
    /// </summary>
    /// <param name="data"></param>
    private void PlayerHeadPanelShow(EventData data)
    {
        Player p = data.Param as Player;
        if (p.GetPlayerId() == 0)
        {
            GetWidget("BattlePlayerHead").SetActive(true);
        }
        else if(p.GetPlayerId()==1)
        {
            GetWidget("BattlePlayerHead_Player2").SetActive(true);
        }
        else
        {
            Debug.LogError("invaild playerID to show it's headPanel");
        }
        //获取头像
        SetPlayerIconPanel(p);
        //获取血量
        SetPlayerHpBar(p);
        //获取蓝量
        SetPlayerMpBar(p);
        //获取等级
        SetPlayerLevel(p);
        //获取能量值
        SetPlayerPowerNumber(p);
    }


    /// <summary>
    /// 设置头像
    /// </summary>
    /// <param name="data"></param>
    private void SetPlayerIconPanel(Player p)
    {
        int playerId = p.GetPlayerId();
        Sprite icon = p.GetHeadIcon();
        if (playerId == 0)
        {
            //玩家1
            GetWidget<Image>("HeadIcon").sprite = icon;
        }
        else
        {
            //玩家2
            GetWidget<Image>("HeadIcon_Player2").sprite = icon;
        }
    }
    /// <summary>
    /// 设置血量
    /// </summary>
    /// <param name="data"></param>
    private void SetPlayerHpBar(Player p)
    {
        //参数1是id，参数2是血量
        int playerId = p.GetPlayerId();
        float value = p.GetHp();
        if (playerId == 0)
        {
            //玩家1
            GetWidget<Slider>("HPBar").value = Mathf.Clamp(value, 0.0f, 1f);
        }
        else
        {
            //玩家2
            GetWidget<Slider>("HPBar_Player2").value = Mathf.Clamp(value, 0.0f, 1f);
        }
    }
    /// <summary>
    /// 设置蓝量
    /// </summary>
    /// <param name="data"></param>
    private void SetPlayerMpBar(Player p)
    {
        //参数1是id，参数2是血量
        int playerId = p.GetPlayerId();
        float value = p.GetMp();
        if (playerId == 0)
        {
            //玩家1
            GetWidget<Slider>("MPBar").value = Mathf.Clamp(value, 0.0f, 1f);
        }
        else
        {
            //玩家2
            GetWidget<Slider>("MPBar_Player2").value = Mathf.Clamp(value, 0.0f, 1f);
        }
    }
    /// <summary>
    /// 设置玩家等级
    /// </summary>
    /// <param name="data"></param>
    private void SetPlayerLevel(Player p)
    {
        //参数1是id，参数2是血量
        int playerId = p.GetPlayerId();
        int level = p.GetLevel();
        if (playerId == 0)
        {
            //玩家1
            GetWidget<Text>("Lv").text = "Lv." + level;
        }
        else
        {
            //玩家2
            GetWidget<Text>("Lv_Player2").text = "Lv." + level;
        }
    }
    /// <summary>
    /// 设置玩家能量值
    /// </summary>
    /// <param name="data"></param>
    private void SetPlayerPowerNumber(Player p)
    {
        //参数1是id，参数2是血量
        int playerId = p.GetPlayerId();
        int power = p.GetPowerNumber();
        if (playerId == 0)
        {
            //玩家1
            switch (power)
            {
                case 1:
                    GetWidget("Power1").SetActive(true);
                    GetWidget("Power2").SetActive(false);
                    GetWidget("Power3").SetActive(false);
                    break;
                case 2:
                    GetWidget("Power1").SetActive(true);
                    GetWidget("Power2").SetActive(true);
                    GetWidget("Power3").SetActive(false);
                    break;
                case 3:
                    GetWidget("Power1").SetActive(true);
                    GetWidget("Power2").SetActive(true);
                    GetWidget("Power3").SetActive(true);
                    break;
            }
        }
        else
        {
            //玩家2
            switch (power)
            {
                case 1:
                    GetWidget("Power1_Player2").SetActive(true);
                    GetWidget("Power2_Player2").SetActive(false);
                    GetWidget("Power3_Player2").SetActive(false);
                    break;
                case 2:
                    GetWidget("Power1_Player2").SetActive(true);
                    GetWidget("Power2_Player2").SetActive(true);
                    GetWidget("Power3_Player2").SetActive(false);
                    break;
                case 3:
                    GetWidget("Power1_Player2").SetActive(true);
                    GetWidget("Power2_Player2").SetActive(true);
                    GetWidget("Power3_Player2").SetActive(true);
                    break;
            }
        }
    }
    /// <summary>
    /// 对话面板被唤出
    /// </summary>
    /// <param name="data"></param>
    private void ShowDiaLoguePanel(EventData data)
    {
        //初始化
        hideDialogueTrigger = false;
        //显示
        GetWidget("Dialogue").SetActive(true);
        GetWidget("Hand").SetActive(true);
        GetWidget("HandEffect").SetActive(true);
        //隐藏
        GetWidget("PlayerDialogue").SetActive(false);
        GetWidget("EnemyDialogue").SetActive(false);
        //动画
        GetWidget("HandEffect").transform.DOScale(1f, 0f);
        GetWidget("HandEffect").transform.DOScale(1.2f, 0.4f).SetLoops(-1, LoopType.Yoyo).SetId("HandEffect");
        GetWidget("Hand").GetComponent<RectTransform>().DOAnchorPos(new Vector2(-46.30078f, 70.7f),0f);
        GetWidget("Hand").GetComponent<RectTransform>().DOAnchorPos(new Vector2(-46.30078f, 100f), 0.4f).SetLoops(-1, LoopType.Yoyo).SetId("Hand");
        //模拟一次点击
        DialogueClickLogic();
    }

    //对话框被点击
    private void OnDiaLogueClick(BaseEventData data)
    {
        //声音
        EventCenter.Instance.SendEvent(SGEventType.SoundPlay, new EventData(
                new SoundParam(UIManager.Instance.Source, "Click03"), null));
        //除了声音外的逻辑
        DialogueClickLogic();
    }
    /// <summary>
    /// 对话框被点击的逻辑
    /// </summary>
    private void DialogueClickLogic()
    {
        if (hideDialogueTrigger == true)
        {
            //隐藏面板
            GetWidget("Dialogue").SetActive(false);
            //关闭动画
            DOTween.Kill("Hand");
            DOTween.Kill("HandEffect");
            //发送 dialoguefinish 事件
            return;
        }
        Dialogue d = GameController.Instance.GetCurrentDialogue();
        string iconName = null;
        if (d.type.Equals("enemy"))
        {
            iconName = d.iconName;
            Texture icon = TextureManager.Instance.GetTexture<Texture>(iconName);
            //显示
            GetWidget("EnemyDialogue").SetActive(true);
            GetWidget("PlayerDialogue").SetActive(false);
            //设置
            GetWidget<Text>("EnemyDialogueText").text = d.context;
            GetWidget<RawImage>("EnemyDialogue").texture = icon;
        }
        else
        {
            int characterId = GameController.Instance.GetCharacterId(0); //获得玩家1的角色id
            iconName = BattleController.Instance.GetBattleCharacter(characterId).headIconName;//获得玩家1的角色头像名称
            Texture icon = TextureManager.Instance.GetTexture<Texture>(iconName);
            //显示
            GetWidget("EnemyDialogue").SetActive(false);
            GetWidget("PlayerDialogue").SetActive(true);
            //设置
            GetWidget<Text>("PlayerDialogueText").text = d.context;
            GetWidget<RawImage>("PlayerDialogue").texture = icon;
        }

        //改变对话次序
        EventCenter.Instance.SendEvent(SGEventType.NextDialogue, null);
    }

    /// <summary>
    /// 隐藏对话面板
    /// </summary>
    /// <param name="data"></param>
    private void HideDialoguePanel(EventData data)
    {
        hideDialogueTrigger = true;
    }

}
