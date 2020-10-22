using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// 连击数控制器
/// </summary>
public class UIComboHit : MonoBehaviour
{
    CanvasGroup group = null;
    Text target;
    int comboCount = 0;
    float exitTime = 1.5f;
    float timeCount= 0.0f;
    bool trigger;
    bool isVisble;
    Tween t;

    public void Initialize()
    {
        
        //引用获取
        group = GetComponent<CanvasGroup>();
        target = GetComponent<Text>();
        //动画设置
        t = group.DOFade(0.0f, 1f).OnComplete(
            () => { Init(); }).
            SetAutoKill(false); //消退完成后的回调，应该初始化所有参数
        t.Pause();
        //ui初始化
        gameObject.SetActive(true);//物体开启，便于及时，但是alpha=0;
        Init();
    }

    public void AddOneHit()
    {
        //有击打，即开启判定开关，并重置计时
        trigger = true;
        timeCount = 0.0f;

        if (comboCount < 3)
        {
            comboCount++;
        }
        else
        {
            comboCount++;
            ShowCombo(); //展示字样
            DoExplode();//显示，并爆炸
            t.Pause();//若此时正在消退，停止该消退动画
        }
      
    }

    private void Init()
    {
        comboCount = 0;
        timeCount = 0;
        trigger = false;
        group.alpha = 0.0f;
        isVisble = false;
    }

    private void DoExplode()
    {
        group.alpha = 1;
        transform.localScale = Vector3.one;
        transform.DOScale(1.3F, 0.2F);
        isVisble = true;
    }

    private void ShowCombo()
    {
        target.text = comboCount.ToString();
    }

    private void Update()
    {
        if (trigger)
        {
            timeCount += Time.deltaTime;
            if (timeCount > exitTime)
            {
                if (isVisble)//若此时可见的。说明连击数大于指定值了。图像已经显示
                {
                    t.Restart();//消退动画重启
                    trigger = false;//消退过程中，不进行判定了
                }
                else //若不可见。说明连击数还未显示，没有达到一定值
                {
                    Init(); //数据初始化重新开始
                }
            }
        }
    }


}
