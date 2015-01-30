#region CHANGE LOG
/*  January, 29, 2015 - Nathan Hanlan - Added NetworkRPC class / file
 * 
 */
#endregion

namespace Gem
{
    /// <summary>
    /// This class is dedicated to providing the RPC function call constants.
    /// </summary>
    public static class NetworkRPC
    {
        public const string ON_REGISTER_USER = "OnRegisterUser";
        public const string ON_REGISTER_USER_STATUS = "OnRegisterUserStatus";
        public const string ON_UPDATE_PLAYER_LIST = "OnUpdatePlayerList";
        public const string ON_UPDATE_PLAYER_LIST_REQUEST = "OnUpdatePlayerListerRequest";

        public const int STATUS_GOOD_LOGIN = 1;
        public const int STATUS_BAD_HOST = 2;
        public const int STATUS_SERVER_FULL = 3;
        public const int STATUS_USER_EXISTS = 4;

    }
}

