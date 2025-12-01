using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    /// <summary>
    /// 面向预制体的资源池管理器
    /// 
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        // 字典映射：Prefab的InstanceID -> 具体的对象池
        private Dictionary<int, IObjectPool> _pools = new Dictionary<int, IObjectPool>();

        // 根节点，用于收纳不用的物体，保持Hierarchy整洁
        private Transform _poolRoot;

        protected PoolManager()
        {
            _poolRoot = new GameObject("---PoolRoot---").transform;
            _poolRoot.gameObject.AddComponent<DontDestroyOnLoad>();
        }

        /// <summary>
        /// 初始化某个Prefab的池子（预加载）
        /// </summary>
        public void Preload<T>(T prefab, int size) where T : Component
        {
            int key = prefab.GetInstanceID();
            if (!_pools.ContainsKey(key))
            {
                // 为每个类型的池子创建一个子节点
                GameObject subGroup = new GameObject($"Pool_{prefab.name}");
                subGroup.transform.SetParent(_poolRoot);

                var pool = new ObjectPool<T>(prefab, size, subGroup.transform);
                _pools.Add(key, pool);
            }
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return Spawn(prefab.transform, position, rotation, parent).gameObject;
        }

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            int key = prefab.GetInstanceID();

            // 如果没有预加载过，自动懒加载创建一个小池子
            if (!_pools.ContainsKey(key))
            {
                Preload(prefab, 10); // 默认初始大小
            }

            ObjectPool<T> pool = (ObjectPool<T>)_pools[key];
            T instance = pool.Spawn();

            // 设置基本变换
            instance.transform.SetPositionAndRotation(position, rotation);
            if (parent != null) instance.transform.SetParent(parent);

            return instance;
        }

        public void Despawn(GameObject instanceGo)
        {
            Despawn(instanceGo.transform);
        }

        public void Despawn(Component component)
        {
            if (!component) return;

            var poolObj = component.GetComponent<PoolObject>();
            if (poolObj != null)
            {
                poolObj.MyPool.Despawn(component);
            }
            else
            {  //兼容性设计：不是池子里的东西直接Destroy
                Object.Destroy(component.gameObject);
            }
        }
        public void Despawn(GameObject instanceGo, float delay)
        {
            Despawn(instanceGo.transform, delay);
        }
        public void Despawn(Component component, float delay)
        {
            ActionKit.Delay(delay, ()=> { Despawn(component); }).StartCurrentScene();
        }
    }
}