using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public interface IConfig
    {
        ScriptableObject ThisSO { get; }
        string ID { get; }
        string DisplayName { get; }
        bool WorkDown { get; }
    }
}