// -- Unity
using UnityEngine;

#region CHANGE LOG
/*  February    16  2015 - Nathan Hanlan - Added GlobalStringData class/file to the project
 * 
 */
#endregion

namespace Gem
{

    /// <summary>
    /// Adds global game data using key value. The keys are Unique Values.
    /// Objects can be queryed in code at a later time.
    /// How To Use:
    /// 
    /// • Attach to GameObject in the Scene
    /// • Enter in the Key and Value
    /// 
    /// </summary>
    public class GlobalStringData : MonoBehaviour
    {
        /// <summary>
        /// The key to use as the Global Game Data
        /// </summary>
        [SerializeField]
        private string m_Key = string.Empty;

        /// <summary>
        /// The string to enter in the global game cache.
        /// </summary>
        [SerializeField]
        private string m_Value = string.Empty;

        /// <summary>
        /// Whether or not to remove the data from the GameCache upon destroy.
        /// </summary>
        [SerializeField]
        private bool m_RemoveOnDestroy = true;

        /// <summary>
        /// A flag to handle application quit destroy vs OnDestroy
        /// </summary>
        private bool m_Removed = false;

        /// <summary>
        /// Adds the key and value. Does not add null or empty keys.
        /// </summary>
        private void Start()
        {
            if (!string.IsNullOrEmpty(m_Key))
            {
                GameCache.Add(m_Key, m_Value);
            }
            else
            {
                DebugUtils.LogError(ErrorCode.INVALID_STRING, LogVerbosity.LevelThree);
            }

        }

        /// <summary>
        /// Removes the key if the key was not already removed and is flagged to be removed.
        /// </summary>
        private void OnApplicationQuit()
        {
            m_Removed = true;
            if (m_RemoveOnDestroy == true && !string.IsNullOrEmpty(m_Key))
            {
                GameCache.Remove(m_Key);
            }
        }

        /// <summary>
        /// Removes the key if the key was not already removed and is flagged to be removed.
        /// </summary>
        private void OnDestroy()
        {
            if (m_RemoveOnDestroy == true && !m_Removed && !string.IsNullOrEmpty(m_Key))
            {
                GameCache.Remove(m_Key);
            }
        }

    }
}


