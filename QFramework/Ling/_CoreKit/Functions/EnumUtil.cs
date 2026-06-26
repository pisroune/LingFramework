
using System;
using System.Collections.Generic;

namespace QFramework
{
    public static class EnumUtil
    {
        private static class Cache<T> where T : struct, Enum
        {
            public static readonly T[] Values = (T[])Enum.GetValues(typeof(T));
        }

        /// <summary>
        /// 从指定枚举项开始，循环遍历所有项（不含起始项本身）
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="start">起始枚举项</param>
        /// <returns>从下一项开始，循环一圈回到起始项前的所有项</returns>
        public static IEnumerable<TEnum> LoopFrom<TEnum>(TEnum start) where TEnum : struct, Enum
        {
            TEnum[] allValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            int total = allValues.Length;
            int startIndex = Array.IndexOf(allValues, start);

            if (startIndex < 0)
                throw new ArgumentException($"枚举值 {start} 不存在", nameof(start));

            // 从 startIndex 开始，循环 total 次（包含起始项本身，共一整圈）
            for (int i = 0; i < total; i++)
            {
                int currentIndex = (startIndex + i) % total;
                yield return allValues[currentIndex];
            }
        }

        /// <summary>
        /// 随机一个枚举元素
        /// </summary>
        /// <param name="excluded">排除</param>
        /// <returns></returns>
        public static T RandomEnumValue<T>(ISet<T> excluded = null) where T : struct, Enum
        {
            var values = Cache<T>.Values;

            int availableCount = 0;
            for (int i = 0; i < values.Length; i++)
                if (excluded == null || !excluded.Contains(values[i]))
                    availableCount++;

            if (availableCount == 0)
                throw new InvalidOperationException($"No available values in enum {typeof(T).Name} after exclusions.");

            int k = UnityEngine.Random.Range(0, availableCount);
            for (int i = 0; i < values.Length; i++)
            {
                var v = values[i];
                if (excluded != null && excluded.Contains(v)) continue;

                if (k == 0) return v;
                k--;
            }
            return values[0];
        }

        /// <summary>
        /// weights: 每个枚举值对应一个非负权重。未出现在字典里的权重视为 0。
        /// excluded: 排除项
        /// </summary>
        public static T RandomWeighted<T>(IReadOnlyDictionary<T, float> weights, ISet<T> excluded = null)
            where T : struct, Enum
        {
            if (weights == null) throw new ArgumentNullException(nameof(weights));

            var values = (T[])Enum.GetValues(typeof(T));

            float total = 0f;
            for (int i = 0; i < values.Length; i++)
            {
                var v = values[i];
                if (excluded != null && excluded.Contains(v)) continue;

                if (weights.TryGetValue(v, out var w) && w > 0f)
                    total += w;
            }

            if (total <= 0f)
                throw new InvalidOperationException($"Total weight is 0 for enum {typeof(T).Name} after exclusions.");

            float r = UnityEngine.Random.value * total;
            for (int i = 0; i < values.Length; i++)
            {
                var v = values[i];
                if (excluded != null && excluded.Contains(v)) continue;

                if (!weights.TryGetValue(v, out var w) || w <= 0f) continue;

                r -= w;
                if (r <= 0f) return v;
            }

            // 浮点误差兜底：返回最后一个有权重的
            for (int i = values.Length - 1; i >= 0; i--)
            {
                var v = values[i];
                if (excluded != null && excluded.Contains(v)) continue;
                if (weights.TryGetValue(v, out var w) && w > 0f) return v;
            }

            return values[0];
        }
    }
}