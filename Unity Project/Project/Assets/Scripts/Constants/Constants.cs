﻿#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added constants file.
 * 
 */
#endregion

using UnityEngine;

namespace Gem
{
    /// <summary>
    /// A class that contains constants used throughout the entire program
    /// </summary>
    public static class Constants
    {
        ///A copy paste implementation of singletons in unity C# with MonoBehaviours
        #region SINGLETON IMPLEMENTATION
        //private static ClassName s_Instance = null;
        //private static ClassName instance
        //{
        //    get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        //}
        //private static void CreateInstance()
        //{
        //    GameObject persistent = GameObject.Find(Constants.GAME_OBJECT_PERSISTENT);
        //    if (persistent == null)
        //    {
        //        if (Application.isPlaying)
        //        {
        //            persistent = new GameObject(Constants.GAME_OBJECT_CLASS_NAME);
        //            persistent.transform.position = Vector3.zero;
        //            persistent.transform.rotation = Quaternion.identity;
        //        }
        //    }
        //    if (persistent != null)
        //    {
        //        s_Instance = persistent.GetComponent<ClassName>();
        //        if (s_Instance == null && Application.isPlaying)
        //        {
        //            s_Instance = persistent.AddComponent<ClassName>();
        //        }
        //    }
        //}
        //private static bool SetInstance(ClassName aInstance)
        //{
        //    if (s_Instance == null)
        //    {
        //        s_Instance = aInstance;
        //        return true;
        //    }
        //    return false;
        //}
        //private static void DestroyInstance(ClassName aInstance)
        //{
        //    if (s_Instance == aInstance)
        //    {
        //        s_Instance = null;
        //    }
        //}
        #endregion

        public const int INVALID_INT = -1;                                              ///Added December,7,2014 - Nathan Hanlan
        public const float INVALID_FLOAT = -1.0f;                                       ///Added December,7,2014 - Nathan Hanlan
        public const string INVALID_STRING = null;                                      ///Added December,7,2014 - Nathan Hanlan
                                                                                        ///

        public const string GAME_OBJECT_PERSISTENT = "_Persistent";                     ///Added December,7,2014 - Nathan Hanlan
                                                       
        ///The name of the field OnUse in items.                          
        public const string ITEM_DESTROY_ON_USE = "OnUse";


        // -- Game Object Name Constants
        public const string GAME_OBJECT_ACTOR_MANAGER = "_ActorManager";
        public const string GAME_OBJECT_GAME = "_Game";
        public const string GAME_OBJECT_DEBUG = "_Debug";
        public const string GAME_OBJECT_GAME_SELECTION = "_GameSelection";
        public const string GAME_OBJECT_UI_GAME = "_UIGame";


        // -- Team and Player Constants

        public const int MAX_PLAYERS = 8;
        public const int MAX_TEAMS = 2;

        
        public const string TEAM_ANCIENTS = "Ancients";
        public const string TEAM_CITIZENS = "Citizens";
        public const TeamIndex TEAM_ANCIENTS_INDEX = TeamIndex.Team_One;
        public const TeamIndex TEAM_CITIZENS_INDEX = TeamIndex.Team_Two;


        // -- Input Constants
        public const int MOUSE_LEFT = 0;
        public const int MOUSE_RIGHT = 1;
        public const int MOUSE_MIDDLE = 2;

        public const int INPUT_SELECTION = MOUSE_LEFT;
        public const int INPUT_ISSUE_ORDER = MOUSE_RIGHT;

        public const KeyCode INPUT_ADD_SELECTION = KeyCode.LeftShift;
        public const KeyCode INPUT_REMOVE_SELECTION = KeyCode.LeftAlt;
        public const KeyCode INPUT_STOP = KeyCode.S;

        // -- Network Constants

        public const int NETWORK_MAX_CONNECTIONS = 16;
        public const int NETWORK_MAX_PLAYERS = MAX_PLAYERS;
        public const int NETWORK_DEFAULT_PORT = 25002;
        public const string NETWORK_GAME_TYPE_NAME = "Ancients_Settlers_Pre_Alpha";

    }
}