using UnityEngine;

namespace Prototype
{
    public class ParticleVfxBase : MonoBehaviour, IPoolablePrefab
    {
        [Header("Particle root")]
        [SerializeField] protected ParticleSystem root;

        protected ParticleSystem[] systems;
        protected bool inPool;
        protected bool _isInstance = false;

        public bool IsAlive => root != null && root.IsAlive(true);

        protected virtual void Awake()
        {
            if (!root) root = GetComponentInChildren<ParticleSystem>(true);
            systems = GetComponentsInChildren<ParticleSystem>(true);

            // 设置回调停止动作（避免估时/轮询）
            foreach (var ps in systems)
            {
                var main = ps.main;
                main.stopAction = ParticleSystemStopAction.Callback;
            }
        }

        public ParticleVfxBase Play(in VfxPlayArgs args)
        {
            if (_isInstance)
            {
                throw new System.Exception("不能用实例创建，请用预制体");
            }
            var vfx = PoolManager.Instance.Spawn(this);
            vfx._isInstance = true;
            return vfx.PlayVfx(args);
        }
        ParticleVfxBase PlayVfx(in VfxPlayArgs args)
        {
            inPool = false;
            transform.SetParent(args.parent, worldPositionStays: args.parent != null);
            transform.SetPositionAndRotation(args.position, args.rotation);
            transform.localScale = Vector3.one * args.scale;

            gameObject.SetActive(true);

            ClearAll();               // 防止残影

            foreach (var ps in systems)
                ps.Play(withChildren: true);
            return this;
        }

        public virtual void Stop(bool immediate = false)
        {
            foreach (var ps in systems)
            {
                ps.Stop(withChildren: true,
                    immediate ? ParticleSystemStopBehavior.StopEmittingAndClear
                              : ParticleSystemStopBehavior.StopEmitting);
            }

            if (immediate)
                RequestRecycle();
        }


        protected void ClearAll()
        {
            foreach (var ps in systems)
                ps.Clear(withChildren: true);
        }


        // 粒子停止回调：可能多个子系统都会触发，因此用 root.IsAlive(true) 做总判断
        protected virtual void OnParticleSystemStopped()
        {
            if (inPool)
            {
                return;
            }
            if (IsAlive)
            {
                return;
            }


            RequestRecycle();
        }

        // 由子类/池决定怎么回收
        protected void RequestRecycle()
        {
            OnRequestRecycle();
            PoolManager.Instance.Despawn(this);
        }
        protected virtual void OnRequestRecycle() { }




        void IPoolablePrefab.OnDespawn()
        {
            inPool = true;
        }

        void IPoolablePrefab.OnSpawn()
        {
            inPool = false;
        }
    }
}