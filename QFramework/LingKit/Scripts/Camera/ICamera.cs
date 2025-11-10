using UnityEngine;

namespace QFramework
{
    public interface ICamera
    {
        Transform ThisTrans { get; }

        void Enter(CameraBase.IInitValue initObj);
        void End();
    }
    public abstract class CameraBase : MonoBehaviour, ICamera
    {
        public interface IInitValue
        {
            string Name { get; }
        }
        public Transform ThisTrans => transform;

        public void Enter(IInitValue initObj)
        {
            gameObject.SetActive(true);
            OnEnter(initObj);
        }
        protected abstract void OnEnter(IInitValue initObj);

        public void End()
        {
            OnEnd();
            gameObject.DestroySelf();
        }
        protected abstract void OnEnd();
    }
}