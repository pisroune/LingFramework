using Prototype;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prototype
{
    public class UIFunction : MonoBehaviour
    {
        public static Canvas Canvas;
        public static UIFunction Instance;
        #region RayHit
        public enum PointerIndicatorMode
        {
            Mouse,
            Transform,
            None
        }
        public static PointerIndicatorMode PointerIndicatorM;
        public static Transform IndicatorTrans;
        public static IPointer Pointer;

        /// <summary>
        /// 判断屏幕射线是否命中UI物体
        /// </summary>
        /// <param name="screenPoint">屏幕射线位置</param>
        /// <param name="transform">UI物体</param>
        /// <param name="penetrate">可以穿透子物体，不限制层级</param>
        /// <returns></returns>
        public static bool IsRayHitTrans(Transform transform, Vector2 screenPoint, bool penetrate = false)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = screenPoint;
            GraphicRaycaster gr = Canvas.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(pointerEventData, results);
            if (penetrate)
            {
                foreach (var result in results)
                {
                    if (result.gameObject.transform == transform)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return results[0].gameObject.transform == transform;
            }
            return false;
        }
        public static bool IsRayHit(Vector2 screenPoint)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = screenPoint;
            GraphicRaycaster gr = Canvas.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(pointerEventData, results);
            return results.Count != 0;
        }
        public static bool IsRayHit(Vector2 screenPoint, out Transform firstRayHitTrans)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = screenPoint;
            GraphicRaycaster gr = Canvas.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(pointerEventData, results);
            if (results.Count > 0)
            {
                firstRayHitTrans = results[0].gameObject.transform;
                return true;
            }
            firstRayHitTrans = null;
            return false;
        }
        public static Transform GetRayHitTrans(Vector2 screenPoint)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = screenPoint;
            GraphicRaycaster gr = Canvas.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(pointerEventData, results);
            if (results.Count != 0)
            {
                return results[0].gameObject.transform;
            }
            return null;
        }
        public static bool TryGetPointer(Vector2 screenPoint, out IPointer pointer)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = screenPoint;
            GraphicRaycaster gr = Canvas.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(pointerEventData, results);
            if (results.Count != 0)
            {
                pointer = results[0].gameObject.GetComponent<IPointer>();
                return pointer != null;
            }
            pointer = null;
            return false;
        }
        public static List<Transform> GetAllRayHitTrans(Vector2 screenPoint)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = screenPoint;
            GraphicRaycaster gr = Canvas.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(pointerEventData, results);
            List<Transform> list = new List<Transform>();
            foreach (var item in results)
            {
                list.Add(item.gameObject.transform);
            }
            return list;
        }
        #endregion

    }
}