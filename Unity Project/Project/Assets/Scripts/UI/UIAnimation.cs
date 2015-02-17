using UnityEngine;
using System.Collections;


namespace Gem
{
    
    public class UIAnimation : MonoBehaviour
    {
        [SerializeField]
        private string m_IsOpenName = "IsOpen";
        [SerializeField]
        protected Animator m_Animator = null;

        private void Start()
        {
            if(m_Animator == null)
            {
                m_Animator = GetComponent<Animator>();
            }
        }

        public void Open()
        {
            m_Animator.SetBool(m_IsOpenName, true);
        }
        public void Close()
        {
            m_Animator.SetBool(m_IsOpenName, false);
        }

        public bool isOpen
        {
            get { return m_Animator.GetBool(m_IsOpenName); }
        }
        public bool isClosed
        {
            get { return !isOpen; }
        }
    }

}

