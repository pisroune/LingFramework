using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// Same as SmartMovebehaviour in "The Patriotic War"
    /// Ensure that some logic is executed every few frames
    /// </summary>
    public class FrameTicker : MonoBehaviour
    {
        int _tickTimer;
        int _randomTime;
        float _deltaTime;

        public virtual int TickTimes => 10;
        public float DeltaTime => _deltaTime;

        private void Update()
        {
            OnUpdate();
            _tickTimer++;
            _deltaTime += Time.deltaTime;
            if (_tickTimer >= TickTimes + _randomTime)
            {
                OnTick(_tickTimer);
                _tickTimer = 0;
                _deltaTime = 0;
                _randomTime = Random.Range(-1, 2);
            }
            OnUpdateAfterTick();
        }
        protected virtual void OnUpdate() { }
        protected virtual void OnUpdateAfterTick() { }
        protected virtual void OnTick(int frame) { }
    }
}