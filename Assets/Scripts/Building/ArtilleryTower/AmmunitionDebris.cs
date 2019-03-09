using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 炮弹破片
/// 砲弾の破片エフェクト
/// </summary>
public class AmmunitionDebris : MonoBehaviour
{
    public static readonly int AnimSpeedHash = Animator.StringToHash("Speed");
    public static readonly int _DissolveCtrl = Shader.PropertyToID("_DissolveCtrl");
    public static readonly int _Seed = Shader.PropertyToID("_Seed");

    public Ammunition Ammunition = null;
    public Animator Animator = null;
    public float AnimSpeed = 1f;
    /// <summary>
    /// 炮弹消失所用时间
    /// なくなるまでの時間
    /// </summary>
    public Timer DissolveTimer = new Timer(2f);

    /// <summary>
    /// 破片材质列表
    /// 破片のマテリアルリスト
    /// </summary>
    public List<Material> DebrisMaterials { get; set; } = new List<Material>();

    /// <summary>
    /// 重置显示
    /// ディスプレイをリセット
    /// </summary>
    public void ResetMaterials()
    {
        var seed = new Vector4(Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f), 0, 0);
        for (var i = 0; i < this.DebrisMaterials.Count; i++)
        {
            this.DebrisMaterials[i].SetFloat(_DissolveCtrl, 0f);
            this.DebrisMaterials[i].SetVector(_Seed, seed);
        }
        this.Animator.SetFloat(AnimSpeedHash, this.AnimSpeed);
        this.Animator.Play("Start");
    }
    private IEnumerator StartDissolve()
    {
        while (true)
        {
            if (this.DissolveTimer.UpdateAndIsReach(Time.deltaTime))
                break;
            for (var i = 0; i < this.DebrisMaterials.Count; i++)            
                this.DebrisMaterials[i].SetFloat(_DissolveCtrl, this.DissolveTimer.AfterPercent);            
            yield return null;
        }
        this.Ammunition.ReleaseThis();
        this.gameObject.SetActive(false);
    }
    /// <summary>
    /// 溶解
    /// </summary>
    public void Dissolve()
    {
        this.DissolveTimer.Reset();
        StartCoroutine(StartDissolve());
    }

    private void Awake()
    {
        var rs = GetComponentsInChildren<Renderer>();
        foreach (var r in rs)
        {
            var ms = r.materials;
            foreach (var m in ms)
            {
                this.DebrisMaterials.Add(m);
            }
        }
    }

}
