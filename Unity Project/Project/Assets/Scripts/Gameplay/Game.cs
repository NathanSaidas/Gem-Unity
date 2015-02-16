﻿#region CHANGE LOG
/* January, 28, 2015 - Nathan Hanlan - Implementing singleton pattern into the game class.
 * January, 28, 2015 - Nathan Hanlan - Implementing game cache information into the game class.
 * January, 29, 2015 - Nathan Hanlan - Implemented basic Network Lobby functionality. Should be able to Register Users.
 */
#endregion

// -- Unity
using UnityEngine;

// -- System
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

// -- Gem
using Gem.Coroutines;


#region NETWORK TODO LIST
/* --- TODO: Add Unregister Functionality.
 * --- TODO: Add Disconnect Functionality
 * --- TODO: Add Kick Player Option
 * --- TODO: Add Ban Player Option w/ BlackList
 * --- TODO: Add White List Option
 * --- TODO: Add Basic Chat functionality
 * --- TODO: Add Game Starting functionality.
 * --- TODO: Add GameObject spawning / despawning - See NetworkID
 * --- TODO: Add Network Accessors
 * --- TODO: Add Network Game Interface Layer
 */
#endregion
#region GAME TODO LIST
/* --- User Issues Order (Click) --> Send Order to server --> Server Processes Action --> Server Sends order to Clients for simulation
 * --- Unit Interaction Event (Eg, Take Damage, Death)[SERVER ONLY] --> Send Events to Clients
 * --- Resources / Currency
 */
#endregion
namespace Gem
{
    public class Game : MonoBehaviour
    {
        private static bool m_IsQuitting = false;
        public static bool isQuitting
        {
            get { return m_IsQuitting; }
        }

        #region COROUTINE EX METHODS

        public static void StartCoroutineEx(IEnumerator<YieldInstruction> aCoroutine)
        {
            if(aCoroutine != null && instance != null)
            {
                instance.StartCoroutine(aCoroutine);
            }
        }
        public static void StopCoroutineEx(IEnumerator<YieldInstruction> aCoroutine)
        {
            if(aCoroutine != null && instance != null)
            {
                instance.StopCoroutine(aCoroutine);
            }
        }

        #endregion

        #region SINGLETON IMPLEMENTATION
        private static Game s_Instance = null;
        private static Game instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }
        private static void CreateInstance()
        {
            GameObject persistent = GameObject.Find(Constants.GAME_OBJECT_PERSISTENT);
            if (persistent == null)
            {
                if (Application.isPlaying)
                {
                    persistent = new GameObject(Constants.GAME_OBJECT_GAME);
                    persistent.transform.position = Vector3.zero;
                    persistent.transform.rotation = Quaternion.identity;
                }
            }
            if (persistent != null)
            {
                s_Instance = persistent.GetComponent<Game>();
                if (s_Instance == null && Application.isPlaying)
                {
                    s_Instance = persistent.AddComponent<Game>();
                }
            }
        }
        private static bool SetInstance(Game aInstance)
        {
            if (s_Instance == null)
            {
                s_Instance = aInstance;
                return true;
            }
            return false;
        }
        private static void DestroyInstance(Game aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        [SerializeField]
        private GameCache m_GameCache = null;

        // -- Server Stuff
        /// <summary>
        /// This is the list of all the current players connected in the network.
        /// </summary>
        private List<PlayerInfo> m_CurrentPlayers = new List<PlayerInfo>();
        /// <summary>
        /// This is the list of all the registered players. This is reused between lobby loading and game loading.
        /// </summary>
        private List<string> m_RegisteringPlayers = new List<string>();

        private NetworkStatus m_NetworkStatus = NetworkStatus.Offline;
        [SerializeField]
        private List<GameObject> m_PrefabDatabase = new List<GameObject>();


        private void Awake()
        {
            Application.runInBackground = true;
            if(!SetInstance(this))
            {
                Destroy(this);
                DebugUtils.MultipleInstances<Game>();
                return;
            }
            DontDestroyOnLoad(gameObject);

            CoroutineEx.InitializeCoroutineExtensions(StartCoroutineEx, StopCoroutineEx);
        }

        private void OnDestroy()
        {
            DestroyInstance(this);
        }


        private void OnApplicationQuit()
        {
            m_IsQuitting = true;
        }


        #region INTERNAL UTILS
        public PlayerIndex GetPlayerIndex(int aIndex)
        {
            return (PlayerIndex)(1 << aIndex);
        }
        public PlayerIndex GetAvailablePlayerIndex()
        {
            return GetPlayerIndex(m_CurrentPlayers.Count + 1);
        }
        public Team GetAvailableTeam()
        {
            if(GetAvailablePlayerIndex() == PlayerIndex.Player_One || GetAvailablePlayerIndex() == PlayerIndex.Player_Two)
            {
                return new Team(Constants.TEAM_ANCIENTS, Constants.TEAM_ANCIENTS_INDEX);
            }
            else
            {
                return new Team(Constants.TEAM_CITIZENS, Constants.TEAM_CITIZENS_INDEX);
            }
            
        }

        #endregion


        #region LOBBY HANDLING
        [SerializeField]
        private string m_LobbyName = string.Empty;
        [SerializeField]
        private string m_Comment = string.Empty;
        [SerializeField]
        private int m_PortNumber = 25006;

        private void OnConnectedToServer()
        {
            m_NetworkStatus = NetworkStatus.MatchMakingPeer;
            SendRegisterUser();
        }
        private void OnDisconnectedFromServer(NetworkDisconnection aInfo)
        {
            m_NetworkStatus = NetworkStatus.Offline;

            if(Network.isServer)
            {
                //Local Disconnect.
                //TODO: Display game over menu to return to the main menu
            }
            else
            {
                if(aInfo == NetworkDisconnection.LostConnection)
                {
                    Debug.LogError("Lost Connection with the server");
                }
                else
                {
                    //Client Disconnect
                    //TODO: Display game over menu to return to the main menu
                }
            }
        }

        private void OnFailedToConnect(NetworkConnectionError aError)
        {
            Debug.LogError("Failed to connect to server for reason: " + aError);
        }

        private void OnNetworkInstantiate(NetworkMessageInfo aInfo)
        {
            
        }

        private void OnPlayerConnected(NetworkPlayer aPlayer)
        {

        }

        private void OnPlayerDisconnected(NetworkPlayer aPlayer)
        {
            //TODO: Start a routine to destroy all objects associated with the disconnecting player.
        }

        private void OnServerInitialized()
        {
            GameCache cache = m_GameCache;

            ///Create a local player info
            PlayerInfo localPlayerInfo = PlayerInfo.ToPlayer(new PlayerName(cache.localUsername, cache.localUsername, PlayerIndex.Player_One),
                                                             new Team(Constants.TEAM_ANCIENTS, Constants.TEAM_ANCIENTS_INDEX),
                                                             Network.player);
            ///Assign the player info data
            cache.currentPlayer = localPlayerInfo;
            m_CurrentPlayers.Clear();
            m_CurrentPlayers.Add(localPlayerInfo);
            ///Assign the proper network status.
            m_NetworkStatus = NetworkStatus.MatchMakingHost;
        }

        private void OnLevelWasLoaded(int aLevelIndex)
        {
            //TODO: Register that the game was loaded on a certain level.
        }


        /// <summary>
        /// A wrapper around an RPC call sequence to register this peer with the server.
        /// </summary>
        private void SendRegisterUser()
        {
            GameCache cache = m_GameCache;

            if(!Network.isClient || string.IsNullOrEmpty(cache.localUsername))
            {
                Debug.LogError("Failed to SendRegisterUser");
                return;
            }
            networkView.RPC(NetworkRPC.ON_REGISTER_USER, RPCMode.Server, cache.localUsername);
        }

        /// <summary>
        /// A wrapper around a sequence of RPC calls.
        /// Client --> Asks the Server for an UpdatedPlayerList
        /// Server --> Serializes all Current Players and Sends that to the Client
        /// Client --> Deserializes all the bytes handed to it by the server.
        /// </summary>
        private void SendUpdatePlayerList()
        {
            if(Network.isClient)
            {
                networkView.RPC(NetworkRPC.ON_UPDATE_PLAYER_LIST_REQUEST, RPCMode.Server);
            }
            else if(Network.isServer)
            {
                OnUpdatePlayerListRequest();
            }
        }

        [RPC] // -- Server Only
        private void OnRegisterUser(string aPlayerName, NetworkMessageInfo aInfo)
        {
            if(!Network.isServer)
            {
                DebugUtils.LogError(ErrorCode.NETWORK_CLIENT_CALL_ON_SERVER);
                return;
            }

            if(m_NetworkStatus != NetworkStatus.MatchMakingHost)
            {
                //Fail Condition A, Clients cannot and should not try to connect to a host that isn't setup yet.
                networkView.RPC(NetworkRPC.ON_REGISTER_USER_STATUS, aInfo.sender, NetworkRPC.STATUS_BAD_HOST);
                return;
            }
            if(m_RegisteringPlayers.Count + 1 >= Constants.MAX_PLAYERS)
            {
                //Fail Condition B, Clients cannot connect because the server is full
                networkView.RPC(NetworkRPC.ON_REGISTER_USER_STATUS, aInfo.sender, NetworkRPC.STATUS_SERVER_FULL);
                return;
            }
            if(m_CurrentPlayers.Any<PlayerInfo>(Element => Element.name.username == aPlayerName))
            {
                //Fail Condition C, Clients cannot connect because the server has already registered / connected a player under the same name.
                networkView.RPC(NetworkRPC.ON_REGISTER_USER_STATUS, aInfo.sender, NetworkRPC.STATUS_USER_EXISTS);
                return;
            }
            //TODO: Connect the Player
            PlayerInfo connectingPlayer = PlayerInfo.ToPlayer(new PlayerName(aPlayerName, aPlayerName, GetAvailablePlayerIndex()),
                                                                GetAvailableTeam(),
                                                                aInfo.sender);
            m_CurrentPlayers.Add(connectingPlayer);

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            connectingPlayer.OnSend(memoryStream, formatter);
            byte[] connectingPlayerBytes = memoryStream.ToArray();
            memoryStream.Close();
            networkView.RPC(NetworkRPC.ON_REGISTER_USER_STATUS, aInfo.sender, NetworkRPC.STATUS_GOOD_LOGIN,connectingPlayerBytes);
            //TODO: Push Updated Current Player List To All Players
            SendUpdatePlayerList();
        }
        [RPC] // -- Client Only
        private void OnRegisterUserStatus(int aStatus, byte[] aPlayerInfo)
        {
            if(!Network.isClient)
            {
                DebugUtils.LogError(ErrorCode.NETWORK_SERVER_CALL_ON_CLIENT);
                return;
            }
            switch(aStatus)
            {
                case NetworkRPC.STATUS_GOOD_LOGIN:
                    if(aPlayerInfo != null && aPlayerInfo.Length > 0)
                    {
                        try
                        {
                            PlayerInfo localPlayerInfo = new PlayerInfo();
                            BinaryFormatter formatter = new BinaryFormatter();
                            MemoryStream memoryStream = new MemoryStream(aPlayerInfo);
                            localPlayerInfo.OnReceive(memoryStream, formatter);
                            memoryStream.Close();
                            m_GameCache.currentPlayer = localPlayerInfo;
                        }
                        catch (Exception aException)
                        {
                            DebugUtils.LogException(aException);
                        }
                    }
                    break;
                case NetworkRPC.STATUS_BAD_HOST:
                case NetworkRPC.STATUS_SERVER_FULL:
                case NetworkRPC.STATUS_USER_EXISTS:

                    break;
            }
        }

        [RPC]
        private void OnUpdatePlayerList(byte[] aPlayerInfoList)
        {
            if(!Network.isClient)
            {
                DebugUtils.LogError(ErrorCode.NETWORK_SERVER_CALL_ON_CLIENT);
                return;
            }

            if(aPlayerInfoList == null || aPlayerInfoList.Length == 0)
            {
                return;
            }



            ///Deserialize the memory and resize the current player list to the correct size.
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream(aPlayerInfoList);
            int count = (int)formatter.Deserialize(memoryStream);
            m_CurrentPlayers.Clear();
            m_CurrentPlayers.AddRange(new PlayerInfo[count]);

            ///Deserialize each player.
            foreach(PlayerInfo playerInfo in m_CurrentPlayers)
            {
                playerInfo.OnReceive(memoryStream, formatter);
            }

            memoryStream.Close();
        }
        [RPC]
        private void OnUpdatePlayerListRequest()
        {
            if(!Network.isServer)
            {
                DebugUtils.LogError(ErrorCode.NETWORK_CLIENT_CALL_ON_SERVER);
                return;
            }
            ///Serialize the list of current players
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            formatter.Serialize(memoryStream, m_CurrentPlayers.Count);
            foreach (PlayerInfo playerInfo in m_CurrentPlayers)
            {
                playerInfo.OnSend(memoryStream, formatter);
            }
            byte[] playerInfoBytes = memoryStream.ToArray();
            memoryStream.Close();
            
            ///Send the current player list over the network to each of the players.
            foreach(PlayerInfo playerInfo in m_CurrentPlayers)
            {
                networkView.RPC(NetworkRPC.ON_UPDATE_PLAYER_LIST, playerInfo.networkPlayer, playerInfoBytes);
            }
        }

        #endregion







        public static GameCache cache
        {
            get { return instance == null ? null : instance.m_GameCache; }
        }
        
    }

}

