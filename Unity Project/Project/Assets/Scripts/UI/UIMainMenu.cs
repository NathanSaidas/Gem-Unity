using UnityEngine;
using System.Collections;


namespace Gem
{
    public class UIMainMenu : MonoBehaviour
    {
        // -- UI Panels...
        // • Login
        // • Register
        // • Lobby Browser
        // • MatchMaking Window

        [SerializeField]
        private UIAnimation m_LoginAnimation = null;

        [SerializeField]
        private UIMultiAnimation m_TestAnimation = null;
        [SerializeField]
        private string m_TestString = string.Empty;
        [SerializeField]
        private KeyCode m_TestKey = KeyCode.Alpha0;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(m_TestKey) && m_TestAnimation != null)
            {
                if(m_TestAnimation.isOpen)
                {
                    m_TestAnimation.Close(m_TestString);
                }
                else
                {
                    m_TestAnimation.Open(m_TestString);
                }
            }

        }

        //private void 
    }

}

