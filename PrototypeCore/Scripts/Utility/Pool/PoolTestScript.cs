using Prototype;
using UnityEngine;

public class PoolTestScript : MonoBehaviour, IPoolable
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
