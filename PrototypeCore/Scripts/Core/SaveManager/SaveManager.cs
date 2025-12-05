using Newtonsoft.Json;
using Prototype.Loader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Prototype
{
    public static class SaveManager
    {
        public static int CurrentSlotID { get; private set; } = 0;

        // 缓存 JsonSerializerSettings，避免每次调用都重新创建
        private static JsonSerializerSettings _jsonSettings;

        static SaveManager()
        {
            InitializeSettings();
        }

        private static void InitializeSettings()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                // 格式化输出 (Pretty Print)，方便调试时阅读 Json 文件
                Formatting = Formatting.Indented,

                // 【关键】自动处理多态类型
                // 只有当申明类型与实际类型不一致时（例如 List<BaseClass> 存了 SubClass），才会写入 $type
                TypeNameHandling = TypeNameHandling.Auto,

                // 忽略循环引用，防止对象互相引用导致堆栈溢出
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

                // 处理空值
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public static string GetSavePath(string baseKey)
        {
            return Path.Combine(Application.persistentDataPath, GetFullFileName(baseKey));
        }

        public static string GetFullFileName(string baseKey)
        {
            return GetPrefixedKey(baseKey) + ".json";
        }

        public static string GetSearchPattern()
        {
#if UNITY_EDITOR
            return $"Slot{CurrentSlotID}_*.json";
#else
            return $"SaveFile_*.json";
#endif
        }
        public static string GetPrefixedKey(string baseKey)
        {
#if UNITY_EDITOR
            return $"EditorSave{CurrentSlotID}_{baseKey}";
#else
            return $"SaveFile_{baseKey}";
#endif
        }

        public static void SwitchToSlot(int slotID)
        {
            if (slotID <= 0)
            {
                Debug.LogError("槽位ID必须大于0！");
                return;
            }

            CurrentSlotID = slotID;
            Debug.LogWarning($"[开发功能] 已切换到存档槽位: {CurrentSlotID}。请手动重置相关模块以加载新数据。");
        }

        /// <summary>
        /// 保存数据到文件
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">数据对象</param>
        /// <param name="fileName">文件名 (例如 "save.json")</param>
        public static void Save<T>(string key, T data)
        {
            try
            {
                // 1. 将对象转换为 Json 字符串
                string json = JsonConvert.SerializeObject(data, _jsonSettings);
                string filePath = GetSavePath(key);

                // 2. 写入文件
                File.WriteAllText(filePath, json);

                Debug.Log($"[SaveManager] 存档成功: {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 存档失败: {e.Message}");
            }
        }

        /// <summary>
        /// 从文件加载数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <returns>读取的对象，如果文件不存在则返回默认值或 null</returns>
        public static T Load<T>(string key) //where T : struct
        {
            TryGet<T>(key, out T value);
            if (value == null)
            {
                Debug.LogError($"加载错误，key为 {key}");
            }

            return value;
        }

        public static bool TryGet<T>(string baseKey, out T value) //where T : class
        {
            value = default(T);
            string jsonStr = null;
            string slotPrefixedKey = GetPrefixedKey(baseKey);
            string filePath = GetSavePath(baseKey);

            if (File.Exists(filePath))
            {
                try
                {
                    jsonStr = File.ReadAllText(filePath);
                }
                catch (Exception e) { Debug.LogError($"[JsonTool] 读取JSON文件失败，Key: '{baseKey}': {e.Message}"); }
            }


            if (string.IsNullOrEmpty(jsonStr)) return false;
            try
            {
                value = JsonConvert.DeserializeObject<T>(jsonStr, _jsonSettings);
                return value != null;
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonTool] 反序列化失败，Key: '{baseKey}'。错误: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 辅助工具：删除存档
        /// </summary>
        public static void DeleteSave(string fileName)
        {
            string path = GetSavePath(fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"[SaveManager] 已删除存档: {fileName}");
            }
        }

        public static void DeleteAll()
        {
            string savePath = Application.persistentDataPath;
            if (Directory.Exists(savePath))
            {
                foreach (var filePath in Directory.GetFiles(savePath, GetSearchPattern()))
                {
                    File.Delete(filePath);
                    Debug.Log($"已删除: {Path.GetFileName(filePath)}");
                }
            }
        }
    }
}