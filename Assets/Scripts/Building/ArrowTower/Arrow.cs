using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 矢　矢塔あるいはセンターの攻撃
/// </summary>
public class Arrow : MonoBehaviour, IPoolObject
{
    [SerializeField] private Rigidbody rigid = null;
    public Audio AudioEmission = null;
    public Audio AudioAttach = null;

    /// <summary>
    /// 存在时间
    /// 存在時間
    /// </summary>
    public Timer ExistenceTime = new Timer(10f);

    /// <summary>
    /// 目标敌人
    /// ターゲット敵
    /// </summary>
    public IEnemy Target { get; set; }
    public bool IsReleased { get; set; }
    public bool IsReleaseing { get; set; }
    public DamageMessage DamageMessage { get; set; }
    public float Speed { get; set; } = 10f;

    /// <summary>
    /// 运动轨迹
    /// 移動軌跡は放物線
    /// </summary>
    private ProjectileMotion projectile;
    private float afterTime = 0f;
    private System.Type launcherType = null;

    void IPoolObject.Initialize()
    {
        this.IsReleaseing = false;
        this.IsReleased = false;
        this.ExistenceTime.Reset();
        this.afterTime = 0f;
        this.gameObject.SetActive(true);
    }
    void IPoolObject.Release()
    {
        this.IsReleaseing = true;
        this.IsReleased = true;
        this.gameObject.SetActive(false);
    }
    /// <summary>
    /// 在从池中取出后调用的初始化
    /// オブジェクトプールから取ってからの実行メッソド
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damage"></param>
    /// <param name="speed"></param>
    public void AfterTakedInitialize(IEnemy target, DamageMessage damage, float speed, System.Type type)
    {
        this.transform.position = damage.Damager.position;
        this.Target = target;
        this.DamageMessage = damage;
        this.Speed = speed;
        var angle = Vector3.Angle(damage.Damager.forward, damage.Damager.forward.SetValue(y: 0));
        this.projectile = ProjectileMotion.CalculateVelocity(damage.Damager.position, target.Transform.position, angle);
        this.launcherType = type;
        this.AudioEmission.PlayRandomAudio();
    }

    public void ReleaseThis(float time)
    {
        this.IsReleaseing = true;
        if (this.launcherType.Name == nameof(ArrowTowerAttackSystem))
            GameObjectFactory.ArrowPoolObject.Release(this, this, time);
        else if (this.launcherType.Name == nameof(CenterBuildingAttackSystem))
            GameObjectFactory.CArrowPoolObject.Release(this, this, time);
        else Destroy(this.gameObject);
    }

    private void Awake()
    {
        if (!this.rigid) this.rigid = GetComponent<Rigidbody>();      
    }
    private void Update()
    {
        if (this.IsReleaseing) return;
        if (this.ExistenceTime.UpdateAndIsReach(Time.deltaTime) || this.Target == null)
        {
            ReleaseThis(0f);
            return;
        }

        if (this.transform.position.y <= 0f)
        {
            ReleaseThis(2f);
            return;
        }
    }
    private void FixedUpdate()
    {
        if (this.IsReleaseing) return;
        this.afterTime += Time.fixedDeltaTime;
        this.rigid.MovePosition(this.projectile.GetPosition(this.afterTime * this.Speed));
        this.rigid.MoveRotation(Quaternion.LookRotation(this.projectile.GetVelocity(this.afterTime * this.Speed)));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (this.IsReleased) return;
        var enemy = other.GetComponent<IEnemy>();
        if (enemy == this.Target)
        {
            enemy.ApplyDamage(this.DamageMessage);
            this.AudioAttach.PlayRandomAudio();

            GameObjectFactory. ArrowTouchEffectPoolObject.Take().AfterTakedInitialize(this.transform.position, -this.transform.forward);
            GameObjectFactory. ArrowContactFlashEffectPoolObject.Take().AfterTakedInitialize(enemy.Transform.position, enemy.Transform);

            this.IsReleased = true;
            ReleaseThis(2f);
        }
    }
}
