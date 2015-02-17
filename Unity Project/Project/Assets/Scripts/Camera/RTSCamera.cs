using UnityEngine;
using System.Collections;

namespace Gem
{
    public class RTSCamera : MonoBehaviour
    {
        private static RTSCamera s_Current = null;
        public static RTSCamera current
        {
            get {return s_Current;}
        }

        [SerializeField]
        private float m_ScreenBorder = 10.0f;
        [SerializeField]
        private float m_CameraSensitivity = 5.0f;
        [SerializeField]
        private Vector3 m_CameraOrigin = Vector3.zero;
        [SerializeField]
        private Vector2 m_CameraBounds = Vector3.zero;

        private bool m_IsFocused = true;

        void Start()
        {
            s_Current = this;
        }

        void OnApplicationFocus(bool aFocusStatus)
        {
            m_IsFocused = aFocusStatus;
        }

        // Update is called once per frame
        void Update()
        {
            if(!m_IsFocused)
            {
                return;
            }
            Vector3 mousePosition = Input.mousePosition;
            Vector3 movementVector = Vector3.zero;
            if(mousePosition.x < 0.0f + m_ScreenBorder)
            {
                movementVector.x = -1.0f;
            }
            else if(mousePosition.x > Screen.width - m_ScreenBorder)
            {
                movementVector.x = 1.0f;
            }

            if(mousePosition.y < 0.0f + m_ScreenBorder)
            {
                movementVector.z = -1.0f;
            }
            else if(mousePosition.y > Screen.height - m_ScreenBorder)
            {
                movementVector.z = 1.0f;
            }

            movementVector *= m_CameraSensitivity;
            Vector3 position = transform.position;
            position += movementVector * Time.deltaTime;
            position.x = Mathf.Clamp(position.x, m_CameraOrigin.x - m_CameraBounds.x * 0.5f, m_CameraOrigin.x + m_CameraBounds.x * 0.5f);
            position.z = Mathf.Clamp(position.z, m_CameraOrigin.z - m_CameraBounds.y * 0.5f, m_CameraOrigin.z + m_CameraBounds.y * 0.5f);

            transform.position = position;
        }

        public float screenBorder
        {
            get { return m_ScreenBorder; }
            set { m_ScreenBorder = value; }
        }
        public float cameraSensitivity
        {
            get { return m_CameraSensitivity; }
            set { m_CameraSensitivity = value; }
        }
        public Vector3 cameraOrigin
        {
            get { return m_CameraOrigin; }
            set { m_CameraOrigin = value; }
        }
        public Vector2 cameraBounds
        {
            get { return m_CameraBounds; }
            set { m_CameraBounds = value; }
        }
    }
}


