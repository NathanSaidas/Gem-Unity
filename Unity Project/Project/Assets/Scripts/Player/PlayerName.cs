#region CHANGE LOG
/*  January, 28, 2015 - Nathan Hanlan - Added file/class - PlayerName
 * 
 */
#endregion

// -- System
using System; //Serializable
// -- Unity
using UnityEngine; //SerializeField


namespace Gem
{
    [Serializable]
    public struct PlayerName
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