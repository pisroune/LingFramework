using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    /// <summary>
    /// 全局事件总线（线程不安全，仅用于主线程游戏逻辑）
    /// </summary>
    public static class EventBus
    {
        /// <summary>
        /// 存储事件类型与监听者列表
        /// </summary>
        private static readonly Dictionary<Type, List<Delegate>> _reguisters = new();

        /// <summary>
        /// 订阅事件
        /// </summary>
        public static void Register<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (!_reguisters.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _reguisters[type] = list;
            }

            if (!list.Contains(listener))
                list.Add(listener);
        }

        /// <summary>
        /// 退订事件
        /// </summary>
        public static void Unregister<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (_reguisters.TryGetValue(type, out var list))
            {
                list.Remove(listener);
                if (list.Count == 0)
                    _reguisters.Remove(type);
            }
        }

        /// <summary>
        /// 只订阅一次（触发后自动退订）
        /// </summary>
        public static void RegisterOnce<T>(Action<T> listener)
        {
            void Wrapper(T evt)
            {
                listener(evt);
                Unregister<T>(Wrapper);
            }
            Register<T>(Wrapper);
        }

        /// <summary>
        /// 发布事件（同步触发）
        /// </summary>
        public static void Trigger<T>(T evt)
        {
            var type = typeof(T);
            if (!_reguisters.TryGetValue(type, out var list))
                return;

            // 复制到临时数组，防止回调中修改列表导致异常

            for (int i = list.Count - 1; i >= 0; i--)
            {
                (list[i] as Action<T>).Invoke(evt);
            }
        }

        /// <summary>
        /// 清空所有订阅（慎用）
        /// </summary>
        public static void ClearAll()
        {
            _reguisters.Clear();
        }
    }
}