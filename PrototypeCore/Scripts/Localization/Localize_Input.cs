using Project_TankSchool;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class Localize_Input : MonoBehaviour, ILocalize
    {
        public bool UseString = false;
        [ShowIf("@UseString")]
        public string KeyCodeStr;
        [ShowIf("@!UseString")]
        public KeyCodeType KeyCodeT;
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
            string text = UseString
                ? GlobalSettingSystem.GetKeyCodeName(KeyCodeStr)
                : GlobalSettingSystem.GetKeyCodeName(KeyCodeT);

            if (this.GetComponentInChildren<Text>() != null)
            {
                this.GetComponentInChildren<Text>().text = text;
                OnLoadKey?.Invoke(text);
            }
            else if (this.GetComponentInChildren<TMP_Text>() != null)
            {
                this.GetComponentInChildren<TMP_Text>().text = text;
                OnLoadKey?.Invoke(text);
            }
        }
    }
}