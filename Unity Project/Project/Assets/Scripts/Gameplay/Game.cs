#region CHANGE LOG
/* January, 28, 2015 - Nathan Hanlan - Implementing singleton pattern into the game class.
 * January, 28, 2015 - Nathan Hanlan - Implementing game cache information into the game class.
 */
#endregion

using UnityEngine;
using System.Collections.Generic;

namespace Gem
{
    public class Game : MonoBehaviour
    {
        private static bool m_IsQuitting = false;
        public static bool isQuitting
        {
            get { return m_IsQuitting; }
        }

        #region SINGLETON IMPLEMENTATION
        private static Game s_Instance = null;
        private static Game instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }
        private static void CreateInstance()
        {
            GameObject persistent = GameObject.Find(Constants.GAME_OBJECT_PERSISTENT);
            if (persistent == null)
            {
                if (Application.isPlaying)
                {
                    persistent = new GameObject(Constants.GAME_OBJECT_GAME);
                    persistent.transform.position = Vector3.zero;
                    persistent.transform.rotation = Quaternion.identity;
                }
            }
            if (persistent != null)
            {
                s_Instance = persistent.GetComponent<Game>();
                if (s_Instance == null && Application.isPlaying)
                {
                    s_Instance = persistent.AddComponent<Game>();
                }
            }
        }
        private static bool SetInstance(Game aInstance)
        {
            if (s_Instance == null)
            {
                s_Instance = aInstance;
                return true;
            }
            return false;
        }
        private static void DestroyInstance(Game aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        [SerializeField]
        private GameCache m_GameCache = null;


        private void Awake()
        {
            if(!SetInstance(this))
            {
                Destroy(this);
                DebugUtils.MultipleInstances<Game>();
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            DestroyInstance(this);
        }


        private void OnApplicationQuit()
        {
            m_IsQuitting = true;
        }

        public static GameCache cache
        {
            get { return instance == null ? null : instance.m_GameCache; }
        }
        
    }

}

