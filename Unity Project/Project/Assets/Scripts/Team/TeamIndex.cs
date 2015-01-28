#region CHANGE LOG
/*  January, 28, 2015 - Nathan Hanlan - Added file/enum TeamIndex
 * 
 */
#endregion

// -- System
using System; // Flags

namespace Gem
{
    /// <summary>
    /// Represents the team indicies.  Refer to Constants file for MAX_PLAYERS definition.
    /// </summary>
    [Flags]
    public enum TeamIndex
    {
        None = 0,
        All = 0xFFFF,
        Team_One = 1,
        Team_Two = 2,
        Team_Three = 4,
        Team_Four = 8,
        Team_Five = 16,
        Team_Six = 32,
        Team_Seven = 64,
        Team_Eight = 128
    }

}