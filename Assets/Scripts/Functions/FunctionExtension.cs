using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 扩展功能
/// </summary>
public static class FunctionExtension
{
#pragma warning disable
    /// <summary>
    /// 该范围是否包含了指定点
    /// </summary>
    /// <param name="origi"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static bool IsInclude(this RectInt origi, Vector2Int pos)
    {
        if ((origi.xMin <= pos.x) && (pos.x < origi.xMax) && (origi.yMin <= pos.y) && (pos.y < origi.yMax))
            return true;
        else return false;
    }

    /// <summary>
    /// 返回 x, y之间的一个随机值
    /// </summary>
    /// <param name="origi"></param>
    /// <returns></returns>
    public static float RandomNumber(this Vector2 origi) => UnityEngine.Random.Range(Mathf.Min(origi.x, origi.y), Mathf.Max(origi.x, origi.y));

    /// <summary>
    /// 设置值并返回
    /// </summary>
    /// <param name="origi"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Vector3 SetValue(this Vector3 origi, float? x = null, float? y = null, float? z = null)
    {
        if (!x.HasValue) x = origi.x;
        if (!y.HasValue) y = origi.y;
        if (!z.HasValue) z = origi.z;
        return new Vector3(x.Value, y.Value, z.Value);
    }
    public static Vector2 xy(this Vector3 origi) => new Vector2(origi.x, origi.y);
    public static Vector2 xz(this Vector3 origi) => new Vector2(origi.x, origi.z);
    public static Vector2 yz(this Vector3 origi) => new Vector2(origi.y, origi.z);

    public static float ToFloat(this string origi) => Convert.ToSingle(origi);
    public static int ToInt32(this string origi) => Convert.ToInt32(origi);
    public static byte ToByte(this string origi) => Convert.ToByte(origi);

    public static bool Equal(this Resolution origi, Resolution contrast) => origi.width == contrast.width && origi.height == contrast.height;


    /// <summary>
    /// 重新映射
    /// </summary>
    /// <param name="value"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static float Remap(float value, Vector4 data) => Remap(value, data.x, data.y, data.z, data.w);
    /// <summary>
    /// 重新映射
    /// </summary>
    /// <param name="value"></param>
    /// <param name="inMin"></param>
    /// <param name="inMax"></param>
    /// <param name="outMin"></param>
    /// <param name="outMax"></param>
    /// <returns></returns>
    public static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
    {
        value = Mathf.Clamp(value, inMin, inMax);
        var result = ((value - inMin) / (inMax - inMin)) * (outMax - outMin) + outMin;
        return result;
    }

    public static List<Dictionary<string, string>> CSVReader(string text)
    {
        var lineArray = text.Split(Environment.NewLine.ToCharArray());
        List<string> columnNames = null;
        var dicList = new List<Dictionary<string, string>>();
        for (var i = 0; i < lineArray.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lineArray[i])) continue;
            var items = lineArray[i].Split(',');
            if (columnNames == null)
            {
                columnNames = new List<string>();
                foreach (var name in items)
                {
                    columnNames.Add(name);
                }
                continue;
            }
            var dic = new Dictionary<string, string>();
            for (var j = 0; j < columnNames.Count; j++)
            {
                dic.Add(columnNames[j], items[j]);
            }
            dicList.Add(dic);
        }

        return dicList;
    }
}
