using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Prototype
{
    public class Localize_TMP : MonoBehaviour, ILocalize
    {
        public string TermsT;
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
            if (this.GetComponentInChildren<TMP_Text>() != null)
            {
                string key = TermsT + "/" + Key;
                string text = Localization.Get(key);
                this.GetComponentInChildren<TMP_Text>().text = text;
                OnLoadKey?.Invoke(text);
            }
        }
    }
}