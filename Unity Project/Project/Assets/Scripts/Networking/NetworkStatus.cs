#region CHANGE LOG
/*  January, 29, 2015 - Nathan Hanlan - Added enum / file NetworkStatus
 * 
 */
#endregion

namespace Gem
{
    public enum NetworkStatus
    {
        Offline,
        Lobby,
        MatchMakingPeer,
        MatchMakingHost,
        GamePeer,
        GameHost
    }
}