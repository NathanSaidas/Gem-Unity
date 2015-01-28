using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gem
{
    /// <summary>
    /// This class is designed based on the singleton pattern and thus should have an object in the scene to instantiate it.
    /// This class keeps a Queue collection of Garbage items and destroys them at the end of each frame.
    /// Only IActors may be placed on the Queue.
    /// </summary>
    public class ActorManager : MonoBehaviour
    {
        #region SINGLETON
        private static ActorManager s_Instance = null;
        private static ActorManager instance
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
                    persistent = new GameObject(Constants.GAME_OBJECT_ACTOR_MANAGER);
                    persistent.transform.position = Vector3.zero;
                    persistent.transform.rotation = Quaternion.identity;
                }
            }
            if (persistent != null)
            {
                s_Instance = persistent.GetComponent<ActorManager>();
                if (s_Instance == null && Application.isPlaying)
                {
                    s_Instance = persistent.AddComponent<ActorManager>();
                }
            }
        }
        private static bool SetInstance(ActorManager aInstance)
        {
            if (s_Instance == null)
            {
                s_Instance = aInstance;
                return true;
            }
            return false;
        }
        private static void DestroyInstance(ActorManager aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        private Queue<IActor> m_GarbageQueue = new Queue<IActor>();

        private void Awake()
        {
            if(!SetInstance(this))
            {
                Destroy(this);
                DebugUtils.LogError("Multiple Actor Managers in the scene.");
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            DestroyInstance(this);
        }


        private void Update()
        {
            StartCoroutine(DestroyObjects());
        }

        private IEnumerator DestroyObjects()
        {
            yield return new WaitForEndOfFrame();
            while(m_GarbageQueue.Count > 0)
            {
                IActor actor = m_GarbageQueue.Dequeue();
                if(actor != null)
                {
                    GameObject goActor = actor.GetGameObject();
                    if(goActor != null)
                    {
                        Destroy(goActor);
                    }
                }
            }
        }

        /// <summary>
        /// Destroys an Actor. Invokes FinalizeActor.
        /// The destroyed actor gets added to the garbage queue and will be handled at the end of the frame.
        /// </summary>
        /// <param name="aActor">The actor to destroy.</param>
        public static void Destroy(IActor aActor)
        {
            if(aActor != null && aActor.IsObjectAlive())
            {
                ActorManager manager = ActorManager.instance;
                if(manager != null)
                {
                    manager.m_GarbageQueue.Enqueue(aActor);
                    aActor.FinalizeActor();
                }
            }
        }

        

    }

}

