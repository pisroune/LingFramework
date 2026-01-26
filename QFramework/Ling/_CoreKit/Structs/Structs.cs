using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{

    public struct FloatValue
    {
        public float Value
        {
            get
            {
                if (NoValue)
                {
                    Debug.LogError("无值，无法Get");
                    return 0;
                }

                return _value;
            }
        }

        public bool Infinity { get; set; }
        public bool HasValue { get; set; }
        public bool NormalValue => HasValue && !Infinity;

        public bool NoValue
        {
            get { return !HasValue; }
        }

        float _value;

        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="value"></param>
        public FloatValue(float value)
        {
            _value = value;
            Infinity = false;
            HasValue = true;
        }

        /// <summary>
        /// true => 有值，无限
        /// false => 无值
        /// </summary>
        /// <param name="inf"></param>
        public FloatValue(bool inf)
        {
            _value = 0;
            Infinity = inf;
            HasValue = inf;
        }

        // 重载 "+"
        public static FloatValue operator +(FloatValue a, float b)
        {
            if (a.NoValue)
            {
                Debug.LogWarning(string.Format("你对{0}进行了无效计算，因为他没有值。", a));
                return a;
            }

            if (a.Infinity)
            {
                Debug.LogWarning(string.Format("你对{0}进行了无效计算，因为他是Infinity的。", a));
                return a;
            }

            return new FloatValue(a.Value + b);
        }

        // 重载 "-"
        public static FloatValue operator -(FloatValue a, float b)
        {
            if (a.NoValue)
            {
                Debug.LogWarning(string.Format("你对{0}进行了无效计算，因为他没有值。", a));
                return a;
            }

            if (a.Infinity)
            {
                Debug.LogWarning(string.Format("你对{0}进行了无效计算，因为他是Infinity的。", a));
                return a;
            }

            return new FloatValue(a.Value - b);
        }

        // 定义从 int 到 StructA 的隐式转换
        public static implicit operator FloatValue(float val)
        {
            return new FloatValue(val);
        }

        // 定义从 int 到 StructA 的隐式转换
        public static implicit operator FloatValue(bool val)
        {
            return new FloatValue(val);
        }


        public bool TryGetValue(out float value)
        {
            if (HasValue)
            {
                value = _value;
                return true;
            }

            value = 0;
            return false;
        }
    }

    public struct IntValue
    {
        int _value;
        public bool Infinity { get; set; }
        public bool HasValue { get; set; }
        public bool NormalValue => HasValue && !Infinity;

        public bool NoValue
        {
            get { return !HasValue; }
        }

        public int Value
        {
            get
            {
                if (NoValue)
                {
                    Debug.LogError("无值，无法Get");
                    return 0;
                }

                return _value;
            }
        }

        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="value"></param>
        public IntValue(int value)
        {
            _value = value;
            Infinity = false;
            HasValue = true;
        }

        /// <summary>
        /// true => 有值，无限
        /// false => 无值
        /// </summary>
        /// <param name="inf"></param>
        public IntValue(bool inf)
        {
            _value = 0;
            Infinity = inf;
            HasValue = inf;
        }

        // 重载 "+"
        public static IntValue operator +(IntValue a, int b)
        {
            if (a.NoValue)
            {
                Debug.LogWarning(string.Format("你对{0}进行了无效计算，因为他没有值。", a));
                return a;
            }

            if (a.Infinity)
            {
                Debug.LogWarning(string.Format("你对{0}进行了无效计算，因为他是Infinity的。", a));
                return a;
            }

            return new IntValue(a.Value + b);
        }

        // 重载 "-"
        public static IntValue operator -(IntValue a, int b)
        {
            if (a.NoValue)
            {
                Debug.LogWarning(string.Format("你对{0}进行了无效计算，因为他没有值。", a));
                return a;
            }

            if (a.Infinity)
            {
                Debug.LogWarning(string.Format("你对{0}进行了无效计算，因为他是Infinity的。", a));
                return a;
            }

            return new IntValue(a.Value - b);
        }

        // 定义从 int 到 StructA 的隐式转换
        public static implicit operator IntValue(int val)
        {
            return new IntValue(val);
        }

        // 定义从 int 到 StructA 的隐式转换
        public static implicit operator IntValue(bool val)
        {
            return new IntValue(val);
        }


        public bool TryGetValue(out int value)
        {
            if (HasValue)
            {
                value = _value;
                return true;
            }

            value = 0;
            return false;
        }
    }


    [System.Serializable]
    public struct Vector3Value
    {
        public bool HasValue { get; set; }
        public Vector3 Value { get; set; }

        public Vector3Value(Vector3 value)
        {
            HasValue = true;
            Value = value;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="autoHasValue">自动设HasValue为true</param>
        public void Set(Vector3 value)
        {
            Value = value;
            HasValue = true;
        }
    }


}