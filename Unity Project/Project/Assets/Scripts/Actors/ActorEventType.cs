#region CHANGE LOG
/*  February 12 2015 - Nathan Hanlan - Added file/enum ActorEventType
 * 
 */
#endregion

namespace Gem
{
    public enum ActorEventType
    {
        Select,
        Deselect,
        AbilityEffect,
        IssueOrderStop,
        IssueOrderMove,
        IssueOrderFollow,
        IssueOrderWarp,
    }
}