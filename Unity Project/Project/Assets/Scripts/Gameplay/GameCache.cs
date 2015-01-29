#region CHANGE LOG
/*  January, 28, 2015 - Nathan Hanlan - Added GameCache class/file. Added one member. m_CurrentPlayer
 * 
 */
#endregion

// -- System
using System;

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
        public static GameCache Get()
        {
            return Game.isQuitting == false ? Game.cache : null;
        }

        private PlayerInfo m_CurrentPlayer = null;

        

        public PlayerInfo currentPlayer
        {
            get { return m_CurrentPlayer; }
            set { m_CurrentPlayer = value; }
        }
    }

}