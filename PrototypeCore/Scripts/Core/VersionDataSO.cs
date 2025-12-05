using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    [CreateAssetMenu(fileName = "VersionData", menuName = ("Prototype/VersionData"))]
    public class VersionDataSO : ScriptableObject
    {
        [Title("基础设定")]
        [LabelText("游戏版本")]
        public string GameVersion;
        [LabelText("版本更新日期")]
        public string VersionDate;
        [LabelText("版本补充")]
        public string VersionDescribe;

        [Title("编辑器相关")]
        [LabelText("应用存档")]
        public int SaveSlot;
    }
}