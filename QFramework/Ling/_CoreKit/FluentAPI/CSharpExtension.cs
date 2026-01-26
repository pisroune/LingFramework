using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("12.Ling", "CSharpExtensions", 0)]
    [APIDescriptionCN("针对 System.Collections 提供的链式扩展，理论上任何集合都可以使用")]
    [APIDescriptionEN("The chain extension provided by System.Collections can theoretically be used by any collection")]
#endif
    public static class CSharpExtension
    {

#if UNITY_EDITOR
        // v1 No.5
        [MethodAPI]
        [APIDescriptionCN("返回List中的随机一项元素")]
        [APIDescriptionEN("返回List中的随机一项元素")]
        [APIExampleCode(@"tList.GetRandom();")]
#endif
        public static T GetRandom<T>(this List<T> selfList)
        {
            return selfList[UnityEngine.Random.Range(0, selfList.Count - 1)];
        }

#if UNITY_EDITOR
        // v1 No.5
        [MethodAPI]
        [APIDescriptionCN("返回数组中的随机一项元素")]
        [APIDescriptionEN("返回数组中的随机一项元素")]
        [APIExampleCode(@"tArray.GetRandom();")]
#endif
        public static T GetRandom<T>(this T[] selfList)
        {
            return selfList[UnityEngine.Random.Range(0, selfList.Length - 1)];
        }


#if UNITY_EDITOR
        // v1 No.5
        [MethodAPI]
        [APIDescriptionCN("返回字典中的随机一项元素")]
        [APIDescriptionEN("返回字典中的随机一项元素")]
        [APIExampleCode(@"selfDict.GetRandom();")]
#endif
        public static KeyValuePair<T, Y> GetRandom<T, Y>(this Dictionary<T, Y> selfDict)
        {
            List<KeyValuePair<T, Y>> pairs = new List<KeyValuePair<T, Y>>();
            foreach (var item in selfDict)
            {
                pairs.Add(item);
            }
            return pairs.GetRandom();
        }


        #if UNITY_EDITOR
        // v1 No.5
        [MethodAPI]
        [APIDescriptionCN("克隆List")]
        [APIDescriptionEN("克隆List")]
        [APIExampleCode(@"aList.Copy();")]
#endif
        public static List<T> Copy<T>(this List<T> aList)
        {
            List<T> tList = new List<T>();
            int tCount = aList.Count;
            for (int i = 0; i < tCount; ++i)
                tList.Add(aList[i]);
            return tList;
        }



#if UNITY_EDITOR
        // v1 No.5
        [MethodAPI]
        [APIDescriptionCN("在列表中按某个键(selector)查找第一个匹配项，返回匹配到的元素")]
        [APIDescriptionEN("在列表中按某个键(selector)查找第一个匹配项，返回匹配到的元素")]
        [APIExampleCode(@"source.TryFindByKey<aClass, int>(0, item => item.PresetID, out aClass item);")]
#endif
        public static bool TryFindByKey<T, TKey>(this IEnumerable<T> source, TKey key, Func<T, TKey> keySelector,
            out T item, IEqualityComparer<TKey> comparer = null)
        {
            comparer ??= EqualityComparer<TKey>.Default;
            foreach (var t in source)
            {
                if (comparer.Equals(key, keySelector(t)))
                {
                    item = t;
                    return true;
                }
            }

            item = default;
            return false;
        }

#if UNITY_EDITOR
        // v1 No.5
        [MethodAPI]
        [APIDescriptionCN("在列表中按某个键(selector)查找第一个匹配项，并返回匹配到的元素索引")]
        [APIDescriptionEN("在列表中按某个键(selector)查找第一个匹配项，并返回匹配到的元素索引")]
        [APIExampleCode(@"")]
#endif
        public static int IndexOfByKey<T, TKey>(this IList<T> list, TKey key, Func<T, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null)
        {
            comparer ??= EqualityComparer<TKey>.Default;
            for (int i = 0; i < list.Count; i++)
                if (comparer.Equals(key, keySelector(list[i])))
                    return i;
            return -1;
        }

#if UNITY_EDITOR
        // v1 No.5
        [MethodAPI]
        [APIDescriptionCN("在列表中按某个键(selector)查找匹配项，返回是否存在")]
        [APIDescriptionEN("在列表中按某个键(selector)查找匹配项，返回是否存在")]
        [APIExampleCode(@"")]
#endif
        public static bool ContainsKey<T, TKey>(this IEnumerable<T> source, TKey key, Func<T, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null)
        {
            return TryFindByKey(source, key, keySelector, out _, comparer);
        }
#if UNITY_EDITOR
        // v1 No.5
        [MethodAPI]
        [APIDescriptionCN("在列表中按某个键(selector)查找第一个匹配项，返回匹配到的所有元素")]
        [APIDescriptionEN("在列表中按某个键(selector)查找第一个匹配项，返回匹配到的所有元素")]
        [APIExampleCode(@"")]
#endif
        public static List<T> FindAllByKey<T, TKey>(this IEnumerable<T> source, TKey key, Func<T, TKey> keySelector, 
            IEqualityComparer<TKey> comparer = null)
        {
            comparer ??= EqualityComparer<TKey>.Default;
            var result = new List<T>();
            foreach (var t in source)
                if (comparer.Equals(key, keySelector(t)))
                    result.Add(t);
            return result;
        }


#if UNITY_EDITOR
        // v1 No.5
        [MethodAPI]
        [APIDescriptionCN("从列表中移除存在某个键(selector)的匹配项")]
        [APIDescriptionEN("从列表中移除存在某个键(selector)的匹配项")]
        [APIExampleCode(@"")]
#endif
        public static bool TryRemoveByKey<T, TKey>(this IList<T> list, TKey key, Func<T, TKey> keySelector, out T removed, IEqualityComparer<TKey> comparer = null)
        {
            comparer ??= EqualityComparer<TKey>.Default;
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[i];
                if (comparer.Equals(key, keySelector(t)))
                {
                    list.RemoveAt(i);
                    removed = t;
                    return true;
                }
            }
            removed = default;
            return false;
        }
    }
}