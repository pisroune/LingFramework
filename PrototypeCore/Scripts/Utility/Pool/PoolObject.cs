using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    /// <summary>
    /// 该脚本会被PoolManager自动挂载到池对象上
    /// 勿继承或手动挂载！
    /// </summary>
    public class PoolObject : MonoBehaviour
    {
        // 直接持有池子的接口引用，无需再次查字典
        public IObjectPool MyPool;

        public void Initialize(IObjectPool pool)
        {
            MyPool = pool;
        }
    }
}