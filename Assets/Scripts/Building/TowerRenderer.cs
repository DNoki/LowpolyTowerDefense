using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 防御タワーのレンダラー調整
/// </summary>
public class TowerRenderer : MonoBehaviour
{
    public static readonly int _MultiplyColor = Shader.PropertyToID("_MultiplyColor");
    public static readonly int _EmissionColor = Shader.PropertyToID("_EmissionColor");

    public List<Renderer> RendererList = null;

    public void SetEnable(bool enable)
    {
        for (var i = 0; i < this.RendererList.Count; i++)        
            this.RendererList[i].enabled = enable;        
    }
    /// <summary>
    /// 设置颜色
    /// </summary>
    /// <param name="multiplyColor">指定乘算色</param>
    /// <param name="emissionColor">指定添加色</param>
    public void SetColor(Color? multiplyColor, Color? emissionColor)
    {
        for (var i = 0; i < this.RendererList.Count; i++)
        {
            for (var j = 0; j < this.RendererList[i].materials.Length; j++)
            {
                if (multiplyColor.HasValue)
                    this.RendererList[i].materials[j].SetColor(_MultiplyColor, multiplyColor.Value);
                if (emissionColor.HasValue)
                    this.RendererList[i].materials[j].SetColor(_EmissionColor, emissionColor.Value);
            }
        }
    }

}
