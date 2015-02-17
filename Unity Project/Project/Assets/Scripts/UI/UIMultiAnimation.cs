using UnityEngine;
using System.Collections.Generic;

namespace Gem
{
    public class UIMultiAnimation : UIAnimation
    {
        [SerializeField]
        private List<RuntimeAnimatorController> m_Controllers = new List<RuntimeAnimatorController>();

        // Use this for initialization
        void Start()
        {
            if(m_Animator == null)
            {
                m_Animator = GetComponent<Animator>();
            }
        }

        public void Open(string aControllerName)
        {
            if(m_Controllers == null || m_Animator == null || string.IsNullOrEmpty(aControllerName))
            {
                return;
            }
            foreach(RuntimeAnimatorController controller in m_Controllers)
            {
                if(controller.name == aControllerName)
                {
                    m_Animator.runtimeAnimatorController = controller;
                    Open();
                    break;
                }
            }
        }

        public void Close(string aControllerName)
        {
            if (m_Controllers == null || m_Animator == null || string.IsNullOrEmpty(aControllerName))
            {
                return;
            }
            foreach (RuntimeAnimatorController controller in m_Controllers)
            {
                if (controller.name == aControllerName)
                {
                    m_Animator.runtimeAnimatorController = controller;
                    Close();
                    break;
                }
            }
        }

        public string currentController
        {
            get { return m_Animator != null ? m_Animator.runtimeAnimatorController.name : null;}
        }
    }

}

