using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 砲弾
/// </summary>
public class Ammunition : MonoBehaviour, IPoolObject
{
    public Renderer MainRenderer = null;
    public AmmunitionDebris[] DebrisList = null;
    /// <summary>
    /// 存在时间
    /// 存在時間
    /// </summary>
    public Timer ExistenceTime = new Timer(10f);
    /// <summary>
    /// 每秒旋转速度上下限
    /// 回転スピードリミット
    /// </summary>
    public Vector2 RotateVelocityLimit = new Vector2(90f, 270f);
    public Audio AudioExplosion = null;
    public Audio AudioEmission = null;

    public bool IsReleased { get; set; }
    public AmmunitionDebris GetRandomDebris => this.DebrisList[Random.Range(0, this.DebrisList.Length)];
    public DamageMessage DamageMessage { get; set; }
    /// <summary>
    /// 炮弹移动速度
    /// 砲弾の移動スピード
    /// </summary>
    public float Speed { get; set; } = 10f;
    /// <summary>
    /// 爆炸范围
    /// 爆発範囲
    /// </summary>
    public float ExplosionRange { get; set; } = 3f;

    private Vector3 target = Vector3.zero;
    /// <summary>
    /// 运动轨迹
    /// 移動軌跡は放物線
    /// </summary>
    private ProjectileMotion projectile;
    private float rotationVelocity = 0f;
    private Vector3 rotationDirection = Vector3.forward;

    void IPoolObject.Initialize()
    {
        this.ExistenceTime.Reset();
        this.rotationVelocity = Random.value >= 0.5f ? 1f : -1f * Random.Range(this.RotateVelocityLimit.x, this.RotateVelocityLimit.y);
        this.rotationDirection = Random.onUnitSphere;
        this.IsReleased = false;
        this.MainRenderer.enabled = true;
    }
    void IPoolObject.Release()
    {
        this.IsReleased = true;
    }
    public void AfterTakedInitialize(Vector3 target, DamageMessage damage, float speed, float range)
    {
        this.transform.position = damage.Damager.position;
        this.target = target;
        this.DamageMessage = damage;
        this.Speed = speed;
        this.ExplosionRange = range;
        var angle = Vector3.Angle(damage.Damager.forward, damage.Damager.forward.SetValue(y: 0));
        this.projectile = ProjectileMotion.CalculateVelocity(damage.Damager.position, target, angle);
        this.AudioEmission.PlayRandomAudio();
    }

    /// <summary>
    /// 爆炸
    /// 爆発
    /// </summary>
    public void Explosion()
    {
        if (this.IsReleased) return;
        this.IsReleased = true;
        this.transform.position = this.target;
        this.transform.rotation = Quaternion.identity;
        this.MainRenderer.enabled = false;
        this.AudioExplosion.PlayRandomAudio();

        var enemys = Physics.OverlapCapsule(this.transform.position.SetValue(y: -1000), this.transform.position.SetValue(y: 1000), this.ExplosionRange, EnemyFactory.ENEMY_LAYER_MASK);
        foreach (var ec in enemys)
        {
            var enemy = ec.GetComponent<IEnemy>();
            if (enemy == null) continue;
            enemy.ApplyDamage(this.DamageMessage);
        }

        var debris = this.GetRandomDebris;

        debris.transform.localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
        debris.gameObject.SetActive(true);
        debris.ResetMaterials();

        GameObjectFactory.AmmunitionSmokeEffectPoolObject.Take().AfterTakedInitialize(this.transform.position);
    }

    public void ReleaseThis()
    {
        GameObjectFactory.AmmunitionPoolObject.Release(this);
    }


    private void Update()
    {
        if (this.IsReleased) return;
        if (this.ExistenceTime.UpdateAndIsReach(Time.deltaTime))
        {
            Explosion();
            return;
        }
        if (this.transform.position.y <= this.target.y)
        {
            Explosion();
            return;
        }
        this.transform.position = this.projectile.GetPosition(this.ExistenceTime.AfterTime * this.Speed);
        this.MainRenderer.transform.Rotate(this.rotationDirection, this.rotationVelocity * Time.deltaTime, Space.Self);
    }
}
