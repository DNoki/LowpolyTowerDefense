using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 创建平面网格
/// </summary>
public class ProceduralPlaneMesh
{
    /// <summary>
    /// 网格顶点
    /// </summary>
    private struct Vertex
    {
        public int ID { get; set; }
        public Vector3 Position { get; set; }
        public Vector2 UV { get; set; }
        //public Vector3 Normal { get; set; }
        //public Vector4 Tangent { get; set; }
        //public Color Color { get; set; }

        /// <summary>
        /// 设置顶点
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Vertex SetID(int value)
        {
            this.ID = value;
            return this;
        }
        public Vertex(int id, Vector3 pos, Vector2 uv)
        {
            this.ID = id;
            this.Position = pos;
            this.UV = uv;
        }

        public override string ToString() => string.Format("{0}: {1}", this.ID, this.Position);
        //public override bool Equals(object obj)
        //{
        //    if (obj is Vertex)
        //    {
        //        var v = (Vertex)obj;
        //        return this == v;
        //    }
        //    else return false;
        //}
        //public override int GetHashCode() => base.GetHashCode();
        //public static bool operator ==(Vertex lhs, Vertex rhs)
        //{
        //    return lhs.Position == rhs.Position;
        //}
        //public static bool operator !=(Vertex lhs, Vertex rhs)
        //{
        //    return lhs.Position != rhs.Position;
        //}
    }
    /// <summary>
    /// 四边形网格单元
    /// </summary>
    private class MeshQuad : IEnumerable<Vertex>
    {
        public Vertex Vertex1 { get; set; }
        public Vertex Vertex2 { get; set; }
        public Vertex Vertex3 { get; set; }
        public Vertex Vertex4 { get; set; }

        public int Count => 4;
        public Vector3[] Vertices => new Vector3[4] { this.Vertex1.Position, this.Vertex2.Position, this.Vertex3.Position, this.Vertex4.Position };
        public Vector2[] UVs => new Vector2[4] { this.Vertex1.UV, this.Vertex2.UV, this.Vertex3.UV, this.Vertex4.UV };
        /// <summary>
        /// 绘制顺序
        /// </summary>
        public int[] DrawOrder => new int[6] { this.Vertex1.ID, this.Vertex2.ID, this.Vertex3.ID, this.Vertex3.ID, this.Vertex2.ID, this.Vertex4.ID };

        public Vertex this[int n]
        {
            get
            {
                switch (n)
                {
                    case 0: return this.Vertex1;
                    case 1: return this.Vertex2;
                    case 2: return this.Vertex3;
                    case 3: return this.Vertex4;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (n)
                {
                    case 0: this.Vertex1 = value; break;
                    case 1: this.Vertex2 = value; break;
                    case 2: this.Vertex3 = value; break;
                    case 3: this.Vertex4 = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<Vertex> GetEnumerator()
        {
            yield return this.Vertex1;
            yield return this.Vertex2;
            yield return this.Vertex3;
            yield return this.Vertex4;
        }

        public override string ToString() => string.Format("{0}|{1}|{2}|{3}", this.Vertex1, this.Vertex2, this.Vertex3, this.Vertex4);
    }


    /// <summary>
    /// 生成平面网格数据
    /// </summary>
    /// <param name="rect">形状范围</param>
    /// <param name="blocks">要绘制的块</param>
    /// <param name="cellSize">单元格大小</param>
    /// <param name="gridOffset">位置偏移</param>
    /// <param name="vertices">输出顶点表</param>
    /// <param name="uv">输出uv表</param>
    /// <param name="triangles">输出绘制顺序</param>
    public static void MakePlaneMesh(RectInt rect, IList<Vector2Int> blocks, float cellSize, Vector3 gridOffset, out List<Vector3> vertices, out List<Vector2> uv, out List<int> triangles)
    {
        var quads = new List<MeshQuad>();
        var vertexDic = new Dictionary<Vector3, Vertex>();
        var id = 0;

        for (var y = 0; y <= rect.height; y++)
            for (var x = 0; x <= rect.width; x++)
            {
                var xy = new Vector2Int(rect.x + x, rect.y + y);
                if (!blocks.Contains(xy)) continue;
                var quad = new MeshQuad();
                quad.Vertex1 = new Vertex(0, new Vector3(xy.x, 0, xy.y) * cellSize + gridOffset, new Vector2((float)x / rect.width, (float)y / rect.height));
                quad.Vertex2 = new Vertex(0, new Vector3(xy.x, 0, xy.y + 1) * cellSize + gridOffset, new Vector2((float)x / rect.width, (float)(y + 1) / rect.height));
                quad.Vertex3 = new Vertex(0, new Vector3(xy.x + 1, 0, xy.y) * cellSize + gridOffset, new Vector2((float)(x + 1) / rect.width, (float)y / rect.height));
                quad.Vertex4 = new Vertex(0, new Vector3(xy.x + 1, 0, xy.y + 1) * cellSize + gridOffset, new Vector2((float)(x + 1) / rect.width, (float)(y + 1) / rect.height));
                quads.Add(quad);

                for (var i = 0; i < quad.Count; i++)
                {
                    //if (vertexList.FindIndex((v) => v.Position == quad[i].Position) == -1)
                    if (vertexDic.ContainsKey(quad[i].Position))// 使用散列表加快查找速度
                        quad[i] = quad[i].SetID(vertexDic[quad[i].Position].ID);
                    else
                    {
                        // 若未找到相同位置的顶点则将该点添加到列表
                        quad[i] = quad[i].SetID(id++);
                        vertexDic.Add(quad[i].Position, quad[i]);
                    }
                }
            }

        triangles = new List<int>();
        vertices = new List<Vector3>();
        uv = new List<Vector2>();

        foreach (var vertex in vertexDic)
        {
            vertices.Add(vertex.Value.Position);
            uv.Add(vertex.Value.UV);
        }
        foreach (var quad in quads)
            triangles.AddRange(quad.DrawOrder);
    }
}
