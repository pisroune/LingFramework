// ==========================================================================================
//  配置说明:
//  请在 Unity 的 "Player Settings" -> "Scripting Define Symbols" 中设置以下一个符号:
//
//  1. HYBRID_MIGRATION_MODE: (推荐的正式发行版模式) 兼容旧存档，写入新存档。
//  2. USE_DUAL_SAVE: (开发调试模式) 同时读写两种格式并校验一致性。
//  3. USE_JSON_FILE_SAVE: (纯净开发模式) 仅使用JSON文件。
//  4. (不设置任何符号): (旧版兼容模式) 完全使用PlayerPrefs。
// ==========================================================================================

using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


#if USE_JSON_FILE_SAVE || USE_DUAL_SAVE || HYBRID_MIGRATION_MODE || UNITY_EDITOR
using System.IO;
#endif

namespace Prototype.Loader
{
    public static class JsonTool
    {
        private const string MASTER_KEY_LIST_UNSLOTTED = "JsonToolMasterKeyList_Unslotted_DEPRECATED";

        #region 初始化与类型注册 (增强版)

        public static void Register()
        {
            RegisterUnityTypes();
            RegisterCustomTypes();
        }

        private static void RegisterUnityTypes()
        {
            // --- Type 处理器 ---
            JsonMapper.RegisterExporter<Type>((obj, writer) => writer.Write(obj.FullName));
            JsonMapper.RegisterImporter<string, Type>(input => Type.GetType(input));

            // --- Vector3 处理器 (已补充导入器) ---
            JsonMapper.RegisterExporter<Vector3>((obj, writer) =>
            {
                writer.WriteObjectStart();
                writer.WritePropertyName("x");
                writer.Write(obj.x);
                writer.WritePropertyName("y");
                writer.Write(obj.y);
                writer.WritePropertyName("z");
                writer.Write(obj.z);
                writer.WriteObjectEnd();
            });
            JsonMapper.RegisterImporter<JsonData, Vector3>(d =>
                new Vector3(Convert.ToSingle(d["x"]), Convert.ToSingle(d["y"]), Convert.ToSingle(d["z"])));

            // --- Vector2 处理器 (已补充导入器) ---
            JsonMapper.RegisterExporter<Vector2>((obj, writer) =>
            {
                writer.WriteObjectStart();
                writer.WritePropertyName("x");
                writer.Write(obj.x);
                writer.WritePropertyName("y");
                writer.Write(obj.y);
                writer.WriteObjectEnd();
            });
            JsonMapper.RegisterImporter<JsonData, Vector2>(d =>
                new Vector2(Convert.ToSingle(d["x"]), Convert.ToSingle(d["y"])));

            // --- Quaternion 处理器 (已补充导入器) ---
            JsonMapper.RegisterExporter<Quaternion>((obj, writer) =>
            {
                writer.WriteObjectStart();
                writer.WritePropertyName("x");
                writer.Write(obj.x);
                writer.WritePropertyName("y");
                writer.Write(obj.y);
                writer.WritePropertyName("z");
                writer.Write(obj.z);
                writer.WritePropertyName("w");
                writer.Write(obj.w);
                writer.WriteObjectEnd();
            });
            JsonMapper.RegisterImporter<JsonData, Quaternion>(d => new Quaternion(Convert.ToSingle(d["x"]),
                Convert.ToSingle(d["y"]), Convert.ToSingle(d["z"]), Convert.ToSingle(d["w"])));

            // --- GameObject 处理器 (保持原样) ---
            JsonMapper.RegisterExporter<GameObject>((obj, writer) =>
            {
                writer.WriteObjectStart();
                writer.WritePropertyName("name");
                writer.Write(obj != null ? obj.name : "Prefab为Null");
                writer.WriteObjectEnd();
            });

            // --- Resolution 处理器 (已修正和优化) ---
            JsonMapper.RegisterExporter<Resolution>((obj, writer) =>
            {
                writer.WriteObjectStart();
                writer.WritePropertyName("width");
                writer.Write(obj.width);
                writer.WritePropertyName("height");
                writer.Write(obj.height);
                writer.WritePropertyName("refreshRate");
                writer.Write(obj.refreshRateRatio.value);
                writer.WriteObjectEnd();
            });
            JsonMapper.RegisterImporter<JsonData, Resolution>(d =>
            {
                int savedWidth = (int)d["width"];
                int savedHeight = (int)d["height"];
                double savedRefreshRate = (double)d["refreshRate"];
                Resolution bestMatch = default;
                double minDiff = double.MaxValue;
                bool foundMatch = false;
                foreach (var supportedRes in Screen.resolutions)
                {
                    if (supportedRes.width == savedWidth && supportedRes.height == savedHeight)
                    {
                        foundMatch = true;
                        double diff = Math.Abs(supportedRes.refreshRateRatio.value - savedRefreshRate);
                        if (diff < minDiff)
                        {
                            minDiff = diff;
                            bestMatch = supportedRes;
                        }
                    }
                }

                return foundMatch ? bestMatch : Screen.currentResolution;
            });
        }

        private static void RegisterCustomTypes()
        {
            JsonMapper.RegisterExporter<Prototype.Test.Float3>((obj, writer) =>
            {
                writer.WriteObjectStart();
                writer.WritePropertyName("x");
                writer.Write(obj.x);
                writer.WritePropertyName("y");
                writer.Write(obj.y);
                writer.WritePropertyName("z");
                writer.Write(obj.z);
                writer.WriteObjectEnd();
            });
            JsonMapper.RegisterImporter<JsonData, Prototype.Test.Float3>(d =>
                new Prototype.Test.Float3(Convert.ToSingle(d["x"]), Convert.ToSingle(d["y"]), Convert.ToSingle(d["z"])));
        }

        #endregion

        #region 核心存档/读档 API (已包含所有模式)

        public static string JsonSave<T>(string baseKey, T t) where T : class
        {
            if (t is MonoBehaviour)
            {
                Debug.LogError("无法持久化MonoBehaviour数据");
                return null;
            }

            if (t == null)
            {
                Debug.LogError($"[JsonTool] 尝试使用键: {baseKey} 保存一个空对象");
                return null;
            }

            string jsonStr = JsonMapper.ToJson(t);
            string slotPrefixedKey = GetSlotPrefixedKey(baseKey);
            string filePath = GetSavePath(baseKey);

#if USE_DUAL_SAVE
            try
            {
                File.WriteAllText(filePath, jsonStr);
                PlayerPrefs.SetString(baseKey, jsonStr);
                PlayerPrefs.Save();
                string fileData = File.ReadAllText(filePath);
                string prefsData = PlayerPrefs.GetString(slotPrefixedKey);
                if (fileData != prefsData)
                {
                    Debug.LogError($"[DUAL_SAVE 数据不一致] Key: '{baseKey}'");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonTool] 双轨保存失败，Key: '{baseKey}': {e.Message}");
            }
#elif HYBRID_MIGRATION_MODE || USE_JSON_FILE_SAVE
            try
            {
                File.WriteAllText(filePath, jsonStr);
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonTool] 保存JSON文件失败，Key: '{baseKey}': {e.Message}");
            }
#else
            PlayerPrefs.SetString(baseKey, jsonStr);
            PlayerPrefs.Save();
#endif
            return jsonStr;
        }

        public static T JsonLoad<T>(string baseKey) where T : class
        {
            TryGet<T>(baseKey, out T value);
            if (value == null)
            {
                Debug.LogError($"加载错误，key为 {baseKey}");
            }

            return value;
        }

        public static bool TryGet<T>(string baseKey, out T value) where T : class
        {
            value = default(T);
            string jsonStr = null;
            string slotPrefixedKey = GetSlotPrefixedKey(baseKey);
            string filePath = GetSavePath(baseKey);

#if USE_DUAL_SAVE || HYBRID_MIGRATION_MODE
            if (File.Exists(filePath))
            {
                try
                {
                    jsonStr = File.ReadAllText(filePath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[JsonTool] 读取JSON文件失败，Key: '{baseKey}': {e.Message}");
                }
            }

            if (string.IsNullOrEmpty(jsonStr) && PlayerPrefs.HasKey(baseKey))
            {
                jsonStr = PlayerPrefs.GetString(baseKey);
            }
#elif USE_JSON_FILE_SAVE
            if (File.Exists(filePath)) { try { jsonStr =
 File.ReadAllText(filePath); } catch (Exception e) { Debug.LogError($"[JsonTool] 读取JSON文件失败，Key: '{baseKey}': {e.Message}"); } }
#else
            if (PlayerPrefs.HasKey(baseKey))
            {
                jsonStr = PlayerPrefs.GetString(baseKey);
            }
#endif

            if (string.IsNullOrEmpty(jsonStr)) return false;
            try
            {
                value = JsonMapper.ToObject<T>(jsonStr);
                return value != null;
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonTool] 反序列化失败，Key: '{baseKey}'。错误: {e.Message}");
                return false;
            }
        }

        #endregion

        #region 管理与辅助方法 (保留并增强)

        public static void JsonDelete(string baseKey)
        {
            string slotPrefixedKey = GetSlotPrefixedKey(baseKey);
            string filePath = GetSavePath(baseKey);
#if USE_DUAL_SAVE
            PlayerPrefs.DeleteKey(baseKey);
            if (File.Exists(filePath)) File.Delete(filePath);
#elif HYBRID_MIGRATION_MODE || USE_JSON_FILE_SAVE
            if (File.Exists(filePath)) File.Delete(filePath);
#else
            PlayerPrefs.DeleteKey(slotPrefixedKey);
#endif
        }

        public static void JsonDeleteAll()
        {
            // 注意：此方法现在只会删除当前激活槽位的存档
#if USE_DUAL_SAVE
            SaveSlotManager.DeleteSlot();
            PlayerPrefs.DeleteAll(); // 原始行为
#elif HYBRID_MIGRATION_MODE || USE_JSON_FILE_SAVE
            SaveSlotManager.DeleteSlot();
            
#else
            PlayerPrefs.DeleteAll(); // 原始行为
#endif
        }

        public static bool HasKey(string baseKey)
        {
            string slotPrefixedKey = GetSlotPrefixedKey(baseKey);
            string filePath = GetSavePath(baseKey);
#if USE_DUAL_SAVE || HYBRID_MIGRATION_MODE
            return File.Exists(filePath) || PlayerPrefs.HasKey(baseKey);
#elif USE_JSON_FILE_SAVE
            return File.Exists(filePath);
#else
            return PlayerPrefs.HasKey(baseKey);
#endif
        }

        public static string GetSavePath(string baseKey)
        {
            return SaveSlotManager.GetSlotFilePath(SaveSlotManager.CurrentSlotID, baseKey);
        }

        public static string GetSlotPrefixedKey(string baseKey)
        {
            return $"Slot{SaveSlotManager.CurrentSlotID}_{baseKey}";
        }

        public static List<string> GetLegacyMasterKeyList()
        {
            return PlayerPrefs.HasKey(MASTER_KEY_LIST_UNSLOTTED)
                ? JsonMapper.ToObject<List<string>>(PlayerPrefs.GetString(MASTER_KEY_LIST_UNSLOTTED)) ??
                  new List<string>()
                : new List<string>();
        }

        #endregion

        #region 字典转换辅助方法 (保留)

        public static List<DictPair<TKey, TValue>> DictionaryToPair<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            List<DictPair<TKey, TValue>> pairList = new List<DictPair<TKey, TValue>>();
            foreach (KeyValuePair<TKey, TValue> item in dict)
            {
                pairList.Add(new DictPair<TKey, TValue> { Key = item.Key, Value = item.Value });
            }

            return pairList;
        }

        public static Dictionary<TKey, TValue> PairToDictionary<TKey, TValue>(List<DictPair<TKey, TValue>> pairList)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            if (pairList != null)
            {
                pairList.ForEach(a => { dict.Add(a.Key, a.Value); });
            }

            return dict;
        }

        #endregion
    }

    #region 辅助类 (保留)

    public class DictPair<T, Y>
    {
        public T Key;
        public Y Value;
    }

    namespace Prototype.Test
    {
        public struct Float3
        {
            public float x, y, z;

            public Float3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }
    }

    #endregion
}