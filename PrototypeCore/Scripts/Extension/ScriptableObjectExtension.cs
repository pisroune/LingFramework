using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public static class ScriptableObjectExtension
    {
#if UNITY_EDITOR
        public static void RenameAssetFile(this ScriptableObject newObject, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                Debug.LogError("名称不能为空！");
                return;
            }

            string oldPath = UnityEditor.AssetDatabase.GetAssetPath(newObject);
            string newFileName = newName.Trim();

            // 可选：清理非法字符
            newFileName = System.Text.RegularExpressions.Regex.Replace(newFileName, @"[<>:""/\\|?*]", "_");

            string newPath = System.IO.Path.GetDirectoryName(oldPath) + "/" + newFileName + ".asset";

            if (oldPath == newPath)
            {
                Debug.Log("文件名无需修改");
                return;
            }

            string result = UnityEditor.AssetDatabase.RenameAsset(oldPath, newFileName);

            if (string.IsNullOrEmpty(result))
                Debug.Log($"重命名成功：{oldPath} → {newPath}");
            else
                Debug.LogError($"重命名失败：{result}");
        }
#endif
    }
}