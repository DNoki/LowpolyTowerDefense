using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人移动路径点
/// </summary>
public class Route : MonoBehaviour
{
    [SerializeField] List<Transform> routeList = null;
    /// <summary>
    /// 要注册的地图区域
    /// </summary>
    [SerializeField] Vector4[] registGroundBlock = null;

    /// <summary>
    /// 敌人移动路径点列表
    /// </summary>
    public List<Transform> RouteList => this.routeList;


    private void Awake()
    {
        // 注册道路区域
        foreach (var area in this.registGroundBlock)
        {
            GameScene.Instance.Ground.RegistRoadGrounds(area);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (this.registGroundBlock == null || this.registGroundBlock.Length <= 0)
            return;

        Gizmos.color = Color.red;
        foreach (var area in this.registGroundBlock)
        {
            for (int y = 0; y < area.w; y++)
            {
                for (int x = 0; x < area.z; x++)
                {
                    var rect = GroundBlock.MapPositionToRect(new Vector2Int((int)area.x + x, (int)area.y + y));
                    Gizmos.DrawLine(new Vector3(rect.xMin, 0f, rect.yMin), new Vector3(rect.xMax, 0f, rect.yMax));
                    Gizmos.DrawLine(new Vector3(rect.xMin, 0f, rect.yMax), new Vector3(rect.xMax, 0f, rect.yMin));
                }
            }

        }
    }
#endif
}
