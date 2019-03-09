using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 对象池
/// オブジェクトプール
/// </summary>
/// <typeparam name="TObj"></typeparam>
public abstract class ObjectPool<TObj> where TObj : class, IPoolObject
{
    /// <summary>
    /// 用来存放闲置对象的队列
    /// </summary>
    private Queue<TObj> queue = new Queue<TObj>();
    /// <summary>
    /// 所有从池中创建的对象都存放在这里
    /// </summary>
    private List<TObj> objList = new List<TObj>();

    /// <summary>
    /// 闲置对象总数
    /// </summary>
    public int IdleCount => this.queue.Count;
    /// <summary>
    /// 激活对象总数
    /// </summary>
    public int ActivationCount => this.Count - this.IdleCount;
    /// <summary>
    /// 对象池一共创建了多少对象
    /// </summary>
    public int Count => this.objList.Count;
    /// <summary>
    /// 对象池允许创建的对象最大数量
    /// </summary>
    public int MaxQuantity { get; set; }
    /// <summary>
    /// 允许溢出创建。是否允许当池拥有了最大数量对象的情况下，继续创建新的对象
    /// </summary>
    public bool IsCanOverflow { get; set; } = false;

    /// <summary>
    /// 从对象池中提取一个对象，若无闲置对象则生成一个新的对象
    /// </summary>
    /// <returns></returns>
    public TObj Take()
    {
        TObj result;
        if (this.queue.Count > 0) result = this.queue.Dequeue();
        else
        {
            if (this.Count >= this.MaxQuantity) // 对象池已创建了所允许的最大数量的对象
            {
                if (this.IsCanOverflow) // 生成一个新的对象（溢出对象），但这个对象不由该对象池管理。
                {
                    var overflowObj = CreateObject();
                    overflowObj.Initialize();
                    return overflowObj;
                }
                else return null;
            }
            result = CreateObject();
            this.objList.Add(result);
        }
        result.Initialize();
        return result;
    }
    /// <summary>
    /// 释放对象，将其放入对象池
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Release(TObj obj)
    {
        obj.Release();
        if (!this.objList.Contains(obj))
        {
            DestroyObject(obj);
            return false;
        }
        this.queue.Enqueue(obj);
        return true;
    }
    private IEnumerator Release(TObj obj, float time)
    {
        yield return new WaitForSeconds(time);
        Release(obj);
    }
    /// <summary>
    /// 经过指定时间后释放对象
    /// </summary>
    /// <param name="mono">处理协程对象</param>
    /// <param name="obj">释放对象</param>
    /// <param name="time">等待时间（秒）</param>
    public void Release(MonoBehaviour mono, TObj obj, float time) => mono.StartCoroutine(Release(obj, time));

    /// <summary>
    /// 释放，并允许放入溢出对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool ReleasePut(TObj obj)
    {
        obj.Release();
        if (!this.objList.Contains(obj))
        {
            if (this.Count >= this.MaxQuantity)
            {
                DestroyObject(obj); // 销毁溢出对象
                return false;
            }
            else this.objList.Add(obj);
        }
        this.queue.Enqueue(obj);
        return true;
    }

    /// <summary>
    /// 释放闲置对象
    /// </summary>
    public virtual void ClearIdleObj()
    {
        foreach (var obj in this.queue)
        {
            this.objList.Remove(obj);
            obj.Release();
            DestroyObject(obj);
        }
        this.queue.Clear();
    }
    /// <summary>
    /// 释放所有对象
    /// </summary>
    public virtual void ClearAllObj()
    {
        //foreach (var obj in this.objList)
        //{
        //    obj.Release();
        //    DestroyObject(obj);
        //}
        this.queue.Clear();
        this.objList.Clear();
    }

    /// <summary>
    /// 创建一个对象
    /// </summary>
    /// <returns></returns>
    protected abstract TObj CreateObject();
    /// <summary>
    /// 销毁一个对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    protected abstract void DestroyObject(TObj obj);

    public ObjectPool(bool isCanOverflow, int maxQuantity)
    {
        this.IsCanOverflow = isCanOverflow;
        this.MaxQuantity = maxQuantity;
    }
}

/// <summary>
/// 池管理的对象
/// プールで管理しているオブジェクト
/// </summary>
public interface IPoolObject
{
    /// <summary>
    /// 指示对象是否被释放（因在初始化设置否，在释放中设置是）
    /// </summary>
    bool IsReleased { get; }
    /// <summary>
    /// 初始化（在Take中调用）
    /// </summary>
    /// <returns></returns>
    void Initialize();
    /// <summary>
    /// 释放（在Release中调用）
    /// </summary>
    void Release();
}
