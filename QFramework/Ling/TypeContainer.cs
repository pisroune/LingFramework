using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 带泛型约束的容器接口
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    public interface ITypeContainer<TBase> where TBase : class
    {
        void Register<T>(T module) where T : TBase;
        void Register<T>(T module, Type type) where T : TBase;  //手动传类型，防止T和TBase完全相同

        T Get<T>() where T : TBase;
        TBase Get(Type type);
        bool TryGet<T>(out T result) where T : TBase;
        bool TryGet(Type type, out TBase result);

        bool Has<T>();
        bool Has(Type type);
        void Clear();

        IEnumerable<TBase> GetAll();
    }

    /// <summary>
    /// 带泛型约束的容器实现
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    public class TypeContainer<TBase> : ITypeContainer<TBase> where TBase : class
    {
        // 核心优化：Value 的类型不再是 object，而是 TBase
        [ShowInInspector] public readonly Dictionary<Type, TBase> _instances = new Dictionary<Type, TBase>();
        // ================== 注册 (Register) ==================

        public void Register<T>(T module) where T : TBase
        {
            // 自动使用 T 的类型作为 Key
            Register(module, typeof(T));
        }

        public void Register<T>(T module, Type type) where T : TBase
        {
            // 1. 基础判空
            if (module == null) throw new ArgumentNullException(nameof(module));
            if (type == null) throw new ArgumentNullException(nameof(type));

            // 2. 关键检查：确保 Key (type) 也是 TBase 的子类
            // 如果容器是装 IEnemy 的，你不能注册一个 Key 叫 IAudioService
            if (!typeof(TBase).IsAssignableFrom(type))
            {
                throw new ArgumentException($"Registered Key Type '{type.Name}' must inherit from Base Type '{typeof(TBase).Name}'.");
            }

            // 3. 关键检查：确保 Value (module) 确实是 Key (type) 的实例
            // 防止出现：Register(new Goblin(), typeof(Dragon)); 这种情况
            if (!type.IsAssignableFrom(module.GetType()))
            {
                throw new ArgumentException($"Instance of type '{module.GetType().Name}' is not assignable to Key Type '{type.Name}'.");
            }

            // 4. 存入字典
            if (_instances.ContainsKey(type))
            {
                _instances[type] = module;
            }
            else
            {
                _instances.Add(type, module); // 这里的 module 本身就是 T，也是 TBase，无需强转
            }
            Debug.Log("成功添加:" + type);
        }

        // ================== 获取 (Get) ==================

        public T Get<T>() where T : TBase
        {
            // 调用下面的非泛型版本
            TBase instance = Get(typeof(T));

            // 这里需要一次强转，从 TBase 转回具体的 T (比如从 IEnemy 转回 Goblin)
            return (T)instance;
        }

        public TBase Get(Type type)
        {
            if (_instances.TryGetValue(type, out TBase instance))
            {
                return instance;
            }

            throw new KeyNotFoundException($"Type '{type.Name}' is not registered in this {typeof(TBase).Name} container.");
        }

        // ================== 尝试获取 (TryGet) ==================

        public bool TryGet<T>(out T result) where T : TBase
        {
            if (TryGet(typeof(T), out TBase baseResult))
            {
                result = (T)baseResult;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryGet(Type type, out TBase result)
        {
            return _instances.TryGetValue(type, out result);
        }

        // ================== 查询与清理 ==================

        public bool Has<T>()
        {
            return Has(typeof(T));
        }

        public bool Has(Type type)
        {
            return _instances.ContainsKey(type);
        }

        public void Clear()
        {
            _instances.Clear();
        }

        // ================== 额外建议功能 ==================

        // 既然是泛型容器，通常会有“获取所有”的需求（比如 Update 所有的 IUpdatable）
        // Dictionary<Type, TBase> 让这个操作变得非常方便且类型安全
        public IEnumerable<TBase> GetAll()
        {
            return _instances.Values;
        }
    }

    /// <summary>
    /// 基础的object容器接口
    /// </summary>
    public interface ITypeContainer
    {
        void Register<T>(T module);
        void Register(object module, Type type);

        T Get<T>() where T : class;
        object Get(Type type);
        bool TryGet<T>(out T result) where T : class;
        bool TryGet(Type type, out object result);

        bool Has<T>();
        bool Has(Type type);
        void Clear();
    }

    /// <summary>
    /// 基础的object容器实现
    /// </summary>
    public class TypeContainer : ITypeContainer
    {
        // 核心存储容器：Key是类型，Value是具体实例
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        #region 注册
        public void Register<T>(T module)
        {
            // 转发给非泛型版本处理，避免代码重复
            Register(module, typeof(T));
        }
        public void Register(object module, Type type)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module), "Cannot register a null instance.");
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), "Registration type cannot be null.");
            }

            // 安全检查：确保传入的实例确实是该类型的（或者继承自该类型）
            // 例如：防止 Register(new String("a"), typeof(int)) 这种错误
            if (!type.IsAssignableFrom(module.GetType()))
            {
                throw new ArgumentException(
                    $"Instance of type '{module.GetType().Name}' is not assignable to registered type '{type.Name}'.");
            }

            // 策略：如果已存在，直接覆盖（Update模式）。
            // 如果你希望重复注册报错，可以用 _instances.Add(type, module);
            if (_instances.ContainsKey(type))
            {
                _instances[type] = module;
            }
            else
            {
                _instances.Add(type, module);
            }
        }
        #endregion

        #region Get
        public T Get<T>() where T : class
        {
            return (T)Get(typeof(T));
        }
        public object Get(Type type)
        {
            if (_instances.TryGetValue(type, out object instance))
            {
                return instance;
            }

            throw new KeyNotFoundException($"Type '{type.Name}' is not registered in the container.");
        }
        public bool TryGet<T>(out T result) where T : class
        {
            if (TryGet(typeof(T), out object objResult))
            {
                result = (T)objResult;
                return true;
            }

            result = null;
            return false;
        }
        public bool TryGet(Type type, out object result)
        {
            return _instances.TryGetValue(type, out result);
        }
        #endregion


        public bool Has<T>()
        {
            return Has(typeof(T));
        }
        public bool Has(Type type)
        {
            return _instances.ContainsKey(type);
        }
        public void Clear()
        {
            _instances.Clear();
        }
    }
}
