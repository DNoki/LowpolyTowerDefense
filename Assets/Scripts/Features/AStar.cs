using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aスター
/// </summary>
public class AStar
{
    /// <summary>
    /// 横纵移动成本
    /// </summary>
    public const float COST = 1f;
    /// <summary>
    /// 斜向移动成本
    /// </summary>
    public const float OBLIQUE_COST = 1.41421356f;
    /// <summary>
    /// 检索步数上限
    /// </summary>
    public const int SEARCHES_LIMIT = 10000;

    /// <summary>
    /// 节点
    /// </summary>
    public class Node
    {
        /// <summary>
        /// 节点位置
        /// </summary>
        public Vector2Int Position { get; set; } = Vector2Int.zero;
        /// <summary>
        /// 开始点到当前节点的移动代价
        /// </summary>
        public float G { get; set; } = 0;
        /// <summary>
        /// 当前节点到结束点的预估移动代价
        /// </summary>
        public float H { get; set; } = 0;
        /// <summary>
        /// 节点的总移动代价
        /// </summary>
        public float F => this.G + this.H;
        /// <summary>
        /// 成本代价因子(森林，海洋等等不同环境的不同值)
        /// </summary>
        public float CostMultiplier { get; set; } = 1;
        //public NodeState State { get; set; } = NodeState.OPEN;
        public Node FatherNode { get; set; } = null;
        public GroundBlock Block => GameScene.Instance.Ground.GetBlock(this.Position);

        public Node(Vector2Int pos, float g, float h, Node father, float costMultiplier = 1)
        {
            this.Position = pos;
            this.G = g;
            this.H = h;
            this.FatherNode = father;
            this.CostMultiplier = costMultiplier;
        }
    }
    /// <summary>
    /// 周围点
    /// </summary>
    private struct AroundPoint
    {
        public Vector2Int Point { get; set; }
        public DirectionType Direction { get; set; }

        public AroundPoint(Vector2Int point, DirectionType direction)
        {
            this.Point = point;
            this.Direction = direction;
        }
    }
    /// <summary>
    /// 八个方向类型
    /// </summary>
    [Flags]
    private enum DirectionType : int
    {
        NONE = 0,
        UP = 1,
        DOWN = 2,
        LEFT = 4,
        RIGHT = 8,
        LEFT_UP = 16,
        LEFT_DOWN = 32,
        RIGHT_UP = 64,
        RIGHT_DOWN = 128,
    }


    /// <summary>
    /// 按序插入节点到开放列表
    /// </summary>
    /// <param name="openList">开放列表，按照F值大小排序，F值越小越靠前</param>
    /// <param name="openDic">开放散列表，用于快速查找开放节点</param>
    /// <param name="node">要插入的节点</param>
    private static void InsertOpenNode(List<Node> openList, Dictionary<Vector2Int, Node> openDic, Node node)
    {
        openDic.Add(node.Position, node);
        if (openList.Count == 0)
        {
            openList.Add(node);
            return;
        }

        var f = node.F;
        var range = Vector2Int.zero;
        if (f < openList[(openList.Count - 1) / 2].F)
            range = new Vector2Int(0, (openList.Count - 1) / 2);
        else range = new Vector2Int((openList.Count - 1) / 2, openList.Count - 1);

        // 思路：折半插入排序法，从开放列表的中间位置判断，如果F值比这个数小，则再从四分之一的位置处才判断，若还是小，则从八分之一处判断，像这样每次折中进行判断
        while (true)
        {
            if (range.y - range.x < 10)
            {
                for (var i = range.x; i <= range.y; i++)
                    if (f <= openList[i].F)
                    {
                        openList.Insert(i, node);
                        return;
                    }
                openList.Insert(range.y + 1, node);
                return;
            }
            var half = range.y - ((range.y - range.x) / 2);
            if (f < openList[half].F)
                range.y = half;
            else range.x = half;
        }
    }
    /// <summary>
    /// 移除开放节点
    /// </summary>
    /// <param name="openList">开放列表，按照F值大小排序，F值越小越靠前</param>
    /// <param name="openDic">开放散列表，用于快速查找开放节点</param>
    /// <param name="node">要移除的节点</param>
    private static void RemoveOpenNode(List<Node> openList, Dictionary<Vector2Int, Node> openDic, Node node)
    {
        openDic.Remove(node.Position);
        openList.Remove(node);
    }

    /// <summary>
    /// 计算H值，允许上下左右以及斜方向移动
    /// </summary>
    /// <param name="currentPos"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public static float GetH(Vector2Int currentPos, Vector2Int targetPos)
    {
        var dx = Mathf.Abs(targetPos.x - currentPos.x);
        var dy = Mathf.Abs(targetPos.y - currentPos.y);
        var hv = Mathf.Abs(dx - dy);// 横向或纵向移动量
        var oblique = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) - hv;// 斜方向移动量
        var result = (hv * COST) + (oblique * OBLIQUE_COST);
        return result;
    }
    /// <summary>
    /// 获取某一点的周围八个点
    /// </summary>
    /// <param name="point"></param>
    /// <param name="cellSize">单元格大小</param>
    /// <returns></returns>
    private static AroundPoint[] GetAroundPoints(Vector2Int point, int cellSize)
    {
        var result = new AroundPoint[8];
        result[0] = new AroundPoint(point + Vector2Int.up * cellSize, DirectionType.UP);
        result[1] = new AroundPoint(point + Vector2Int.down * cellSize, DirectionType.DOWN);
        result[2] = new AroundPoint(point + Vector2Int.left * cellSize, DirectionType.LEFT);
        result[3] = new AroundPoint(point + Vector2Int.right * cellSize, DirectionType.RIGHT);
        result[4] = new AroundPoint(point + (Vector2Int.left + Vector2Int.up) * cellSize, DirectionType.LEFT_UP);
        result[5] = new AroundPoint(point + (Vector2Int.left + Vector2Int.down) * cellSize, DirectionType.LEFT_DOWN);
        result[6] = new AroundPoint(point + (Vector2Int.right + Vector2Int.up) * cellSize, DirectionType.RIGHT_UP);
        result[7] = new AroundPoint(point + (Vector2Int.right + Vector2Int.down) * cellSize, DirectionType.RIGHT_DOWN);
        return result;
    }
    /// <summary>
    /// 添加周围节点并返回F值最小的节点为下一个节点
    /// </summary>
    /// <param name="openList"></param>
    /// <param name="targetNode"></param>
    /// <param name="currentNode"></param>
    /// <param name="cellSize"></param>
    /// <returns></returns>
    private static Node GetNextNode(List<Node> openList, Dictionary<Vector2Int, Node> openDic, Dictionary<Vector2Int, Node> closeDic, Node startNode, Node targetNode, Node currentNode, int cellSize)
    {
        var points = GetAroundPoints(currentNode.Position, cellSize);
        var ignoreDirection = DirectionType.NONE;// 忽略方向
        for (var i = 0; i < 8; i++)
        {
            if (closeDic.ContainsKey(points[i].Point)) continue;// 忽略已在闭合列表中的节点
            if (ignoreDirection.HasFlag(points[i].Direction)) continue;// 忽略方向
            var block = GameScene.Instance.Ground.GetBlock(points[i].Point);

            if (block.State == GroundStateType.OBSTRUCT && !(startNode.Block.State == GroundStateType.OBSTRUCT && block.Owner == startNode.Block.Owner))
            {
                if (block.Owner != targetNode.Block.Owner)
                {
                    // 当横纵方向上不可移动时，同时忽略掉该方向上的角块
                    if (points[i].Direction == DirectionType.UP) ignoreDirection = DirectionType.UP | DirectionType.LEFT_UP | DirectionType.RIGHT_UP;
                    else if (points[i].Direction == DirectionType.DOWN) ignoreDirection = DirectionType.DOWN | DirectionType.LEFT_DOWN | DirectionType.RIGHT_DOWN;
                    else if (points[i].Direction == DirectionType.LEFT) ignoreDirection = DirectionType.LEFT | DirectionType.LEFT_UP | DirectionType.LEFT_DOWN;
                    else if (points[i].Direction == DirectionType.RIGHT) ignoreDirection = DirectionType.RIGHT | DirectionType.RIGHT_UP | DirectionType.RIGHT_DOWN;
                    continue;
                }
            }

            var g = currentNode.G;
            if ((points[i].Direction & (DirectionType.UP | DirectionType.DOWN | DirectionType.LEFT | DirectionType.RIGHT)) != 0)
                g += COST;
            else g += OBLIQUE_COST;
            var h = GetH(points[i].Point, targetNode.Position);
            var node = new Node(points[i].Point, g, h, currentNode);

            if (openDic.ContainsKey(points[i].Point))// 检查是否已存在与开放列表中
            {
                if (node.F < openDic[points[i].Point].F)// 判断是否经由当前点的F值更小，若是，则替换节点，否则什么也不做
                    openDic[points[i].Point] = node;
            }
            else InsertOpenNode(openList, openDic, node);
        }
        if (openList.Count > 0) return openList[0];
        else return null;
    }

    public static Stack<Vector2Int> DoAStar(Vector2Int startPos, Vector2Int targetPos)
    {
        // 开放散列表，用于快速查找开放节点
        var openDic = new Dictionary<Vector2Int, Node>();
        // 闭合散列表，用于存放闭合节点
        var closeDic = new Dictionary<Vector2Int, Node>();
        // 开放列表，按照F值大小排序，F值越小越靠前
        var openList = new List<Node>();

        var distance = (targetPos - startPos).magnitude;// 起始点到目标点的距离
        var cellSize = 1;

        var startNode = new Node(startPos, 0f, GetH(startPos, targetPos), null);
        var targetNode = new Node(targetPos, 0f, 0f, null);

        InsertOpenNode(openList, openDic, startNode);
        var currentNode = startNode;

        for (var i = 0; i < SEARCHES_LIMIT; i++)
        {
            if (currentNode == null || i == SEARCHES_LIMIT - 1)
            {
                // 查找失败，开放列表为空，或超过允许查询的最大数量，此时没有路径
                return null;
            }
            // 将当前节点从开放列表移除并放入闭合列表
            closeDic.Add(currentNode.Position, currentNode);
            RemoveOpenNode(openList, openDic, currentNode);
            if (currentNode.Position == targetPos) break;
            currentNode = GetNextNode(openList, openDic, closeDic, startNode, targetNode, currentNode, cellSize);
        }

        var result = new Stack<Vector2Int>();
        result.Push(currentNode.Position);
        while (currentNode.Position != startPos)
        {
            result.Push(currentNode.FatherNode.Position);
            currentNode = currentNode.FatherNode;
        }
        return result;
    }
    public static void DoAStar(Vector2Int startPos, Vector2Int targetPos, out Stack<Vector2Int> result) => result = DoAStar(startPos, targetPos);

}
