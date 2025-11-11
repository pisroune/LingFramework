using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class StarterArchitecture<T> : LingArchitecture<T> where T : Architecture<T>, new()
    {
        protected override void OnInitalized()
        {
            this.RegisterSystem<CameraSystem>(new CameraSystem());
            this.RegisterSystem<Cursor3DSystem>(new Cursor3DSystem());
        }
    }
}