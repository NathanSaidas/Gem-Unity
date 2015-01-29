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
        ///A copy paste implementation of singletons in unity C# with MonoBehaviours
        #region SINGLETON IMPLEMENTATION
        //private static ClassName s_Instance = null;
        //private static ClassName instance
        //{
        //    get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        //}
        //private static void CreateInstance()
        //{
        //    GameObject persistent = GameObject.Find(Constants.GAME_OBJECT_PERSISTENT);
        //    if (persistent == null)
        //    {
        //        if (Application.isPlaying)
        //        {
        //            persistent = new GameObject(Constants.GAME_OBJECT_CLASS_NAME);
        //            persistent.transform.position = Vector3.zero;
        //            persistent.transform.rotation = Quaternion.identity;
        //        }
        //    }
        //    if (persistent != null)
        //    {
        //        s_Instance = persistent.GetComponent<ClassName>();
        //        if (s_Instance == null && Application.isPlaying)
        //        {
        //            s_Instance = persistent.AddComponent<ClassName>();
        //        }
        //    }
        //}
        //private static bool SetInstance(ClassName aInstance)
        //{
        //    if (s_Instance == null)
        //    {
        //        s_Instance = aInstance;
        //        return true;
        //    }
        //    return false;
        //}
        //private static void DestroyInstance(ClassName aInstance)
        //{
        //    if (s_Instance == aInstance)
        //    {
        //        s_Instance = null;
        //    }
        //}
        #endregion

        public const int INVALID_INT = -1;                                              ///Added December,7,2014 - Nathan Hanlan
        public const float INVALID_FLOAT = -1.0f;                                       ///Added December,7,2014 - Nathan Hanlan
        public const string INVALID_STRING = null;                                      ///Added December,7,2014 - Nathan Hanlan
        public const string GAME_OBJECT_PERSISTENT = "_Persistent";                      ///Added December,7,2014 - Nathan Hanlan
                                                       
        ///The name of the field OnUse in items.                          
        public const string ITEM_DESTROY_ON_USE = "OnUse";


        public const string GAME_OBJECT_ACTOR_MANAGER = "_ActorManager";
        public const string GAME_OBJECT_GAME = "_Game";

        public const int MAX_PLAYERS = 8;
        public const int MAX_TEAMS = 8;
    }
}