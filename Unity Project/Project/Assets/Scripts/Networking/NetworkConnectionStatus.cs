using UnityEngine;

#region CHANGE LOG
/*  February    16  2015 - Nathan Hanlan - Added NetworkConnectionStatus enum/file to project.
 * 
 */
#endregion

namespace Gem
{
    public enum NetworkConnectionStatus
    {
        Offline,
        // -- Authentication
        ConnectingToAuthentication,
        ConnectedToAuthentication,
        AuthenticatingAccount,
        AuthenticationSuccessful,
        AuthenticationUnsuccessful,

        // -- Lobby
        ConnectingToLobbyBrowser,
        ConnectedToLobbyBrowser,

        // -- Hosted Game
        ConnectingToGameServer,
        ConnectedToGameServerAsClient,
        ConnectedToGameServerAsHost,


    }
}

