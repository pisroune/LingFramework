using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// Close as SmartMovebehaviour in "The Patriotic War"
    /// Ensure that some logic is executed every few seconds
    /// </summary>
    public class TimeTicker : MonoBehaviour
    {
        float _randomTime;
        float _deltaTime;

        public virtual float TickTimes => 0.2f;
        public float DeltaTime => _deltaTime;

        private void Update()
        {
            OnUpdate();
            _deltaTime += Time.deltaTime;
            float currentDeltaTime = TickTimes * _randomTime;
            if (_deltaTime >= currentDeltaTime)
            {
                _deltaTime -= currentDeltaTime;
                OnTick(currentDeltaTime);
                _deltaTime = 0;
                _randomTime = Random.Range(0.9f, 1.1f);
            }
            OnUpdateAfterTick();
        }
        protected virtual void OnUpdate() { }
        protected virtual void OnUpdateAfterTick() { }
        protected virtual void OnTick(float deltaTime) { }
    }
}