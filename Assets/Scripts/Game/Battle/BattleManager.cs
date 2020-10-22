using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using System;
using DG.Tweening;

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

/// <summary>
/// 攻击者
/// </summary>
public interface IAttacker
{
    Vector3 GetAttackerLocalPostion();
    Vector3 GetAttackerLocalScale();
    bool IsParticleAttack();
    MoveParticleBase GetParticle();
    GameObject GetGameObject();
}

/// <summary>
/// 技能强化接口
/// </summary>
public interface IStrengthenSkill
{
    Skill StrengthenSkill(Skill skill);
}
/// <summary>
/// 特效粒子速度获取接口
/// </summary>
public interface IParticleMoveSpeed
{
    float GetParticleMoveSpeed();
}
/// <summary>
/// 游戏运行状态
/// </summary>
public enum GameState
{
    None,
    /// <summary>
    /// 运行中
    /// </summary>
    Running,
    /// <summary>
    /// 运行中
    /// </summary>
    Pause
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
    ///  等级上升经验值增加比例
    /// </summary>
    private float levelUpExpRate;
    /// <summary>
    /// 每个敌人击打获得的经验值
    /// </summary>
    private int perEnemyExp;
    /// <summary>
    /// 每个敌人击打时获得的蓝量
    /// </summary>
    private int perEnemyMp;
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
    /// <summary>
    /// 玩家移动的左边界
    /// </summary>
    public float MoveXMin { get; set; }
    /// <summary>
    /// 玩家移动的右边界
    /// </summary>
    public float MoveXMax { get; set; }
    /// <summary>
    /// 玩家移动的下边界
    /// </summary>
    public float MoveYMin { get; set; }
    /// <summary>
    /// 玩家移动的上边界
    /// </summary>
    public float MoveYMax { get; set; }
    /// <summary>
    /// 相机左移的坐标
    /// </summary>
    public float CameraXMin { get; set; }
    /// <summary>
    /// 相机右移的坐标
    /// </summary>
    public float CameraXMax { get; set; }
    /// <summary>
    /// 触发相机移动的坐标1
    /// </summary>
    public float TriggerOne { get; set; }
    /// <summary>
    /// 触发相机移动的坐标2
    /// </summary>
    public float TriggerTwo { get; set; }
    /// <summary>
    /// 触发相机移动的坐标3
    /// </summary>
    public float TriggerThree { get; set; }
    /// <summary>
    /// 触发相机移动的坐标4
    /// </summary>
    public float TriggerFour { get; set; }
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
    /// 获得战场相机
    /// </summary>
    /// <returns></returns>
    public Camera GetBattleCamera() { return camera; }
    /// <summary>
    /// 所有游戏物体在游戏空间中的原点
    /// </summary>
    private Transform basePoint;
    /// <summary>
    /// 游戏物体的产生坐标集合，原点为basePoint
    /// </summary>
    private Dictionary<Produce, Transform> producePoint = new Dictionary<Produce, Transform>();
    /// <summary>
    /// 挂方向提示判断脚本的物体
    /// </summary>
    private GameObject DirTip { get; set; } 
    /// <summary>
    /// 战场上的音乐
    /// </summary>
    public AudioSource audioSource { get; set; }

    #endregion
    #region 只读常量，不改变
    /// <summary>
    /// 产生的游戏物体预制体根目录
    /// </summary>
    private readonly string GAMEOBJECT_RESOURCES_PATH = "GameObject";
    public string GetGameObjectResourcesPath()
    {
        return GAMEOBJECT_RESOURCES_PATH;
    }
    /// <summary>
    /// 能量特效存储地址
    /// </summary>
    private readonly string POWER_PARTICLE_RESOURCES_FULLPATH = "Particle/QiChangParticle";
    public string GetPowerParticleResourcesFullPath()
    {
        return POWER_PARTICLE_RESOURCES_FULLPATH;
    }
    /// <summary>
    /// 死亡特效地址
    /// </summary>
    private readonly string DIE_PARTICLE_RESOURCES_FULLPATH = "Particle/DieParticle";
    public string GetDieParticleResourcesFullPath()
    {
        return DIE_PARTICLE_RESOURCES_FULLPATH;
    }
    /// <summary>
    /// 特效根目录
    /// </summary>
    private readonly string PARTICLE_RESOURCES_PATH = "Particle";
    public string GetParticleResourcesRootPath()
    {
        return PARTICLE_RESOURCES_PATH;
    }
    /// <summary>
    /// 最大能量数目
    /// </summary>
    private readonly int POWER_MAX = 3;
    /// <summary>
    /// BOSS1 的角色id
    /// </summary>
    public readonly int BOSS1_ID = 5;
    /// <summary>
    /// BOSS2的角色id
    /// </summary>
    public readonly int BOSS2_ID = 6;
    /// <summary>
    /// BOSS3 的角色id
    /// </summary>
    public readonly int BOSS3_ID = 7;

    #endregion

    #region 变量
    /// <summary>
    /// 当前相机的位置
    /// </summary>
    private CameraPos currentCameraPos;

    /// <summary>
    /// 场景中的敌人集合
    /// </summary>
    public List<Enemy> enemyInBattle=new List<Enemy>();
    /// <summary>
    /// 场景中的玩家集合
    /// </summary>
    public List<Player> playerInBattle = new List<Player>();
    /// <summary>
    /// 场景中的所有可移动的特效
    /// </summary>
    public List<IAttacker> moveParticleInBattle = new List<IAttacker>();

    /// <summary>
    /// 游戏状态
    /// </summary>
    public GameState GameState { get; set; }
   
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
        //敌人集合清空
        enemyInBattle.Clear();
        //玩家集合清空
        playerInBattle.Clear();
        //清空所有可移动的特效
        moveParticleInBattle.Clear();
        //创建方向提示脚本的挂载物体
        DirTip = new GameObject("DirTip");
        DirTip.AddComponent<BattleDirTip>();
        //设置初始状态
        GameState = GameState.None;
        //添加音源
        audioSource= new GameObject("BattleSource").AddComponent<AudioSource>();
    }
    /// <summary>
    /// 战场完成退出或中途退出时
    /// </summary>
    public void BattleExit()
    {
        //清理产生的东西
        //1.提示标志删除
        UnityEngine.Object.Destroy(DirTip);
        DirTip = null;
        //2.设置状态
        GameState = GameState.None;
        //3.音源删除
        UnityEngine.Object.Destroy(GameObject.Find("BattleSource"));
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
        levelUpExpRate = bp.expUpRate;
        perEnemyExp = bp.perEnemyExp;
        perEnemyMp = bp.perEnemyMp;
      
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
    /// 战场地面参数回调
    /// </summary>
    /// <param name="context"></param>
    public void BattleGroundParamLoadCallBack(string context)
    {
        Boundary boundary = JsonUtility.FromJson<Boundary>(context);
        MoveXMin = boundary.moveXMin;
        MoveXMax = boundary.moveXMax;
        MoveYMin = boundary.moveYMin;
        MoveYMax = boundary.moveYMax;
        CameraXMin = boundary.cameraXMin;
        CameraXMax = boundary.cameraXMax;
        TriggerOne = boundary.triggerOne;
        TriggerTwo = boundary.triggerTwo;
        TriggerThree = boundary.triggerThree;
        TriggerFour = boundary.triggerFour;
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
                currentCameraPos = CameraPos.Mid;
                camera.transform.DOMove(new Vector3(0,0,-5),0.8f);
                break;
            case CameraPos.Left:
                currentCameraPos = CameraPos.Left;
                camera.transform.DOMove(new Vector3(CameraXMin, 0, -5), 0.8f);
                break;
            case CameraPos.Right:
                currentCameraPos = CameraPos.Right;
                camera.transform.DOMove(new Vector3(CameraXMax, 0, -5), 0.8f);
                break;
        }
    }
    /// <summary>
    /// 获得当前相机的位置
    /// </summary>
    /// <returns></returns>
    public CameraPos GetCameraPos()
    {
        return currentCameraPos;
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
        //设置当前关卡
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
        //target 一定是依据角色id取出的模型。因此即使绑定过脚本也一定是对应的角色脚本。因此没必要重新绑定
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
    /// 初始化玩家的位置
    /// </summary>
    /// <param name="p"></param>
    public void ResetPlayerTransform(Player p)
    {
        //设置初始位置
        p.transform.localPosition = producePoint[Produce.LeftMid].localPosition;
        //初始化面向
        float x = p.transform.localScale.x;
        if (x > 0)
            x *= -1;
        p.transform.localScale = new Vector3(x, p.transform.localScale.y, p.transform.localScale.z);
    }


   /// <summary>
   /// 生成敌人生成器
   /// </summary>
    public void CreateEnemyGenerator()
    {
        GameObject go = GameObject.Find("EnemyGenerator");
        if (go == null)
            go = new GameObject("EnemyGenerator");
        EnemyGenerator eg = go.GetComponent<EnemyGenerator>();
        if (eg == null)
            eg = go.AddComponent<EnemyGenerator>();
        eg.Init();
    }
    /// <summary>
    /// 摧毁敌人生成器
    /// </summary>
    public void DestroyEnemyGenerator()
    {
        GameObject go = GameObject.Find("EnemyGenerator");
        if (go != null)
            UnityEngine.Object.Destroy(go);
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

    /// <summary>
    /// 获得玩家最大等级
    /// </summary>
    /// <returns></returns>
    public int GetPlayerLevelMax()
    {
        return this.levelMax;
    }

    /// <summary>
    /// 获得经验上限基础值。readonly
    /// </summary>
    /// <returns></returns>
    public int GetPlayerLevelUpExp()
    {
        return this.levelUpExp;
    }
    /// <summary>
    /// 获得经验上限随等级提升率
    /// </summary>
    /// <returns></returns>
    public float GetPlayerLevelUpExpRate()
    {
        return this.levelUpExpRate;
    }
    /// <summary>
    /// 获得击打每个敌人的经验值
    /// </summary>
    /// <returns></returns>
    public int GetPerEnemyExp()
    {
        return this.perEnemyExp;
    }
    /// <summary>
    /// 获得击打每个敌人的蓝量
    /// </summary>
    /// <returns></returns>
    public int GetPerEnemyMp()
    {
        return this.perEnemyMp;
    }
    /// <summary>
    /// 获得能量值的最大值
    /// </summary>
    /// <returns></returns>
    public int GetPowerMax()
    {
        return POWER_MAX;
    }

    
}
