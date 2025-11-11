using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class Cursor3DSystem : AbstractSystem, IWithGameObject, IUpdate
    {
        CameraSystem _cameraSystem;
        public GameObject ExternalGameObject { get; set; }
        string IWithGameObject.ObjectName => "Cursor3DSystem";

        public bool RayHit { get; private set; }
        public Vector3 Cursor_Terrain { get; private set; }  //光标处的地面


        protected override void OnInit()
        {
            _cameraSystem = this.GetSystem<CameraSystem>();
            ExternalGameObject.name = this.GetType().Name;
        }

        protected override void OnDeinit()
        {

        }

        public void Update()
        {
            Ray ray_Cursor = _cameraSystem.Ray_Cursor;

            //Terrain
            RayHit = Physics.Raycast(ray_Cursor, out RaycastHit terrain, 300, 1 << 0);
            Cursor_Terrain = RayHit ? terrain.point : Vector3.zero;

            return;
        }
    }
}