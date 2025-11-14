using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Prototype
{
#if UNITY_EDITOR
    public class StringDrawerExtension
    {
        public static void StartDraw(Rect position, SerializedProperty property, GUIContent label, string[] options)
        {
            // 开始绘制属性
            EditorGUI.BeginProperty(position, label, property);

            // 获取当前选中的选项
            SerializedProperty selectedOptionProperty = property.FindPropertyRelative("SelectedOption");

            // 确保当前选项在 options 中有效
            int selectedIndex = System.Array.IndexOf(options, selectedOptionProperty.stringValue);
            if (selectedIndex < 0) selectedIndex = 0; // 如果找不到，默认选择第一个

            // 绘制下拉框
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, options);

            // 更新选中的选项
            if (options.Length > 0)
            {
                selectedOptionProperty.stringValue = options[selectedIndex];
            }

            // 结束绘制属性
            EditorGUI.EndProperty();
        }
    }
#endif
}