using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QFramework.UI
{
    /// <summary>
    /// uiÊÂ¼þ¼àÌý²å¼þ
    /// </summary>
    public class Listener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerExitHandler
    {
        public Action<PointerEventData> onClickDown;
        public Action<PointerEventData> onClickUp;
        public Action<PointerEventData> onDrag;
        public Action<PointerEventData> onPointerEnter;
        public Action<PointerEventData> onPOinterExit;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (onClickDown != null)
            {
                onClickDown(eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (onClickUp != null)
            {
                onClickUp(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null)
            {
                onDrag(eventData);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (onPointerEnter != null)
            {
                onPointerEnter(eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (onPOinterExit != null)
            {
                onPOinterExit(eventData);
            }
        }
    }
}