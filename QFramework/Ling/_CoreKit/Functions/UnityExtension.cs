using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("13.Ling", "UnityExtension", 0)]
    [APIDescriptionCN("针对 UnityEngine 提供的扩展方法")]
    [APIDescriptionEN("针对 UnityEngine 提供的扩展方法")]
#endif
    public static class UnityExtension
    {
        private static readonly System.Random _rng = new System.Random();

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("删除Tranform的全部子物体")]
        [APIDescriptionEN("删除Tranform的全部子物体")]
        [APIExampleCode(@"trans.DestroyChildren();")]
#endif
        public static void DestroyChildren(this Transform trans)
        {
            List<Transform> childs = trans.GetChildren(true);
            foreach (var item in childs)
            {
                GameObject.Destroy(item.gameObject);
            }
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("立即删除Tranform的全部子物体")]
        [APIDescriptionEN("立即删除Tranform的全部子物体")]
        [APIExampleCode(@"trans.DestroyChildrenImmediate();")]
#endif
        public static void DestroyChildrenImmediate(this Transform trans)
        {
            List<Transform> childs = trans.GetChildren(true);
            foreach (var item in childs)
            {
                GameObject.DestroyImmediate(item.gameObject);
            }
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("可以直接激活Transform")]
        [APIDescriptionEN("可以直接激活Transform")]
        [APIExampleCode(@"trans.SetActive(true);")]
#endif
        public static void SetActive(this Transform trans, bool boolen)
        {
            trans.gameObject.SetActive(boolen);
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN(" 将数组原地重新排序（洗牌）")]
        [APIDescriptionEN(" 将数组原地重新排序（洗牌）")]
        [APIExampleCode(@"array.Shuffle();")]
#endif
        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN(" 将列表原地重新排序（洗牌）")]
        [APIDescriptionEN(" 将列表原地重新排序（洗牌）")]
        [APIExampleCode(@"list.Shuffle();")]
#endif
        public static void Shuffle<T>(this List<T> list)
        {
           int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN(" 将数组重新排序并返回为一个新的列表")]
        [APIDescriptionEN(" 将数组重新排序并返回为一个新的列表")]
        [APIExampleCode(@"array.ToShuffledList();")]
#endif
        public static List<T> ToShuffledList<T>(this T[] array)
        {
            // 先复制一份，不破坏原数组
            List<T> newList = new List<T>(array);
            newList.Shuffle();
            return newList;
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN(" 返回洗牌后的新数组")]
        [APIDescriptionEN(" 返回洗牌后的新数组")]
        [APIExampleCode(@"array.GetShuffled();")]
#endif
        /// <summary>
        /// 1. 返回洗牌后的新数组（不影响原数组）
        /// </summary>
        public static T[] GetShuffled<T>(this T[] array)
        {
            if (array == null) return null;

            // 克隆一份新数组
            T[] newArray = (T[])array.Clone();
            Shuffle(newArray);
            return newArray;
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN(" 返回洗牌后的新列表")]
        [APIDescriptionEN(" 返回洗牌后的新列表")]
        [APIExampleCode(@"list.GetShuffled();")]
#endif
        public static List<T> GetShuffled<T>(this List<T> list)
        {
            if (list == null) return null;

            // 通过构造函数创建一份新拷贝
            List<T> newList = new List<T>(list);
            Shuffle(newList);
            return newList;
        }



#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("返回一级子物体列表")]
        [APIDescriptionEN("返回一级子物体列表")]
        [APIExampleCode(@"trans.GetChildren(true);")]
#endif
        public static List<Transform> GetChildren(this Transform parent, bool includeInactive = false)
        {
            List<Transform> list = new List<Transform>();
            foreach (Transform transform in parent)
            {
                if (transform.IsActive() || (!transform.IsActive() && includeInactive))
                {
                    list.Add(transform);
                }
            }
            return list;
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("返回一级子物体的某组件列表")]
        [APIDescriptionEN("返回一级子物体的某组件列表")]
        [APIExampleCode(@"trans.GetChildren<T>(true);")]
#endif
        public static List<T> GetChildren<T>(this Transform parent, bool includeInactive = false) where T : Component
        {
            List<T> list = new List<T>();
            foreach (Transform transform in parent)
            {
                if (transform.IsActive() || (!transform.IsActive() && includeInactive))
                {
                    T t = transform.GetComponent<T>();
                    if (t != null)
                        list.Add(transform.GetComponent<T>());
                }
            }
            return list;
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("返回所有子物体")]
        [APIDescriptionEN("返回所有子物体")]
        [APIExampleCode(@"trans.GetAllChildren<T>(true);")]
#endif
        public static List<Transform> GetAllChildren(this Transform parent, bool includeInactive = false)
        {
            List<Transform> list = parent.GetComponentsInChildren<Transform>(includeInactive).ToList();
            list.Remove(parent);
            return list;
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("返回所有同名子物体")]
        [APIDescriptionEN("返回所有同名子物体")]
        [APIExampleCode(@"trans.FindDirectChildrenByName(name);")]
#endif
        public static List<Transform> FindDirectChildrenByName(this Transform root, string name)
        {
            var results = new List<Transform>();
            for (int i = 0; i < root.childCount; i++)
            {
                var c = root.GetChild(i);
                if (c.name == name) results.Add(c);
            }
            return results;
        }
        public static List<Transform> FindAllChildrenByName(this Transform root, string name, bool includeInactive = false)
        {
            var results = new List<Transform>();
            foreach (var item in root.GetComponentsInChildren<Transform>(includeInactive))
            {
                if (item.name == name)
                {
                    results.Add(item);
                }
            }
            return results;
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("获取物体本身以及一级子物体上的所有指定类型组件")]
        [APIDescriptionEN("获取物体本身以及一级子物体上的所有指定类型组件")]
        [APIExampleCode(@"trans.GetComponentInFirstChildren<T>();")]
#endif
        public static T GetComponentInFirstChildren<T>(this Component component, bool includeInactive = false)
        {
            // 1. 先检查自身
            if (component.TryGetComponent<T>(out var self))
                return self;

            // 2. 遍历一级子物体
            foreach (Transform child in component.transform)
            {
                if (!includeInactive && !child.gameObject.activeSelf) continue;

                if (child.TryGetComponent<T>(out var comp))
                    return comp;
            }

            return default(T);
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("获取物体本身以及一级子物体上的所有指定类型组件")]
        [APIDescriptionEN("获取物体本身以及一级子物体上的所有指定类型组件")]
        [APIExampleCode(@"trans.GetComponentsInFirstChildren<T>();")]
#endif
        public static List<T> GetComponentsInFirstChildren<T>(this Component component, bool includeInactive = false)
        {
            List<T> results = new List<T>();

            // 1. 检查物体本身
            T selfComp = component.GetComponent<T>();
            if (selfComp != null)
            {
                results.Add(selfComp);
            }

            // 2. 遍历一级子物体（transform 的迭代器只包含直接子物体）
            foreach (Transform child in component.transform)
            {
                if (!includeInactive && !child.gameObject.activeSelf) continue;

                T childComp = child.GetComponent<T>();
                
                if (childComp != null)
                {
                    results.Add(childComp);
                }
            }

            return results;
        }



#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("设置父物体并归零")]
        [APIDescriptionEN("设置父物体并归零")]
        [APIExampleCode(@"trans.SetParentAndReset<T>(tarentTrans);")]
#endif
        public static void SetParentAndReset(this Transform transform, Transform parent)
        {
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("获取Transform的RectTransform组件")]
        [APIDescriptionEN("获取Transform的RectTransform组件")]
        [APIExampleCode(@"trans.Rect();")]
#endif
        public static RectTransform Rect(this Transform trans)
        {
            return trans.GetComponent<RectTransform>();
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("看向某点的XZ片面，y轴与自身一样")]
        [APIDescriptionEN("看向某点的XZ片面，y轴与自身一样")]
        [APIExampleCode(@"trans.XZLookAt(position);")]
#endif
        public static void XZLookAt(this Transform transform, Vector3 position)
        {
            transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("绕Y轴旋转")]
        [APIDescriptionEN("绕Y轴旋转")]
        [APIExampleCode(@"trans.DoXZRotate(rotation, target);")]
#endif
        public static void DoXZRotate(this Transform transform, float rotation, Vector3 target)
        {
            // 记录转身前的角度
            Quaternion raw_rotation = transform.rotation;
            // 记录目标角度
            XZLookAt(transform, target);

            Quaternion lookat_rotation = transform.rotation;
            // 还原当前角度
            transform.rotation = raw_rotation;
            // 计算旋转角度
            float rotate_angle = Quaternion.Angle(raw_rotation, lookat_rotation);
            // 获得lerp速度
            float lerp_speed = rotation / rotate_angle;
            float lerp_tm = 0.0f;

            lerp_tm += Time.deltaTime * lerp_speed;
            transform.rotation = Quaternion.Lerp(raw_rotation, lookat_rotation, lerp_tm);
            if (lerp_tm >= 1)
            {
                transform.rotation = lookat_rotation;
            }
        }
#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("旋转")]
        [APIDescriptionEN("旋转")]
        [APIExampleCode(@"trans.DoRotate(rotation, target);")]
#endif
        public static void DoRotate(this Transform transform, float rotation, Vector3 target)
        {
            // 记录转身前的角度
            Quaternion raw_rotation = transform.rotation;
            // 记录目标角度
            transform.LookAt(target);

            Quaternion lookat_rotation = transform.rotation;
            // 还原当前角度
            transform.rotation = raw_rotation;
            // 计算旋转角度
            float rotate_angle = Quaternion.Angle(raw_rotation, lookat_rotation);
            // 获得lerp速度
            float lerp_speed = rotation / rotate_angle;
            float lerp_tm = 0.0f;

            lerp_tm += Time.deltaTime * lerp_speed;
            transform.rotation = Quaternion.Lerp(raw_rotation, lookat_rotation, lerp_tm);
            if (lerp_tm >= 1)
            {
                transform.rotation = lookat_rotation;
            }
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("IsActive")]
        [APIDescriptionEN("IsActive")]
        [APIExampleCode(@"component.IsActive();")]
#endif
        public static bool IsActive(this Component component)
        {
            return component.gameObject.activeSelf;
        }
    }
}