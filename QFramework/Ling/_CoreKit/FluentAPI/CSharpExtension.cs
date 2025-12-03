using QFramework;
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
            return selfList[Random.Range(0, selfList.Count - 1)];
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
            return selfList[Random.Range(0, selfList.Length - 1)];
        }
    }
}