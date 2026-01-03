using MoonSharp.VsCodeDebugger.SDK;
using QFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    /// <summary>
    /// 全局事件总线（线程不安全，仅用于主线程游戏逻辑）
    /// </summary>
    public static class EventBus
    {
        public enum EventPriority
        {
            Before,
            Default,
            Late,
        }
        // 定义一个空的接口，用于在字典里存储泛型容器
        private interface IEventContainer { }

        // 泛型容器：直接存储 Action<T>，避免 Invoke 时的拆箱和转换
        private class EventContainer<T> : IEventContainer
        {
            public readonly List<Action<T>> Listeners = new List<Action<T>>(8); // 默认容量8，减少扩容
        }

        private static readonly Dictionary<Type, IEventContainer> _beforeEvents = new Dictionary<Type, IEventContainer>();
        private static readonly Dictionary<Type, IEventContainer> _defaultevents = new Dictionary<Type, IEventContainer>();
        private static readonly Dictionary<Type, IEventContainer> _lateEvents = new Dictionary<Type, IEventContainer>();

        static Dictionary<Type, IEventContainer> GetDictionary(EventPriority priority)
        {
            switch (priority)
            {
                case EventPriority.Before:
                    return _beforeEvents;
                case EventPriority.Default:
                    return _defaultevents;
                case EventPriority.Late:
                    return _lateEvents;
            }
            throw new Exception("检查");
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        public static IUnRegister Register<T>(Action<T> listener, EventPriority priority = EventPriority.Default) where T : struct
        {
            var type = typeof(T);
            var eventDict = GetDictionary(priority);
            if (!eventDict.TryGetValue(type, out var container))
            {
                container = new EventContainer<T>();
                eventDict[type] = container;
            }
          
            var specificContainer = (EventContainer<T>)container;  //强转一次，获取具体的 List
            if (!specificContainer.Listeners.Contains(listener))
            {
                specificContainer.Listeners.Add(listener);
            }

            return new CustomUnRegister(() => { Unregister(listener, priority); });
        }

        /// <summary>
        /// 退订事件
        /// </summary>
        public static void Unregister<T>(Action<T> listener, EventPriority priority = EventPriority.Default) where T : struct
        {
            var type = typeof(T);
            var eventDict = GetDictionary(priority);
            if (eventDict.TryGetValue(type, out var container))
            {
                var specificContainer = (EventContainer<T>)container;
                specificContainer.Listeners.Remove(listener);
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        public static void Trigger<T>(T evt) where T : struct
        {
            var type = typeof(T);

            Trigger(GetDictionary(EventPriority.Before));
            Trigger(GetDictionary(EventPriority.Default));
            Trigger(GetDictionary(EventPriority.Late));

            
            void Trigger(Dictionary<Type, IEventContainer> dict)
            {
                if (dict.TryGetValue(type, out var container))
                {
                    // 【关键优化】：这里只需要做 1 次类型转换，而不是 N 次
                    var listeners = ((EventContainer<T>)container).Listeners;

                    // 倒序遍历，允许在回调中 Unregister
                    for (int i = listeners.Count - 1; i >= 0; i--)
                    {
                        // 直接调用，无需 as 转换，速度极快
                        // try-catch 放在这里是可选的，RTS中为了性能通常不加，报错直接炸出来修 bug
                        listeners[i].Invoke(evt);
                    }
                }
            }
        }

        public static void ClearAll()
        {
            _defaultevents.Clear();
        }
    }
}