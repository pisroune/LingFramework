using MoonSharp.Interpreter.CoreLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class CameraSetting : MonoBehaviour
    {
        public bool SyncImmediately = false;
        public float SyncSpeed = 5;

        private void LateUpdate()
        {
            if (Vector3.Distance(transform.localPosition, Vector3.one) > 0.05f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.one, SyncSpeed * Time.deltaTime);
            }
        }
    }
}