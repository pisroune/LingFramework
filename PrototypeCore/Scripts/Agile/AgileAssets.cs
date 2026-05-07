using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Prototype
{
    [CreateAssetMenu(fileName = "AgileAssets", menuName = ("Prototype/AgileAssets"), order = 0)]
    public class AgileAssets : ScriptableObject
    {
        Dictionary<string, AgileAudioClip> _audioClipDicts;
        Dictionary<string, AgileGameObject> _gameObjects;
        Dictionary<string, AgileVfx> _vfx;

        [Title("音频资源")]
        public AgileAudioClip[] AgileAudioClips;
        [Title("预制体资源")]
        public AgileGameObject[] AgileGameObjects;
        [Title("特效资源")]
        public AgileVfx[] AgileVfx;

        public static AgileAssets Instance;

        [RuntimeInitializeOnLoadMethod]
        static void InitGameSetting()
        {
            Instance = AssetManager.Load<AgileAssets>("ScriptableObject/AgileAssets");
            Instance.InitInstance();
        }
        void InitInstance()
        {
            Instance._audioClipDicts = new Dictionary<string, AgileAudioClip>();
            Instance._gameObjects = new Dictionary<string, AgileGameObject>();
            Instance._vfx = new Dictionary<string, AgileVfx>();

            foreach (var item in AgileAudioClips)
            {
                Instance._audioClipDicts.TryAdd(item.name, item);
            }
            foreach (var item in AgileGameObjects)
            {
                Instance._gameObjects.TryAdd(item.name, item);
            }
            foreach (var item in AgileVfx)
            {
                Instance._vfx.TryAdd(item.name, item);
            }
        }

        public static void PlayAudio(string key, Vector3 position = default)
        {
            if (Instance._audioClipDicts.TryGetValue(key, out var value))
            {
                value.Play(position);
                return;
            }
            Debug.LogError("未找到：" + key);
            return;
        }
        public static GameObject GetPrefab(string key)
        {
            if (Instance._gameObjects.TryGetValue(key, out var value))
            {
                return value.GetPrefab();
            }
            Debug.LogError("未找到：" + key);
            return null;
        }
        public static T GetPrefab<T>(string key) where T : Component, IPoolablePrefab
        {
            if (Instance._gameObjects.TryGetValue(key, out var value))
            {
                return value.GetPrefab<T>();
            }
            Debug.LogError("未找到：" + key);
            return null;
        }
        public static GameObject Spawn(string key, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (Instance._gameObjects.TryGetValue(key, out var value))
            {
                return value.Spawn(position, rotation, parent);
            }
            Debug.LogError("未找到：" + key);
            return null;
        }
        public static T Spawn<T>(string key, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component, IPoolablePrefab
        {
            if (Instance._gameObjects.TryGetValue(key, out var value))
            {
                return value.Spawn<T>(position, rotation, parent);
            }
            Debug.LogError("未找到：" + key);
            return null;
        }
        public static GameObject Generate(string key, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (Instance._gameObjects.TryGetValue(key, out var value))
            {
                return value.Generate(position, rotation, parent);
            }
            Debug.LogError("未找到：" + key);
            return null;
        }
        public static ParticleVfxBase PlayVfx(string key, VfxPlayArgs args)
        {
            if (Instance._vfx.TryGetValue(key, out var value))
            {
                return value.Play(args);
            }
            Debug.LogError("未找到：" + key);
            return null;
        }


        #region 注册

        [Button("统一注册")]
        public void Register()
        {
            RegisterAgileAudioClips();
            RegisterGameObjects();
            RegisterVfx();
        }
        public static void RegisterAgileAudioClips()
        {
            var assets = AssetManager.Load<AgileAssets>("ScriptableObject/AgileAssets");
            if (assets == null)
            {
                Debug.LogError("请创建AgileAssets");
                return;
            }

            assets.AgileAudioClips = AssetDatabase.FindAssets("t:AgileAudioClip")
           .Select(guid => AssetDatabase.LoadAssetAtPath<AgileAudioClip>(AssetDatabase.GUIDToAssetPath(guid)))
           .ToArray();
            EditorUtility.SetDirty(assets);
            Debug.Log("Register Agile Audio: Success");
        }
        public static void RegisterGameObjects()
        {
            var assets = AssetManager.Load<AgileAssets>("ScriptableObject/AgileAssets");
            if (assets == null)
            {
                Debug.LogError("请创建AgileAssets");
                return;
            }

            assets.AgileGameObjects = AssetDatabase.FindAssets("t:AgileGameObject")
           .Select(guid => AssetDatabase.LoadAssetAtPath<AgileGameObject>(AssetDatabase.GUIDToAssetPath(guid)))
           .ToArray();
            EditorUtility.SetDirty(assets);

            Debug.Log("Register Agile GameObjects: Success");
        }
        public static void RegisterVfx()
        {
            var assets = AssetManager.Load<AgileAssets>("ScriptableObject/AgileAssets");
            if (assets == null)
            {
                Debug.LogError("请创建AgileAssets");
                return;
            }

            assets.AgileVfx = AssetDatabase.FindAssets("t:AgileVfx")
           .Select(guid => AssetDatabase.LoadAssetAtPath<AgileVfx>(AssetDatabase.GUIDToAssetPath(guid)))
           .ToArray();
            EditorUtility.SetDirty(assets);

            Debug.Log("Register Agile Vfx: Success");
        }
        #endregion

    }
}