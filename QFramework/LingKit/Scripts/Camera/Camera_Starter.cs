using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 初始包自带的自由视角相机
    /// </summary>
    public class Camera_Starter : CameraBase
    {
        public struct InitValue : IInitValue
        {
            public string Name => "Starter";
            public Transform Parent;
            public Vector3 StartPosition;
            public Quaternion StartRotation;
        }

        Transform _movieFocus;  //自由视角下的焦点
        SmoothRotation _rotationX;
        SmoothRotation _rotationY;
        public float MoveSpeed { get; private set; } = 10;

        protected override void OnEnter(IInitValue initObj)
        {
            InitValue value = (InitValue)initObj;
            _movieFocus = new GameObject("MovieFocus").transform;
            _movieFocus.parent = value.Parent;
            _movieFocus.transform.position = value.StartPosition;
            _movieFocus.transform.rotation = value.StartRotation;
            transform.position = _movieFocus.transform.position;
            transform.rotation = _movieFocus.transform.rotation;

            _rotationX = new SmoothRotation(Input.GetAxisRaw("Mouse X"));
            _rotationY = new SmoothRotation(Input.GetAxisRaw("Mouse Y"));
        }

        protected override void OnEnd()
        {
            Time.timeScale = 1;
            GameObject.Destroy(_movieFocus.gameObject);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.D))
                _movieFocus.transform.position += _movieFocus.right * Time.deltaTime * MoveSpeed;
            if (Input.GetKey(KeyCode.A))
                _movieFocus.transform.position -= _movieFocus.right * Time.deltaTime * MoveSpeed;
            if (Input.GetKey(KeyCode.W))
                _movieFocus.transform.position += _movieFocus.forward * Time.deltaTime * MoveSpeed;
            if (Input.GetKey(KeyCode.S))
                _movieFocus.transform.position -= _movieFocus.forward * Time.deltaTime * MoveSpeed;
            if (Input.GetKey(KeyCode.E))
                _movieFocus.transform.position += _movieFocus.up * Time.deltaTime * MoveSpeed;
            if (Input.GetKey(KeyCode.Q))
                _movieFocus.transform.position -= _movieFocus.up * Time.deltaTime * MoveSpeed;

            var rotationX = _rotationX.Update(Input.GetAxisRaw("Mouse X"), 0.05f);
            var rotationY = -_rotationY.Update(Input.GetAxisRaw("Mouse Y"), 0.05f);
            _movieFocus.localEulerAngles += new Vector3(rotationY, rotationX, 0);
            if (_movieFocus.localEulerAngles.x > 80 && _movieFocus.localEulerAngles.x < 180)
            {
                _movieFocus.localEulerAngles = new Vector3(80, _movieFocus.localEulerAngles.y, _movieFocus.localEulerAngles.z);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Time.timeScale = Mathf.Clamp(Time.timeScale - 0.2f, 0.00001f, 3);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Time.timeScale += 0.2f;
                Time.timeScale = Mathf.Clamp(Time.timeScale, 0.00001f, 3);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveSpeed -= 2;
                if (MoveSpeed < 2)
                    MoveSpeed = 2;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveSpeed += 2;
                if (MoveSpeed > 30)
                    MoveSpeed = 30;
            }
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _movieFocus.transform.position, 0.1f * Time.deltaTime * 60);  //*
            transform.rotation = Quaternion.Lerp(transform.rotation, _movieFocus.rotation, 0.3f * Time.deltaTime * 60);
        }


        private class SmoothRotation
        {
            private float _current;
            private float _currentVelocity;

            public SmoothRotation(float startAngle)
            {
                _current = startAngle;
            }

            /// Returns the smoothed rotation.
            public float Update(float target, float smoothTime)
            {
                return _current = Mathf.SmoothDampAngle(_current, target, ref _currentVelocity, smoothTime);
            }

            public float Current
            {
                set { _current = value; }
            }
        }
    }
}