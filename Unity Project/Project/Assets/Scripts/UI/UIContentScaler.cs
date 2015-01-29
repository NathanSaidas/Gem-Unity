using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

namespace Gem
{
    public class UIContentScaler : MonoBehaviour
    {
        [SerializeField]
        private float m_Width = 0.0f;
        [SerializeField]
        private float m_MinHeight = 100.0f;
        [SerializeField]
        private float m_StartOffset = 0.0f;
        [SerializeField]
        private RectTransform m_ContentPanel = null;
        [SerializeField]
        private List<RectTransform> m_Content = new List<RectTransform>();
        [SerializeField]
        private Scrollbar m_ScrollBar = null;
        [SerializeField]
        private bool m_ConstantUpdate = false;

        private void Start()
        {
            CalculateScale();
        }
        private void Update()
        {
            if(m_ConstantUpdate)
            {
                CalculateScale();
            }
        }

        public void AddItem(RectTransform aTransform)
        {
            AddItem(aTransform, 1.0f);
        }
        public void AddItem(RectTransform aTransform, float aScale)
        {
            if (aTransform != null)
            {
                aTransform.SetParent(m_ContentPanel);
                aTransform.localScale = Vector3.one * aScale;
                m_Content.Add(aTransform);
                CalculateScale();
            }
        }

        public void RemoveItem(RectTransform aTransform)
        {
            if(m_Content.Remove(aTransform))
            {
                CalculateScale();
            }
        }

        public RectTransform RemoveLast()
        {
            if(m_Content.Count > 0)
            {
                RectTransform rt = m_Content[m_Content.Count - 1];
                m_Content.RemoveAt(m_Content.Count - 1);
                CalculateScale();
                return rt;
            }
            return null;
        }
        public RectTransform RemoveFirst()
        {
            if(m_Content.Count >0 )
            {
                RectTransform rt = m_Content[0];
                m_Content.RemoveAt(0);
                CalculateScale();
                return rt;
            }
            return null;
        }

        public void CalculateScale()
        {
            ///Remove All null elements
            m_Content.RemoveAll(Element => Element == null);

            ///Create a temp height to store positions / total height
            float height = 0.0f;
            height += m_StartOffset;

            ///Set the transform positions then their height.
            for (int i = 0; i < m_Content.Count; i++)
            {
                RectTransform current = m_Content[i];
                current.anchoredPosition = new Vector2(0.0f, height);
                height -= current.sizeDelta.y * current.transform.localScale.x;
            }
            ///Get the larger of the two. height must always be greater than or equal to m_MinHeight
            height = Mathf.Max(Mathf.Abs(height), m_MinHeight);
            if(m_ContentPanel != null)
            {
                m_ContentPanel.sizeDelta = new Vector2(m_Width, height);
            }

        }

        public void Scroll(float aValue)
        {
            if(m_ScrollBar != null)
            {
                m_ScrollBar.value = Mathf.Clamp01(aValue);
            }
        }

        public void Clear()
        {
            Clear(false);
        }

        public void Clear(bool aDestroy)
        {
            for (int i = m_Content.Count - 1; i >= 0; i--)
            {
                Destroy(m_Content[i]);
            }
            m_Content.Clear();
            CalculateScale();
        }

        public float width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }
        public float minHeight
        {
            get{return m_MinHeight;}
            set{m_MinHeight = value;}
        }
        public float startOffset
        {
            get{return m_StartOffset;}
            set{m_StartOffset = value;}
        }
        public RectTransform contentPanel
        {
            get{return m_ContentPanel;}
            set{m_ContentPanel = value;}
        }
        public bool constantUpdate
        {
            get { return m_ConstantUpdate; }
            set { m_ConstantUpdate = value; }
        }
    }
}


