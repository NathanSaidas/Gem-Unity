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

            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                DebugUtils.LogError("Cannot login with invalid username / password");
                return;
            }

            //TODO: Enter username / password to game for logging in.
            Game.SendAuthenticationServerRequest(Constants.NETWORK_AUTHENTICATION_REQUEST_AUTHENTICATE, username, password, OnRequestComplete);
        }

        private void OnRequestComplete(int aResult)
        {
            if(aResult != Constants.NETWORK_SUCCESS)
            {
                string result = Constants.ParseNetworkErrorCode(aResult);
                Game.ShowErrorWindow(result);
            }
        }
    }
}


