using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 敵のベースクラス
/// </summary>
public abstract class Enemy : MonoBehaviour, IEnemy
{
    public static readonly int AnimSpeedHash = Animator.StringToHash("Speed");
    public static readonly int AnimAttackHash = Animator.StringToHash("Attack");
    public static readonly int AnimResetHash = Animator.StringToHash("Reset");
    public static readonly int AnimHitHash = Animator.StringToHash("Hit");
    public static readonly int AnimDieHash = Animator.StringToHash("Die");

    [SerializeField] private Animator anim = null;
    /// <summary>
    /// 血量条
    /// </summary>
    [SerializeField, Tooltip("血量条")] private EnemyHP hpBar = null;
    /// <summary>
    /// 血量上限
    /// </summary>
    [SerializeField, Tooltip("血量上限")] private float hpLimit = 20f;
    /// <summary>
    /// 攻击力
    /// </summary>
    [SerializeField] private float attackPower = 10f;
    /// <summary>
    /// 掉落金币
    /// </summary>
    [SerializeField, Tooltip("掉落金币")] private int dropMoney = 10;
    /// <summary>
    /// 消灭得分
    /// </summary>
    [SerializeField, Tooltip("消灭得分")] private int dropScore = 10;
    /// <summary>
    /// 移动速度
    /// </summary>
    [SerializeField, Tooltip("移动速度")] private float moveSpeed = 1f;
    /// <summary>
    /// 旋转速度
    /// </summary>
    [SerializeField, Tooltip("旋转速度")] private float rotateSpeed = 1f;
    /// <summary>
    /// Y轴偏移
    /// </summary>
    [SerializeField, Tooltip("Y轴偏移")] private float offsetY = 0f;
    /// <summary>
    /// 攻击距离
    /// </summary>
    [Header("FreeMode"), SerializeField] private float attackDistance = 1f;
    /// <summary>
    /// 攻击速度
    /// </summary>
    [SerializeField] private Timer attackSpeed = new Timer(2f);

    /// <summary>
    /// 当前血量
    /// </summary>
    protected float hp = 20f;
    /// <summary>
    /// 敌人移动目标点索引
    /// </summary>
    protected int NormalTargetIndex = 0;
    /// <summary>
    /// Buff列表
    /// </summary>
    protected List<IBuff> BuffList = new List<IBuff>();
    /// <summary>
    /// 减速Buff影响系数
    /// </summary>
    protected float SlowDownInfluenceCoefficient = 1f;

    public Transform Transform => this.transform;
    public Animator Anim => this.anim;
    public bool IsReleased { get; set; }
    /// <summary>
    /// 当前血量
    /// </summary>
    public float HP
    {
        get { return this.hp; }
        set
        {
            this.hp = Mathf.Max(value, 0);
            this.hpBar.Bar.fillAmount = this.hp / this.hpLimit;
        }
    }
    /// <summary>
    /// 血量上限
    /// </summary>
    public float HPLimit => this.hpLimit;
    /// <summary>
    /// 是否死亡
    /// </summary>
    public bool IsDie => this.IsReleased || this.HP <= 0;
    /// <summary>
    /// 行为模式
    /// </summary>
    public GameMode BehaviourMode { get; set; }
    /// <summary>
    /// 掉落金钱
    /// </summary>
    public int DropMoney => this.dropMoney;
    /// <summary>
    /// 消灭得分
    /// </summary>
    public int DropScore => this.dropScore;
    public DamageMessage GetDamageMessage => new DamageMessage() { Damager = this.Transform, Amount = this.attackPower };
    public float MoveSpeed => this.moveSpeed * this.SlowDownInfluenceCoefficient;
    public float RotateSpeed => this.rotateSpeed;
    int IEnemy.SerialNumber { get; set; }
    public float OffsetY => this.offsetY;

    /// <summary>
    /// 有限状态机
    /// </summary>
    public FiniteStateMachine<Enemy> StateMachine { get; protected set; }
    /// <summary>
    /// 路径
    /// </summary>
    public Stack<Vector2Int> PathStack { get; set; } = new Stack<Vector2Int>();
    /// <summary>
    /// 攻击目标
    /// </summary>
    public IEnemyAttackTarget Target { get; set; }
    /// <summary>
    /// 攻击距离
    /// </summary>
    public float AttackDistance => this.attackDistance;
    /// <summary>
    /// 攻击速度
    /// </summary>
    public Timer AttackSpeed => this.attackSpeed;

    protected void CommonInitialize()
    {
        this.IsReleased = false;
        this.BuffList.Clear();

        this.BehaviourMode = GameScene.Instance.GameMode;
        
        this.gameObject.SetActive(true);

        if (this.BehaviourMode == GameMode.NORMAL)
        {
            this.NormalTargetIndex = 1;
            this.Anim.SetInteger(AnimSpeedHash, 2);
        }
    }
    protected void CommonRelease()
    {
        if (this.IsDie)
        {
            GameScene.Instance.Money += this.DropMoney;
            GameScene.Instance.Score += this.DropScore;
        }
        this.IsReleased = true;
        GameScene.Instance.RemoveEnemy(this);
        this.gameObject.SetActive(false);
    }
    /// <summary>
    /// 后初始化
    /// </summary>
    /// <param name="enemy">将预制体信息拷贝到当前怪物</param>
    /// <returns></returns>
    public virtual IEnemy AfterTakedInitialize(IEnemy enemy)
    {
        var e = enemy as Enemy;
        this.hpLimit = e.hpLimit;
        this.dropMoney = e.dropMoney;
        this.dropScore = e.dropScore;
        this.moveSpeed = e.moveSpeed;
        this.rotateSpeed = e.rotateSpeed;
        this.attackPower = e.attackPower;
        this.attackDistance = e.attackDistance;
        this.attackSpeed = e.attackSpeed;
        this.transform.localScale = enemy.Transform.localScale;

        this.HP = this.HPLimit;
        return this;
    }
    public virtual IEnemy AfterTakedInitialize(EnemyGenerator generator)
    {
        this.hpLimit = generator.HPLimit;
        this.dropMoney = generator.DropMoney;
        this.dropScore = generator.DropScore;
        this.moveSpeed = generator.MoveSpeed;
        this.rotateSpeed = generator.RotateSpeed;
        this.attackPower = generator.AttackPower;
        this.attackDistance = generator.AttackDistance;
        this.attackSpeed = new Timer(generator.AttackSpeed);
        this.transform.localScale = Vector3.one * generator.Scale;

        this.HP = this.HPLimit;
        return this;
    }
    /// <summary>
    /// 释放对象
    /// </summary>
    protected abstract void ReleaseThis();

    /// <summary>
    /// 更新路径
    /// </summary>
    /// <param name="targetPos"></param>
    public void UpdatePathAsyn() => StartCoroutine(PathFindingAsyn(this.Target.CenterPosition));
    /// <summary>
    /// 设定目标并更新路径
    /// </summary>
    /// <param name="target"></param>
    public void UpdatePathAsyn(IEnemyAttackTarget target)
    {
        this.Target = target;
        UpdatePathAsyn();
    }
    /// <summary>
    /// 根据现在为止异步计算到目标的路径并更新目标位置栈
    /// </summary>
    /// <returns></returns>
    private IEnumerator PathFindingAsyn(Vector2Int targetPos)
    {
        this.PathStack.Clear();
        var currentPos = GroundBlock.WorldPositionToMapPos(this.transform.position);
        if (currentPos == targetPos) yield break;

        Stack<Vector2Int> posStack = null;
        while (posStack == null)
        {
            var task = new Task(() => AStar.DoAStar(currentPos, targetPos, out posStack));
            task.Start();
            // 等待直到异步计算结束
            while (!task.IsCompleted) yield return null;
        }
        if (posStack.Count <= 0) yield break;

        this.PathStack = posStack;// 更新目标位置栈
    }

    /// <summary>
    /// 施加伤害
    /// </summary>
    /// <param name="data"></param>
    public virtual void ApplyDamage(DamageMessage data)
    {
        if (!this.IsDie)
        {
            this.HP -= data.Amount;
        }
        else return;

        if(this.IsDie)
            GameObjectFactory.CoinEffectPoolObject.Take().Play(this.transform.position, 2, 5);
    }
    /// <summary>
    /// 添加BUFF
    /// </summary>
    /// <param name="buff"></param>
    public void AddBuff(IBuff buff)
    {
        if (this.IsDie) return;
        this.BuffList.Add(buff);
    }

    /// <summary>
    /// 攻击，由动画调用
    /// </summary>
    protected void FreeAttack()
    {
        if (this.IsDie) return;
        var data = this.GetDamageMessage;
        this.Target.ApplyDamage(data);
    }
    /// <summary>
    /// 普通模式的移动行为
    /// </summary>
    protected void NormalMove()
    {
        if (this.IsDie) return;
        var gameScene = GameScene.Instance as GameSceneNormal;
        var route = gameScene.Route.RouteList;
        var currentRoute = route[this.NormalTargetIndex - 1].position;
        var targetRoute = route[this.NormalTargetIndex].position;

        var targetDistance = (targetRoute - currentRoute).sqrMagnitude;
        var currentDistance = (this.transform.position.SetValue(y: 0f) - currentRoute).sqrMagnitude;
        if (targetDistance - currentDistance <= 1 || currentDistance >= targetDistance)
        {
            this.NormalTargetIndex++;
            if (this.NormalTargetIndex >= route.Count)
            {
                // 已抵达目标，应用伤害
                NormalAttack();
                return;
            }
            currentRoute = targetRoute;
            targetRoute = route[this.NormalTargetIndex].position;
            this.transform.position = currentRoute.SetValue(null, this.transform.position.y, null);
        }

        var direction = (targetRoute - currentRoute).normalized;
        this.transform.Translate(direction * this.MoveSpeed * Time.deltaTime, Space.World);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction), this.RotateSpeed * Time.deltaTime);
    }
    /// <summary>
    /// 普通模式的攻击行为
    /// </summary>
    protected void NormalAttack()
    {
        if (this.IsReleased) return;
        // 应用伤害并移除自身
        GameScene.Instance.CenterBuilding.ApplyDamage(this.GetDamageMessage);
        ReleaseThis();
    }
    /// <summary>
    /// 更新Buffs
    /// </summary>
    protected void UpdateBuffs()
    {
        // 重置被Buff影响的效果
        this.SlowDownInfluenceCoefficient = 1f;

        for (var i = 0; i < this.BuffList.Count; i++)
        {
            var buff = this.BuffList[this.BuffList.Count - 1 - i];
            if (buff.IsOver)
            {
                this.BuffList.RemoveAt(this.BuffList.Count - 1 - i);
                continue;
            }
            else
            {
                if (buff is SlowDownDebuff)
                {
                    // 只有当影响更强时才更新
                    var coefficient = buff.UpdateAndCalculation(Time.deltaTime);
                    if (coefficient < this.SlowDownInfluenceCoefficient)
                        this.SlowDownInfluenceCoefficient = coefficient;
                }
            }

        }
    }

    protected virtual void Awake() { }
    protected virtual void Update()
    {
        UpdateBuffs();

        if (this.BehaviourMode == GameMode.FREE)
            this.StateMachine.Execute();
        else NormalMove();
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (this.IsReleased) return;
        if (this.BehaviourMode == GameMode.FREE) return;
        var obj = other.GetComponent<CenterBuilding>();
        if (obj == null) return;
        NormalAttack();
    }
}



public class EnemyIdleState : StateBase<Enemy>
{
    protected override void OnEnter(object data = null)
    {
        if (this.Owner.IsDie) return;

        this.Owner.Anim.SetInteger(Enemy.AnimSpeedHash, 0);
        this.Owner.Anim.SetTrigger(Enemy.AnimResetHash);
        if (this.Owner.Target != null)
            this.Owner.UpdatePathAsyn();// 更新路径
    }

    public override void CheckTurn()
    {
        if (this.Owner.IsDie) return;

        if (this.Owner.PathStack.Count > 0)
        {
            ChangeState<EnemyMoveState>();
            return;
        }
    }

    public override void UpdateState()
    {

    }
}

public class EnemyMoveState : StateBase<Enemy>
{
    private Stack<Vector2Int> pathStack => this.Owner.PathStack;
    private IEnemyAttackTarget target => this.Owner.Target;
    private float attackDistance => this.Owner.AttackDistance;

    private Vector2Int currentPos;
    private Vector2Int nextPos;


    protected override void OnEnter(object data = null) => this.Owner.Anim.SetInteger(Enemy.AnimSpeedHash, 2);
    protected override void OnExit(object data = null) => this.Owner.Anim.SetInteger(Enemy.AnimSpeedHash, 0);

    public override void CheckTurn()
    {
        if (this.Owner.IsDie)
        {
            ChangeState<EnemyIdleState>();
            return;
        }
        if (this.target == null)
        {
            ChangeState<EnemyIdleState>();// 没有目标，切换到待机状态
            return;
        }
        if (this.pathStack == null)
            this.Owner.PathStack = new Stack<Vector2Int>();
        if (this.pathStack.Count <= 0)
        {
            ChangeState<EnemyIdleState>();// 没有移动目标，切换到待机状态
            return;
        }

        this.currentPos = GroundBlock.WorldPositionToMapPos(this.Owner.transform.position);
        this.nextPos = this.pathStack.Peek();
        //this.targetPos = GroundBlock.WorldPositionToMapPos(this.target.Transform.position);

        if (this.currentPos == this.nextPos)
        {
            // 当前位置和下一位置相同，移动到下下一位置
            this.pathStack.Pop();
            CheckTurn();
            return;
        }

        if (this.target is IBuilding)
        {
            if (this.target.CalculateDistance(this.Owner.transform.position) <= this.attackDistance)
            {
                // 目标已在攻击范围内，开始攻击
                this.pathStack.Clear();
                ChangeState<EnemyAttackState>();
                return;
            }

            var block = GameScene.Instance.Ground.GetBlock(this.nextPos);
            var currentBlock = GameScene.Instance.Ground.GetBlock(this.currentPos);

            if (block.State == GroundStateType.OBSTRUCT)
            {
                if (currentBlock.State == GroundStateType.OBSTRUCT && currentBlock.Owner != this.target)
                    // 对象目前处于建筑内，将继续按路径移动
                    return;
                if (block.Owner == this.target)
                {
                    // 下一个位置是目标建筑,以抵达目标建筑
                    this.pathStack.Clear();
                    ChangeState<EnemyAttackState>();
                    return;
                }
                else
                {
                    // 下一个位置是不可通过区域，重新计算路径
                    ChangeState<EnemyIdleState>();
                    return;
                }
            }
        }
    }
    public override void UpdateState()
    {
        if (this.Owner.Anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;
        // 向下一位置移动
        var nextWorldPos = GroundBlock.MapPostioinCenterToWorldPosition(this.nextPos, this.Owner.transform.position.y);
        var direction = (nextWorldPos - this.Owner.transform.position).normalized;
        this.Owner.transform.Translate(direction * this.Owner.MoveSpeed * Time.deltaTime, Space.World);
        this.Owner.transform.rotation = Quaternion.Lerp(this.Owner.transform.rotation, Quaternion.LookRotation(direction), this.Owner.RotateSpeed * Time.deltaTime);
    }
}
public class EnemyAttackState : StateBase<Enemy>
{
    /// <summary>
    /// 指示攻击动画是否结束
    /// </summary>
    public bool IsAttackOver = false;

    protected override void OnEnter(object data = null)
    {
        if (!this.Owner.Target.IsDie)
        {
            this.IsAttackOver = false;
            this.Owner.Anim.SetTrigger(Enemy.AnimAttackHash);
        }
    }

    public override void CheckTurn()
    {
        if (this.Owner.IsDie)
        {
            ChangeState<EnemyIdleState>();
            return;
        }
        if (!this.Owner.AttackSpeed.IsReach()) return;

        if (!this.IsAttackOver) return;
        else this.IsAttackOver = false;

        if (this.Owner.Target.IsDie) return;

        if (this.Owner.Target.CalculateDistance(this.Owner.transform.position) <= this.Owner.AttackDistance)
        {
            // 目标在攻击范围内，开始攻击
            ChangeState<EnemyAttackState>();
            return;
        }
        else
        {
            // 目标大于攻击距离，重新计算路径
            ChangeState<EnemyIdleState>();
            return;
        }
    }
    public override void UpdateState()
    {
        this.Owner.AttackSpeed.UpdateTimer(Time.deltaTime);
    }
}

