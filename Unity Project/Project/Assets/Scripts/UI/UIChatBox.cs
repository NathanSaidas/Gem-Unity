using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Gem
{
    public class UIChatBox : MonoBehaviour
    {
        [SerializeField]
        private int m_MessageLimit = 256;
        [SerializeField]
        private GameObject m_MessagePrefab = null;
        [SerializeField]
        private List<string> m_Messages = new List<string>();
        [SerializeField]
        private InputField m_InputField = null;
        [SerializeField]
        private UIContentScaler m_ContentScaler = null;

        public void AddMessage(string aMessage)
        {
            if (string.IsNullOrEmpty(aMessage) || m_ContentScaler == null)
            {
                return;
            }
            ///Remove Messages past the limit
            while (m_Messages.Count >= m_MessageLimit)
            {
                RectTransform goMessage = m_ContentScaler.RemoveLast();
                m_Messages.RemoveAt(m_Messages.Count - 1);
                if(goMessage != null)
                {
                    Destroy(goMessage.gameObject);
                }
            }

            GameObject message = Instantiate(m_MessagePrefab) as GameObject;
            RectTransform rt = message.GetComponent<RectTransform>();
            Text text = message.GetComponent<Text>();

            m_Messages.Insert(0, aMessage);

            if(text != null)
            {
                text.text = aMessage;
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, text.preferredHeight);
            }

            m_ContentScaler.AddItem(rt,0.5f);
            m_ContentScaler.Scroll(0.0f);
        }
        
        public void OnAddMessage()
        {
            if(m_InputField != null)
            {
                AddMessage(m_InputField.text);
                m_InputField.text = string.Empty;
            }
        }

        public void Clear()
        {
            if(m_ContentScaler!= null)
            {
                m_ContentScaler.Clear(true);
            }
        }

    }
}


