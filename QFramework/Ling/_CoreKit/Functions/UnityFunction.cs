using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class UnityFunction
    {
        /// <summary>
        /// 接受目标Vector3，返回一个距离他最近的元素
        /// </summary>
        /// <returns></returns>
        public static T FindClosest<T>(Vector3 targetPosition, List<T> items, Func<T, Vector3> positionSelector)
        {
            return FindClosest(targetPosition, items, positionSelector, (item) => { return true; }, out _);
        }

        public static T FindClosest<T>(Vector3 targetPosition, List<T> items, Func<T, Vector3> positionSelector,
            out float minDistance)
        {
            return FindClosest(targetPosition, items, positionSelector, (item) => { return true; }, out minDistance);
        }
        public static T FindClosest<T>(Vector3 targetPosition, List<T> items, Func<T, Vector3> positionSelector,
            Condition<T> otherCondition,
            out float minDistance)
        {
            if (items == null || items.Count == 0)
            {
                minDistance = float.MaxValue;
                return default(T); // 如果列表为空，返回默认值
            }

            minDistance = float.MaxValue;
            T closest = default(T);
            foreach (T item in items)
            {
                if (item == null)
                {
                    continue;
                }

                Vector3 itemPosition = positionSelector(item); // 使用委托获取位置
                float distance = Vector3.Distance(targetPosition, itemPosition);
                if (distance < minDistance && otherCondition(item))
                {
                    minDistance = distance;
                    closest = item;
                }
            }

            return closest;
        }
        /// <summary>
        /// 接受目标Vector3，返回一个距离他最远的元素
        /// </summary>
        /// <returns></returns>
        public static T FindFurthest<T>(Vector3 targetPosition, List<T> items, Func<T, Vector3> positionSelector)
        {
            return FindFurthest(targetPosition, items, positionSelector, out _);
        }
        public static T FindFurthest<T>(Vector3 targetPosition, List<T> items, Func<T, Vector3> positionSelector,
            out float maxDistance)
        {
            return FindFurthest(targetPosition, items, positionSelector, (item) => { return true; }, out maxDistance);
        }
        public static T FindFurthest<T>(Vector3 targetPosition, List<T> items, Func<T, Vector3> positionSelector,
            Condition<T> otherCondition,
            out float maxDistance)
        {
            if (items == null || items.Count == 0)
            {
                maxDistance = 0;
                return default(T); // 如果列表为空，返回默认值
            }

            maxDistance = 0;
            T furthest = default(T);

            foreach (T item in items)
            {
                Vector3 itemPosition = positionSelector(item); // 使用委托获取位置
                float distance = Vector3.Distance(targetPosition, itemPosition);
                if (distance > maxDistance && otherCondition(item))
                {
                    maxDistance = distance;
                    furthest = item;
                }
            }

            return furthest;
        }

        public static GameObject NewGameObject(Vector3 position, Quaternion rotation, Transform parent, string name = "new GameObject", Space space = Space.World)
        {
            var go = new GameObject(name);
            go.transform.parent = parent;

            if (space == Space.World)
            {
                go.transform.position = position;
                go.transform.rotation = rotation;
            }
            else
            {
                go.transform.localPosition = position;
                go.transform.localRotation = rotation;
            }
            return go;
        }
        public static T NewGameObjectWithComponnet<T>(Vector3 position, Quaternion rotation, Transform parent, string name = "new GameObject", Space space = Space.World) where T : Component
        {
            var go = new GameObject(name);
            go.transform.parent = parent;
            T component = go.AddComponent<T>();
            if (space == Space.World)
            {
                go.transform.position = position;
                go.transform.rotation = rotation;
            }
            else
            {
                go.transform.localPosition = position;
                go.transform.localRotation = rotation;
            }
            return component;
        }
    }
}