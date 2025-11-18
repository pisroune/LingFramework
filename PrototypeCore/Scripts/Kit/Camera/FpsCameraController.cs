using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    /// <summary>
    /// First-person camera controller with many exposed parameters.
    /// Attach to a parent GameObject representing the player body (for yaw rotation).
    /// Assign the Camera (or camera holder) to cameraTransform.
    /// This script uses the old Input system (Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).
    /// </summary>
    [DisallowMultipleComponent]
    public class FpsCameraController : MonoBehaviour
    {
        [Header("移动速度设置")]
        public float moveSpeed = 10f;       // 基础移动速度
        public float fastMoveSpeed = 25f;   // 按住 Shift 时的加速速度

        [Header("鼠标灵敏度设置")]
        public float mouseSensitivity = 2f; // 鼠标灵敏度

        private float rotationX = 0f; // 当前X轴旋转角
        private float rotationY = 0f; // 当前Y轴旋转角

        void Start()
        {
            // 隐藏并锁定鼠标
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // 初始化旋转角
            Vector3 rot = transform.localRotation.eulerAngles;
            rotationX = rot.x;
            rotationY = rot.y;
        }

        void Update()
        {
            HandleMouseLook();
            HandleMovement();
        }

        /// <summary>
        /// 处理鼠标控制视角
        /// </summary>
        void HandleMouseLook()
        {
            if (Input.GetMouseButton(1)) // 按住右键旋转
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

                rotationY += mouseX;
                rotationX -= mouseY;
                rotationX = Mathf.Clamp(rotationX, -90f, 90f);

                transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
            }
        }

        /// <summary>
        /// 处理键盘移动
        /// </summary>
        void HandleMovement()
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;

            Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) move += transform.forward;
            if (Input.GetKey(KeyCode.S)) move -= transform.forward;
            if (Input.GetKey(KeyCode.A)) move -= transform.right;
            if (Input.GetKey(KeyCode.D)) move += transform.right;
            if (Input.GetKey(KeyCode.E)) move += transform.up;
            if (Input.GetKey(KeyCode.Q)) move -= transform.up;

            transform.position += move * speed * Time.deltaTime;
        }
    }

}