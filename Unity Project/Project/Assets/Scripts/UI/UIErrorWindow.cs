using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Gem
{
    /// <summary>
    /// Used to show an error window popup.
    /// 
    /// Use Game.ShowErrorWindow(string) to show the window. Dont instantiate directly.
    /// </summary>
    public class UIErrorWindow : MonoBehaviour
    {
        private static UIErrorWindow s_Current = null;
        public static UIErrorWindow current
        {
            get { return s_Current; }
        }

        [SerializeField]
        private Text m_Text = null;
        [SerializeField]
        private Button m_Button = null;
        [SerializeField]
        private GameObject m_EventSystemPrefab = null;
        
        private void Start()
        {
            s_Current = this;
            if (m_Button != null)
            {
                m_Button.onClick.AddListener(() => OnClicked());
            }
            if(EventSystem.current == null)
            {
                Instantiate(m_EventSystemPrefab);
            }

        }

        private void OnDestroy()
        {
            s_Current = null;
        }

        private void OnClicked()
        {
            Destroy(gameObject);
        }

        public void SetText(string aText)
        {
            if(m_Text != null)
            {
                m_Text.text = aText;
            }
        }
    }
}


