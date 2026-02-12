using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Prototype.Editors
{
    public abstract class ConfigurationMenu : OdinMenuEditorWindow
    {
        protected abstract string ProjectName { get; }
        protected abstract Dictionary<string, string> ENTRYSTR { get; }
        protected abstract Dictionary<string, Type> TypeSO { get; }

        protected abstract string AssetPath { get; }

        /// <summary>
        /// 复制此静态方法到子类
        /// 将DefinitionMenu替换为子类类名
        /// </summary>
        //[MenuItem("Rocket✫/核心数据 &w", false, 1)]
        //public void Open()
        //{
        //    var window = GetWindow<DefinitionMenu>();
        //    window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        //}

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;

            OnBuildMenuTree(tree);

            tree.SortMenuItemsByName();

            return tree;
        }
        protected abstract void OnBuildMenuTree(OdinMenuTree tree);

        protected void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => Sirenix.Utilities.Editor.DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {  //battery entry
                    GUILayout.Label(selected.Name);
                    if (ENTRYSTR.ContainsKey(selected.Name))
                    {
                        if (SirenixEditorGUI.ToolbarButton(new GUIContent("定位文件夹")))
                        {
                            LocateEntry(string.Format("{0}/{1}", AssetPath, ENTRYSTR[selected.Name]));
                        }
                        if (SirenixEditorGUI.ToolbarButton(new GUIContent(string.Format("新建 {0}", selected.Name))))
                        {
                            NewEntry(ENTRYSTR[selected.Name]);
                        }
                    }
                    if (selected.Parent != null && ENTRYSTR.ContainsKey(selected.Parent.Name))
                    {
                        if (SirenixEditorGUI.ToolbarButton(new GUIContent("定位文件")))
                        {
                            LocateEntry(string.Format("{0}/{1}/{2}.asset", AssetPath, ENTRYSTR[selected.Parent.Name], selected.Name));
                        }
                        if (SirenixEditorGUI.ToolbarButton(new GUIContent(string.Format("新建 {0}", selected.Parent.Name))))
                        {
                            NewEntry(ENTRYSTR[selected.Parent.Name]);
                        }
                    }
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("刷新文件")))
                {
                    foreach (var item in ENTRYSTR)
                    {
                        ResetSOID(item.Value);
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        public abstract void NewEntry(string typeName);
        protected void ShowDialog<T>(string typeName) where T : ScriptableObject
        {
            ScriptableObjectCreator.ShowDialog<T>(AssetPath + "/" + typeName, obj =>
            {
                base.TrySelectMenuItemWithObject(obj);
            });
        }

        public void ResetSOID(string typeName)
        {
            foreach (var item in PrintFileName(AssetPath + "/" + typeName, TypeSO[typeName], true))
            {
                SyncSOID(item as IConfig);
            }
        }

        public void LocateEntry(string path)
        {
            // 使用 AssetDatabase 加载 Asset
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

            if (obj != null)
            {
                // 使用 Selection 类选择这个对象，这会在 Project 面板中高亮显示这个对象
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
            }
            else
            {
                Debug.LogError("Cannot find Asset at path: " + path);
            }
        }
        public List<IConfig> PrintFileName(string assetFolderPath, Type type, bool includeSubDirectories = false)
        {
            assetFolderPath = (assetFolderPath ?? "").TrimEnd(new char[1] { '/' }) + "/";
            string text = assetFolderPath.ToLower();
            if (!text.StartsWith("assets/") && !text.StartsWith("packages/"))
            {
                assetFolderPath = "Assets/" + assetFolderPath;
            }

            assetFolderPath = assetFolderPath.TrimEnd(new char[1] { '/' }) + "/";
            IEnumerable<string> enumerable = from x in AssetDatabase.GetAllAssetPaths()
                                             where includeSubDirectories ? x.StartsWith(assetFolderPath, StringComparison.InvariantCultureIgnoreCase) : (string.Compare(PathUtilities.GetDirectoryName(x).Trim(new char[1] { '/' }), assetFolderPath.Trim(new char[1] { '/' }), ignoreCase: true) == 0)
                                             select x;

            List<IConfig> tList = new List<IConfig>();
            foreach (var item in enumerable)
            {
                UnityEngine.Object @object = AssetDatabase.LoadAssetAtPath(item, type);
                if (@object == null)
                {
                    continue;
                }
                tList.Add(@object as IConfig);
            }
            return tList;
        }

        private void SyncSOID(IConfig entry)
        {
            string resultStr = AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(entry.ThisSO), entry.ID);
            if (!string.IsNullOrEmpty(resultStr))
            {
                throw new Exception(resultStr);
            }
        }
    }


    public static class ScriptableObjectCreator
    {
        public static void ShowDialog<T>(string defaultDestinationPath, Action<T> onScritpableObjectCreated = null)
            where T : ScriptableObject
        {
            var selector = new ScriptableObjectSelector<T>(defaultDestinationPath, onScritpableObjectCreated);

            if (selector.SelectionTree.EnumerateTree().Count() == 1)
            {
                // If there is only one scriptable object to choose from in the selector, then 
                // we'll automatically select it and confirm the selection. 
                selector.SelectionTree.EnumerateTree().First().Select();
                selector.SelectionTree.Selection.ConfirmSelection();
            }
            else
            {
                // Else, we'll open up the selector in a popup and let the user choose.
                selector.ShowInPopup(200);
            }
        }

        // Here is the actual ScriptableObjectSelector which inherits from OdinSelector.
        // You can learn more about those in the documentation: http://sirenix.net/odininspector/documentation/sirenix/odininspector/editor/odinselector(t)
        // This one builds a menu-tree of all types that inherit from T, and when the selection is confirmed, it then prompts the user
        // with a dialog to save the newly created scriptable object.

        private class ScriptableObjectSelector<T> : OdinSelector<Type> where T : ScriptableObject
        {
            private Action<T> onScritpableObjectCreated;
            private string defaultDestinationPath;

            public ScriptableObjectSelector(string defaultDestinationPath, Action<T> onScritpableObjectCreated = null)
            {
                this.onScritpableObjectCreated = onScritpableObjectCreated;
                this.defaultDestinationPath = defaultDestinationPath;
                this.SelectionConfirmed += this.ShowSaveFileDialog;
            }

            protected override void BuildSelectionTree(OdinMenuTree tree)
            {
                var scriptableObjectTypes = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
                    .Where(x => x.IsClass && !x.IsAbstract && x.InheritsFrom(typeof(T)));

                tree.Selection.SupportsMultiSelect = false;
                tree.Config.DrawSearchToolbar = true;
                tree.AddRange(scriptableObjectTypes, x => x.GetNiceName())
                    .AddThumbnailIcons();
            }

            private void ShowSaveFileDialog(IEnumerable<Type> selection)
            {
                var obj = ScriptableObject.CreateInstance(selection.FirstOrDefault()) as T;

                string dest = this.defaultDestinationPath.TrimEnd('/');

                if (!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                    AssetDatabase.Refresh();
                }

                dest = EditorUtility.SaveFilePanel("Save object as", dest, "New " + typeof(T).GetNiceName(), "asset");

                if (!string.IsNullOrEmpty(dest) && PathUtilities.TryMakeRelative(Path.GetDirectoryName(Application.dataPath), dest, out dest))
                {
                    AssetDatabase.CreateAsset(obj, dest);
                    AssetDatabase.Refresh();

                    if (this.onScritpableObjectCreated != null)
                    {
                        this.onScritpableObjectCreated(obj);
                    }
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }
        }
    }
}