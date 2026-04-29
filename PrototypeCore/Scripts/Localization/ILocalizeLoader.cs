using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototype
{
    public interface ILocalizeLoader
    {
        Dictionary<string, string> LoadLocalizationDict(string language);
    }

    public class DefaultLocalizeLoader : ILocalizeLoader
    {
        public string filePath = "ResLocalization/";  //先遍历文件夹，在各个文件夹中找TextAsset和SheetData，然后搜索对应语言
        List<string> doubleKeyCheck = new List<string>();  //重复添加检测
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        public Dictionary<string, string> LoadLocalizationDict(string language)
        {
            doubleKeyCheck.Clear();
            dictionary.Clear();
            Object[] obj = Resources.LoadAll<Object>(filePath);
            //int sheetNum = 0;
            int textNum = 0;
            foreach (var item in obj)
            {
                //if (item.GetType() == typeof(YadeSheetData))
                //{
                //    if (LoadSheetData((YadeSheetData)item, language))
                //    {
                //        sheetNum++;
                //    }
                //}
                if (item.GetType() == typeof(TextAsset))
                {
                    if (item.name == language)
                    {
                        textNum++;
                        LoadTextAsset((TextAsset)item);
                    }
                }
            }
            return dictionary;
        }
        void LoadTextAsset(TextAsset asset)
        {
            string[] separator = new string[] { "\n" };   //跳过行
            List<string> tempString = new List<string>();
            tempString = asset.text.Split(separator, System.StringSplitOptions.None).ToList();
            separator = new string[] { " = " };           //按" = "分割key value
            foreach (string str in tempString)
            {
                string[] tString = str.Split(separator, System.StringSplitOptions.None).ToArray();
                if (tString.Length > 2)
                {
                    foreach (string tempStr in tString)
                    {
                        if (tempStr != tString[0] && tempStr != tString[1])
                        {
                            tString[1] = tString[1] + " = " + tempStr;
                        }
                    }
                }

                if (tString.Length > 1)
                {
                    tString[1] = tString[1].Replace("\\n", System.Environment.NewLine);
                    if (!doubleKeyCheck.Contains(tString[0]))
                    {
                        doubleKeyCheck.Add(tString[0]);
                        dictionary.Add(tString[0], tString[1]);
                    }
                    else Debug.LogError("Key:" + tString[0] + "被重复添加");
                }
            }
        }
    }
}
