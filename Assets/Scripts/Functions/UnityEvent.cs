using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnityEventT1floatT2float : UnityEvent<float, float> { }

[Serializable]
public class UnityEventT1int : UnityEvent<int> { }

[Serializable]
public class UnityEventT1intT2int : UnityEvent<int, int> { }