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
            Loader.Recycle2Cache(); 
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
            foreach (var item in _withGameObject)
            {
                item.InstantiateDefaultGameObject();
            }
            foreach (var item in _withLoader)
            {
                item.AllocateLoader();
            }
            foreach (var item in _lateActive)
            {
                item.LateInit();
            }
            ActionKit.OnUpdate.Register(_instance.Update);
            ActionKit.OnFixedUpdate.Register(_instance.FixedUpdate);
            ActionKit.OnLateUpdate.Register(_instance.LateUpdate);
            Debug.Log("初始化架构：" + this.GetType().Name);
        }
        protected abstract void OnInitalized();

        protected override void OnDeinit()
        {
            foreach (var withGameObject in _withGameObject)
            {
                withGameObject.DestroyDefaultGameObject();
            }
            foreach (var item in _withLoader)
            {
                item.RemoveLoader();
            }
            ActionKit.OnUpdate.UnRegister(_instance.Update);
            ActionKit.OnFixedUpdate.UnRegister(_instance.FixedUpdate);
            ActionKit.OnLateUpdate.UnRegister(_instance.LateUpdate);
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
            AllComponent.Add(component);
            if (component is ILateInit)
            {
                var lateInit = (ILateInit)component;
                _lateActive.Add(lateInit);
                if (component.Initialized)
                {
                    lateInit.LateInit();
                }
            }
            if (component is IWithGameObject)
            {
                var withGameObject = (IWithGameObject)component;
                _withGameObject.Add(withGameObject);
                if (component.Initialized)
                {
                    withGameObject.InstantiateDefaultGameObject();
                }
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
            foreach (var updater in _updates)
            {
                updater.Update();
            }
        }
        void ILingArchitecture.FixedUpdate()
        {
            foreach (var updater in _fixedUpdates)
            {
                updater.FixedUpdate();
            }
        }
        void ILingArchitecture.LateUpdate()
        {
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
            if (Container.TryGet<TSystem>(out result))
            {
                return true;
            }

            foreach (var item in CitationArchitectures)
            {
                if (item.TryGetSystem(out result))
                {
                    return true;
                }
            }
            result = null;
            return false;
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
            if (Container.TryGet<TModel>(out result))
            {
                return true;
            }

            foreach (var item in CitationArchitectures)
            {
                if (item.TryGetModel(out result))
                {
                    return true;
                }
            }
            result = null;
            return false;
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
            foreach (var item in CitationArchitectures)
            {
                if (item.TryGetUtility(out result))
                {
                    return true;
                }
            }
            result = null;
            return false;
        }
    }
}