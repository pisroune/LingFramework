using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Action<T>(T arg1);
public delegate void Action<T1, T2>(T1 arg1, T2 arg2);
public delegate void Action<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
public delegate void Action<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
public delegate bool Condition();
public delegate bool Condition<T>(T arg1);
public delegate bool Condition<T1, T2>(T1 arg1, T2 arg2);
public delegate bool Condition<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);