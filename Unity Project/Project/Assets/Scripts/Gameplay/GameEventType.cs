#region CHANGE LOG
/* December, 11, 2014 - Nathan Hanlan Added enum GameEventType
 * 
 */
#endregion

namespace Gem
{
    /// <summary>
    /// Defines the types of game events handled in the GameEvent system.
    /// </summary>
    public enum GameEventType
    {
        None,
        Game,
        Actor,
        Unit,
        Interactive,
        Item,
        Network
    }
}