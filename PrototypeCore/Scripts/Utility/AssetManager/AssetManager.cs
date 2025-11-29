using Prototype;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype
{
    public class AssetManager
    {
        private static ILoader m_Loader;

        static AssetManager()
        {
            m_Loader = new ResourcesLoader();
        }

        //加载音频资源
        public static AudioClip LoadAudio(string path, bool cache = false)
        {
            AudioClip clip = m_Loader.LoadAudio(path, cache);
            if (clip == null)
            {
                Debug.LogError("资源加载失败，路径为：" + path);
            }
            return clip;
        }

        //加载预制体
        public static GameObject LoadPrefab(string path, bool cache = false)
        {
            GameObject go = m_Loader.LoadPrefab(path, cache);
            if (go == null)
            {
                Debug.LogError("资源加载失败，路径为：" + path);
            }
            return go;
        }

        //精灵
        public static Sprite LoadSprite(string path, bool cache = false)
        {
            Sprite sprite = m_Loader.LoadSprite(path, cache);
            if (sprite == null)
            {
                Debug.LogError("资源加载失败，路径为：" + path);
            }
            return sprite;
        }

        //贴图
        public static Texture LoadTexture(string path, bool cache = false)
        {
            Texture texture = m_Loader.LoadTexture(path, cache);
            if (texture == null)
            {
                Debug.LogError("资源加载失败，路径为：" + path);
            }
            return texture;
        }

        //加载泛型类
        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            T t = m_Loader.Load<T>(path);
            if (t == null)
            {
                Debug.LogError("资源加载失败，路径为：" + path);
            }
            return t;
        }

        //加载泛型类
        public static T[] LoadAll<T>(string path) where T : UnityEngine.Object  //文件夹的路径
        {
            return m_Loader.LoadAll<T>(path);
        }

        //加载xml配置文件
        private static XmlDocument LoadXml(string path)
        {
            XmlDocument document = m_Loader.LoadXml(path);
            if (document == null)
            {
                Debug.LogError("资源加载失败，路径为：" + path);
            }
            return document;
        }
    }
}