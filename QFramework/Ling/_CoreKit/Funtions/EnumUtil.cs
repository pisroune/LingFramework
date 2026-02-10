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