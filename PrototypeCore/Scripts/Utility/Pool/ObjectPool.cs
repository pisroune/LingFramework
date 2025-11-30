using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototype
{   
    public interface IObjectPool
    {
        void Despawn(Component component);
    }
    public class ObjectPool<T> : IObjectPool where T : Component
    {
        private readonly Stack<T> _pool;
        private readonly T _prefab;
        private readonly Transform _parent;

        // 性能监控：当前激活数量，池中数量
        public int ActiveCount { get; private set; }
        public int InactiveCount => _pool.Count;

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            _pool = new Stack<T>(initialSize); // 预分配容量，减少Resize开销

            for (int i = 0; i < initialSize; i++)
            {
                T instance = CreateNewInstance();
                _pool.Push(instance);
            }
        }

        private T CreateNewInstance()
        {
            T instance = Object.Instantiate(_prefab, _parent);
            var tracker = instance.gameObject.AddComponent<PoolObject>();
            tracker.Initialize(this);
            instance.gameObject.SetActive(false);
            return instance;
        }

        public T Spawn()
        {
            T instance;
            if (_pool.Count > 0)
            {
                instance = _pool.Pop();
            }
            else
            {  //自动扩容
                instance = CreateNewInstance();
            }

            instance.gameObject.SetActive(true);
            ActiveCount++;

            // 接口调用：通知对象它活了
            if (instance is IPoolable poolable)
            {
                poolable.OnSpawn();
            }

            return instance;
        }

        public void Despawn(Component component)
        {   
            if (component is T tInstance)
            {  //这里进行一次安全的类型转换
                Despawn(tInstance);
            }
            else
            {
                Debug.LogError($"Type mismatch: Trying to return {component.GetType()} to pool of {_prefab.GetType()}");
            }

           
        }
        void Despawn(T instance)
        {
#if UNITY_EDITOR
            if (_pool.Contains(instance))
            {
                Debug.LogWarning($"Trying to despawn {instance.name} which is already in pool!");
                return;
            }
#endif

            ActiveCount--;

            // 接口调用：通知对象它要睡了
            if (instance is IPoolable poolable)
            {
                poolable.OnDespawn();
            }

            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_parent); // 归位，保持层级整洁
            _pool.Push(instance);
        }
    }
}