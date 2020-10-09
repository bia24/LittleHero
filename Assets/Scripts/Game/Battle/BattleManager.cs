using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;

/// <summary>
/// 相机摆放位置
/// </summary>
public enum CameraPos
{
    Mid,
    Left,
    Right
}
/// <summary>
/// 产生物体的位置
/// </summary>
public enum Produce
{
    LeftBottom,
    LeftUp,
    LeftMid,
    RightBottom,
    RightUp,
    RightMid
}

public class BattleManager : Singleton<BattleManager>
{
    #region 配置文件中读入，运行时不做改变
    /// <summary>
    /// 玩家升级所需经验
    /// </summary>
    private int levelUpExp;
    /// <summary>
    /// 玩家最大级数
    /// </summary>
    private int levelMax;
    /// <summary>
    /// 难度参数
    /// </summary>
    private Dictionary<DifficultyType, DifficultyParam> dps = new Dictionary<DifficultyType, DifficultyParam>();
    /// <summary>
    /// 角色的战斗信息
    /// </summary>
    private Dictionary<int, BattleCharacter> bcs = new Dictionary<int, BattleCharacter>();
    /// <summary>
    /// 技能信息
    /// </summary>
    private Dictionary<int, Skill> sks = new Dictionary<int, Skill>();
    /// <summary>
    /// 关卡信息
    /// </summary>
    private Dictionary<int, Level> levels = new Dictionary<int, Level>();
    /// <summary>
    /// 玩家生成计划
    /// </summary>
    private Dictionary<DifficultyType, Dictionary<int, Dictionary<int, List<EnemyDetail>>>> enemyGenPlans = new Dictionary<DifficultyType, Dictionary<int, Dictionary<int, List<EnemyDetail>>>>();
    #endregion
    #region 运行时进行初始化的值
    /// <summary>
    /// 当前关卡
    /// </summary>
    private int currentLevel=1;
    /// <summary>
    /// 背景
    /// </summary>
    private SpriteRenderer background;
    /// <summary>
    /// 场景 相机
    /// </summary>
    private Camera camera;
    /// <summary>
    /// 所有游戏物体在游戏空间中的原点
    /// </summary>
    private Transform basePoint;
    /// <summary>
    /// 游戏物体的产生坐标集合，原点为basePoint
    /// </summary>
    private Dictionary<Produce, Transform> producePoint = new Dictionary<Produce, Transform>();
    #endregion
    #region 只读常量，不改变
    /// <summary>
    /// 产生的游戏物体预制体根目录
    /// </summary>
    private readonly string GAMEOBJECT_RESOURCES_PATH = "GameObject";
    /// <summary>
    /// 相机中点位置x轴
    /// </summary>
    private readonly float CAMERA_POS_MID = 0.0f;
    /// <summary>
    /// 相机左边坐标位置x轴
    /// </summary>
    private readonly float CAMERA_POS_LEFT = -2.81f;
    /// <summary>
    /// 相机右边坐标位置x轴
    /// </summary>
    private readonly float CAMERA_POS_RIGHT = 2.85f;
    /// <summary>
    /// 最大能量数目
    /// </summary>
    private readonly int POWER_MAX = 3;
    #endregion
    /// <summary>
    /// 初始化战斗信息，每次场景进入要重新初始化。因为场景切换。manager不摧毁，但是场景物体会摧毁，因此每次进入场景都要重新初始化
    /// </summary>
    public void BattleManagerRefInit()
    {
        //获取当前背景引用
        background = GameObject.Find("BattleBackground").GetComponent<SpriteRenderer>();
        //获取当前相机引用
        camera = Camera.main;
        //获得产生游戏物体所需要的点
        PointTransformGet();
        //重置当前关卡
        currentLevel = 1;
    }
    
    /// <summary>
    /// 角色信息加载回调
    /// </summary>
    /// <param name="context"></param>
    public void BattleCharacterInfosLoadCallBack(string context)
    {
        BattleCharacterInfos bci = JsonUtility.FromJson<BattleCharacterInfos>(context);
        if(bci==null)
            Debug.LogError("BattleCharacterInfos json transform failed");

        foreach(var t in bci.battleCharacterEntities)
        {
            bcs.Add(t.id, t);
        }
    }

    /// <summary>
    /// 技能伤害加载 回调函数
    /// </summary>
    /// <param name="context"></param>
    public void SkillInfosLoadCallBack(string context)
    {
        SkillInfos sis = JsonUtility.FromJson<SkillInfos>(context);
        if (sis == null)
            Debug.LogError("SkillInfos json transform failed");

        foreach (var t in sis.skills)
            sks.Add(t.id, t);
    }

    /// <summary>
    /// 战斗相关参数加载 回调函数
    /// </summary>
    /// <param name="context"></param>
    public void BattleParamLoadCallBack(string context)
    {
        BattleParam bp = JsonUtility.FromJson<BattleParam>(context);
        if (bp == null)
            Debug.LogError("BattleParam json transform failed");

        levelUpExp = bp.levelUpExp;
        levelMax = bp.levelMax;
        foreach (var t in bp.difficultyParams)
        {
            dps.Add((DifficultyType)Enum.Parse(typeof(DifficultyType), t.type), t);
        }
    }

    /// <summary>
    /// 关卡参数加载 回调函数
    /// </summary>
    public void LevelInfoLoadCallBack(string context)
    {
        LevelInfos lis = JsonUtility.FromJson<LevelInfos>(context);
        if (lis == null)
            Debug.LogError("LevelInfos json transform failed");

        foreach(var t in lis.levels)
        {
            this.levels.Add(t.id, t);
        }
    }
    /// <summary>
    /// 关卡生成敌人信息加载  回调函数
    /// </summary>
    /// <param name=""></param>
    public void EnemyGenPlanLoadCallBack(string context)
    {
        EnemyGenPlan egp = JsonUtility.FromJson<EnemyGenPlan>(context);
        if (egp == null)
            Debug.LogError("EnemyGenPlan json  trasform failed");

        foreach(EachDifficult ed in egp.eachDifficults)
        {
            Dictionary<int, Dictionary<int, List<EnemyDetail>>> t_0 = new Dictionary<int, Dictionary<int, List<EnemyDetail>>>();

            foreach (EachLevel el in ed.eachLevels)
            {
                Dictionary<int, List<EnemyDetail>> t_1 = new Dictionary<int, List<EnemyDetail>>();
                foreach(EachTime et in el.eachTimes)
                {
                    t_1.Add(et.number, et.enemyDetails);
                }
                t_0.Add(el.level, t_1);
            }
            enemyGenPlans.Add((DifficultyType)Enum.Parse(typeof(DifficultyType),ed.difficulty), t_0);
        }
    }


    /// <summary>
    /// 设置相机位置
    /// </summary>
    /// <param name="pos"></param>
    public void SetCameraPos(CameraPos pos)
    {
        switch (pos)
        {
            case CameraPos.Mid:
                camera.transform.position = new Vector3(CAMERA_POS_MID, 0, 0);
                break;
            case CameraPos.Left:
                camera.transform.position = new Vector3(CAMERA_POS_LEFT, 0, 0);
                break;
            case CameraPos.Right:
                camera.transform.position = new Vector3(CAMERA_POS_RIGHT, 0, 0);
                break;
        }
    }


    /// <summary>
    /// 依据 角色id 获得角色的战斗信息
    /// </summary>
    /// <param name="characterId"></param>
    /// <returns></returns>
    public BattleCharacter GetBattleCharacter(int characterId)
    {
        return bcs[characterId];
    }
 
    /// <summary>
    /// 获得关键点
    /// </summary>
    private void PointTransformGet()
    {
        //原点
        basePoint = GameObject.Find("BasePoint").transform;
        producePoint.Clear();
        Transform p = GameObject.Find("Producer").transform;
        producePoint.Add(Produce.LeftBottom,p.Find("LeftBottom"));
        producePoint.Add(Produce.LeftMid, p.Find("LeftMid"));
        producePoint.Add(Produce.LeftUp, p.Find("LeftUp"));
        producePoint.Add(Produce.RightBottom, p.Find("RightBottom"));
        producePoint.Add(Produce.RightMid, p.Find("RightMid"));
        producePoint.Add(Produce.RightUp, p.Find("RightUp"));
    }
   

    /// <summary>
    /// 获得当前关卡
    /// </summary>
    /// <returns></returns>
    public int GetCurrentLevel()
    {
        return this.currentLevel;
    }

    /// <summary>
    /// 改变当前关卡,1开始
    /// </summary>
    public void SetLevel(int level)
    {
        if (level < 1 || level > 3)
        {
            Debug.LogError("invaild level");
        }
        currentLevel = level;
    }


    /// <summary>
    /// 返回指定的关卡的信息
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public Level GetLevelInfo(int level)
    {
        Level res = null;
        if(!levels.TryGetValue(level,out res))
        {
            Debug.LogError("can not  find this level infos : "+level);
        }
        return res;
    }

    /// <summary>
    /// 设置背景图片
    /// </summary>
    public void SetBackground(Sprite pic)
    {
        background.sprite = pic;
    }
    /// <summary>
    /// 依据目标角色id加上对应的player脚本
    /// </summary>
    /// <param name="target"></param>
    /// <param name="characterId"></param>
    /// <returns></returns>
    private Player AddPlayerScript(GameObject target,int characterId)
    {
        Player res = target.GetComponent<Player>();
        if (res == null)
        {
            if (characterId == 1)
                res = target.AddComponent<XiaoMa>();
            else
                res = target.AddComponent<XiaoLiang>();
        }
        return res;
    }

    private Enemy AddEnemyScript(GameObject target,int characterId)
    {
        Enemy res = target.GetComponent<Enemy>();
        if (res != null)
            return res;
        switch (characterId)
        {
            case 3:
                res = target.AddComponent<JinZhan>();
                break;
            case 4:
                res = target.AddComponent<YuanChen>();
                break;
            case 5:
                res = target.AddComponent<ChuiZi>();
                break;
            case 6:
                res = target.AddComponent<XiaoHong>();
                break;
            case 7:
                res = target.AddComponent<XiaoBu>();
                break;
            default:
                Debug.LogError("AddEnemyScript faild");
                break;
        }
        return res;
    }

    /// <summary>
    /// 产生游戏玩家
    /// </summary>
    public Player CreatPlayer(int characterId)
    {
        string objectName = bcs[characterId].objectName;
        GameObject player = PoolManager.Instance.getPrefab(GAMEOBJECT_RESOURCES_PATH + "/" + objectName,
            objectName, basePoint);
        //设置初始位置
        player.transform.localPosition = producePoint[Produce.LeftMid].localPosition;
        player.transform.localScale = new Vector3(player.transform.localScale.x * -1, player.transform.localScale.y, player.transform.localScale.z);
        //todo:加上脚本
        Player p = AddPlayerScript(player, characterId);
        return p;
    }

   /// <summary>
   /// 生成敌人生成器
   /// </summary>
    public void CreateEnemyGenerator()
    {
        GameObject go = GameObject.Find("EnemyGenerator");
        if (go == null)
            go = new GameObject("EnemyGenerator");
        if (go.GetComponent<EnemyGenerator>() == null)
            go.AddComponent<EnemyGenerator>();
    }

    /// <summary>
    /// 按照难度返回当前关卡的产兵计划
    /// </summary>
    /// <param name="difficulty"></param>
    /// <returns></returns>
    public Dictionary<int, List<EnemyDetail>> GetCurrentGenEnemyPlan(DifficultyType difficulty)
    {
        return enemyGenPlans[difficulty][currentLevel];
    }

    /// <summary>
    /// 产生敌人
    /// </summary>
    /// <param name="characterId"></param>
    /// <param name="pos"></param>
    public Enemy GenerateEnemy(int characterId, Produce pos)
    {
        BattleCharacter bc = bcs[characterId];
        //产生一个实体
        GameObject go= PoolManager.Instance.getPrefab(GAMEOBJECT_RESOURCES_PATH + "/" + bc.objectName, 
            bc.objectName, basePoint);
        //初始化实体
        go.transform.localPosition = producePoint[pos].localPosition;

        int forward = 1;//默认朝右
        if (pos == Produce.LeftBottom || pos == Produce.LeftMid || pos == Produce.LeftUp)
            forward *= -1;
        //设置朝向
        go.transform.localScale = new Vector3(go.transform.localScale.x * forward, go.transform.localScale.y, go.transform.localScale.z);

        //添加脚本
        Enemy e= AddEnemyScript(go, characterId);

        return e;
    }
    /// <summary>
    /// 依据技能id 获得技能
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Skill GetSkill(int id)
    {
        Skill res = null;
        if(!sks.TryGetValue(id,out res))
        {
            Debug.LogError("can not find this skill  : "+id);
        }
        return res;
    }
}
