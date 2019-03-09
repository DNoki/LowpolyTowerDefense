using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 有限状态机
/// 状態マシン
/// </summary>
/// <typeparam name="TOwnerType">持有者类型</typeparam>
public class FiniteStateMachine<TOwnerType>
{
    /// <summary>
    /// 对象
    /// </summary>
    public TOwnerType Owner { get; private set; }

    /// <summary>
    /// 当前状态
    /// </summary>
    public StateBase<TOwnerType> CurrentState { get; set; }
    /// <summary>
    /// 状态列表
    /// </summary>
    public Dictionary<string, StateBase<TOwnerType>> StateArray { get; private set; }

    /// <summary>
    /// 获取状态
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public StateBase<TOwnerType> GetState<TState>() where TState : StateBase<TOwnerType>, new()
    {
        var name = typeof(TState).Name;
        TState nextState = null;
        if (this.StateArray.ContainsKey(name))
            nextState = this.StateArray[name] as TState;
        else
        {
            nextState = new TState().SetMachine(this) as TState;
            this.StateArray.Add(nextState.Name, nextState);
        }
        return nextState;
    }

    /// <summary>
    /// 执行状态方法
    /// </summary>
    public void Execute()
    {
        this.CurrentState.CheckTurn();
        this.CurrentState.UpdateState();
    }

    /// <summary>
    /// 有限状态机
    /// </summary>
    /// <param name="owner">对象</param>
    /// <param name="initial">初始状态</param>
    public FiniteStateMachine(TOwnerType owner, StateBase<TOwnerType> initial)
    {
        this.Owner = owner;
        this.CurrentState = initial.SetMachine(this);
        this.StateArray = new Dictionary<string, StateBase<TOwnerType>>();
        this.StateArray.Add(this.CurrentState.Name, this.CurrentState);
    }
}

/// <summary>
/// 状态抽象类（有限状态机）
/// </summary>
/// <typeparam name="TOwnerType"></typeparam>
public abstract class StateBase<TOwnerType>
{
    /// <summary>
    /// 控制该状态的状态机
    /// </summary>
    protected FiniteStateMachine<TOwnerType> Machine { get; private set; }
    /// <summary>
    /// 对象
    /// </summary>
    protected TOwnerType Owner => this.Machine.Owner;
    /// <summary>
    /// 状态标识
    /// </summary>
    public string Name => this.GetType().Name;

    /// <summary>
    /// 改变状态，并可以传递需要做收尾或初始化需要的数据（若无需参数的 将其指定为null）
    /// </summary>
    /// <typeparam name="TState">要改变的状态类型</typeparam>
    /// <typeparam name="TOnExitData">当前状态收尾需要的参数类型</typeparam>
    /// <typeparam name="TOnEnterData">下一个状态初始化需要的参数类型</typeparam>
    /// <param name="onExitData">当前状态收尾需要的参数</param>
    /// <param name="onEnterData">下一个状态初始化需要的参数</param>
    /// <returns>下一个状态</returns>
    public TState ChangeState<TState>(object onExitData = null, object onEnterData = null) where TState : StateBase<TOwnerType>, new()
    {
        // 执行当前状态的收尾
        OnExit(onExitData);

        var nextState = this.Machine.GetState<TState>() as TState;
        this.Machine.CurrentState = nextState;// 设置新状态

        // 执行下一个状态的初始化
        nextState.OnEnter(onEnterData);
        return nextState;
    }

    /// <summary>
    /// 设置状态机对象
    /// </summary>
    /// <param name="machine"></param>
    /// <returns></returns>
    public StateBase<TOwnerType> SetMachine(FiniteStateMachine<TOwnerType> machine)
    {
        this.Machine = machine;
        return this;
    }

    /// <summary>
    /// 检查并跳转
    /// </summary>
    public abstract void CheckTurn();
    /// <summary>
    /// 执行状态逻辑行为
    /// </summary>
    public abstract void UpdateState();
    /// <summary>
    /// 初始化工作
    /// </summary>
    /// <param name="data">需要用到的数据</param>
    protected virtual void OnEnter(object data = null) { }
    /// <summary>
    /// 收尾工作
    /// </summary>
    /// <param name="data">需要用到的数据</param>
    protected virtual void OnExit(object data = null) { }

    public StateBase() { }
}



//public class Example
//{
//    public int Value { get; set; } = 0;

//    public FiniteStateMachine<Example> Machine { get; set; }

//    public Example()
//    {
//        this.Machine = new FiniteStateMachine<Example>(this, new ExampleState1());
//    }
//}

//public class ExampleState1 : StateBase<Example>
//{
//    public override void CheckTurn()
//    {
//        if (true)// 在这里检查要跳转的条件
//        {
//            ChangeState<ExampleState2>();
//            return;
//        }
//    }
//    public override void UpdateState()
//    {
//        // 在这里执行该状态的逻辑行为
//    }
//}

//public class ExampleState2 : StateBase<Example>
//{
//    public override void CheckTurn()
//    {
//        if (this.Owner.Value == 10)// 在这里检查要跳转的条件            
//        {
//            ChangeState<ExampleState1>();
//            return;
//        }
//    }
//    public override void UpdateState()
//    {
//        // 在这里执行该状态的逻辑行为
//    }
//}
