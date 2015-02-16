#region CHANGE LOG
/* December, 1, 2014 - Nathan Hanlan - Adding Class DebugConstants and Enum ErrorCode
 * 
 */ 
#endregion

namespace Gem
{
    public static class DebugConstants
    {
        public static readonly string[] ERROR_STRINGS = new string[]
    {
        "Invalid Error Code",
        "Failed to upload item database.",
        "Failed to download item database.",
        "The path given was invalid.",
        "Removed multiple instances of an item. Possible error?",
        "Cannot save or load while saving or loading.",
        "Invalid file version for item database.",
        "Missing team for saving, using default data.",
        "Missing team for loading.",
        "Multiple Instances Of Class Not Allowed: ",

#region NETWORK
        "Call made for Server was made on a Client",
        "Call made for Client was made on a Server",
#endregion

#region ACTOR
        "Actor manager registering multiple of the same key ",
        "Actor manager cannot register an invalid key. (Null or Empty) ",
#endregion

#region COROUTINE
        "Coroutine has not been initialized. Call CoroutineEx.InitializeCoroutineExtensions",
#endregion

#region ABILITY
        "Cannot cast ability, ability is on cooldown.",
        "Cannot cast ability, invalid target.",
#endregion
    };

        public static string GetError(int aCode)
        {
            if (aCode > ERROR_STRINGS.Length || aCode < 0)
            {
                return ERROR_STRINGS[0];
            }
            return ERROR_STRINGS[aCode];
        }
        public static string GetError(ErrorCode aCode)
        {
            int code = (int)aCode;
            if (code > ERROR_STRINGS.Length || code < 0)
            {
                return ERROR_STRINGS[0];
            }
            return ERROR_STRINGS[code];
        }
    }

    /// <summary>
    /// Must match DebugConstants ERROR_STRINGS
    /// </summary>
    public enum ErrorCode
    {
        INVALID_CODE,
        ITEM_UPLOAD_FAILED,
        ITEM_DOWNLOAD_FAILED,
        ITEM_INVALID_FILE_PATH,
        ITEM_MULTIPLE_REMOVED_INSTANCES,
        ITEM_CANNOT_SAVE_LOAD,
        ITEM_INVALID_FILE_VERSION,
        TEAM_MISSING_TEAM_FOR_SAVE,
        TEAM_MISSING_TEAM_FOR_LOAD,
        SINGLETON_MULTIPLE_INSTANCE,
        #region NETWORK
        NETWORK_SERVER_CALL_ON_CLIENT,
        NETWORK_CLIENT_CALL_ON_SERVER,
        #endregion

        #region ACTOR
        ACTOR_MULTIPLE_SAME_KEY,
        ACTOR_INVALID_KEY,
        #endregion

        #region COROUTINE
        COROUTINE_NOT_INITIALIZED,
        #endregion

        #region ABILITY
        CAST_FAILED_COOLDOWN,
        CAST_FAILED_INVALID_TARGET,
        #endregion
    }
}