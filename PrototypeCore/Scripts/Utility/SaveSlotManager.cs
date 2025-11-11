using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Prototype.Loader
{
    public static class SaveSlotManager
    {
        public static int CurrentSlotID { get; private set; } = DefaultSlotID;
        public const int DefaultSlotID = 1;
        private const string LegacyMigrationKey = "LegacyPrefsToJsonMigrated_v1";

        public static void Initialize()
        {
            if (PlayerPrefs.GetInt(LegacyMigrationKey, 0) == 1)
            {
                CurrentSlotID = DefaultSlotID;
                Debug.Log($"存档系统已初始化。当前激活的槽位是: {CurrentSlotID}");
                return;
            }

            CurrentSlotID = DefaultSlotID;
            Debug.Log($"存档系统已初始化，并完成了旧存档迁移。当前激活的槽位是: {CurrentSlotID}");
        }

        public static string GetSlotFilePath(int slotId, string baseKey)
        {
            string fileName = $"Slot{slotId}_{baseKey}.json";
            return Path.Combine(Application.persistentDataPath, fileName);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
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

        public static List<int> GetExistingSlotIDs()
        {
            var ids = new HashSet<int>();
            string savePath = Application.persistentDataPath;
            if (Directory.Exists(savePath))
            {
                foreach (var filePath in Directory.GetFiles(savePath, "Slot*_*.json"))
                {
                    string fileName = Path.GetFileName(filePath);
                    if (fileName.StartsWith("Slot"))
                    {
                        string idStr = fileName.Substring(4, fileName.IndexOf('_') - 4);
                        if (int.TryParse(idStr, out int id)) ids.Add(id);
                    }
                }
            }

            return ids.OrderBy(id => id).ToList();
        }
#endif

        public static void DeleteSlot(int slotID = DefaultSlotID)
        {
            Debug.LogWarning($"[开发功能] 正在删除槽位 {slotID} 的所有存档...");
            string savePath = Application.persistentDataPath;
            if (Directory.Exists(savePath))
            {
                foreach (var filePath in Directory.GetFiles(savePath, $"Slot{slotID}_*.json"))
                {
                    File.Delete(filePath);
                    Debug.Log($"已删除: {Path.GetFileName(filePath)}");
                }
            }
        }
    }
}