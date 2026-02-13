using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.UI.Button;
using static UnityEngine.UI.Slider;

namespace QFramework
{
    public static class UIExtension
    {
        /// <summary>
        /// 使Button的按键响应事件仅注册这一个回调方法
        /// </summary>
        /// <param name="click">按键响应</param>
        /// <param name="action">回调</param>
        public static void OnlyListener(this ButtonClickedEvent click, UnityAction action)
        {
            click.RemoveAllListeners();
            click.AddListener(action);
        }

        /// <summary>
        /// 使DropDown的响应事件仅注册这一个回调方法
        /// </summary>
        /// <param name="dropdown">按键响应</param>
        /// <param name="action">回调</param>
        public static void OnlyListener(this Dropdown.DropdownEvent dropdown, UnityAction<int> action)
        {
            dropdown.RemoveAllListeners();
            dropdown.AddListener(action);
        }

        /// <summary>
        /// 使Slider的滑动响应事件仅注册这一个回调方法
        /// </summary>
        /// <param name="slider">滑动相应</param>
        /// <param name="action">回调</param>
        public static void OnlyListener(this SliderEvent slider, UnityAction<float> action)
        {
            slider.RemoveAllListeners();
            slider.AddListener(action);
        }

        public static void OnlyListener(this Toggle.ToggleEvent toggle, UnityAction<bool> action)
        {
            toggle.RemoveAllListeners();
            toggle.AddListener(action);
        }
    }
}