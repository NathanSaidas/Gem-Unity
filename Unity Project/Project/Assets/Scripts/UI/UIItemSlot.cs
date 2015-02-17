using UnityEngine;
using UnityEngine.UI;
using System;

namespace Gem
{
    [Serializable]
    public class UIItemSlot
    {
        [SerializeField]
        private Image m_Image = null;
        [SerializeField]
        private Image m_Overlay = null;
        [SerializeField]
        private Button m_Button = null;
        [SerializeField]
        private GameObject m_GameObject = null;

        public string name
        {
            get { return m_GameObject != null ? m_GameObject.name : null; }
            set { if (m_GameObject != null) { m_GameObject.name = value; } }
        }
        public Image image
        {
            get { return m_Image; }
            set { m_Image = value; }
        }
        public Image overlay
        {
            get { return m_Overlay; }
            set { m_Overlay = value; }
        }
        public Button button
        {
            get { return m_Button; }
            set { m_Button = value; }
        }
        public GameObject gameObject
        {
            get { return m_GameObject; }
            set { m_GameObject = value; }
        }
    }
}


