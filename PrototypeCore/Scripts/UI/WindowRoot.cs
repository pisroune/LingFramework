using UnityEngine;
using QFramework;


namespace Prototype
{
    public interface IWindowRoot
    {
        WindowType WindowT { get; }
        GameObject ThisGameObject { get; }
        string WindowName { get; }

        void InitOnce();
        void Dispose();
        void SetWndState(bool active, object initData = null);
        void SetWndActive(bool active);

        void UpdateWindow();
        void LateUpdateWindow();
        void Pause();
        void Resume();

        void OnShowUI();   //全局显示UI，极端情况可以不响应
        void OnHideUI();   //全局隐藏UI，极端情况可以不响应
    }
    public abstract class WindowRoot : ViewController, IWindowRoot
    {
        CanvasGroup _canvasgroup;

        public virtual bool UseCanvasGroup => false;
        public abstract WindowType WindowT { get; }
        string IWindowRoot.WindowName => GetType().Name;
        GameObject IWindowRoot.ThisGameObject => gameObject;

        /// <summary>
        /// Invoke while game launch, instead of Awake()
        /// Only invoke once
        /// </summary>
        void IWindowRoot.InitOnce()
        {
            OnInitOnce();
            _canvasgroup = gameObject.GetOrAddComponent<CanvasGroup>();
            OnInitOnceFinal();
        }
        protected abstract void OnInitOnce();
        protected virtual void OnInitOnceFinal() { }

        /// <summary>
        /// Dispose window, instead of OnDestroy()
        /// </summary>
        void IWindowRoot.Dispose()
        {
            OnDispose();
        }
        protected virtual void OnDispose() { }


        /// <summary>
        /// Set windows active or inactive
        /// </summary>
        /// <param name="active"></param>
        void IWindowRoot.SetWndState(bool active, object initData)
        {
            if (active)
            {  //先激活在执行代码
                (this as IWindowRoot).SetWndActive(active);
                OnShow(initData);
            }
            else
            {  //先执行代码再隐藏
                OnHide();
                (this as IWindowRoot).SetWndActive(active);
            }
        }
        void IWindowRoot.SetWndActive(bool active)
        {
            if (active)
            {
                if (UseCanvasGroup)
                {
                    _canvasgroup.alpha = 1;
                    _canvasgroup.interactable = true;
                    _canvasgroup.blocksRaycasts = true;
                }
                else
                {
                    gameObject.SetActive(true);
                }
            }
            else
            {
                if (UseCanvasGroup)
                {
                    _canvasgroup.alpha = 0;
                    _canvasgroup.interactable = false;
                    _canvasgroup.blocksRaycasts = false;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
        protected virtual void OnShow(object initData) { }
        protected virtual void OnHide() { }

        void IWindowRoot.UpdateWindow()
        {
            OnUpdate();
        }
        protected virtual void OnUpdate() { }

        void IWindowRoot.LateUpdateWindow()
        {
            OnLateUpdate();
        }
        protected virtual void OnLateUpdate() { }

        void IWindowRoot.Pause()
        {
            OnPause();
        }
        protected virtual void OnPause() { }
        void IWindowRoot.Resume()
        {
            OnResume();
        }
        protected virtual void OnResume() { }

        protected void SetMyState(bool active, object initData = null)
        {
            UIManager.SetWndState(GetType(), active, initData);
        }

        void IWindowRoot.OnShowUI()
        {
            OnShowUI();
        }
        protected virtual void OnShowUI()
        {
            _canvasgroup.alpha = 1;
        }


        void IWindowRoot.OnHideUI()
        {
            OnHideUI();
        }
        protected virtual void OnHideUI()
        {
            _canvasgroup.alpha = 0;
        }
    }
}