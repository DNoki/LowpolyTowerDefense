using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 绘制领域
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldArea : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter = null;
    [SerializeField] private new MeshRenderer renderer = null;

    /// <summary>
    /// 领域
    /// </summary>
    public Dictionary<Vector2Int, GroundBlock> Field { get; private set; } = new Dictionary<Vector2Int, GroundBlock>();
    /// <summary>
    /// 领域范围
    /// </summary>
    public RectInt FieldRect { get; private set; }

    /// <summary>
    /// 异步创建领域网格
    /// </summary>
    /// <returns></returns>
    private IEnumerator MakeFieldMesh(ICollection<GroundBlock> fieldBlock, RectInt range)
    {
        var posList = new List<Vector2Int>();
        foreach (var block in fieldBlock)
            posList.Add(block.MapPosition);

        List<Vector3> vertices = null;
        List<Vector2> uv = null;
        List<int> triangles = null;
        var task = new Task(new System.Action(() => ProceduralPlaneMesh.MakePlaneMesh(range, posList, GroundBlock.UNIT, Vector3.zero, out vertices, out uv, out triangles)));
        task.Start();
        while (!task.IsCompleted) yield return null;

        var mesh = new Mesh();
        mesh.name = "Field";
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        this.meshFilter.mesh = mesh;
        this.renderer.material.SetVector("_Scale", new Vector4(range.size.x, range.size.y));
    }
    /// <summary>
    /// 设置显示绘制领域
    /// </summary>
    /// <param name="value"></param>
    public void SetFieldDisplayEnable(bool value)
    {
#warning zanshi quxiao xianshi lingyu gongneng 暂时取消领域显示功能和更新领域功能
        //if (this.renderer.enabled != value)
        //    this.renderer.enabled = value;
    }

    /// <summary>
    /// 添加领域
    /// </summary>
    /// <param name="blocks"></param>
    public void AddField(List<GroundBlock> blocks)
    {
        if (GameScene.Instance.GameMode == GameMode.NORMAL) return;

        foreach (var block in blocks)
        {
            if (this.Field.ContainsKey(block.MapPosition)) continue;
            this.Field.Add(block.MapPosition, block);
        }
        UpdateFieldRange();
    }
    /// <summary>
    /// 移除领域
    /// </summary>
    /// <param name="blocks"></param>
    public void RemoveField(List<GroundBlock> blocks)
    {
        if (GameScene.Instance.GameMode == GameMode.NORMAL) return;

        foreach (var block in blocks)
        {
            if (this.Field.ContainsKey(block.MapPosition))
                this.Field.Remove(block.MapPosition);
        }
        UpdateFieldRange();
    }
    /// <summary>
    /// 更新领域范围
    /// </summary>
    /// <returns></returns>
    private void UpdateFieldRange()
    {
        if (GameScene.Instance.GameMode == GameMode.NORMAL) return;

        var newFieldRect = new RectInt();
        if (this.Field.Count > 0)
            foreach (var block in this.Field)
            {
                newFieldRect.SetMinMax(block.Value.MapPosition, block.Value.MapPosition);
                break;
            }
        else return;

        foreach (var block in this.Field)
        {
            newFieldRect.xMin = Mathf.Min(block.Value.MapPosition.x, newFieldRect.xMin);
            newFieldRect.xMax = Mathf.Max(block.Value.MapPosition.x, newFieldRect.xMax);
            newFieldRect.yMin = Mathf.Min(block.Value.MapPosition.y, newFieldRect.yMin);
            newFieldRect.yMax = Mathf.Max(block.Value.MapPosition.y, newFieldRect.yMax);
        }
        this.FieldRect = newFieldRect;

        StartCoroutine(this.MakeFieldMesh(this.Field.Values, this.FieldRect));// 异步更新领域网格

        return;
    }

    private void Awake()
    {
        if (!this.meshFilter) this.meshFilter = GetComponent<MeshFilter>();
        if (!this.renderer) this.renderer = GetComponent<MeshRenderer>();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var block in this.Field)
        {
            var range = GroundBlock.MapPositionToRect(block.Value.MapPosition);
            //Gizmos.DrawLine(new Vector3(range.xMin, 0, range.yMax), new Vector3(range.xMax, 0, range.yMin));
            Gizmos.DrawLine(new Vector3(range.xMin, 0, range.yMin), new Vector3(range.xMin, 0, range.yMax));
            Gizmos.DrawLine(new Vector3(range.xMin, 0, range.yMax), new Vector3(range.xMax, 0, range.yMax));
            Gizmos.DrawLine(new Vector3(range.xMax, 0, range.yMax), new Vector3(range.xMax, 0, range.yMin));
            Gizmos.DrawLine(new Vector3(range.xMax, 0, range.yMin), new Vector3(range.xMin, 0, range.yMin));
        }
    }
#endif
}
