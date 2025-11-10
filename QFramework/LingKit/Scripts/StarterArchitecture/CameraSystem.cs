using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

namespace QFramework
{
    public class CameraSystem : AbstractSystem, IWithGameObject, IUpdate
    {
        public GameObject ExternalGameObject { get; set; }
        string IWithGameObject.ObjectName => "CameraSystem";

        public ICamera ICamera;
        public Transform CameraTrans;
        public CameraSetting Setting;
        public Ray Ray_Cursor;

        //Camera entity
        public GameObject Camera_Starter;
        public GameObject Camera_RTS;
        public GameObject Camera_TopDown;

        protected override void OnInit()
        {
            ExternalGameObject.name = "CameraSystem";
            Setting = ExternalGameObject.AddComponent<CameraSetting>();
            CameraTrans = GameObject.Instantiate(Resources.Load<GameObject>("Camera"), ExternalGameObject.transform).transform;

            Camera_Starter = Resources.Load<GameObject>("Camera_Starter");
            NewCamera_Starter(Vector3.zero + Vector3.up * 10, Quaternion.identity);
        }

        protected override void OnDeinit()
        {
            ICamera?.End();
        }

        public Camera_Starter NewCamera_Starter(Vector3 startPosition, Quaternion startRotation)
        {
            if (ICamera != null)
            {
                CameraTrans.Parent(ExternalGameObject);
                ICamera.End();
            }

            ICamera = GameObject.Instantiate(Camera_Starter, ExternalGameObject.transform).GetComponent<ICamera>();
            ICamera.Enter(new Camera_Starter.InitValue()
            {
                Parent = ExternalGameObject.transform,
                StartPosition = startPosition,
                StartRotation = startRotation
            });
            CameraTrans.Parent(ICamera.ThisTrans);
            CameraTrans.LocalIdentity();
            return ICamera as Camera_Starter;
        }

        public void NewCamera_RTS()
        {

        }

        public void NewCamera_Topdown(Transform followTarget)
        {

        }

        public void Update()
        {
            Ray_Cursor = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}