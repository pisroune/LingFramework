using Prototype;
using UnityEngine;

namespace Prototype.Demo.PoolablePrefab
{
    public class PoolTestScript : MonoBehaviour, IPoolablePrefab
    {
        public void OnDespawn()
        {
            Debug.LogError("回收");
        }

        public void OnSpawn()
        {
            Debug.LogError("生成");
        }
    }
}