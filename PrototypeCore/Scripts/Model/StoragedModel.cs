using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Loader
{
    /// <summary>
    /// 存档模型
    /// 一个类是一个单例，用Key获取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class StoragedModel<T> : AbstractModel where T : class, new()
    {
        List<Migrate> _migrates;
        private T _storage;

        public T Storage
        {
            get { return _storage; }
            set { _storage = value; }
        }
        public List<Migrate> Migrates => _migrates;

        public abstract string Key { get; }
        public abstract string ResetVersion { get; }  //重置临界版本，如果玩家本地存档落后于这个版本，直接进行删档重置

        protected sealed override void OnInit()
        {
            _migrates = new List<Migrate>();
            OnInitSpecific();

            if (JsonTool.TryGet<T>(Key, out _storage) && !VersionModel.IsVersionBehine(ResetVersion))
            {
                Debug.Log("加载系统设置");
                TryDoMigrate(VersionModel.DesktopSave.Version, _storage);
                GetMemory(_storage);
            } 
            else
            {
                Debug.Log("新用户");
                _storage = new T();
                NewMemory(_storage);
                Save();
            }
        }
        protected virtual void OnInitSpecific() { }
        protected abstract void NewMemory(T storage);
        protected abstract void GetMemory(T storage);

        public void Save()
        {
            BeforeSave();
            JsonTool.JsonSave(Key, _storage);
        }
        protected virtual void BeforeSave() { }
        public void Delete()
        {
            JsonTool.JsonDelete(Key);
            OnInit();
        }

        protected void RegisterMigrate(string critcalVersion, Action<T> onMigrate)
        {
            _migrates.Add(new Migrate(critcalVersion, onMigrate));
        }
        void TryDoMigrate(string version, T storage)
        {
            int startIndex = -1;
            for (int i = 0; i < _migrates.Count; i++)
            {
                if (VersionModel.IsVersionBehine(version, _migrates[i].Version))
                {  //落后于映射的临界值，从这里开始执行映射
                    startIndex = i;
                    break;
                }
            }
            if (startIndex == -1)
            {  //无需执行映射
                return;
            }

            for (int i = startIndex; i < _migrates.Count; i++)
            {
                _migrates[i].OnMigrate(storage);
            }
        }

        public class Migrate
        {
            public string Version;     //临界版本，如果存档版本落后于该版本则执行迁移
            public Action<T> OnMigrate;

            public Migrate(string version, Action<T> onMigrate)
            {
                Version = version;
                OnMigrate = onMigrate;
            }
        }
    }
}