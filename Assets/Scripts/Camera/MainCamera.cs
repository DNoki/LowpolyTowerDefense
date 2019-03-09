using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
    /// <summary>
    /// 相机变换目标
    /// </summary>
    public Transform Target = null;
    [SerializeField] private float smoothPower = 100f;

    /// <summary>
    /// 平滑移动强度
    /// </summary>
    public float SmoothPower { get { return this.smoothPower; } set { this.smoothPower = value; } }

    void Update()
    {
        var pos = Vector3.Lerp(this.transform.position, this.Target.position, this.SmoothPower * Time.unscaledDeltaTime);
        var rotate = Quaternion.Lerp(this.transform.rotation, this.Target.rotation, this.SmoothPower * Time.unscaledDeltaTime);
        this.transform.SetPositionAndRotation(pos, rotate);
    }
}
