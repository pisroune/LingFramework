using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        public static T NewGameObjectWithComponent<T>(string name = "new GameObject") where T : Component
        {
            var go = new GameObject(name);
            T component = go.AddComponent<T>();
            return component;
        }
        public static T NewGameObjectWithComponent<T>(Vector3 position, Quaternion rotation, Transform parent, string name = "new GameObject", Space space = Space.World) where T : Component
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



        /// <summary>
        /// 持续射线检测：撞击到目标后，由外部委托决定是停止还是穿透继续。
        /// </summary>
        /// <param name="origin">起点</param>
        /// <param name="direction">方向</param>
        /// <param name="hit">最终撞击到的结果</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">层级掩码</param>
        /// <param name="shouldContinue">过滤器：传入当前命中的 RaycastHit，返回 true 表示“穿透并继续”，false 表示“这就是我要的目标，停止”</param>
        /// <param name="queryTriggerInteraction">触发器交互选项</param>
        /// <returns>是否最终撞击到了有效目标</returns>
        public static bool RaycastContinuous(
            Vector3 origin,
            Vector3 direction,
            out RaycastHit hit,
            float maxDistance,
            int layerMask,
            Func<RaycastHit, bool> shouldContinue,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            float remainingDistance = maxDistance;
            Vector3 currentOrigin = origin;
            direction.Normalize(); // 确保方向是单位向量

            // 默认初始化输出
            hit = default;

            while (remainingDistance > 0)
            {
                if (Physics.Raycast(currentOrigin, direction, out RaycastHit tempHit, remainingDistance, layerMask, queryTriggerInteraction))
                {
                    // 如果外部委托认为需要“穿透”这个物体
                    if (shouldContinue != null && shouldContinue(tempHit))
                    {
                        // 核心逻辑：微调起点，继续向前探索
                        // 使用极小的偏移量 (0.0005f) 跨过当前撞击点，防止逻辑死循环
                        float offset = 0.0005f;
                        currentOrigin = tempHit.point + direction * offset;
                        remainingDistance -= (tempHit.distance + offset);
                        continue;
                    }

                    // 外部委托认为这就是最终目标，或者没有提供委托
                    hit = tempHit;
                    return true;
                }

                // 没撞到任何东西
                break;
            }

            return false;
        }
    }
}