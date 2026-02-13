using Project_TankSchool;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 打点计时器类
    /// 可以按固定时间间隔回调委托
    /// 方便快速地申请或注销
    /// </summary>
    public static class TimerKit
    {
        static TimerKit()
        {
            ActionKit.OnUpdate.Register(Update);
        }

        static List<CyclicTimer> _removingTimers = new List<CyclicTimer>();
        static Dictionary<int, CyclicTimer> _cyclicTimers = new Dictionary<int, CyclicTimer>();

        /// <summary>
        /// 申请一个打点计时器
        /// 按ratePerSec的频率回调委托
        /// </summary>
        /// <param name="ratePerSec">每秒次数</param>
        /// <returns></returns>
        public static CyclicTimer RegisterBySecond(int ratePerSec, System.Action<float> onTick)
        {
            int ratePerMin = ratePerSec * 60;
            if (_cyclicTimers.TryGetValue(ratePerMin, out var timer))
            {
                timer.Register(onTick);
                return timer;
            }
            else
            {
                CyclicTimer newTimer = new CyclicTimer(ratePerMin);
                _cyclicTimers.Add(ratePerMin, newTimer);
                newTimer.Register(onTick);
                return newTimer;
            }
        }

        /// <summary>
        /// 申请一个打点计时器
        /// 按ratePerMin的频率回调委托
        /// </summary>
        /// <param name="ratePerMin">每分钟次数</param>
        /// <returns></returns>
        public static CyclicTimer RegisterByMinute(int ratePerMin, System.Action<float> onTick)
        {
            if (_cyclicTimers.TryGetValue(ratePerMin, out var timer))
            {
                timer.Register(onTick);
                return timer;
            }
            else
            {
                CyclicTimer newTimer = new CyclicTimer(ratePerMin);
                _cyclicTimers.Add(ratePerMin, newTimer);
                newTimer.Register(onTick);
                return newTimer;
            }
        }

        /// <summary>
        /// 注销一个计时器委托
        /// </summary>
        public static void UnregisterBySecond(int ratePerSec, System.Action<float> onTick, bool requireReceiver = true)
        {
            int ratePerMin = ratePerSec * 60;
            if (_cyclicTimers.TryGetValue(ratePerMin, out var timer))
            {
                Unregister(timer, onTick);
            }
            else if(requireReceiver)
            {
                Debug.LogError("没有找到对应的计时器，请检查");
            }
        }

        /// <summary>
        /// 注销一个计时器委托
        /// </summary>
        public static void UnregisterByMinute(int ratePerMin, System.Action<float> onTick, bool requireReceiver = true)
        {
            if (_cyclicTimers.TryGetValue(ratePerMin, out var timer))
            {
                Unregister(timer, onTick);
            }
            else if (requireReceiver)
            {
                Debug.LogError("没有找到对应的计时器，请检查");
            }
        }

        /// <summary>
        /// 注销一个计时器委托
        /// </summary>
        public static void Unregister(this CyclicTimer timer, System.Action<float> onTick)
        {
            timer.Unregister(onTick);
            if (timer.IsNull)
            {
                _removingTimers.Add(timer);
            }
        }

        static void Update()
        {
            _removingTimers.ForEachReverse((timer) =>
            {
                _cyclicTimers.Remove(timer.RatePerMin);
            });
            _removingTimers.Clear();

            foreach (var timer in _cyclicTimers.Values)
            {
                timer.Update();
            }
        }
    }

    public class CyclicTimer
    {
        float _tickInterval;      // 单次 Tick 的时间间隔：1 / tickRate
        float _accumulator;       // 时间累加器，单位秒
        float _lastTickTime = -1; // 上一次 Tick 时的时间戳（Time.time），-1 表示尚未 Tick 过

        //逻辑 Tick 频率（单位：次/秒），例如 30 表示 30Hz。
        public float TickRate;

        //缓存 int 型，用于字典查找
        public int RatePerMin;

        //单帧最多补多少次 Tick，防止极端掉帧导致 while 循环过长。
        public int MaxTicksPerFrame;

        public EasyEvent<float> _onTick = new EasyEvent<float>();

        public bool IsNull => _onTick.IsNull;
        public CyclicTimer(int ratePerMin, int maxTicksPerFrame = 5)
        {
            RatePerMin = ratePerMin;
            TickRate = ratePerMin / 60f;
            if (TickRate > 240)
            {
                Debug.LogError("TickRate不能超过240hz，已重置到240");
            }
            if (TickRate < 0.01f)
            {
                Debug.LogError("Tick不能低于0.01hz，已重置到0.01");
            }
            TickRate = Mathf.Clamp(TickRate, 0.01f, 240);
            MaxTicksPerFrame = maxTicksPerFrame;
            _tickInterval = 1f / TickRate;
        }

        public void Update()
        {
            _accumulator += Time.deltaTime;

            int ticksThisFrame = 0;
            while (_accumulator >= _tickInterval && ticksThisFrame < MaxTicksPerFrame)
            {
                _accumulator -= _tickInterval;
                DispatchTick(ticksThisFrame);
                ticksThisFrame++;
            }
        }

        public IUnRegister Register(System.Action<float> onTick)
        {
            return _onTick.Register(onTick);
        }

        public void Unregister(System.Action<float> onTick)
        {
            _onTick.UnRegister(onTick);
        }


        /// <summary>
        /// 发送一次 Tick。
        /// ticksAlreadyInThisFrame：本帧已经发送了多少次 Tick。
        /// </summary>
        private void DispatchTick(int ticksAlreadyInThisFrame)
        {
            float now = Time.time;   // 如需不受 timeScale 影响可改 Time.unscaledTime
            float delta;

            if (_lastTickTime < 0f)
            {
                // 第一次 Tick，没有“上一帧”，可以认为间隔为 0 或 _tickInterval
                delta = 0f;
            }
            else
            {
                delta = now - _lastTickTime;
            }

            // 如果同一帧内已经Tick过，则间隔按0
            if (ticksAlreadyInThisFrame > 0)
            {
                delta = 0f;
            }

            _lastTickTime = now;
            _onTick.Trigger(delta);
            //TickEvent.Trigger(delta);
        }
    }
}