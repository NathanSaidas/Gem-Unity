#region CHANGE LOG
/*  December, 12, 2014 - Nathan Hanlan - Added class Team
 *  January,  29, 2015 - Nathan Hanlan - Refactored the class to contain needed data. Added/Implemented INetworkSendable
 */
#endregion

// -- System
using System; // -Serializeable
using System.IO;
using System.Runtime.Serialization;
// -- Unity
using UnityEngine; // -SerializeField


namespace Gem
{

    /// <summary>
    /// Defines the properties of a team.
    /// </summary>
    [Serializable]
    public struct Team : INetworkSendable
    {
        [SerializeField]
        private string m_TeamName;
        [SerializeField]
        private TeamIndex m_TeamIndex;

        public Team(string aTeamName, TeamIndex aTeamIndex)
        {
            m_TeamName = aTeamName;
            m_TeamIndex = aTeamIndex;
        }

        public void OnSend(Stream aStream, IFormatter aFormatter)
        {
            aFormatter.Serialize(aStream, m_TeamName);
            aFormatter.Serialize(aStream, m_TeamIndex);
        }

        public bool OnReceive(Stream aStream, IFormatter aFormatter)
        {
            try
            {
                m_TeamName = (string)aFormatter.Deserialize(aStream);
                m_TeamIndex = (TeamIndex)aFormatter.Deserialize(aStream);
                return true;
            }
            catch(Exception aException)
            {
                DebugUtils.LogException(aException);
                return false;
            }
        }

        public string teamName
        {
            get { return m_TeamName; }
            set { m_TeamName = value; }
        }
        public TeamIndex teamIndex
        {
            get { return m_TeamIndex; }
            set { m_TeamIndex = value; }
        }



        
    }
}