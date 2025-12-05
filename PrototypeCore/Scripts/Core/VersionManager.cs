using Prototype.Loader;
using QFramework;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototype
{
    public class VersionManager
    {
        static VersionDataSO _dataSO;
        static VersionStorage _desktopSave;   //玩家客户端存档
        static VersionStorage _gameVersion;   //当前游戏版本

        public const string DataSOPath = "ScriptableObject/VersionData";
        public static string Key => "GameVersion";
        public static VersionStorage DesktopSave => _desktopSave;
        public static VersionStorage GameVersion => _gameVersion;

        [RuntimeInitializeOnLoadMethod]
        public static void StaticInit()
        {
            _dataSO = Resources.Load<VersionDataSO>(DataSOPath);
            if (_dataSO == null)
            {
                Debug.Log("没有找到版本控制文件，你可以在ScriptableObject路径下右键，Create/Prototype/VersionData，即可自动启动版本控制系统");
                return;
            }

            Debug.Log("当前游戏版本：" + _dataSO.GameVersion + "\n版本更新日期：" + _dataSO.VersionDate);
            SaveManager.SwitchToSlot(_dataSO.SaveSlot);


            //尝试加载玩家客户端存档版本
            if (SaveManager.TryGet<VersionStorage>(Key, out _desktopSave))
            {  //加载本地版本成功
                Debug.Log("本地存档版本：" + _desktopSave.Version + "\n本地版本日期：" + _desktopSave.Date);
                if (IsVersionBehine(_dataSO.GameVersion))
                {
                    Debug.Log("版本落后，已尝试同步");
                }
                else if (IsVersionAfter(_dataSO.GameVersion))
                {
                    Debug.Log("版本领先，已回溯（谨慎执行回溯操作）");
                }
                else
                {
                    Debug.Log("版本完全相同");
                }
            }
            else
            {  //未找到本地版本，完全新档
                _desktopSave = new VersionStorage()
                {
                    Version = null,
                    Date = null
                };
                Debug.Log("未找到本地存档版本，同步新版本：" + _dataSO.GameVersion);
            }

            //加载当前游戏版本
            _gameVersion = new VersionStorage()
            {
                Version = _dataSO.GameVersion,
                Date = _dataSO.VersionDate,
            };

            //同步当前版本
            SaveManager.Save(Key, _gameVersion);
        }

        public static bool FullNewGame()
        {
            return _desktopSave.Version == null;
        }

        /// <summary>
        /// 检查玩家客户端的存档版本是否落后于参数版本
        /// 假设传了0.80.140.2898
        /// 当前版本0.80.141.2344
        /// 从头判断，至第三节落后，则判定落后（true），无需判断第四节
        /// 
        /// 假设传了0.80.141.2898
        /// 当前版本0.80.140.2344
        /// 从头判断，至第三节领先，随通常不会发生，但仍判断为领先（false），也无需判断第四节
        /// 
        /// 假设传了0.80.140
        /// 当前版本0.80.141.2344
        /// 这是允许的，从头判断，至某节落后，则判定落后（true）
        /// 
        /// 假设传了0.80
        /// 亦或传了0.80.141
        /// 当前版本0.80.141.2344
        /// 这也是允许的，从头判断，至最后一位同步，则判定同步（false）
        /// </summary>
        /// <param name="saveVersion"></param>
        /// <returns></returns>
        public static bool IsVersionBehine(string targetVersion)
        {
            if (_desktopSave.Version == null)
            {  //新档
                return true;
            }

            int[] saveVersions = ParseVersion(_desktopSave.Version);  //存档版本
            int[] targetVersions = ParseVersion(targetVersion);       //目标版本

            for (int i = 0; i < targetVersions.Length; i++)
            {
                if (saveVersions[i] < targetVersions[i])
                    return true;   //存档版本落后
                else if (saveVersions[i] > targetVersions[i])
                    return false;  //该位存档版本更新，无需检查下一位
            }
            return false;   //版本号完全相同
        }
        public static bool IsVersionAfter(string targetVersion)
        {
            if (_desktopSave.Version == null)
            {  //新档
                return true;
            }

            int[] saveVersions = ParseVersion(_desktopSave.Version);  //存档版本
            int[] targetVersions = ParseVersion(targetVersion);       //目标版本

            for (int i = 0; i < targetVersions.Length; i++)
            {
                if (saveVersions[i] > targetVersions[i])
                    return true;   //存档版本领先
                else if (saveVersions[i] < targetVersions[i])
                    return false;  //该位存档版本落后，无需检查下一位
            }
            return false;   //版本号完全相同
        }
        /// <summary>
        /// 判断checkVersion落后于targetVersion
        /// </summary>
        /// <param name="checkVersion">检查</param>
        /// <param name="targetVersion">是否落后于</param>
        /// <returns></returns>
        public static bool IsVersionBehine(string checkVersion, string targetVersion)
        {
            //checkVersion落后于targetVersion?
            int[] checkVersions = ParseVersion(checkVersion);  //存档版本
            int[] targetVersions = ParseVersion(targetVersion);       //目标版本

            for (int i = 0; i < targetVersions.Length; i++)
            {
                if (checkVersions[i] < targetVersions[i])
                    return true;   //存档版本落后
                else if (checkVersions[i] > targetVersions[i])
                    return false;  //该位存档版本更新，无需检查下一位
            }
            return false;   //版本号完全相同
        }

        private static int[] ParseVersion(string version)
        {
            return version.Split('.').Select(part => int.Parse(part)).ToArray();
        }


        public class VersionStorage
        {
            public string Version;
            public string Date;
        }
    }
}