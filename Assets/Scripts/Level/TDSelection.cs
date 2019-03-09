using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 选择对象
/// 实现单选，复选，框选，同类选（双击某个对象，则屏幕范围内同一类对象全选）
/// </summary>
public class TDSelection : MonoBehaviour
{
    ///// <summary>
    ///// 选框绘制边框宽度（最小为1）
    ///// </summary>
    //[Header("QuadSelect")] [SerializeField] private float quadDrawWidth = 2;
    ///// <summary>
    ///// 选框绘制颜色
    ///// </summary>
    //[SerializeField] private Color quadDrawColor = Color.white;


    /// <summary>
    /// 可选对象层遮罩
    /// </summary>
    public int SelectableObjMask => LayerMask.GetMask("Building", "Enemy");
    /// <summary>
    /// 被选择的对象列表
    /// </summary>
    public List<ISelectableObject> SelectionObjects { get; } = new List<ISelectableObject>();
    /// <summary>
    /// 已选择对象的数量
    /// </summary>
    public int Count => this.SelectionObjects.Count;

    private ISelectableObject previewObject = null;

    /// <summary>
    /// 添加选择对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public ISelectableObject Add(ISelectableObject obj)
    {
        if (this.SelectionObjects.Contains(obj)) return obj;
        this.SelectionObjects.Add(obj);
        obj.Selected(true);
        return obj;
    }
    /// <summary>
    /// 取消对象选择
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public ISelectableObject Remove(ISelectableObject obj)
    {
        if (!this.SelectionObjects.Contains(obj)) return obj;
        this.SelectionObjects.Remove(obj);
        obj.Selected(false);
        return obj;
    }
    public bool Contains(ISelectableObject obj) => this.SelectionObjects.Contains(obj);
    /// <summary>
    /// 清空选择的对象
    /// </summary>
    public void Clear()
    {
        for (var i = 0; i < this.SelectionObjects.Count; i++)
            this.SelectionObjects[i].Selected(false);
        this.SelectionObjects.Clear();
    }

    private ISelectableObject MouseRayHitObj()
    {
        var ray = InputCtrl.MainMouseRay;
        RaycastHit hit;
        Physics.Raycast(ray, out hit, float.MaxValue, this.SelectableObjMask);
        if (hit.transform != null)
            return hit.transform.GetComponent<ISelectableObject>();
        else return null;
    }
    /// <summary>
    /// 预览鼠标下的对象
    /// </summary>
    private void PreviewSelect()
    {
        var obj = MouseRayHitObj();
        if (obj != null && !Contains(obj))
        {
            if (this.previewObject == null)
            {
                this.previewObject = obj;
                obj.PreviewSelect(true);
                return;
            }
            else return;
        }
        if (this.previewObject != null)
        {
            this.previewObject.PreviewSelect(false);
            this.previewObject = null;
        }
    }
    /// <summary>
    /// 单选
    /// </summary>
    private void SingleSelect()
    {
        if (InputCtrl.IsLeftMouseDown)
        {
            var obj = MouseRayHitObj();
            if (obj != null)
            {
                Clear();
                Add(obj);
            }
            else Clear();
        }
        else if (InputCtrl.IsRightMouseDown)
        {
            Clear();
        }
    }


    private void Update()
    {
        // 单击选择一个单位
        // 摁住Shift复选单位
        // 双击选择相同单位
        // 框选选择多个单位
        if (GameScene.Instance.IsGameOver)
        {
            Clear();
            return;
        }
        if (GameScene.Instance.BuildingFactory.IsPreviewing)// 正在放置建筑
        {
            Clear();
            return;
        }

        if (BuildingItemCanvas.IsMouseEnter)// 鼠标在UI上
            return;

        if (Mathf.Approximately(Time.timeScale, 0f))
            return;

        PreviewSelect();
        SingleSelect();
    }

    ///// <summary>
    ///// 相机渲染完成后，渲染选择范围框
    ///// </summary>
    //private void OnRenderObject()
    //{
    //    if (this.Quad.IsDraw)
    //    {
    //        this.Quad.Material.SetPass(0);// 设定使用的材质，如果不设定将会随机（上一个）使用一个材质渲染

    //        GL.PushMatrix();
    //        GL.LoadOrtho();
    //        GL.LoadPixelMatrix();

    //        //GL.MultMatrix(this.transform.localToWorldMatrix);

    //        GL.Begin(GL.QUADS);
    //        GL.Color(this.quadDrawColor);

    //        foreach (var pos in QuadSelect.GetQuadFrameVertex(this.Quad.StartPos, this.Quad.EndPos, this.quadDrawWidth))
    //        {
    //            GL.Vertex(pos);
    //        }

    //        GL.End();
    //        GL.PopMatrix();
    //    }
    //}
}

//public class QuadSelect
//{
//    /// <summary>
//    /// 选框绘制材质
//    /// </summary>
//    private Material quadMaterial;

//    /// <summary>
//    /// 绘制边框所使用的材质
//    /// </summary>
//    public Material Material
//    {
//        get
//        {
//            if (!this.quadMaterial)
//            {
//                this.quadMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
//                this.quadMaterial.hideFlags = HideFlags.HideAndDontSave;
//                // Turn on alpha blending
//                this.quadMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
//                this.quadMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//                // Turn backface culling off
//                this.quadMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
//                // Turn off depth writes
//                this.quadMaterial.SetInt("_ZWrite", 0);
//            }
//            return this.quadMaterial;
//        }
//    }
//    /// <summary>
//    /// 是否绘制
//    /// </summary>
//    public bool IsDraw { get; set; } = false;
//    public Vector2 StartPos { get; set; } = Vector2.zero;
//    public Vector2 EndPos { get; set; } = Vector2.zero;

//    /// <summary>
//    /// 获取矩形
//    /// </summary>
//    /// <param name="pos1">位置1</param>
//    /// <param name="pos2">位置2</param>
//    /// <returns></returns>
//    public static Rect GetQuad(Vector2 pos1, Vector2 pos2)
//    {
//        return Rect.MinMaxRect(Mathf.Min(pos1.x, pos2.x), Mathf.Min(pos1.y, pos2.y), Mathf.Max(pos1.x, pos2.x), Mathf.Max(pos1.y, pos2.y));
//    }
//    /// <summary>
//    /// 获取绘制矩形边框所需要的顶点
//    /// </summary>
//    /// <param name="pos1"></param>
//    /// <param name="pos2"></param>
//    /// <param name="width">宽度</param>
//    /// <returns></returns>
//    public static Vector2[] GetQuadFrameVertex(Vector2 pos1, Vector2 pos2, float width)
//    {
//        var outRect = GetQuad(pos1, pos2);
//        width = Mathf.Max(Mathf.Min(width, outRect.width, outRect.height), 1);

//        var outVertex = new Vector2[4]
//        {
//            outRect.position,
//            new Vector2(outRect.xMin, outRect.yMax),
//            outRect.position + outRect.size,
//            new Vector2(outRect.xMax, outRect.yMin),
//        };
//        var inVertex = new Vector2[4]
//        {
//            outVertex[0] + new Vector2(width, width),
//            outVertex[1] + new Vector2(width, -width),
//            outVertex[2] + new Vector2(-width, -width),
//            outVertex[3] + new Vector2(-width, width)
//        };

//        var result = new Vector2[16]
//        {
//            outVertex[0],outVertex[1],inVertex[1],inVertex[0],
//            outVertex[1],outVertex[2],inVertex[2],inVertex[1],
//            outVertex[2],outVertex[3],inVertex[3],inVertex[2],
//            outVertex[3],outVertex[0],inVertex[0],inVertex[3],
//        };

//        return result;
//    }
//    /// <summary>
//    /// 检测投射范围内的对象
//    /// </summary>
//    /// <param name="pos1"></param>
//    /// <param name="pos2"></param>
//    public List<ISelectableObject> Check(Vector2 pos1, Vector2 pos2)
//    {
//        // 从相机发射两点到地面，构成一个盒子检测
//        // 使用BoxCast检测所有碰撞到的物体，盒子垂直于地面
//        // 用BoxCast做的
//        var camera = Camera.main;
//        var centerView = (pos1 + pos2) * 0.5f;
//        var centerRay = camera.ScreenPointToRay(centerView);
//        var leftRay = camera.ScreenPointToRay(new Vector3(pos1.x, centerView.y, 0));
//        var forwardRay = camera.ScreenPointToRay(new Vector3(centerView.x, pos1.y, 0));

//        RaycastHit centerHit, leftHit, forwardHit;
//        if (!(Physics.Raycast(centerRay, out centerHit, float.MaxValue, 1 << LayerMask.NameToLayer("Ground")) &&
//            Physics.Raycast(leftRay, out leftHit, float.MaxValue, 1 << LayerMask.NameToLayer("Ground")) &&
//            Physics.Raycast(forwardRay, out forwardHit, float.MaxValue, 1 << LayerMask.NameToLayer("Ground"))))
//            return null;

//        var forward = forwardHit.point - centerHit.point;
//        var extents = new Vector3((leftHit.point - centerHit.point).magnitude, 0f, (forward).magnitude);
//        // 关于盒子投射： 延一个方向投射一个盒子，
//        // position 和 halfExtents决定了盒子的起始位置和大小。
//        // distance 决定了投射的方向  orientation 决定了盒子的旋转
//        var hitResult = Physics.BoxCastAll(centerHit.point, extents, Vector3.up, forward.normalized == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(forward.normalized));

//        var result = new List<ISelectableObject>();
//        foreach (var item in hitResult)
//        {
//            var obj = item.transform.root.GetComponentInChildren<ISelectableObject>();
//            if (obj == null) continue;
//            result.Add(obj);
//        }
//        return result;
//    }
//}

/// <summary>
/// 可选择的物体
/// </summary>
public interface ISelectableObject
{
    Transform Transform { get; }
    /// <summary>
    /// 是否被选择到
    /// </summary>
    bool IsSelected { get; set; }
    /// <summary>
    /// 设置是否被选择到
    /// </summary>
    void Selected(bool enable);
    /// <summary>
    /// 预览选择
    /// </summary>
    void PreviewSelect(bool enable);
}