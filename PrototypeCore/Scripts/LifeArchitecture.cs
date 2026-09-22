using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;

namespace QFramework
{
    public interface ILateInit
    {
        void LateInit() { return; }
    }
    public interface IWithGameObject
    {
        GameObject ExternalGameObject { get; set; }
        string ObjectName { get; }
        sealed Transform ThisTransform { get => ExternalGameObject.transform; }
        sealed void InstantiateDefaultGameObject()
        {
            ExternalGameObject = new GameObject(ObjectName);
            ExternalGameObject.DontDestroyOnLoad();
        }
        sealed void DestroyDefaultGameObject()
        {
            GameObject.Destroy(ExternalGameObject);
            ExternalGameObject = null;
        }
    }
    public interface IWithLoader
    {
        ResLoader Loader { get; set; }

        sealed void AllocateLoader()
        {
            Loader = ResLoader.Allocate();
        }
        sealed void RemoveLoader()
        {
            Loader?.Recycle2Cache();
            Loader = null;
        }
    }

    public interface IUpdate
    {
        void Update();
    }
    public interface IFixedUpdate
    {
        void FixedUpdate();
    }
    public interface ILateUpdate
    {
        void LateUpdate();
    }

    public interface ILingArchitecture
    {
        void Update();
        void FixedUpdate();
        void LateUpdate();

        TSystem GetSystem<TSystem>() where TSystem : class, ISystem;
        bool TryGetSystem<TSystem>(out TSystem result) where TSystem : class, ISystem;

        TModel GetModel<TModel>() where TModel : class, IModel;
        bool TryGetModel<TModel>(out TModel result) where TModel : class, IModel;

        TUtility GetUtility<TUtility>() where TUtility : class, IUtility;
        bool TryGetUtility<TUtility>(out TUtility result) where TUtility : class, IUtility;
    }
    public abstract class LingArchitecture<T> : Architecture<T>, ILingArchitecture where T : Architecture<T>, new()
    {
        static ILingArchitecture _instance;
        private IUnRegister _updateRegistration;
        private IUnRegister _fixedUpdateRegistration;
        private IUnRegister _lateUpdateRegistration;
        public static ILingArchitecture LingInterface
        {
            get
            {
                return _instance;
            }
        }
        public HashSet<IBelongToArchitecture> AllComponent { get; private set; } = new HashSet<IBelongToArchitecture>();
        private HashSet<ILateInit> _lateActive = new HashSet<ILateInit>();
        private HashSet<IWithGameObject> _withGameObject = new HashSet<IWithGameObject>();
        private HashSet<IUpdate> _updates = new HashSet<IUpdate>();
        private HashSet<IFixedUpdate> _fixedUpdates = new HashSet<IFixedUpdate>();
        private HashSet<ILateUpdate> _lateUpdates = new HashSet<ILateUpdate>();
        private HashSet<IWithLoader> _withLoader = new HashSet<IWithLoader>();
        protected HashSet<ILingArchitecture> CitationArchitectures = new HashSet<ILingArchitecture>();   //引用架构，获取模块时，如果自身未找到，会尝试从引用架构中查找
        protected override bool InitAutomatic => false;

        protected sealed override void Init()
        {
            _instance = this;
            OnInitalized();
            _updateRegistration = ActionKit.OnUpdate.Register(((ILingArchitecture)this).Update);
            _fixedUpdateRegistration = ActionKit.OnFixedUpdate.Register(((ILingArchitecture)this).FixedUpdate);
            _lateUpdateRegistration = ActionKit.OnLateUpdate.Register(((ILingArchitecture)this).LateUpdate);
            Debug.Log("初始化架构：" + this.GetType().Name);
        }
        protected abstract void OnInitalized();

        protected override void LateInit()
        {
            foreach (var item in _lateActive)
            {
                item.LateInit();
            }
        }

        protected override void OnDeinit()
        {
            // Stop callbacks first; module cleanup still needs its host and loader.
            _updateRegistration?.UnRegister();
            _fixedUpdateRegistration?.UnRegister();
            _lateUpdateRegistration?.UnRegister();
            _updateRegistration = null;
            _fixedUpdateRegistration = null;
            _lateUpdateRegistration = null;
        }

        protected override void OnAfterDeinit()
        {
            var errors = new List<Exception>();
            foreach (var item in _withLoader)
                TryCleanup(item.RemoveLoader, errors);
            foreach (var item in _withGameObject)
                TryCleanup(item.DestroyDefaultGameObject, errors);

            _lateActive.Clear();
            _withGameObject.Clear();
            _withLoader.Clear();
            _updates.Clear();
            _fixedUpdates.Clear();
            _lateUpdates.Clear();
            AllComponent.Clear();
            CitationArchitectures.Clear();
            if (ReferenceEquals(_instance, this)) _instance = null;
            if (errors.Count > 0)
                throw new AggregateException("Failed to release LingArchitecture resources.", errors);
        }

        protected override void OnModuleInitialized(ICanInit module)
        {
            // Initial startup has a shared late-init phase; hot registration completes here.
            if (IsInitialized && module is ILateInit lateInit) lateInit.LateInit();
        }

        protected override void OnRegisterModel<TModel>(TModel model)
        {
            GetComponent(model);
        }
        protected override void OnRegisterSystem<TSystem>(TSystem system)
        {
            GetComponent(system);
        }
        void GetComponent<TComponent>(TComponent component) where TComponent : IBelongToArchitecture, ICanInit
        {
            if (!AllComponent.Add(component)) return;
            if (component is ILateInit)
            {
                var lateInit = (ILateInit)component;
                _lateActive.Add(lateInit);
            }
            if (component is IWithGameObject)
            {
                var withGameObject = (IWithGameObject)component;
                _withGameObject.Add(withGameObject);
                withGameObject.InstantiateDefaultGameObject();
            }
            if (component is IWithLoader)
            {
                var withLoader = (IWithLoader)component;
                _withLoader.Add(withLoader);
                withLoader.AllocateLoader();
            }
            if (component is IUpdate)
            {
                _updates.Add(component as IUpdate);
            }
            if (component is IFixedUpdate)
            {
                _fixedUpdates.Add(component as IFixedUpdate);
            }
            if (component is ILateUpdate)
            {
                _lateUpdates.Add(component as ILateUpdate);
            }
        }

        void ILingArchitecture.Update()
        {
            if (!IsInitialized) return;
            foreach (var updater in _updates)
            {
                updater.Update();
            }
        }
        void ILingArchitecture.FixedUpdate()
        {
            if (!IsInitialized) return;
            foreach (var updater in _fixedUpdates)
            {
                updater.FixedUpdate();
            }
        }
        void ILingArchitecture.LateUpdate()
        {
            if (!IsInitialized) return;
            foreach (var updater in _lateUpdates)
            {
                updater.LateUpdate();
            }
        }

        public override TSystem GetSystem<TSystem>()
        {
            if (TryGetSystem<TSystem>(out var result))
            {
                return result;
            }
            return null;
        }
        public override bool TryGetSystem<TSystem>(out TSystem result)
        {
            result = null;
            if (!LingArchitectureLookup<TSystem>.Enter(this)) return false;
            try
            {
                if (Container.TryGet<TSystem>(out result)) return true;
                foreach (var item in CitationArchitectures)
                {
                    if (item != null && item.TryGetSystem(out result)) return true;
                }
                return false;
            }
            finally { LingArchitectureLookup<TSystem>.Exit(this); }
        }

        public override TModel GetModel<TModel>()
        {
            if (TryGetModel<TModel>(out var result))
            {
                return result;
            }
            return null;
        }
        public override bool TryGetModel<TModel>(out TModel result)
        {
            result = null;
            if (!LingArchitectureLookup<TModel>.Enter(this)) return false;
            try
            {
                if (Container.TryGet<TModel>(out result)) return true;
                foreach (var item in CitationArchitectures)
                {
                    if (item != null && item.TryGetModel(out result)) return true;
                }
                return false;
            }
            finally { LingArchitectureLookup<TModel>.Exit(this); }
        }

        public override TUtility GetUtility<TUtility>()
        {
            if (TryGetUtility<TUtility>(out var result))
            {
                return result;
            }
            return null;
        }
        public override bool TryGetUtility<TUtility>(out TUtility result)
        {
            result = null;
            if (!LingArchitectureLookup<TUtility>.Enter(this)) return false;
            try
            {
                if (Container.TryGet<TUtility>(out result)) return true;
                foreach (var item in CitationArchitectures)
                {
                    if (item != null && item.TryGetUtility(out result)) return true;
                }
                return false;
            }
            finally { LingArchitectureLookup<TUtility>.Exit(this); }
        }
    }

    // Shared across architecture types so A -> B -> A cannot recurse indefinitely.
    internal static class LingArchitectureLookup<TModule>
    {
        [ThreadStatic] private static HashSet<ILingArchitecture> _visiting;

        internal static bool Enter(ILingArchitecture architecture)
        {
            if (_visiting == null) _visiting = new HashSet<ILingArchitecture>();
            return _visiting.Add(architecture);
        }

        internal static void Exit(ILingArchitecture architecture) => _visiting.Remove(architecture);
    }

}