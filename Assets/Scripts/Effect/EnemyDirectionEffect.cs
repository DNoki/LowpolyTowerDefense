using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 当敌人过远时，在基地旁显示敌人方向
/// </summary>
public class EnemyDirectionEffect : MonoBehaviour, IPoolObject
{
    public Transform Contrller = null;
    public Renderer Renderer = null;
    public Vector4 RemapDistance = new Vector4(4f, 40f, -1.5f, 3f);
    public float DisplayDistance = 20f;

    public bool IsReleased { get; set; }

    public IEnemy Target { get; set; }

    void IPoolObject.Initialize()
    {
        this.IsReleased = false;
        this.transform.position = GameScene.Instance.CenterBuilding.WorldCenterPosition;
    }
    void IPoolObject.Release()
    {
        this.IsReleased = true;
        this.Renderer.enabled = false;
    }

    /// <summary>
    /// 设置渲染器开关
    /// </summary>
    /// <param name="enabled"></param>
    private void SetRendererEnabled(bool enabled)
    {
        if (this.Renderer.enabled != enabled)
            this.Renderer.enabled = enabled;
    }
    /// <summary>
    /// 设置朝向位置
    /// </summary>
    /// <param name="targetPos">朝向位置</param>
    private void SetLookRotation(Vector3 targetPos)
    {
        if (this.IsReleased) return;
        var direction = (targetPos - this.transform.position).SetValue(null, 0, null);
        var distance = direction.magnitude;
        if (distance < this.DisplayDistance)// 距离过近则不显示效果
        {
            SetRendererEnabled(false);
            return;
        }
        else SetRendererEnabled(true);
        this.transform.rotation = Quaternion.LookRotation(direction);
        this.Contrller.localPosition = this.Contrller.localPosition.SetValue(z: FunctionExtension.Remap(distance, this.RemapDistance));
    }

    private void Update()
    {
        if (this.IsReleased) return;
        if (this.Target == null || this.Target.IsDie)
            GameObjectFactory.EnemyDirectionEffectPoolObject.Release(this);
        SetLookRotation(this.Target.Transform.position);
    }
}

