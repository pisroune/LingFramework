using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using QFramework;

namespace Prototype
{
    /// <summary>
    /// 本地化
    /// </summary>
    public static class Localization
    {
        public static ILocalizeLoader ILoader;

        static Dictionary<string, string> dictionary = new Dictionary<string, string>();

        public static bool localizationLoaded = false;
        private static string _language;
        public static string currentLanguage
        {
            get
            {
                //Debug.LogError("已弃用，联系马凌宇改");
                return _language;
            }
        }

        public static List<ILocalize> LocalizeList = new List<ILocalize>();
        public static Action<string> OnSetLanguage;


        public static void SetLanguage(string language)
        {
            //加载新语言文件
            _language = language;
            LoadLocalization(_language);
            foreach (ILocalize local in LocalizeList)
            {
                local.LoadKey();
            }
            OnSetLanguage?.Invoke(language);
        }

        /// <summary>
        /// 加载本地化文件，并存进字典
        /// </summary>
        public static void LoadLocalization(string language)
        {
            if (ILoader == null)
            {
                ILoader = new DefaultLocalizeLoader();
                //ILoader = new SheetDataLocalizeLoader();
            }
            dictionary = ILoader.LoadLocalizationDict(language);
            localizationLoaded = true;
        }

        /// <summary>
        /// 通过key获得本地化语言
        /// </summary>
        public static string Get(string key)
        {
            //确保已经加载本地化
            if (!localizationLoaded)
                return "";

            if (dictionary.TryGetValue(key, out var value) && !value.IsNullOrEmpty())
            {  //获取value
                value = value.Replace("\\n", "\n");
                return value;
            }
            Debug.LogError(_language + "语言中没有找到这个Key：" + key);
            return key;
        }

        public static bool TryGet(string key, out string value)
        {
            //确保已经加载本地化
            if (!localizationLoaded)
            {
                value = "";
                return false;
            }

            if (dictionary.ContainsKey(key) && !key.IsNullOrEmpty())
            {  //获取value
                value = dictionary[key];
                return true;
            }

            value = key;
            return false;
        }

        /// <summary>
        /// 获取包含查找内容的所有key
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static List<string> TryGetKeyList(string content)
        {
            List<string> checkList = new List<string>();

            if (!localizationLoaded)
            {
                return checkList;
            }

            foreach (var item in dictionary)
            {
                if (item.Key.Contains(content))
                    checkList.Add(item.Key);
            }

            return checkList;
        }

        /// <summary>
        /// 修改Key对应的值
        /// </summary>
        public static void Set(string key, string value)
        {
            //确保已经加载本地化
            if (!localizationLoaded)
                LoadLocalization(_language);

            if (dictionary.ContainsKey(key))
            {
                // Saves dictionary into temporary dictionary
                Dictionary<string, string> tempDic = new Dictionary<string, string>(dictionary);
                // Dictionary keys go in to temporary list
                List<string> tempList = new List<string>(dictionary.Keys);
                // clear the main dictionary
                dictionary.Clear();
                // aplly list into dictionary
                foreach (string str in tempList)
                {
                    // Adds the new key and value to the dictionary And restores the keys and values that shouldn't be changed
                    if (str == key) dictionary.Add(key, value);
                    else dictionary.Add(str, tempDic[str]);
                }
                // Returns if done
                return;
            }
            // Adds a new key and value to the dictionary
            dictionary.Add(key, value);
        }
    }
}