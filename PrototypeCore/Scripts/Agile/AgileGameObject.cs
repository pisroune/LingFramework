using Project_TankSchool;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Prototype
{
    [CreateAssetMenu(fileName = "New AgileGameObject", menuName = "Prototype/Agile GameObject")]
    public class AgileGameObject : ScriptableObject
    {
        public GameObject GameObjects;



        public GameObject GetPrefab()
        {
            return GameObjects;
        }
        public T GetPrefab<T>() where T : Component, IPoolablePrefab
        {
            return GameObjects.GetComponent<T>();
        }
        public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return PoolManager.Instance.Spawn(GameObjects, position, rotation, parent);
        }
        public T Spawn<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : Component, IPoolablePrefab
        {
            T component = GameObjects.GetComponent<T>();
            return PoolManager.Instance.Spawn(component, position, rotation, parent);
        }
        public GameObject Generate(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return GameObject.Instantiate(GameObjects, position, rotation, parent);
        }

        [Button("Ô¤ÖÆÌå×¢²á")]
        public void Register()
        {
            AgileAssets.RegisterGameObjects();
        }
    }
}