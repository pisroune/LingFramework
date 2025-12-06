using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prototype
{
    public enum WindowType
    {
        /// <summary>
        /// 永远显示在最底层
        /// 游戏Logo....
        /// HUD..
        /// 
        /// 优先级低：通常不会遮挡其他UI。
        /// 持久存在：不会随着场景切换而销毁。
        /// 交互性较弱：多为信息展示。
        /// </summary>
        BaseLayer,

        /// <summary>
        /// 主界面上的功能按钮（如"开始游戏"、"设置"、"退出"）。
        /// 游戏内的主要操作面板。（大本营里的大部分按钮）
        /// 
        /// 优先级中等：通常不会完全覆盖底层类。
        /// 交互性较强：直接与玩家产生频繁交互。
        /// 状态稳定：通常在场景中一直存在，直至玩家主动切换。
        /// </summary>
        MainFunctionalLayer,

        /// <summary>
        /// 弹出面板
        /// 设置面板...
        /// 选择作战单位（《卫国战争》）...
        /// </summary>
        PopupLayer,

        /// <summary>
        /// 显示动态信息的临时UI
        /// 战斗伤害数字...
        /// 提示信息（如“按F打开箱子”）。
        /// 
        /// 优先级中等到高：通常不遮挡玩家的主要操作区域。
        /// 短暂存在：展示一段时间后自动消失。
        /// 交互性弱：通常只进行信息提示。
        /// </summary>
        FloatingLayer,

        /// <summary>
        /// 完全覆盖屏幕的UI，用于特定场景或阶段的完整交互。
        /// 战斗结算界面（如胜利或失败结算）。
        /// 剧情动画界面（如对话框、过场动画）。
        /// 加载界面（如场景加载动画）。
        /// 
        /// 最高优先级：完全遮挡其他UI。
        /// 交互性强：是当前场景的核心交互界面。
        /// 状态独立：与其他UI层级隔离。
        /// </summary>
        FullScreenLayer,

        /// <summary>
        /// 环境类
        /// 贴合游戏世界的UI，与3D场景或实体对象绑定。
        /// 
        /// NPC头顶的名字或其他提示。
        /// 游戏内的3D指引箭头。
        /// 玩家角色的浮动血条。
        /// 
        /// 优先级动态：根据视角和距离自动调整显示。
        /// 交互性弱：大部分为信息展示。
        /// 与场景绑定：通常挂在场景对象或角色上。
        /// </summary>
        EnvironmentUI,
    }

    public interface IPopupWindow
    {
        enum PopupType
        {
            Inactive,
            Destroy,
        }

        PopupType PopUpT { get; }
        bool TryRemove();
    }

    /// <summary>
    /// UI管理器类
    /// </summary>
    public static class UIManager
    {
        static List<IPopupWindow> PopupWindows = new List<IPopupWindow>();
        public static bool UseCanvasGroup = false;    //Use CanvasGroup to active or inactive window, set alpha, interactable, blocks raycasts while SetWinState
        public static Canvas Canvas;
        public static Transform CanvasTrans;
        public static Dictionary<WindowType, GameObject> WindowTypeParent;
        public static Dictionary<Type, IWindowRoot> WindowDict;
        public static Dictionary<Type, IWindowRoot> ActiveWindows;

        static UIManager()
        {
            PointerIndicatorM = PointerIndicatorMode.Mouse;
            WindowDict = new Dictionary<Type, IWindowRoot>();
            ActiveWindows = new Dictionary<Type, IWindowRoot>();
            WindowTypeParent = new Dictionary<WindowType, GameObject>();
            CanvasTrans = GameObject.Find("Canvas").transform;
            Canvas = CanvasTrans.GetComponent<Canvas>();

            NewWindowTypeParent<EnvironmentUI>(WindowType.EnvironmentUI, "Environment UI");
            NewWindowTypeParent<BaseLayer>(WindowType.BaseLayer, "Base Layer");
            NewWindowTypeParent<FloatingLayer>(WindowType.FloatingLayer, "Floating Layer");
            NewWindowTypeParent<MainFunctionalLayer>(WindowType.MainFunctionalLayer, "Main Functional Layer");
            NewWindowTypeParent<PopupLayer>(WindowType.PopupLayer, "Popup Layer");
            NewWindowTypeParent<FullScreenLayer>(WindowType.FullScreenLayer, "Full Screen Layer");
            ActionKit.OnUpdate.Register(Update);
            ActionKit.OnLateUpdate.Register(LateUpdate);



            void NewWindowTypeParent<T>(WindowType windowType, string name) where T : UILayer
            {
                GameObject parentGO = new GameObject(name);
                parentGO.Parent(Canvas);
                parentGO.AddComponent<T>();
                WindowTypeParent.Add(windowType, parentGO);
            }
        }

        #region MakeSure Initialized

        /// <summary>
        /// 实例化新面板
        /// </summary>
        static T MakeSureWindow<T>() where T : class, IWindowRoot
        {
            return MakeSureWindow(typeof(T)) as T;
        }
        /// <summary>
        /// 实例化新面板
        /// </summary>
        static IWindowRoot MakeSureWindow(Type type)
        {
            if (WindowDict.TryGetValue(type, out var value))
            {
                return value;
            }

            string windowName = type.Name;
            string path = $"UI/Window/{windowName}";
            var tmpWinds = Resources.Load<WindowRoot>(path);
            if (tmpWinds == null)
            {
                Debug.LogError("未找到：" + path);
            }
            var window = GameObject.Instantiate(tmpWinds, Canvas.transform).GetComponent<IWindowRoot>();
            window.ThisGameObject.Parent(WindowTypeParent[window.WindowT].transform);
            window.InitOnce();
            window.SetWndActive(false);
            WindowDict.Add(type, window);
            return window;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// 按类型手动卸载面板
        /// </summary>
        public static void DisposeWindow<T>()
        {
            Type type = typeof(T);
            DisposeWindow(type);
        }
        /// <summary>
        /// 按类型手动卸载面板
        /// </summary>
        public static void DisposeWindow(Type type)
        {
            if (WindowDict.TryGetValue(type, out var value))
            {
                SetWndState(type, false).Dispose();
                WindowDict.Remove(type);
                GameObject.Destroy(value.ThisGameObject);
            }
        }
        #endregion

        #region Get or Init

        /// <summary>
        /// 按类型获取面板，如果没有则创建
        /// </summary>
        public static T GetWindow<T>() where T : class, IWindowRoot
        {
            T t = MakeSureWindow<T>();
            return t;
        }
        /// <summary>
        /// 按类型获取面板，如果没有则创建
        /// </summary>
        public static IWindowRoot GetWindow(Type type)
        {
            return MakeSureWindow(type);
        }

        #endregion

        #region SetWndState

        /// <summary>
        /// 设置面板激活/隐藏
        /// </summary>
        /// <typeparam name="T">面板类</typeparam>
        /// <param name="state">状态</param>
        /// <param name="initValue">初始化数据</param>
        /// <returns></returns>
        public static T SetWndState<T>(bool state, object initValue = null) where T : class, IWindowRoot
        {
            Type type = typeof(T);
            return SetWndState(type, state, initValue) as T;
        }

        /// <summary>
        /// 设置面板激活/隐藏
        /// </summary>
        /// <param name="type">面板类</param>
        /// <param name="state">状态</param>
        /// <param name="initValue">初始化数据</param>
        /// <returns></returns>
        public static IWindowRoot SetWndState(Type type, bool state, object initValue = null)
        {
            IWindowRoot window = GetWindow(type);
            if (state)
            {
                if (!ActiveWindows.ContainsKey(type))
                {
                    window.SetWndState(state, initValue);
                    ActiveWindows.Add(type, window);
                    if (ShowingUI)
                    {
                        window.OnShowUI();
                    }
                    else
                    {
                        window.OnHideUI();
                    }
                    if (window is IPopupWindow popUpWnd)
                    {
                        PopupWindows.Add(popUpWnd);
                    }
                    Debug.Log(window.WindowName.ToString() + "激活，当前有" + ActiveWindows.Count + "个激活页面");
                }
            }
            else
            {
                if (ActiveWindows.ContainsKey(type))
                {
                    window.SetWndState(state, initValue);
                    ActiveWindows.Remove(type);
                    if (window is IPopupWindow popUpWnd)
                    {
                        PopupWindows.Remove(popUpWnd);
                    }
                    Debug.Log(window.WindowName.ToString() + "隐藏，当前有" + ActiveWindows.Count + "个激活页面");
                }
            }
            return window;
        }

        /// <summary>
        /// 判断面板是否正处于激活状态
        /// </summary>
        public static bool IsWndActive<T>() where T : class, IWindowRoot
        {
            Type type = typeof(T);
            return IsWndActive(type);
        }
        /// <summary>
        /// 判断面板是否正处于激活状态
        /// </summary>
        public static bool IsWndActive(Type type)
        {
            return ActiveWindows.ContainsKey(type);
        }

        #endregion

        public static void Update()
        {
            foreach (var activeWindow in ActiveWindows.Values)
            {
                activeWindow.UpdateWindow();
            }


            bool hasPointer = false;
            IPointer pointer = null;
            switch (PointerIndicatorM)
            {
                case PointerIndicatorMode.Mouse:
                    hasPointer = TryGetPointer(Input.mousePosition, out pointer);
                    break;
                case PointerIndicatorMode.Transform:
                    hasPointer = IndicatorTrans ? TryGetPointer(Input.mousePosition, out pointer) : false;
                    break;
            }
            if (Pointer != null)
            {
                if (Pointer == pointer)
                {
                    Pointer.PointerStay();
                }
                else
                {
                    Pointer.PointerExit();
                }
            }
            if (hasPointer)
            {
                Pointer = pointer;
                Pointer.PointerEnter();
            }
        }
        public static bool TryPopup()
        {
            if (PopupWindows.Count > 0)
            {
                var window = PopupWindows[PopupWindows.Count - 1];
                if (window.TryRemove())
                {
                    switch (window.PopUpT)
                    {
                        case IPopupWindow.PopupType.Inactive:
                            SetWndState(window.GetType(), false);
                            break;
                        case IPopupWindow.PopupType.Destroy:
                            DisposeWindow(window.GetType());
                            break;
                        default:
                            throw new Exception("检查" + window.PopUpT);
                    }
                }
                return true;
            }
            return false;
        }
        public static void LateUpdate()
        {
            foreach (var activeWindow in ActiveWindows.Values)
            {
                activeWindow.LateUpdateWindow();
            }
        }
        public static void PauseGame()
        {
            foreach (var item in WindowDict)
            {
                item.Value.Pause();
            }
        }
        public static void ResumeGame()
        {
            foreach (var item in WindowDict)
            {
                item.Value.Resume();
            }
        }


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


        public static bool ShowingUI = true;
        public static void HideAllUI()
        {
            ShowingUI = false;
            foreach (var item in ActiveWindows)
            {
                item.Value.OnHideUI();
            }
        }
        public static void ShowAllUI()
        {
            ShowingUI = true;
            foreach (var item in ActiveWindows)
            {
                item.Value.OnShowUI();
            }
        }

        #region Extend Func
        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
        }
        #endregion
    }
}