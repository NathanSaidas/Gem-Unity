using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gem
{
    [Serializable]
    public class TeamReputation : ISaveable
    {
        [SerializeField]
        private Team m_Team = null;
        [SerializeField]
        private int m_Amount = 0;




        public void OnSave(BinaryFormatter aFormatter, Stream aStream)
        {
            if(aFormatter != null && aStream != null)
            {
                if (m_Team != null)
                {
                    m_Team.OnSave(aFormatter, aStream);
                }
                else
                {
                    aFormatter.Serialize(aStream, "DefaultTeam");
                    aFormatter.Serialize(aStream, 0);
                    DebugUtils.LogWarning(ErrorCode.TEAM_MISSING_TEAM_FOR_SAVE);
                }
                aFormatter.Serialize(aStream, m_Amount);
            }
            
        }

        public void OnLoad(BinaryFormatter aFormatter, Stream aStream)
        {
            if (aFormatter != null && aStream != null)
            {
                if (m_Team != null)
                {
                    m_Team.OnLoad(aFormatter, aStream);
                }
                else
                {
                    //TODO: Find Team
                    if(m_Team != null)
                    {

                    }
                    else if (m_Team == null)
                    {
                        aFormatter.Deserialize(aStream);
                        aFormatter.Deserialize(aStream);
                        DebugUtils.LogWarning(ErrorCode.TEAM_MISSING_TEAM_FOR_LOAD);
                    }
                }
                m_Amount = (int)aFormatter.Deserialize(aStream);
            }
        }
    }
}