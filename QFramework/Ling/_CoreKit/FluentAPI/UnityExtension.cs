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