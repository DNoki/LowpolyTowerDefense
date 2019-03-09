using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBakudan : Enemy, IPoolObject
{
    public static readonly int _FresnelColor = Shader.PropertyToID("_FresnelColor");
    public static readonly int _MainColor = Shader.PropertyToID("_MainColor");

    [Header("EBakudan")]
    public Renderer BodyRenderer = null;
    /// <summary>
    /// 漫反射颜色
    /// </summary>
    [ColorUsage(true, true)] public Color DiffuseColor;
    /// <summary>
    /// 受伤颜色
    /// </summary>
    [ColorUsage(true, true)] public Color DamageColor;

    void IPoolObject.Initialize()
    {
        CommonInitialize();
        // 添加一个随机选择Y
        this.Anim.transform.eulerAngles = this.Anim.transform.eulerAngles.SetValue(y: Random.value * 360f);
    }
    void IPoolObject.Release()
    {
        CommonRelease();
    }
    public override IEnemy AfterTakedInitialize(IEnemy enemy)
    {
        base.AfterTakedInitialize(enemy);
        var e = enemy as EBakudan;

        this.DiffuseColor = e.DiffuseColor;
        this.DamageColor = e.DamageColor;

        this.DamageColor.a = 0f;
        this.BodyRenderer.material.SetColor(_MainColor, this.DiffuseColor);
        this.BodyRenderer.material.SetColor(_FresnelColor, this.DamageColor);
        return this;
    }
    public override IEnemy AfterTakedInitialize(EnemyGenerator generator)
    {
        base.AfterTakedInitialize(generator);

        var r1 = generator.Reserve1.Split('-');
        var r2 = generator.Reserve2.Split('-');
        this.DiffuseColor = new Color32(r1[0].ToByte(), r1[1].ToByte(), r1[2].ToByte(), r1[3].ToByte());
        this.DamageColor = new Color32(r2[0].ToByte(), r2[1].ToByte(), r2[2].ToByte(), r2[3].ToByte());

        this.DamageColor.a = 0f;
        this.BodyRenderer.material.SetColor(_MainColor, this.DiffuseColor);
        this.BodyRenderer.material.SetColor(_FresnelColor, this.DamageColor);
        return this;
    }

    protected override void ReleaseThis()
    {
        EnemyFactory.EBakudanPoolObject.Release(this);
    }

    public override void ApplyDamage(DamageMessage data)
    {
        if (this.IsDie) return;
        base.ApplyDamage(data);
        if (this.IsDie)
        {
            GameObjectFactory.BakuhatuEffectPoolObject.Take().AfterTakedInitialize(this.transform.position);
            ReleaseThis();
        }
        else
        {
            this.DamageColor.a = 1f - this.HP / this.HPLimit;
            this.BodyRenderer.material.SetColor(_FresnelColor, this.DamageColor);
        }
    }
}
