using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 魔法塔の攻撃
/// </summary>
public class MagicLine : MonoBehaviour, IPoolObject
{
    private readonly int _Length = Shader.PropertyToID("_Length");
    private readonly int _TilingAndOffset = Shader.PropertyToID("_TilingAndOffset");
    private readonly int _TilingAndOffset2 = Shader.PropertyToID("_TilingAndOffset2");
    private readonly int _TilingAndOffset3 = Shader.PropertyToID("_TilingAndOffset3");

    public LineRenderer LineRenderer = null;
    public Material Mat = null;
    public Transform EndMagicBall = null;
    public Transform StartMagicBall = null;
    /// <summary>
    /// 更新频率
    /// アップデート率
    /// </summary>
    public Timer UpdateFrequency = new Timer(0.01f);
    /// <summary>
    /// 伸缩上下限
    /// 伸縮リミット
    /// </summary>
    public Vector2 LimitTiling = new Vector2(0.5f, 1.5f);
    /// <summary>
    /// 细分数
    /// </summary>
    public int Subdivision = 20;
    public float MagicBallOffsetY = 0f;
    public Audio Audio = null;

    /// <summary>
    /// 线长度
    /// 線の長さ
    /// </summary>
    private float length = 1f;

    public bool IsReleased { get; private set; }
    //public Transform Lancher { get; set; }
    public DamageMessage DamageMessage { get; set; }
    public IEnemy Target { get; set; }

    void IPoolObject.Initialize()
    {
        this.IsReleased = false;
        this.gameObject.SetActive(true);
        this.Mat.SetFloat("_RandomSeed", Random.Range(0, 10000f));
        this.Audio.PlayRandomAudio();
    }
    void IPoolObject.Release()
    {
        this.IsReleased = true;
        this.gameObject.SetActive(false);
        this.Audio.StopAllAudio();
    }
    public void AfterTakedInitialize(IEnemy target, DamageMessage damage)
    {
        this.Target = target;
        this.DamageMessage = damage;
        SetLinePosition(this.DamageMessage.Damager.position, this.Target.Transform.position);
        this.StartMagicBall.position = this.DamageMessage.Damager.position;
    }

    private float GetTilingX()
    {
        if (Random.value >= 0.5f)
            return FunctionExtension.Remap(Random.value, 0f, 1f, 1f, this.LimitTiling.y);
        else return FunctionExtension.Remap(Random.value, 0f, 1f, this.LimitTiling.x, 1f);
    }
    private Vector4 GetTilingAndOffset(Vector4 to)
    {
        to.x = GetTilingX();
        to.z = Random.value;
        to.w = 1f / 8f * Random.Range(0, 8);
        return to;
    }
    /// <summary>
    /// 设置线位置
    /// 線の位置をセットする
    /// </summary>
    /// <param name="start">起始</param>
    /// <param name="end">结束</param>
    private void SetLinePosition(Vector3 start, Vector3 end)
    {
        this.length = Vector3.Distance(start, end);

        this.LineRenderer.positionCount = this.Subdivision + 1;
        for (var i = 0; i < this.Subdivision + 1; i++)
        {
            var pos = Vector3.Lerp(start, end, (float)i / (this.Subdivision));
            this.LineRenderer.SetPosition(i, pos);
        }
        this.EndMagicBall.position = end.SetValue(y: end.y + this.MagicBallOffsetY);
    }

    private void Awake()
    {
        if (!this.LineRenderer) this.LineRenderer = GetComponent<LineRenderer>();
        this.Mat = this.LineRenderer.material;
    }
    private void Update()
    {
        if (this.IsReleased) return;
        SetLinePosition(this.DamageMessage.Damager.position, this.Target.Transform.position);
        this.Target.ApplyDamage(new DamageMessage(this.DamageMessage.Damager, this.DamageMessage.Amount * Time.deltaTime));

        // 材质动画
        if (this.UpdateFrequency.UpdateAndIsReach(Time.deltaTime))
        {
            this.Mat.SetFloat(this._Length, this.length);

            var to = this.Mat.GetVector(this._TilingAndOffset);
            this.Mat.SetVector(this._TilingAndOffset, GetTilingAndOffset(to));

            to = this.Mat.GetVector(this._TilingAndOffset2);
            this.Mat.SetVector(this._TilingAndOffset2, GetTilingAndOffset(to));

            to = this.Mat.GetVector(this._TilingAndOffset3);
            this.Mat.SetVector(this._TilingAndOffset3, GetTilingAndOffset(to));
        }
    }
}
