using Sirenix.OdinInspector;
using UnityEngine;

namespace Prototype.Demo.StringDrawers
{
    public class StringDrawer : MonoBehaviour
    {
        [LabelText("СЎПо")]
        public Selections ItemHk;
    }

    [System.Serializable]
    public class Selections
    {
        public string SelectedOption = OnUseStart;

        public const string None = "None";
        public const string OnAcquire = "OnAcquire";
        public const string OnLose = "OnLose";
        public const string OnEquip = "OnEquip";
        public const string OnUnequip = "OnUnequip";
        public const string OnHoldStart = "OnHoldStart";
        public const string OnHoldTick = "OnHoldTick";
        public const string OnHoldEnd = "OnHoldEnd";
        public const string OnUseStart = "OnUseStart";

        public static string[] AllSelections = { None, OnAcquire, OnLose, "OnEquip", OnUnequip, "OnHoldStart", OnHoldTick, OnHoldEnd, OnUseStart };
    }


#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(Selections))]
    public class DemoSelectionDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            StringDrawerExtension.StartDraw(position, property, label, Selections.AllSelections);
        }
    }
#endif

}