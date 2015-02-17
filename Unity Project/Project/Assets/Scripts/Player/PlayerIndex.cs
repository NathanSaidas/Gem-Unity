#region CHANGE LOG
/*  January  28  2015 - Nathan Hanlan - Renamed enum Player to PlayerIndex
 *  February 16  2015 - Nathan Hanlan - Removed None and All enums from PlayerIndex. Changed data format to use bitshifting instead to make it more readable.
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
        Player_One      = 1 << 0,
        Player_Two      = 1 << 1,
        Player_Three    = 1 << 2,
        Player_Four     = 1 << 3,
        Player_Five     = 1 << 4,
        Player_Six      = 1 << 5,
        Player_Seven    = 1 << 6,
        Player_Eight    = 1 << 7,
    }

}