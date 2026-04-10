using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Prototype
{
    public class StringSearchableDropdown : AdvancedDropdown
    {
        private string[] _options;
        private Action<string> _onSelected;

        public StringSearchableDropdown(AdvancedDropdownState state, string[] options, Action<string> onSelected)
            : base(state)
        {
            _options = options;
            _onSelected = onSelected;
            // 设置窗口大小
            this.minimumSize = new UnityEngine.Vector2(200, 300);
        }

        // 构建菜单项
        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("选项列表");

            foreach (var option in _options)
            {
                root.AddChild(new AdvancedDropdownItem(option));
            }

            return root;
        }

        // 当某一项被选中时回调
        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            _onSelected?.Invoke(item.name);
        }
    }
#if UNITY_EDITOR
    public class StringDrawerExtension
    {
        public static void StartDraw(Rect position, SerializedProperty property, GUIContent label, string[] options)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 1. 绘制左侧的 Label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // 2. 获取当前选中的属性值
            SerializedProperty selectedOptionProperty = property.FindPropertyRelative("Selected");
            string currentValue = selectedOptionProperty.stringValue;

            // 如果当前值不在选项里，显示提示
            if (string.IsNullOrEmpty(currentValue)) currentValue = "请选择...";

            // 3. 绘制下拉按钮
            if (EditorGUI.DropdownButton(position, new GUIContent(currentValue), FocusType.Passive))
            {
                // 4. 实例化并显示搜索窗口
                var dropdown = new StringSearchableDropdown(
                    new AdvancedDropdownState(),
                    options,
                    (selectedString) =>
                    {
                        // 这里是回调：当用户在窗口选中某个值时更新属性
                        selectedOptionProperty.stringValue = selectedString;
                        selectedOptionProperty.serializedObject.ApplyModifiedProperties();
                    }
                );

                dropdown.Show(position);
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}