using UnityEngine;
using System.Collections;

namespace Gem
{
    public class RotateTransform : MonoBehaviour
    {
        [SerializeField]
        private Space m_Space = Space.World;
        [SerializeField]
        private Vector3 m_RotationSpeeds = Vector3.zero;

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(m_RotationSpeeds * Time.deltaTime, m_Space);
        }

        public Vector3 rotationSpeeds
        {
            get { return m_RotationSpeeds; }
            set { m_RotationSpeeds = value; }
        }
    }
}


