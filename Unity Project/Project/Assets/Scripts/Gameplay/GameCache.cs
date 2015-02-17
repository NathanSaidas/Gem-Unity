#region CHANGE LOG
/*  January     28  2015 - Nathan Hanlan - Added GameCache class/file. Added one member. m_CurrentPlayer
 *  February    16  2015 - Nathan Hanlan - Get renamed to GetCache
 *  February    16  2015 - Nathan Hanlan - Added global game data dictionary
 *  February    16  2015 - Nathan Hanlan - Modified the GameCache to support add/remove/get operations with the global game data dictionary
 */
#endregion

// -- System
using System;
using System.Collections.Generic;

// -- Unity
using UnityEngine;

namespace Gem
{
    /// <summary>
    /// This class holds a bunch of data held within Game and can be accessed via the game.
    /// </summary>
    [Serializable]
    public class GameCache
    {
        public static GameCache GetCache()
        {
            return Game.isQuitting == false ? Game.cache : null;
        }

        private PlayerInfo m_CurrentPlayer = null;
        private string m_LocalUsername = string.Empty;
        private string m_LocalPassword = string.Empty;
        /// <summary>
        /// A Dictionary containing all of the global object data. 
        /// This dictionary is unique entry only meaning items that, no two same keys can be entered in.
        /// </summary>
        private Dictionary<string, object> m_GlobalGameData = new Dictionary<string, object>();


        /// <summary>
        /// Adds a value to the GameCache using the key.
        /// Unique Key entries only.
        /// 
        /// ErrorCode.GAME_CACHE_ADD_FAIL is logged at LevelThree if the key already exists.
        /// </summary>
        /// <param name="aKey">The key to put in the dictionary.</param>
        /// <param name="aValue">The value associated with the key.</param>
        public static void Add(string aKey, object aValue)
        {
            GameCache cache = GetCache();
            if(cache != null)
            {
                if(cache.m_GlobalGameData.ContainsKey(aKey))
                {
                    DebugUtils.LogError(ErrorCode.GAME_CACHE_ADD_FAIL, LogVerbosity.LevelThree);
                }
                else
                {
                    cache.m_GlobalGameData.Add(aKey, aValue);
                }
            }
            else
            {
                DebugUtils.LogError(ErrorCode.GAME_CACHE_MISSING, LogVerbosity.LevelThree);
            }
        }

        /// <summary>
        /// Removes a value from the GameCache using the key.
        /// 
        /// ErrorCode.GAME_CACHE_REMOVE_FAIL is logged at LevelThree if the key to remove does not exist.
        /// </summary>
        /// <param name="aKey">The key to remove</param>
        public static void Remove(string aKey)
        {
            GameCache cache = GetCache();
            if(cache != null)
            {
                if(!cache.m_GlobalGameData.Remove(aKey))
                {
                    DebugUtils.LogError(ErrorCode.GAME_CACHE_REMOVE_FAIL, LogVerbosity.LevelThree);
                }
            }
            else
            {
                DebugUtils.LogError(ErrorCode.GAME_CACHE_MISSING, LogVerbosity.LevelThree);
            }
        }

        /// <summary>
        /// Attempts to get a value from the GameCache using the key.
        /// 
        /// ErrorCode.GAME_CACHE_REMOVE_FAIL is logged at LevelThree if the key does not exist.
        /// </summary>
        /// <param name="aKey">The key to use for searching the dictionary</param>
        /// <returns>The object found or null if the key was invalid.</returns>
        public static object Get(string aKey)
        {
            GameCache cache = GetCache();
            if(cache != null)
            {
                object value = null;
                if(cache.m_GlobalGameData.TryGetValue(aKey, out value))
                {
                    return value;
                }
                else
                {
                    DebugUtils.LogError(ErrorCode.GAME_CACHE_GET_FAIL, LogVerbosity.LevelThree);
                }
            }
            else
            {
                DebugUtils.LogError(ErrorCode.GAME_CACHE_MISSING, LogVerbosity.LevelThree);
            }
            return null;
        }

        public PlayerInfo currentPlayer
        {
            get { return m_CurrentPlayer; }
            set { m_CurrentPlayer = value; }
        }

        public string localUsername
        {
            get { return m_LocalUsername; }
            set { m_LocalUsername = value; }
        }

        public string localPassword
        {
            get { return m_LocalPassword; }
            set { m_LocalPassword = value; }
        }

    }

}