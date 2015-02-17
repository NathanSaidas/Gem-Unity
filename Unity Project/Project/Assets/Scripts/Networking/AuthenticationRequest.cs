using UnityEngine;
using System.Collections;

namespace Gem
{
    public class AuthenticationRequest
    {
        public delegate void Callback(int aResult);

        private int m_Request = 0;
        private string m_Username = string.Empty;
        private string m_Password = string.Empty;
        private Callback m_Callback = null;

        public int request
        {
            get { return m_Request; }
            set { m_Request = value; }
        }
        public string username
        {
            get { return m_Username; }
            set { m_Username = value; }
        }
        public string password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }
        public Callback callback
        {
            get { return m_Callback; }
            set { m_Callback = value; }
        }
    }

}

