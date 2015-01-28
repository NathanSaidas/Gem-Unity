#region CHANGE LOG
/*  January, 28, 2015 - Nathan Hanlan - Renamed enum Player to PlayerIndex
 * 
 */
#endregion

// -- System
using System; // Flags

namespace Gem
{
    /// <summary>
    /// Represents the player indicies.  Refer to Constants file for MAX_PLAYERS definition.
    /// </summary>
    [Flags]
    public enum PlayerIndex
    {
        None = 0,
        All = 0xFFFF,
        Player_One = 1,
        Player_Two = 2,
        Player_Three = 4,
        Player_Four = 8,
        Player_Five = 16,
        Player_Six = 32,
        Player_Seven = 64,
        Player_Eight = 128,
    }

}