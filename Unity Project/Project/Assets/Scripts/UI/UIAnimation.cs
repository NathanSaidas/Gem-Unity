using UnityEngine;
using System.Collections;


namespace Gem
{
    
    public class UIAnimation : MonoBehaviour
    {
        [SerializeField]
        private string m_IsOpenName = "IsOpen";
        private Animator m_Animator = null;

        private void Start()
        {
            m_Animator = GetComponent<Animator>();
        }

        public void Open()
        {
            m_Animator.SetBool(m_IsOpenName, true);
        }
        public void Close()
        {
            m_Animator.SetBool(m_IsOpenName, false);
        }
    }

}

