using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public enum BetaMode
    {
        [LabelText("禁用")]
        Develop,
        [LabelText("测试中")]
        Beta,
        [LabelText("已实装")]
        Done,
    }
    public interface IConfig
    {
        ScriptableObject ThisSO { get; }
        string ID { get; }
        string DisplayName { get; }
        BetaMode BetaM { get; }
    }
}