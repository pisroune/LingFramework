using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QFramework.UI
{
    /// <summary>
    /// UI组件基类，提供一些方法
    /// </summary>
    public class WindowComponent : MonoBehaviour
    {
        public RectTransform Rect => FindPath<RectTransform>("");

        /// <summary>
        /// 这是一种为UI组件快速赋值，查找的办法
        /// 省略了初始化的过程，只需要设置属性，return FindPath<T>(path) 即可
        /// </summary>
        private Dictionary<string, Transform> _transformDict = new Dictionary<string, Transform>();
        protected T FindPath<T>(string path = "") where T : Object
        {
            if (_transformDict.ContainsKey(path))
            {
                return _transformDict[path].GetComponent<T>();
            }
            Transform trans = transform;
            if (path != "")
            {
                string[] paths = path.Split('/');
                foreach (var item in paths)
                {
                    if (item != null && trans != null)
                        trans = trans.Find(item);
                }
            }
            _transformDict.Add(path, trans);
            return trans.GetComponent<T>();
        }

        #region Tool Functions

        protected void SetActive(GameObject go, bool isActive = true)
        {
            go.SetActive(isActive);
        }
        protected void SetActive(Transform trans, bool state = true)
        {
            trans.gameObject.SetActive(state);
        }
        protected void SetActive(RectTransform rectTrans, bool state = true)
        {
            rectTrans.gameObject.SetActive(state);
        }
        protected void SetActive(Image img, bool state = true)
        {
            img.transform.gameObject.SetActive(state);
        }
        protected void SetActive(Text txt, bool state = true)
        {
            txt.transform.gameObject.SetActive(state);
        }

        protected void SetText(Text txt, string context = "")
        {
            txt.text = context;
        }
        protected void SetText(Transform trans, int num = 0)
        {
            SetText(trans.GetComponent<Text>(), num);
        }
        protected void SetText(Transform trans, string context = "")
        {
            SetText(trans.GetComponent<Text>(), context);
        }
        protected void SetText(Text txt, int num = 0)
        {
            SetText(txt, num.ToString());
        }

        protected void SetColor(Image img, Color col)
        {
            img.color = col;
        }
        protected void SetColor(Text txt, Color col)
        {
            txt.color = col;
        }

        protected T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T t = go.GetComponent<T>();
            if (t == null)
            {
                t = go.AddComponent<T>();
            }
            return t;
        }

        #endregion

        #region ClickEvts
        protected void OnClickDown(GameObject go, Action<PointerEventData> cb)
        {
            Listener listener = GetOrAddComponent<Listener>(go);
            listener.onClickDown = cb;
        }

        protected void OnClickUp(GameObject go, Action<PointerEventData> cb)
        {
            Listener listener = GetOrAddComponent<Listener>(go);
            listener.onClickUp = cb;
        }
        protected void OnDrag(GameObject go, Action<PointerEventData> cb)
        {
            Listener listener = GetOrAddComponent<Listener>(go);
            listener.onDrag = cb;
        }
        protected void OnPointerEnter(GameObject go, Action<PointerEventData> cb)
        {
            Listener listener = GetOrAddComponent<Listener>(go);
            listener.onPointerEnter = cb;
        }
        protected void OnpointerExit(GameObject go, Action<PointerEventData> cb)
        {
            Listener listener = GetOrAddComponent<Listener>(go);
            listener.onPOinterExit = cb;
        }
        #endregion

    }
}