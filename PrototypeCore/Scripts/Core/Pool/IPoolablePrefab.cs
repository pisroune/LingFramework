using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    /// <summary>
    /// 池对象接口
    /// </summary>
    public interface IPoolablePrefab
    {
        // 当对象从池中取出时调用
        void OnSpawn();

        // 当对象回收到池中时调用
        void OnDespawn();
    }
}