using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#region CHANGE LOG
/* December, 12, 2014 - Nathan Hanlan - Added class Team
 * 
 */
#endregion

namespace Gem
{
    /// <summary>
    /// Defines the properties of a team.
    /// </summary>
    [Serializable]
    public class Team : ISaveable
    {
        [SerializeField]
        private string m_TeamName = string.Empty;
        [SerializeField]
        private int m_ID = 0;

        public void OnSave(BinaryFormatter aFormatter, Stream aStream)
        {
            if(aFormatter != null && aStream != null)
            {
                aFormatter.Serialize(aStream, m_TeamName);
                aFormatter.Serialize(aStream, m_ID);
            }
        }

        public void OnLoad(BinaryFormatter aFormatter, Stream aStream)
        {
            if (aFormatter != null && aStream != null)
            {
                m_TeamName = (string)aFormatter.Deserialize(aStream);
                m_ID =  (int)aFormatter.Deserialize(aStream);
            }
        }

        public string teamName
        {
            get { return m_TeamName; }
        }
        public int id
        {
            get { return m_ID; }
        }

        
    }
}