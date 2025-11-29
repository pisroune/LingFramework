using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class DebugManager
    {
        static GameObject DebugSpherePrefab { get { return AssetManager.LoadPrefab("Debug/Sphere"); } }
        static GameObject DebugLinePrefab { get { return AssetManager.LoadPrefab("Debug/LineRenderer"); } }

        public static GameObject DebugSphere(Vector3 position, float timer = 3, float scale = 1)
        {
            GameObject go = GameObject.Instantiate(DebugSpherePrefab, position, Quaternion.identity);
            go.GetComponent<DestroyMe>().Deathtimer = timer;
            go.transform.localScale = new Vector3(scale, scale, scale);
            return go;
        }

        public static GameObject DebugLine(Vector3 start, Vector3 end, float timer = 3)
        {
            GameObject go =
                GameObject.Instantiate(DebugLinePrefab, Vector3.zero, Quaternion.identity);
            go.GetComponent<LineRenderer>().SetPosition(0, start);
            go.GetComponent<LineRenderer>().SetPosition(1, end);
            go.GetComponent<DestroyMe>().Deathtimer = timer;
            return go;
        }
    }
}