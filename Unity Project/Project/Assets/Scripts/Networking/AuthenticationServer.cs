// -- Unity
using UnityEngine;

// -- System
using System;
using System.Collections.Generic;
// -- System File IO
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

#region CHANGE LOG
/*  February    16  2015 - Nathan Hanlan - Added in the Authentication Server class/file to the project.
 * 
 */ 
#endregion

namespace Gem
{
    /// <summary>
    /// This component is the authentication server used to communicate with before entering the lobby for the game Ancients vs Settlers.
    /// 
    /// • It stores a collection of Accounts using a Dictionary which allows O(1) search time.
    /// • It autosaves based on the interval set in the constants file.
    /// • Provides Create, Destroy, Authenticate request functionality with codes sent back to the sender.
    /// 
    /// How To Use:
    /// • Attach to a component in the scene.
    /// • Attach a DebugUtils as well for extra debugging while in build.

    /// </summary>
    [RequireComponent(typeof(NetworkView))]
    public class AuthenticationServer : MonoBehaviour , ICommandProcessor
    {
        [SerializeField]
        private bool m_MakeCommandProcessor = false;
        [SerializeField]
        private bool m_StartServerOnLoad = false;


        private Dictionary<string, Account> m_Accounts = new Dictionary<string, Account>();
        private bool m_IsDirty = false; //Whether or not to save the server.

        private float m_CurrentTime = 0.0f;


        private bool m_LogConnections = false;



        private void Start()
        {
            Application.runInBackground = true;
            LoadData();
            m_CurrentTime = Constants.NETWORK_AUTHENTICATION_SERVER_AUTOSAVE_INTERVAL;

            if(m_MakeCommandProcessor)
            {
                DebugUtils.processor = this;
            }
            if(m_StartServerOnLoad)
            {
                StartServer();
            }
            
        }

        private void OnDestroy()
        {
            if(m_StartServerOnLoad)
            {
                StopServer(true);
            }
        }

        private void Update()
        {
            m_CurrentTime -= Time.deltaTime;
            if(m_CurrentTime < 0.0f)
            {
                m_CurrentTime = Constants.NETWORK_AUTHENTICATION_SERVER_AUTOSAVE_INTERVAL;
                SetDirty();
            }
        }

        private void OnPlayerConnected(NetworkPlayer aPlayer)
        {
            if(m_LogConnections)
            {
                DebugUtils.Log("Player Connected: " + aPlayer.externalIP);
            }
        }

        private void OnPlayerDisconnected(NetworkPlayer aPlayer)
        {
            if(m_LogConnections)
            {
                DebugUtils.Log("Player Disconnected: " + aPlayer.externalIP);
            }
        }

        /// <summary>
        /// Starts the server up.
        /// </summary>
        private void StartServer()
        {
            Network.InitializeSecurity();
            NetworkConnectionError error = Network.InitializeServer(Constants.NETWORK_AUTHENTICATION_MAX_CONNECTIONS, Constants.NETWORK_AUTHENTICATION_DEFAULT_PORT, !Network.HavePublicAddress());
            if(error == NetworkConnectionError.NoError)
            {
                MasterServer.RegisterHost(Constants.NETWORK_GAME_TYPE_NAME, Constants.NETWORK_AUTHENTICATION_NAME);
            }
            else
            {
                DebugUtils.LogError(error);
            }
        }

        /// <summary>
        /// Stops the server from running.
        /// </summary>
        /// <param name="aSave">Whether or not to save the server before stopping.</param>
        private void StopServer(bool aSave)
        {
            if (Network.isServer)
            {
                if (aSave)
                {
                    SaveData();
                }
                MasterServer.UnregisterHost();
                Network.Disconnect();
            }

        }

        /// <summary>
        /// Processes authentication requests.
        /// </summary>
        /// <param name="aRequest">The request ID to be made.</param>
        /// <param name="aUsername">The username of the sender.</param>
        /// <param name="aPassword">The password of the sender.</param>
        /// <param name="aInfo">The info of the sender.</param>
        [RPC]
        private void AuthenticationRequest(int aRequest, string aUsername, string aPassword, NetworkMessageInfo aInfo)
        {
            int result = Constants.NETWORK_BAD_REQUEST;
            switch(aRequest)
            {
                case Constants.NETWORK_AUTHENTICATION_REQUEST_CREATE:
                    result = AddAccount(aUsername, aPassword);
                    break;
                case Constants.NETWORK_AUTHENTICATION_REQUEST_DESTROY:
                    result = RemoveAccount(aUsername, aPassword);
                    break;
                case Constants.NETWORK_AUTHENTICATION_REQUEST_AUTHENTICATE:
                    result = Authenticate(aUsername, aPassword);
                    break;
            }
            networkView.RPC(NetworkRPC.ON_REQUEST_RECEIVE, aInfo.sender, result);
        }

        /// <summary>
        /// Adds an account under the following conditions.
        /// If NETWORK_SUCCESS is returned the account is added and the server is SetDirty
        /// 
        /// • Username cannot be empty or null
        /// • Password cannot be empty or null
        /// • Account must not already exist with the same username
        /// 
        /// </summary>
        /// <param name="aUsername">The username of the new account</param>
        /// <param name="aPassword">The password of the new account </param>
        /// <returns>Returns a code signalling the result of adding the account.</returns>
        private int AddAccount(string aUsername, string aPassword)
        {
            if(string.IsNullOrEmpty(aUsername))
            {
                return Constants.NETWORK_BAD_USERNAME_STRING;
            }
            else if(string.IsNullOrEmpty(aPassword))
            {
                return Constants.NETWORK_BAD_PASSWORD_STRING;
            }

            Account account = GetAccount(aUsername);
            if(account != null)
            {
                return Constants.NETWORK_USER_EXISTS;
            }

            ///Create the account
            account = new Account();
            account.username = aUsername;
            account.password = aPassword;

            m_Accounts.Add(account.username, account);
            SetDirty();
            return Constants.NETWORK_SUCCESS;
        }

        /// <summary>
        /// Removes an account under the following conditions.
        /// 
        /// 
        /// • Username cannot be empty or null
        /// • Password cannot be empty or null
        /// • The account must exist
        /// • The username and password must match
        /// 
        /// </summary>
        /// <param name="aUsername">The username of the account to be removed</param>
        /// <param name="aPassword">The password of the account to be removed</param>
        /// <returns>Returns a code signalling the result of removing the account</returns>
        private int RemoveAccount(string aUsername, string aPassword)
        {
            if (string.IsNullOrEmpty(aUsername))
            {
                return Constants.NETWORK_BAD_USERNAME_STRING;
            }
            else if (string.IsNullOrEmpty(aPassword))
            {
                return Constants.NETWORK_BAD_PASSWORD_STRING;
            }

            Account account = GetAccount(aUsername);
            if(account == null)
            {
                return Constants.NETWORK_INVALID_USERNAME;
            }

            if(account.username == aUsername && account.password == aPassword)
            {
                m_Accounts.Remove(account.username);
                SetDirty();
                return Constants.NETWORK_SUCCESS;
            }
            return Constants.NETWORK_INVALID_USERNAME_OR_PASSWORD;

        }

        /// <summary>
        /// Authenticates a user under the following conditions.
        /// 
        /// • Username cannot be empty or null
        /// • Password cannot be empty or null
        /// • The account must exist
        /// • The username and password must match
        /// 
        /// </summary>
        /// <param name="aUsername">The username of the account to be authenticated.</param>
        /// <param name="aPassword">The password of the account to be authenticated.</param>
        /// <returns>Returns a code signalling the result of authenticating the account.</returns>
        private int Authenticate(string aUsername, string aPassword)
        {
            if (string.IsNullOrEmpty(aUsername))
            {
                return Constants.NETWORK_BAD_USERNAME_STRING;
            }
            else if (string.IsNullOrEmpty(aPassword))
            {
                return Constants.NETWORK_BAD_PASSWORD_STRING;
            }

            Account account = GetAccount(aUsername);
            if (account == null)
            {
                return Constants.NETWORK_INVALID_USERNAME;
            }

            if (account.username == aUsername && account.password == aPassword)
            {
                return Constants.NETWORK_SUCCESS;
            }
            return Constants.NETWORK_INVALID_USERNAME_OR_PASSWORD;
        }

        /// <summary>
        /// Saves all the profiles to disk.
        /// </summary>
        private void SaveData()
        {
            DebugUtils.Log("Begin Save");
            try
            {
                FileStream stream = File.Open(Application.persistentDataPath + Constants.NETWORK_AUTHENTICATION_SERVER_FILE, FileMode.Create, FileAccess.Write);
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(stream, Constants.NETWORK_AUTHENTICATION_SERVER_FILE);
                formatter.Serialize(stream, Constants.NETWORK_AUTHENTICATION_SERVER_FILE_VERSION);
                formatter.Serialize(stream, m_Accounts.Count);

                foreach (KeyValuePair<string, Account> account in m_Accounts)
                {
                    formatter.Serialize(stream, account.Value.username);
                    formatter.Serialize(stream, account.Value.password);
                }

                stream.Close();
            }
            catch(Exception aException)
            {
                DebugUtils.LogException(aException);
            }
            DebugUtils.Log("Saving Complete");

        }

        /// <summary>
        /// Loads all the profiles from disk.
        /// </summary>
        private void LoadData()
        {
            DebugUtils.Log("Being Load");
            m_Accounts.Clear();

            try
            {

                if(!Directory.Exists(Application.persistentDataPath + Constants.NETWORK_AUTHENTICATION_SERVER_DIRECTORY ))
                {
                    Directory.CreateDirectory(Application.persistentDataPath + Constants.NETWORK_AUTHENTICATION_SERVER_DIRECTORY);
                }

                if(!File.Exists(Application.persistentDataPath + Constants.NETWORK_AUTHENTICATION_SERVER_FILE))
                {
                    SaveData();
                }

                FileStream stream = File.Open(Application.persistentDataPath + Constants.NETWORK_AUTHENTICATION_SERVER_FILE, FileMode.Open, FileAccess.Read);
                if (stream != null)
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    string header = (string)formatter.Deserialize(stream);
                    int version = (int)formatter.Deserialize(stream);

                    if (version != Constants.NETWORK_AUTHENTICATION_SERVER_FILE_VERSION)
                    {
                        DebugUtils.LogError("Invalid file version");
                        stream.Close();
                        return;
                    }

                    int count = (int)formatter.Deserialize(stream);

                    for (int i = 0; i < count; i++)
                    {
                        string username = (string)formatter.Deserialize(stream);
                        string password = (string)formatter.Deserialize(stream);
                        Account account = new Account();
                        account.username = username;
                        account.password = password;
                        m_Accounts.Add(username, account);
                    }

                    stream.Close();
                }
            }
            catch(Exception aException)
            {
                DebugUtils.LogException(aException);
            }
            DebugUtils.Log("Load Complete");
        }

        /// <summary>
        /// Retrieves an account using the username.
        /// </summary>
        /// <param name="aUsername"></param>
        /// <returns></returns>
        public Account GetAccount(string aUsername)
        {
            Account account = null;

            try
            {
                if(m_Accounts.TryGetValue(aUsername, out account))
                {
                    return account;
                }
            }
            catch (Exception aException)
            {
                DebugUtils.LogException(aException);
            }
            return null;
        }

        /// <summary>
        /// Used to make the server save at the end of frame.
        /// </summary>
        private void SetDirty()
        {
            if(!m_IsDirty)
            {
                m_IsDirty = true;
                StartCoroutine(CleanUpRoutine());
            }
        }

        /// <summary>
        /// Saves the server at the end of the frame.
        /// </summary>
        /// <returns></returns>
        IEnumerator<YieldInstruction> CleanUpRoutine()
        {
            yield return new WaitForEndOfFrame();
            SaveData();
            m_IsDirty = false;
        }


        #region COMMAND LIST
        // • clear
        // • isonline
        // • create <username> <password>
        // • delete <username> <password>
        // • authenticate <username> <password>
        // • save
        // • load
        // • logconnections <on/off>
        // • start
        // • stop [opt]<true/false>

        #endregion

        public void Process(List<string> aWords, List<string> aLowerWords)
        {
            if(aLowerWords == null || aLowerWords.Count == 0)
            {
                return;
            }
            switch(aLowerWords[0])
            {
                case "clear":
                    DebugUtils.ConsoleClear();
                    break;
                case "help":
                case "?":
                case "cmd":
                case "command":
                    {
                        DebugUtils.Log("--- List Of Available Commands ---");
                        DebugUtils.Log("clear");
                        DebugUtils.Log("isonline");
                        DebugUtils.Log("create <username> <password>");
                        DebugUtils.Log("delete <username> <password>");
                        DebugUtils.Log("authenticate <username> <password>");
                        DebugUtils.Log("save");
                        DebugUtils.Log("load");
                        DebugUtils.Log("logconnections <on/off>");
                        DebugUtils.Log("start");
                        DebugUtils.Log("stop [opt]<true/false>");
                    }
                    break;
                case "isonline":
                    {
                        if(Network.isServer)
                        {
                            DebugUtils.Log("Server is online");
                        }
                        else
                        {
                            DebugUtils.Log("Server is offline");
                        }
                    }
                    break;
                case "save":
                    {
                        SetDirty();
                    }
                    break;
                case "load":
                    {
                        LoadData();
                    }
                    break;
                case "authenticate":
                    {
                        if(aWords.Count >= 3)
                        {
                            int result = Authenticate(aWords[1], aWords[2]);
                            PrintResult(result);
                        }
                    }
                    break;
                case "create":
                    {
                        if (aWords.Count >= 3)
                        {
                            int result = AddAccount(aWords[1], aWords[2]);
                            PrintResult(result);
                        }
                    }
                    break;
                case "delete":
                    {
                        if (aWords.Count >= 3)
                        {
                            int result = RemoveAccount(aWords[1], aWords[2]);
                            PrintResult(result);
                        }
                    }
                    break;
                case "logconnections":
                    {
                        if(aLowerWords.Count >= 2)
                        {
                            if(aLowerWords[1] == "on")
                            {
                                m_LogConnections = true;
                            }
                            else if(aLowerWords[1] == "off")
                            {
                                m_LogConnections = false;
                            }
                            else if(aLowerWords[1] == "?")
                            {
                                if(m_LogConnections)
                                {
                                    DebugUtils.Log("Logging Connections is On");
                                }
                                else
                                {
                                    DebugUtils.Log("Logging Connections is Off");
                                }
                            }
                        }
                    }
                    break;
                case "start":
                    {
                        if(!Network.isServer)
                        {
                            StartServer();
                        }
                    }
                    break;
                case "stop":
                    {
                        if(Network.isServer)
                        {
                            if(aLowerWords.Count > 1)
                            {
                                if(aLowerWords[1] == "true")
                                {
                                    StopServer(true);
                                }
                                else if(aLowerWords[1] == "false")
                                {
                                    StopServer(false);
                                }
                                else
                                {
                                    StopServer(true);
                                }
                            }
                            else
                            {
                                StopServer(true);
                            }
                        }
                        Application.Quit();
                    }
                    break;
                
            }
        }

        /// <summary>
        /// Log out a network request result to the console.
        /// </summary>
        /// <param name="aResult">The result to be logged out.</param>
        private void PrintResult(int aResult)
        {
            switch(aResult)
            {
                case Constants.NETWORK_BAD_USERNAME_STRING:
                    DebugUtils.Log("Bad Username String");
                    break;
                case Constants.NETWORK_BAD_PASSWORD_STRING:
                    DebugUtils.Log("Bad Password String");
                    break;
                case Constants.NETWORK_BAD_REQUEST:
                    DebugUtils.Log("Bad Request");
                    break;
                case Constants.NETWORK_INVALID_PASSWORD:
                    DebugUtils.Log("Invalid Password");
                    break;
                case Constants.NETWORK_INVALID_USERNAME:
                    DebugUtils.Log("Invalid Username");
                    break;
                case Constants.NETWORK_INVALID_USERNAME_OR_PASSWORD:
                    DebugUtils.Log("Invalid username or password");
                    break;
                case Constants.NETWORK_USER_EXISTS:
                    DebugUtils.Log("User Exists");
                    break;
            }
        }
    }

}

