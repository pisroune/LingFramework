using ParadoxNotion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using VInspector.Libs;
using Object = UnityEngine.Object;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("12.Ling", "ResLoader", 10)]
    [APIDescriptionCN("针对 QFramework.ResLoader 提供的链式扩展，提供QFramework没有提供的一些资源加载方法")]
    [APIDescriptionEN("针对 QFramework.ResLoader 提供的链式扩展，提供QFramework没有提供的一些资源加载方法")]
#endif
    public static class ResLoaderExtension
    {
        private static Type ComponentType = typeof(Component);
        private static Type GameObjectType = typeof(GameObject);


#if UNITY_EDITOR
        // v1 No.5
        [MethodAPI]
        [APIDescriptionCN("加载一个bundle下的全部资源")]
        [APIDescriptionEN("加载一个bundle下的全部资源")]
        [APIExampleCode(@"self.LoadAllSync<T>(bundleName);")]
#endif
        public static T[] LoadAllSync<T>(this IResLoader self, string bundleName) where T : Object
        {
            var type = typeof(T);
            if (AssetBundlePathHelper.SimulationMode)
            {
                List<T> tList = new List<T>();
#if UNITY_EDITOR
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
                foreach (var path in assetPaths)
                {
                    T asset = AssetBundlePathHelper.LoadAssetAtPath<T>(path);
                    if (asset != null)
                    {
                        tList.Add(asset);
                    }
                }
#endif
                return tList.ToArray();
            } 
            else
            {
                var resSearchKeys = ResSearchKeys.Allocate(bundleName, null, typeof(AssetBundle));
                var abR = self.LoadResSync(resSearchKeys) as AssetBundleRes;
                resSearchKeys.Recycle2Cache();

                if (abR == null || !abR.AssetBundle)
                {
                    Debug.LogError("Failed to Load Asset, Not Find AssetBundleImage:" + bundleName);
                    return null;
                }

                return abR.AssetBundle.LoadAllAssets<T>();
            }
        }

        public static void Add2LoadAll<T>(this IResLoader self, string bundleName, Action<bool, T[]> listener = null) where T : Object
        {
            T[] result = self.LoadAllSync<T>(bundleName);
            if (result.IsNotNull())
            {
                listener(true, result);
            }
            else
            {
                listener(false, result);
            }
        }
    }
}