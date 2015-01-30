#region CHANGE LOG
/*  January, 28, 2015 - Nathan Hanlan - Added file/class - PlayerName
 *  January, 29, 2015 - Nathan Hanlan - Added/Implemented INetworkSendable interface. This object may now be transported over the network.
 */
#endregion

// -- System
using System; //Serializable
using System.IO;
using System.Runtime.Serialization;
// -- Unity
using UnityEngine; //SerializeField


namespace Gem
{
    [Serializable]
    public struct PlayerName : INetworkSendable
    {
        /// <summary>
        /// The username to identify the player through chat and other various commands
        /// </summary>
        [SerializeField]
        private string m_Username;
        /// <summary>
        /// A screenname to display to all other players in chat. (This is nickname)
        /// </summary>
        [SerializeField]
        private string m_ScreenName;
        /// <summary>
        /// The index of the player. This value should not have multiple masks.
        /// </summary>
        [SerializeField]
        private PlayerIndex m_PlayerIndex;

        public PlayerName(string aUsername, string aScreenName)
        {
            m_Username = aUsername;
            m_ScreenName = aScreenName;
            m_PlayerIndex = PlayerIndex.None;
        }
        public PlayerName(string aUsername, string aScreenName, PlayerIndex aPlayerIndex)
        {
            m_Username = aUsername;
            m_ScreenName = aScreenName;
            m_PlayerIndex = aPlayerIndex;
        }

        public void OnSend(Stream aStream, IFormatter aFormatter)
        {
            aFormatter.Serialize(aStream, m_Username);
            aFormatter.Serialize(aStream, m_ScreenName);
            aFormatter.Serialize(aStream, m_PlayerIndex);
        }

        public bool OnReceive(Stream aStream, IFormatter aFormatter)
        {
            try
            {
                m_Username = (string)aFormatter.Deserialize(aStream);
                m_ScreenName = (string)aFormatter.Deserialize(aStream);
                m_PlayerIndex = (PlayerIndex)aFormatter.Deserialize(aStream);
                return true;
            }
            catch(Exception aException)
            {
                DebugUtils.LogException(aException);
                return false;
            }
        }

        public string username
        {
            get { return m_Username; }
            set { m_Username = value; }
        }
        public string screenName
        {
            get { return m_ScreenName; }
            set { m_ScreenName = value; }
        }
        public PlayerIndex playerIndex
        {
            get { return m_PlayerIndex; }
            set { m_PlayerIndex = value; }
        }

        
    }
}