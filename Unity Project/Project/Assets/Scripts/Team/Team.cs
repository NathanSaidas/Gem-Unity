
#region CHANGE LOG
/* December, 12, 2014 - Nathan Hanlan - Added class Team
 * 
 */
#endregion

// -- System
using System; // -Serializeable
// -- Unity
using UnityEngine; // -SerializeField


namespace Gem
{

    /// <summary>
    /// Defines the properties of a team.
    /// </summary>
    [Serializable]
    public struct Team
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