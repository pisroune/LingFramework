using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    /// <summary>
    /// 语言本地化脚本
    /// 挂载在UI物体，Test物体的父物体上
    /// </summary>
    public class Localize : MonoBehaviour, ILocalize
    {
        public string Terms;
        public string Key;
        public Action<string> OnLoadKey;

        /// <summary>
        /// Calls the LoadKey on Awake
        /// </summary>
        private void OnEnable()
        {
            Localization.LocalizeList.Add(this);
            LoadKey();
        }

        private void OnDisable()
        {
            Localization.LocalizeList.Remove(this);
        }

        /// <summary>
        /// Loads the text from the localization file and images from resouces folder
        /// </summary>
        public void LoadKey()
        {
            if (this.GetComponentInChildren<Text>() != null)
            {
                string key = Terms + "/" + Key;
                string text = Localization.Get(key);
                this.GetComponentInChildren<Text>().text = text;
                OnLoadKey?.Invoke(text);
            }
        }
    }
}