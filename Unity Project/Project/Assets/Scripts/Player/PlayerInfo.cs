#region CHANGE LOG
/*  January, 28, 2015 - Nathan Hanlan - Added class PlayerInfo. (Restructured for Gem. Taken from Parrador GGJ Framework).
 *  January, 29, 2015 - Nathan Hanlan - Completed TODO: Implemented Network Serialization with INetworkSendable interface
 */
#endregion
// -- System
using System;
using System.IO;
using System.Runtime.Serialization;
// -- Unity
using UnityEngine;


namespace Gem
{
    [Serializable]
    public class PlayerInfo : INetworkSendable
    {
        // -- Custom Info

        /// <summary>
        /// A struct to define the name of the player.
        /// </summary>
        [SerializeField]
        private PlayerName m_Name = new PlayerName(string.Empty, string.Empty);
        /// <summary>
        /// A struct to define the team of the player.
        /// </summary>
        private Team m_Team = new Team(string.Empty, 0);
        /// <summary>
        /// A player index definition of all players with trade permissoins
        /// </summary>
        private PlayerIndex m_TradeMask = PlayerIndex.None;
        /// <summary>
        /// A player index definition of all players who had control permissions of this players units.
        /// </summary>
        private PlayerIndex m_ControlMask = PlayerIndex.None;
        /// <summary>
        /// A definition of whether or not this Player is an AI player.
        /// </summary>
        private bool m_IsAI = false;

        // -- Network Player Implementation Info
        private string m_ExternalIP = string.Empty;
        private int m_ExternalPort = 0;
        private string m_Guid = string.Empty;
        private string m_IpAddress = string.Empty;
        private int m_Port = 0;

        // -- Server only information.
        [NonSerialized]
        private NetworkPlayer m_NetworkPlayer = default(NetworkPlayer); 


        /// <summary>
        /// Returns true if the player is considered an ally.
        /// This is determined by compare against self or if the team force ids are the same.
        /// </summary>
        /// <param name="aOtherPlayer">The player to compare against.</param>
        public bool IsAlly(PlayerInfo aOtherPlayer)
        {
            if(aOtherPlayer == this)
            {
                return true;
            }
            if (aOtherPlayer.team.teamIndex == team.teamIndex)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the incoming player can trade items TO this player.
        /// This is done by comparing THIS players Trade mask with the INCOMING players player index.
        /// </summary>
        /// <param name="aOtherPlayer"></param>
        public bool IsTradeable(PlayerInfo aOtherPlayer)
        {
            return (aOtherPlayer.m_Name.playerIndex & m_TradeMask) == aOtherPlayer.m_Name.playerIndex;
        }
        /// <summary>
        /// Checks to see if the INCOMING player can trade items TO this player.
        /// This is done by comparing THIS players Controllable mask with the INCOMING players player index
        /// </summary>
        /// <param name="aOtherPlayer"></param>
        /// <returns></returns>
        public bool IsControllable(PlayerInfo aOtherPlayer)
        {
            return (aOtherPlayer.m_Name.playerIndex & m_TradeMask) == aOtherPlayer.m_Name.playerIndex;
        }

        //TODO: Create Serialize / Deserize methods for a memory stream.
        public void OnSend(Stream aStream, IFormatter aFormatter)
        {
            m_Name.OnSend(aStream, aFormatter);
            m_Team.OnSend(aStream, aFormatter);
            aFormatter.Serialize(aStream, m_TradeMask);
            aFormatter.Serialize(aStream, m_ControlMask);
            aFormatter.Serialize(aStream, m_IsAI);
            aFormatter.Serialize(aStream, m_ExternalIP);
            aFormatter.Serialize(aStream, m_ExternalPort);
            aFormatter.Serialize(aStream, m_Guid);
            aFormatter.Serialize(aStream, m_IpAddress);
            aFormatter.Serialize(aStream, m_Port);
        }
        public bool OnReceive(Stream aStream, IFormatter aFormatter)
        {
            
            if(!m_Name.OnReceive(aStream, aFormatter))
            {
                return false;
            }
           
            if(!m_Team.OnReceive(aStream, aFormatter))
            {
                return false;
            }
            try
            {
                m_TradeMask = (PlayerIndex)aFormatter.Deserialize(aStream);
                m_ControlMask = (PlayerIndex)aFormatter.Deserialize(aStream);
                m_IsAI = (bool)aFormatter.Deserialize(aStream);
                m_ExternalIP = (string)aFormatter.Deserialize(aStream);
                m_ExternalPort = (int)aFormatter.Deserialize(aStream);
                m_Guid = (string)aFormatter.Deserialize(aStream);
                m_IpAddress = (string)aFormatter.Deserialize(aStream);
                m_Port = (int)aFormatter.Deserialize(aStream);
                return true;
            }
            catch(Exception aException)
            {
                Debug.LogException(aException);
                return false;
            }
        }

        /// <summary>
        /// Converts a NetworkPlayer to PlayerInfo class.
        /// </summary>
        public static PlayerInfo ToPlayer(PlayerName aPlayerName, Team aTeam, bool aIsAI, NetworkPlayer aPlayer)
        {
            PlayerInfo player = new PlayerInfo();
            player.name = aPlayerName;
            player.team = aTeam;
            player.tradeMask = PlayerIndex.None;
            player.controlMask = PlayerIndex.None;
            player.isAI = aIsAI;
            player.externalIP = aPlayer.externalIP;
            player.externalPort = aPlayer.externalPort;
            player.guid = aPlayer.guid;
            player.ipAddress = aPlayer.ipAddress;
            player.port = aPlayer.port;
            player.networkPlayer = aPlayer;
            return player;
        }

       
        /// <summary>
        /// Converts a NetworkPlayer to PlayerInfo class.
        /// </summary>
        public static PlayerInfo ToPlayer(PlayerName aPlayerName, Team aTeam, NetworkPlayer aPlayer)
        {
            PlayerInfo player = new PlayerInfo();
            player.name = aPlayerName;
            player.team = aTeam;
            player.tradeMask = PlayerIndex.None;
            player.controlMask = PlayerIndex.None;
            player.isAI = false;
            player.externalIP = aPlayer.externalIP;
            player.externalPort = aPlayer.externalPort;
            player.guid = aPlayer.guid;
            player.ipAddress = aPlayer.ipAddress;
            player.port = aPlayer.port;
            player.networkPlayer = aPlayer;
            return player;
        }

        public PlayerName name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public Team team
        {
            get { return m_Team; }
            set { m_Team = value; }
        }
        public PlayerIndex tradeMask
        {
            get { return m_TradeMask; }
            set { m_TradeMask = value; }
        }
        public PlayerIndex controlMask
        {
            get { return m_ControlMask; }
            set { m_ControlMask = value; }
        }
        public bool isAI
        {
            get { return m_IsAI; }
            set { m_IsAI = value; }
        }
        public string externalIP
        {
            get { return m_ExternalIP; }
            set { m_ExternalIP = value; }
        }
        public int externalPort
        {
            get { return m_ExternalPort; }
            set { m_ExternalPort = value; }
        }
        public string guid
        {
            get { return m_Guid; }
            set { m_Guid = value; }
        }
        public string ipAddress
        {
            get { return m_IpAddress; }
            set { m_IpAddress = value; }
        }
        public int port
        {
            get { return m_Port; }
            set { m_Port = value; }
        }
        public NetworkPlayer networkPlayer
        {
            get { return m_NetworkPlayer; }
            set { m_NetworkPlayer = value; }
        }
    }
}


