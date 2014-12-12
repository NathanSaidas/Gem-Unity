#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added constants file.
 * 
 */
#endregion

namespace Gem
{
    /// <summary>
    /// A class that contains constants used throughout the entire program
    /// </summary>
    public static class Constants
    {
        public const int INVALID_INT = -1;                                              ///Added December,7,2014 - Nathan Hanlan
        public const float INVALID_FLOAT = -1.0f;                                       ///Added December,7,2014 - Nathan Hanlan
        public const string INVALID_STRING = null;                                      ///Added December,7,2014 - Nathan Hanlan
        public const string GAME_OBJECT_PERSISTENT = "_Persistent";                      ///Added December,7,2014 - Nathan Hanlan
                                                       
        ///The name of the field OnUse in items.                          
        public const string ITEM_DESTROY_ON_USE = "OnUse";
    }
}