#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan, Adding enum ItemType
 * 
 */
#endregion

namespace Gem
{
    /// <summary>
    /// Represents all the types of items there are.
    /// </summary>
    public enum ItemFlags
    {
        None = 0,
        All = 0xFFFF,
        Other = 1,
        Quest = 2,
        Stackable = 4,
        Useable = 8,
        Equippable = 16,
        Consumeable = 32
    }
}