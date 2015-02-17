using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Gem
{
    public class UILogin : MonoBehaviour
    {
        [SerializeField]
        private InputField m_Username = null;
        [SerializeField]
        private InputField m_Password = null;

        public void OnEnterClick()
        {
            if(m_Username == null || m_Password == null)
            {
                Debug.LogError("Username/Password fields not assigned to UILogin.");
                return;
            }

            string username = m_Username.text;
            string password = m_Password.text;

            //TODO: Enter username / password to game for logging in.

        }
    }
}


