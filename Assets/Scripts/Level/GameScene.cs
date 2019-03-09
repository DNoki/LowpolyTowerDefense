using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 游戏场景
/// </summary>
public abstract class GameScene : MonoBehaviour
{
    private static GameScene instance = null;

    [SerializeField] private UIManager uiManager = null;
    [SerializeField] private Ground ground = null;
    [SerializeField] private BuildingFactory buildingFactory = null;
    [SerializeField] private EnemyFactory enemyFactory = null;
    [SerializeField] private GameObjectFactory gameObjectFactory = null;
    [SerializeField] private CenterBuilding centerBuilding = null;
    [SerializeField] private TDSelection selection = null;


    /// <summary>
    /// 每回合开始时等待时间
    /// </summary>
    [Space(10f)] public int[] RoundIntervalTimeList = new int[25];
    /// <summary>
    /// 生成数据源
    /// </summary>
    public List<EnemyGenerator> GeneratorsList = null;
    /// <summary>
    /// 初始资金
    /// </summary>
    public int InitialMoney = 200;

    [SerializeField] protected UnityEventT1int onScoreChange = null;
    [SerializeField] protected UnityEventT1int onMoneyChange = null;
    [SerializeField] protected UnityEventT1intT2int onRoundChange = null;

    private int money = 200;
    private int score = 0;
    /// <summary>
    /// 当前回合是否已经生成完所有敌人
    /// </summary>
    protected bool isGenerateOver = true;

    /// <summary>
    /// 单例对象
    /// </summary>
    public static GameScene Instance => instance == null ? (instance = FindObjectOfType<GameScene>()) : instance;
    /// <summary>
    /// 游戏模式是否为普通
    /// </summary>
    public GameMode GameMode
    {
        get
        {
            if (this is GameSceneFree) return GameMode.FREE;
            else return GameMode.NORMAL;
        }
    }
    /// <summary>
    /// UI管理器
    /// </summary>
    public UIManager UIManager => this.uiManager == null ? (this.uiManager = FindObjectOfType<UIManager>()) : this.uiManager;
    /// <summary>
    /// 地面
    /// </summary>
    public Ground Ground => this.ground == null ? (this.ground = FindObjectOfType<Ground>()) : this.ground;
    /// <summary>
    /// 建筑工厂
    /// </summary>
    public BuildingFactory BuildingFactory => this.buildingFactory == null ? (this.buildingFactory = FindObjectOfType<BuildingFactory>()) : this.buildingFactory;
    /// <summary>
    /// 敌人工厂
    /// </summary>
    public EnemyFactory EnemyFactory => this.enemyFactory == null ? (this.enemyFactory = FindObjectOfType<EnemyFactory>()) : this.enemyFactory;
    /// <summary>
    /// 游戏对象工厂
    /// </summary>
    public GameObjectFactory GameObjectFactory => this.gameObjectFactory == null ? (this.gameObjectFactory = FindObjectOfType<GameObjectFactory>()) : this.gameObjectFactory;
    /// <summary>
    /// 中心建筑
    /// </summary>
    public CenterBuilding CenterBuilding => this.centerBuilding == null ? (this.centerBuilding = FindObjectOfType<CenterBuilding>()) : this.centerBuilding;
    /// <summary>
    /// 选择对象
    /// </summary>
    public TDSelection Selection => this.selection == null ? (this.selection = FindObjectOfType<TDSelection>()) : this.selection;
    public List<EnemyGenerator> CSVGeneratorsList { get; set; } = null;

    /// <summary>
    /// 拥有金钱
    /// </summary>
    public int Money
    {
        get { return this.money; }
        set
        {
            value = Mathf.Max(0, value);
            this.money = value;
            this.onMoneyChange.Invoke(this.money);
        }
    }
    /// <summary>
    /// 得分
    /// </summary>
    public int Score
    {
        get { return this.score; }
        set
        {
            this.score = value;
            this.onScoreChange.Invoke(this.score);
        }
    }
    /// <summary>
    /// 当前回合
    /// </summary>
    public int CurrentRound { get; set; } = 0;
    /// <summary>
    /// 总回合数
    /// </summary>
    public abstract int TotalRound { get; }
    ///// <summary>
    ///// 当前回合是否已结束
    ///// </summary>
    //public bool IsCurrentRoundOver => this.EnemyList.Count <= 0 && this.isGenerateOver;
    /// <summary>
    /// 游戏是否已经结束
    /// </summary>
    public bool IsGameOver { get; set; } = false;
    /// <summary>
    /// 场上怪物列表
    /// </summary>
    public List<IEnemy> EnemyList { get; set; } = new List<IEnemy>();

    /// <summary>
    /// 初始化场景
    /// </summary>
    public void Initialize()
    {
        this.BuildingFactory.Initialize();
        this.EnemyFactory.Initialize();
        this.GameObjectFactory.Initialize();
        this.UIManager.Initialize();

        this.Money = this.InitialMoney;
    }
    /// <summary>
    /// 开始生成
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator StartNextRound();

    public void AddEnemy(IEnemy enemy)
    {
        RemoveEnemy(enemy);
        this.EnemyList.Add(enemy);
    }
    /// <summary>
    /// 从列表移除怪物
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemy(IEnemy enemy)
    {
        if (this.EnemyList.Contains(enemy))
            this.EnemyList.Remove(enemy);
    }
    public void GameOver(bool isWin)
    {
        this.IsGameOver = true;
        
        if (isWin)
        {
            AudioManager.Instance.Play("Win");
            this.UIManager.OverWin.Play();
        }
        else
        {
            AudioManager.Instance.PlayRandomAudio("Lose1", "Lose2");
            AudioManager.Instance.Play("LoseBG");
            this.UIManager.OverLose.Play();
        }
    }


    protected virtual void Awake()
    {
        try
        {
            var ini = new INIParser();
            ini.Open(Application.dataPath + "/Config.txt");
            this.InitialMoney = ini.ReadValue("Debug", "InitialMoney", this.InitialMoney);
            this.CenterBuilding.HPLimit = ini.ReadValue("Debug", "CenterHPLimit", this.CenterBuilding.HPLimit);
            this.CurrentRound = ini.ReadValue("Debug", "StartRound", this.CurrentRound);
        }
        catch (System.Exception) { }

        Initialize();

        try
        {
            var info = new FileInfo(Application.dataPath + "/Generator.csv");
            var text = File.ReadAllText(info.FullName, Encoding.UTF8);
            var data = FunctionExtension.CSVReader(text);
            this.CSVGeneratorsList = new List<EnemyGenerator>();
            foreach (var item in data)
            {
                this.CSVGeneratorsList.Add(EnemyGenerator.CreateEnemyGenerator(this, item));
            }
        }
        catch (System.Exception)
        {
            this.CSVGeneratorsList = null;
        }

    }
    private void Update()
    {
        //GameOver(true);
        if (this.IsGameOver) return;
        if (this.EnemyList.Count <= 0 && this.isGenerateOver)
        {
            if (this.CurrentRound >= this.TotalRound)
                GameOver(true);
            else StartCoroutine(StartNextRound());
        }
    }
    private void OnDestroy()
    {
        if (this.BuildingFactory != null) this.BuildingFactory.ClearPool();
        if (this.EnemyFactory != null) this.EnemyFactory.ClearPool();
        if (this.GameObjectFactory != null) this.GameObjectFactory.ClearPool();
        if (this.UIManager != null) this.UIManager.ClearPool();
    }
}

/// <summary>
/// 游戏模式
/// </summary>
public enum GameMode
{
    NORMAL,
    FREE,
}