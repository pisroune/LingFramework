using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace Prototype
{
    /// <summary>
    /// 文件夹隔离型存档管理器
    /// 核心逻辑：所有存取操作都被强制限定在 Application.persistentDataPath/SaveSlots/Slot_N/ 目录下。
    /// </summary>
    public static class SaveManager
    {
        // --- 1. 配置与路径常量 ---
        public static int CurrentSlotID { get; private set; } = 0;
        private const string ROOT_FOLDER = "SaveSlots"; // 存档总根目录名称
        private static readonly JsonSerializerSettings _jsonSettings;

        static SaveManager()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        #region 槽位管理

        public static void SwitchToSlot(int slotID)
        {
            if (slotID < 0)
            {
                Debug.LogError("[SaveManager] 槽位ID不能小于0！");
                return;
            }

            CurrentSlotID = slotID;
            Debug.Log($"[SaveManager] 已切换至目录存档槽位: {GetSlotRelativePath()}");
        }

        /// <summary>
        /// 获取当前槽位的相对根路径 (例如 "SaveSlots/Slot_0")
        /// </summary>
        private static string GetSlotRelativePath()
        {
            return Path.Combine(ROOT_FOLDER, $"Slot_{CurrentSlotID}");
        }

        /// <summary>
        /// 获取当前槽位的绝对根路径
        /// </summary>
        private static string GetSlotAbsolutePath()
        {
            return Path.Combine(Application.persistentDataPath, GetSlotRelativePath());
        }

        #endregion

        #region 路径安全核心 (强制锁定在槽位文件夹内)

        /// <summary>
        /// 输入一个槽位内的相对路径，返回一个被锁定在当前槽位文件夹内的绝对路径。
        /// </summary>
        public static string GetValidatedFullPath(string pathInSlot)
        {
            if (string.IsNullOrEmpty(pathInSlot)) return null;

            // 获取当前槽位的绝对基准路径
            string slotRoot = Path.GetFullPath(GetSlotAbsolutePath());

            // 组合目标路径
            string combinedPath = Path.Combine(slotRoot, pathInSlot);

            try
            {
                string normalizedPath = Path.GetFullPath(combinedPath);

                // 安全校验：最终路径必须以当前槽位的根路径开头
                // 这防止了通过 "../" 逃离当前 Slot 文件夹的操作
                if (!normalizedPath.StartsWith(slotRoot, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.LogError($"[SaveManager] 越权访问被拦截！尝试逃离槽位目录: {pathInSlot}");
                    return null;
                }

                return normalizedPath;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 路径解析错误: {e.Message}");
                return null;
            }
        }

        #endregion

        #region 统一存取 API

        /// <summary>
        /// 保存数据到当前槽位。
        /// pathInSlot 可以是文件名 "player.json"，也可以是子路径 "Settings/audio.json"
        /// </summary>
        public static void Save<T>(string pathInSlot, T data)
        {
            // 自动补全 .json 后缀（如果用户没写的话）
            if (!pathInSlot.EndsWith(".json")) pathInSlot += ".json";

            string fullPath = GetValidatedFullPath(pathInSlot);
            AtomicWrite(fullPath, data);
        }

        /// <summary>
        /// 从当前槽位加载数据
        /// </summary>
        public static bool TryLoad<T>(string pathInSlot, out T value)
        {
            if (!pathInSlot.EndsWith(".json")) pathInSlot += ".json";

            string fullPath = GetValidatedFullPath(pathInSlot);
            return TryRead(fullPath, out value);
        }

        /// <summary>
        /// 检查当前槽位内是否存在某文件
        /// </summary>
        public static bool Exists(string pathInSlot)
        {
            if (!pathInSlot.EndsWith(".json")) pathInSlot += ".json";
            string fullPath = GetValidatedFullPath(pathInSlot);
            return !string.IsNullOrEmpty(fullPath) && File.Exists(fullPath);
        }

        /// <summary>
        /// 删除当前槽位内的特定文件
        /// </summary>
        public static void Delete(string pathInSlot)
        {
            if (!pathInSlot.EndsWith(".json")) pathInSlot += ".json";
            string fullPath = GetValidatedFullPath(pathInSlot);
            SafeDelete(fullPath);
        }

        #endregion

        #region 原子 IO 底层实现

        private static void AtomicWrite<T>(string fullPath, T data)
        {
            if (string.IsNullOrEmpty(fullPath)) return;

            try
            {
                string json = JsonConvert.SerializeObject(data, _jsonSettings);
                string tempPath = fullPath + ".tmp";
                string backupPath = fullPath + ".bak";

                string dir = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                File.WriteAllText(tempPath, json);

                if (File.Exists(fullPath))
                {
                    File.Replace(tempPath, fullPath, backupPath);
                    if (File.Exists(backupPath)) File.Delete(backupPath);
                }
                else
                {
                    File.Move(tempPath, fullPath);
                }

                Debug.Log($"[SaveManager] 写入成功: {fullPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 写入异常: {e.Message}");
            }
        }

        private static bool TryRead<T>(string fullPath, out T value)
        {
            value = default;
            if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath)) return false;

            try
            {
                string json = File.ReadAllText(fullPath);
                value = JsonConvert.DeserializeObject<T>(json, _jsonSettings);
                return value != null;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 读取反序列化失败: {e.Message}");
                return false;
            }
        }

        private static void SafeDelete(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return;
            try
            {
                if (File.Exists(fullPath)) File.Delete(fullPath);
                if (File.Exists(fullPath + ".bak")) File.Delete(fullPath + ".bak");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 删除失败: {e.Message}");
            }
        }

        #endregion

        #region 槽位整体操作

        /// <summary>
        /// 删除整个当前槽位文件夹及其所有内容
        /// </summary>
        public static void DeleteCurrentSlotFolder()
        {
            string slotPath = GetSlotAbsolutePath();
            try
            {
                if (Directory.Exists(slotPath))
                {
                    Directory.Delete(slotPath, true);
                    Debug.Log($"[SaveManager] 已清空槽位目录: {slotPath}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 删除槽位目录失败: {e.Message}");
            }
        }

        /// <summary>
        /// 重命名或移动当前槽位内的文件/子目录
        /// </summary>
        public static bool MoveInSlot(string sourceInSlot, string destInSlot)
        {
            string src = GetValidatedFullPath(sourceInSlot);
            string dst = GetValidatedFullPath(destInSlot);

            if (src == null || dst == null || !File.Exists(src)) return false;

            try
            {
                string destDir = Path.GetDirectoryName(dst);
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

                if (File.Exists(dst)) File.Delete(dst);
                File.Move(src, dst);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 槽位内移动失败: {e.Message}");
                return false;
            }
        }

        #endregion

        #region 过时函数

        public static bool TryGet<T>(string key, out T value)
        {
            Debug.LogError("已重命名，建议替换为TryLoad");
            return TryLoad<T>(key, out value);
        }

        public static void DeleteSave(string key)
        {
            Debug.LogError("已重命名，建议替换为Delete");
            Delete(key);
        }

        #endregion
    }
}